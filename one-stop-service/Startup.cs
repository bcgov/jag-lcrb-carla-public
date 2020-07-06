using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.OneStopService;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using Serilog.Events;
using Serilog.Exceptions;
using SoapCore;
using System;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace Gov.Lclb.Cllb.OneStopService
{

    public static class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is object) // same as !(endpoint is null)
            {
                return string.Equals(
                    endpoint.DisplayName,
                    "Health checks",
                    StringComparison.Ordinal);
            }
            // No endpoint, so not a health check endpoint
            return false;
        }

        public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) =>
                ex != null
                    ? LogEventLevel.Error
                    : ctx.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                            ? LogEventLevel.Verbose // Was a health check, use Verbose
                            : LogEventLevel.Information;
     }    


    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration _configuration { get; }
        public IWebHostEnvironment _env { get; }

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
            _env = env;
            _configuration = builder.Build();

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
            var x = services.AddMemoryCache();

            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            services.AddSingleton<IReceiveFromHubService>(new ReceiveFromHubService(dynamicsClient, _configuration, _env));


            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(_loggerFactory.CreateLogger("OneStopUtils"));
            services.AddSingleton<Serilog.ILogger>(Log.Logger);

            services.AddMvc(config =>
            {
                config.EnableEndpointRouting = false;
                if (!string.IsNullOrEmpty(_configuration["JWT_TOKEN_KEY"]))
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAG LCRB One Stop Service", Version = "v1" });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            if (!string.IsNullOrEmpty(_configuration["JWT_TOKEN_KEY"]))
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
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = false,
                        ValidIssuer = _configuration["JWT_VALID_ISSUER"],
                        ValidAudience = _configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]))
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
            if (!string.IsNullOrEmpty(_configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(_configuration["SPLUNK_TOKEN"])
                )
            {

                Serilog.Sinks.Splunk.CustomFields fields = new Serilog.Sinks.Splunk.CustomFields();
                if (!string.IsNullOrEmpty(_configuration["SPLUNK_CHANNEL"]))
                {
                    fields.CustomFieldList.Add(new Serilog.Sinks.Splunk.CustomField("channel", _configuration["SPLUNK_CHANNEL"]));
                }
                var splunkUri = new Uri(_configuration["SPLUNK_COLLECTOR_URL"]);
                var upperSplunkHost = splunkUri.Host?.ToUpperInvariant() ?? string.Empty;

                // Fix for bad SSL issues 


                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.EventCollector(splunkHost: _configuration["SPLUNK_COLLECTOR_URL"],
                       sourceType: "manual", eventCollectorToken: _configuration["SPLUNK_TOKEN"],
                       restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
#pragma warning disable CA2000 // Dispose objects before losing scope
                       messageHandler: new HttpClientHandler()
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



            // , serializer: SoapSerializer.XmlSerializer, caseInsensitivePath: true

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            if (!string.IsNullOrEmpty(_configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app, cache);
            }

            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB One Stop Service");
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // by positioning this after the health check, no need to filter out health checks from request logging.
            app.UseSerilogRequestLogging();

            app.UseMvc();

            app.UseSoapEndpoint<IReceiveFromHubService>(path: "/receiveFromHub", binding: new BasicHttpBinding());

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
            /// <param name="loggerFactory"></param>
            private void SetupHangfireJobs(IApplicationBuilder app, IMemoryCache cache)
        {

            Log.Logger.Information("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Log.Logger.Information("Creating Hangfire jobs for License issuance check ...");

                    
                    RecurringJob.AddOrUpdate(() => new OneStopUtils(_configuration, cache).CheckForNewLicences(null), Cron.Hourly());

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

    }
}
