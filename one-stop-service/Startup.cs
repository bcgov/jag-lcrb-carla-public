using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using SoapCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace Gov.Jag.Lcrb.OneStopService
{


    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        public Startup(IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (!System.Diagnostics.Debugger.IsAttached)
                builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
            Env = env;
            Configuration = builder.Build();

        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddSerilog(dispose: true));

            // Adjust Kestrel options to allow sync IO
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // Add a memory cache
            services.AddMemoryCache();

            services.AddSoapCore();
            services.AddSingleton<IReceiveFromHubService>(new ReceiveFromHubService(Configuration, Env));


            services.AddSingleton(_loggerFactory.CreateLogger("OneStopUtils"));
            services.AddSingleton(Log.Logger);

            services.AddMvc(config =>
            {
                config.EnableEndpointRouting = false;
                if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                {
                    var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }

            });

            // Other ConfigureServices() code...
            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAG OneStop Service", Version = "v1" });
                c.ParameterFilter<AutoRestParameterFilter>();
                string baseUri = Configuration["BASE_URI"];
                if (baseUri != null)
                {
                    // ensure baseUri is in the right format.
                    baseUri = baseUri.TrimEnd('/') + @"/";
                    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(baseUri + "api/authentication/redirect/" + Configuration["JWT_TOKEN_KEY"]),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"openid", "oidc standard"}
                                }
                            }
                        }
                    });
                }

                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                // Configure JWT authentication
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        RequireExpirationTime = false,
                        ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                        ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
                    };
                });
            }

            services.AddHangfire(config =>
            {
                // Change this line if you wish to have Hangfire use persistent storage.
                config.UseMemoryStorage();
                // enable console logs for jobs
                config.UseConsole();
            });

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("one-stop-service", () => HealthCheckResult.Healthy("OK"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {

            // enable Splunk logger using Serilog
            if (!string.IsNullOrEmpty(Configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(Configuration["SPLUNK_TOKEN"])
                )
            {

                Serilog.Sinks.Splunk.CustomFields fields = new Serilog.Sinks.Splunk.CustomFields();
                if (!string.IsNullOrEmpty(Configuration["SPLUNK_CHANNEL"]))
                {
                    fields.CustomFieldList.Add(new Serilog.Sinks.Splunk.CustomField("channel", Configuration["SPLUNK_CHANNEL"]));
                }
                var splunkUri = new Uri(Configuration["SPLUNK_COLLECTOR_URL"]);

                // Fix for bad SSL issues 


                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.EventCollector(splunkHost: Configuration["SPLUNK_COLLECTOR_URL"],
                       sourceType: "onestop", eventCollectorToken: Configuration["SPLUNK_TOKEN"],
                       restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
#pragma warning disable CA2000 // Dispose objects before losing scope
                       messageHandler: new HttpClientHandler
                       {
                           ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                       }
#pragma warning restore CA2000 // Dispose objects before losing scope
                     )
                    .CreateLogger();



            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .CreateLogger();
            }

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            Log.Logger.Information("Onestop-Service Container Starting");

            // OneStop does not seem to set the SoapAction properly

            app.Use(async (context, next) =>
            {

                if (context.Request.Path.Value.Equals("/receiveFromHub"))
                {
                    string soapAction = context.Request.Headers["SOAPAction"];
                    if (string.IsNullOrEmpty(soapAction) || soapAction.Equals("\"\""))
                    {
                        context.Request.Headers["SOAPAction"] = "http://tempuri.org/IReceiveFromHubService/receiveFromHub";
                    }
                }

                await next();

            });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                });

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB OneStop Service");
                });
            }

            bool startHangfire = true;
#if DEBUG
            // do not start Hangfire if we are running tests.        
            foreach (var assem in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                if (assem.FullName.ToLowerInvariant().StartsWith("xunit"))
                {
                    startHangfire = false;
                    break;
                }
            }
#endif

            if (startHangfire)
            {
                // enable Hangfire, using the default authentication model (local connections only)
                app.UseHangfireServer();

                DashboardOptions dashboardOptions = new DashboardOptions
                {
                    AppPath = null
                };

                app.UseHangfireDashboard("/hangfire", dashboardOptions);
            }

            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app, cache);
            }

            app.UseAuthentication();


            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // by positioning this after the health check, no need to filter out health checks from request logging.
            app.UseSerilogRequestLogging();

            app.UseMvc();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.UseSoapEndpoint<IReceiveFromHubService>("/receiveFromHub", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
            });


            // tell the soap service about the cache.
            using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {

                var memoryCache = serviceScope.ServiceProvider.GetService<IMemoryCache>();
                var soap = serviceScope.ServiceProvider.GetService<IReceiveFromHubService>();
                soap.SetCache(memoryCache);
            }
        }

        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cache"></param>
        private void SetupHangfireJobs(IApplicationBuilder app, IMemoryCache cache)
        {

            Log.Logger.Information("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Log.Logger.Information("Creating Hangfire jobs for License issuance check ...");

                    RecurringJob.AddOrUpdate(() => new OneStopUtils(Configuration, cache).CheckForNewLicences(null), Cron.Hourly());

                    Log.Logger.Information("Hangfire License issuance check jobs setup.");
                }
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Failed to setup Hangfire job.");

                Log.Logger.Error(e, "Hangfire setup failed.");
            }
        }

        public class AuthenticationRequirementsOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Security == null)
                    operation.Security = new List<OpenApiSecurityRequirement>();


                var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }

    }
}
