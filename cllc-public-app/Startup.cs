// change to a #define to enable MSSQL
#undef USE_MSSQL
#undef USE_GEOCODER_CHECK 

using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Authorization;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Services.FileManager;
using Grpc.Net.Client;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NWebsec.AspNetCore.Mvc;
using NWebsec.AspNetCore.Mvc.Csp;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using StackExchange.Redis;
using static Gov.Lclb.Cllb.Services.FileManager.FileManager;

namespace Gov.Lclb.Cllb.Public
{
    public class Startup
    {

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        readonly string MyAllowSpecificOrigins = "script-src 'self' 'unsafe-eval' 'unsafe-inline' https://apis.google.com https://maxcdn.bootstrapcdn.com https://cdnjs.cloudflare.com https://code.jquery.com https://stackpath.bootstrapcdn.com https://fonts.googleapis.com";

        public IConfiguration _configuration { get; }

        public IWebHostEnvironment _env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add a singleton for data access.
#if (USE_MSSQL)
            string connectionString = DatabaseTools.GetConnectionString(Configuration);
            string databaseName = DatabaseTools.GetDatabaseName(Configuration);

            DatabaseTools.CreateDatabaseIfNotExists(Configuration);

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connectionString));
#endif
            // add singleton to allow Controllers to query the Request object
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // determine if we wire up Dynamics.
            if (!string.IsNullOrEmpty(_configuration["DYNAMICS_ODATA_URI"]))
            {
                SetupServices(services);
            }


            // Add a memory cache
            services.AddMemoryCache();

            // for security reasons, the following headers are set.
            services.AddMvc(opts =>
            {
                opts.EnableEndpointRouting = false;
                // default deny
                var policy = new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();
                opts.Filters.Add(new AuthorizeFilter(policy));

                opts.Filters.Add(typeof(NoCacheHttpHeadersAttribute));
                opts.Filters.Add(new XRobotsTagAttribute { NoIndex = true, NoFollow = true });
                opts.Filters.Add(typeof(XContentTypeOptionsAttribute));
                opts.Filters.Add(typeof(XDownloadOptionsAttribute));
                opts.Filters.Add(typeof(XFrameOptionsAttribute));
                opts.Filters.Add(typeof(XXssProtectionAttribute));
                //CSPReportOnly
                opts.Filters.Add(typeof(CspReportOnlyAttribute));
                opts.Filters.Add(new CspScriptSrcReportOnlyAttribute { None = true });
            })
            .AddNewtonsoftJson(opts =>
           {
               opts.SerializerSettings.Formatting = Formatting.Indented;
               opts.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
               opts.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

               // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
               opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           })
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // setup siteminder authentication 
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SiteMinderAuthOptions.AuthenticationSchemeName;
                options.DefaultChallengeScheme = SiteMinderAuthOptions.AuthenticationSchemeName;
            }).AddSiteminderAuth(options =>
            {

            });

            // setup authorization
            services.AddAuthorization(options =>
            {
                // This policy is used for existing business accounts
                options.AddPolicy("Business-User", policy =>
                                  policy.RequireAssertion(context =>
                                  {
                                      var res = context.User.HasClaim(c => c.Type == User.UserTypeClaim && c.Value == "Business")
                                      && context.User.HasClaim(c => c.Type == User.PermissionClaim && c.Value == Permission.ExistingUser);
                                      return res;
                                  }));
                // This policy is used for existing business accounts and also during account registration
                options.AddPolicy("Can-Create-Account", policy =>
                                  policy.RequireAssertion(context =>
                                  {
                                      var res = context.User.HasClaim(c => c.Type == User.UserTypeClaim && c.Value == "Business")
                                      && (context.User.HasClaim(c => c.Type == User.PermissionClaim && c.Value == Permission.ExistingUser)
                                      || context.User.HasClaim(c => c.Type == User.PermissionClaim && c.Value == Permission.NewUserRegistration));
                                      return res;
                                  }));
                // This policy is used for existing bc services card accounts
                options.AddPolicy("Service-Card-User", policy =>
                                  policy.RequireAssertion(context =>
                                  {
                                      var res = context.User.HasClaim(c => c.Type == User.UserTypeClaim && c.Value == "VerifiedIndividual")
                                      && context.User.HasClaim(c => c.Type == User.PermissionClaim && c.Value == Permission.ExistingUser);
                                      return res;
                                  }));

            });
            services.RegisterPermissionHandler();
            if (!string.IsNullOrEmpty(_configuration["KEY_RING_DIRECTORY"]))
            {
                // setup key ring to persist in storage.
                services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(_configuration["KEY_RING_DIRECTORY"]));
            }

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            // allow for large files to be uploaded
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1073741824; // 1 GB
            });

            var orgBook = new OrgBookClient(new HttpClient());
            orgBook.ReadResponseAsString = true;
            services.AddTransient(_ => (IOrgBookClient)orgBook);


            if (!string.IsNullOrEmpty(_configuration["REDIS_SERVER"]))
            {
                string config = _configuration["REDIS_SERVER"];
                if (!string.IsNullOrEmpty(_configuration["REDIS_PASSWORD"]))
                {
                    string redisPassword = _configuration["REDIS_PASSWORD"];
                    config += $",password={redisPassword}";
                }
                // Abort Connect is a setting that controls if Redis will try a connection if it thinks the service is not available.
                // For cloud installations such as Azure it should be set to false.
                if (!string.IsNullOrEmpty(_configuration["REDIS_DISABLE_ABORT_CONNECT"]))
                {
                    config += ",abortConnect=false";
                }
                
                services.AddDistributedRedisCache(o =>
                {
                    o.Configuration = config;
                });

                // health checks
                services.AddHealthChecks()
#if USE_GEOCODER_CHECK
                    .AddCheck<GeocoderHealthCheck>("Geocoder")
#endif
                    .AddCheck("cllc_public_app", () => HealthCheckResult.Healthy())
                    // No longer checking SQL Server in health checks as the SQL components are no longer active.
#if (USE_MSSQL)
                .AddSqlServer(DatabaseTools.GetConnectionString(Configuration), name: "Sql server")
#endif
                    .AddRedis(config, name: "Redis");
                /*
                 * .AddCheck<DynamicsHealthCheck>("Dynamics")
                 *
                 */

            }
            else // checks with no redis.
            {
                // health checks
                services.AddHealthChecks()
#if USE_GEOCODER_CHECK
                    .AddCheck<GeocoderHealthCheck>("Geocoder")
#endif
                    .AddCheck("cllc_public_app", () => HealthCheckResult.Healthy());
                // No longer checking SQL Server in health checks as the SQL components are no longer active.
#if (USE_MSSQL)
                .AddSqlServer(DatabaseTools.GetConnectionString(Configuration), name: "Sql server")
#endif
                //.AddCheck<DynamicsHealthCheck>("Dynamics")

            }

            // session will automatically use redis or another distributed cache if it is available.
            services.AddSession(x =>
            {
                x.IdleTimeout = TimeSpan.FromHours(4.0);
                x.Cookie.IsEssential = true;
            });

        }

        /// <summary>
        /// Shared Http Retry Policy - with Jitter and exponential back-off.
        /// </summary>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            Random jitterer = new Random();
            var retryWithJitterPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.GatewayTimeout || msg.StatusCode == HttpStatusCode.ServiceUnavailable)
                .WaitAndRetryAsync(6,    // exponential back-off plus some jitter
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                  + TimeSpan.FromMilliseconds(jitterer.Next(0, 100))
                );
            return retryWithJitterPolicy;
        }

        /// <summary>
        /// circuit breaker policy is configured so it breaks or opens the circuit when there have been five consecutive faults when retrying the Http requests. When that happens, the circuit will break for 30 seconds.
        /// </summary>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        private void SetupServices(IServiceCollection services)
        {

            AuthenticationResult authenticationResult = null;
            
            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("https://localhost",
                                        "https://lcrb-cllcms-portal-dev.silver.devops.bcgov",
                                        "https://lcrb-cllcms-portal-test.silver.devops.bcgov",
                                        "https://lcrb-cllcms-portal-prod.silver.devops.bcgov",
                                        "https://dev.justice.gov.bc.ca",
                                        "https://test.justice.gov.bc.ca",
                                        "https://justice.gov.bc.ca");
                });
            });

            services.AddHttpClient<IDynamicsClient, DynamicsClient>();
            
            // add BCeID Web Services

            string bceidUrl = _configuration["BCEID_SERVICE_URL"];
            string bceidSvcId = _configuration["BCEID_SERVICE_SVCID"];
            string bceidUserid = _configuration["BCEID_SERVICE_USER"];
            string bceidPasswd = _configuration["BCEID_SERVICE_PASSWD"];

            services.AddTransient(_ => new BCeIDBusinessQuery(bceidSvcId, bceidUserid, bceidPasswd, bceidUrl));

            // add BC Express Pay (Bambora) service
            services.AddHttpClient<IBCEPService, BCEPService>()
                .AddPolicyHandler(GetRetryPolicy());

            // add the PDF client.
            services.AddHttpClient<IPdfService, PdfService>()
                .AddPolicyHandler(GetRetryPolicy());

            // add the GeoCoder Client.
            services.AddHttpClient<IGeocoderService, GeocoderService>()
                .AddPolicyHandler(GetRetryPolicy());

            // add the file manager.
            string fileManagerURI = _configuration["FILE_MANAGER_URI"];
            if (!_env.IsProduction()) // needed for macOS TLS being turned off
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }
            if (!string.IsNullOrEmpty(fileManagerURI))
            {
                var httpClientHandler = new HttpClientHandler();

                if (!_env.IsProduction()) // Ignore certificate errors in non-production modes.  
                                          // This allows you to use OpenShift self-signed certificates for testing.
                {
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var initialChannel = GrpcChannel.ForAddress(fileManagerURI, new GrpcChannelOptions { HttpClient = httpClient, MaxSendMessageSize = null, MaxReceiveMessageSize = null});

                var initialClient = new FileManagerClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new TokenRequest
                {
                    Secret = _configuration["FILE_MANAGER_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == ResultStatus.Success)
                {
                    // Add the bearer token to the client.

                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(fileManagerURI, new GrpcChannelOptions { HttpClient = httpClient, MaxSendMessageSize = null, MaxReceiveMessageSize = null });

                    services.AddTransient(_ => new FileManagerClient(channel));

                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var log = loggerFactory.CreateLogger("Startup");

            string connectionString = "unknown.";

#if (USE_MSSQL)

            if (!string.IsNullOrEmpty(Configuration["DB_PASSWORD"]))
            {

                try
                {
                    using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        log.LogDebug("Fetching the application's database context ...");
                        AppDbContext context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                        IDynamicsClient dynamicsClient = serviceScope.ServiceProvider.GetService<IDynamicsClient>();

                        connectionString = context.Database.GetDbConnection().ConnectionString;

                        log.LogDebug("Migrating the database ...");
                        context.Database.Migrate();
                        log.LogDebug("The database migration complete.");

                        // run the database seeders
                        log.LogDebug("Adding/Updating seed data ...");

                        Seeders.SeedFactory<AppDbContext> seederFactory = new Seeders.SeedFactory<AppDbContext>(Configuration, env, loggerFactory, dynamicsClient);
                        seederFactory.Seed((AppDbContext)context);
                        log.LogDebug("Seeding operations are complete.");
                    }
                }
                catch (Exception e)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendLine("The database migration failed!");
                    msg.AppendLine("The database may not be available and the application will not function as expected.");
                    msg.AppendLine("Please ensure a database is available and the connection string is correct.");
                    msg.AppendLine("If you are running in a development environment, ensure your test database and server configuration match the project's default connection string.");
                    msg.AppendLine("Which is: " + connectionString);
                    log.LogCritical(new EventId(-1, "Database Migration Failed"), e, msg.ToString());
                }

            }
#endif


            string pathBase = _configuration["BASE_PATH"];

            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";

                        await context.Response.WriteAsync("An unexpected server error occurred.\r\n");

                        var exceptionHandlerPathFeature =
                            context.Features.Get<IExceptionHandlerPathFeature>();

                        if (exceptionHandlerPathFeature?.Error != null)
                        {
                            Log.Logger.Error(exceptionHandlerPathFeature?.Error, "Unexpected Error");
                        }
                    });
                });
                app.UseHsts(); // Strict-Transport-Security
                app.UseCors(MyAllowSpecificOrigins);
            }

            var healthCheckOptions = new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonConvert.SerializeObject(
                       new
                       {
                           checks = r.Entries.Select(e =>
                      new
                      {
                          description = e.Key,
                          status = e.Value.Status.ToString(),
                          responseTime = e.Value.Duration.TotalMilliseconds
                      }),
                           totalResponseTime = r.TotalDuration.TotalMilliseconds
                       });
                    await c.Response.WriteAsync(result);
                }
            };
            app.UseHealthChecks("/hc/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/hc/live", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = _ => false
            });



            app.UseXContentTypeOptions();
            app.UseXfo(xfo => xfo.Deny());

            StaticFileOptions staticFileOptions = new StaticFileOptions();

            staticFileOptions.OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate, private";
                ctx.Context.Response.Headers[HeaderNames.Pragma] = "no-cache";
                ctx.Context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                ctx.Context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                ctx.Context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            };

            app.UseStaticFiles(staticFileOptions);

            app.UseSpaStaticFiles(staticFileOptions);
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseNoCacheHttpHeaders();
            // IMPORTANT: This session call MUST go before UseMvc()
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            
            

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
                       sourceType: "portal", eventCollectorToken: _configuration["SPLUNK_TOKEN"],
                       restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
#pragma warning disable CA2000 // Dispose objects before losing scope
                       messageHandler: new HttpClientHandler
                       {
                           ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                       }
#pragma warning restore CA2000 // Dispose objects before losing scope
                     )
                    .CreateLogger();

                Serilog.Debugging.SelfLog.Enable(Console.Error);

                Log.Logger.Information("CARLA Portal Container Started");

            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .CreateLogger();
            }

            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501
                    if (string.IsNullOrEmpty(_configuration["ANGULAR_DEV_SERVER"]))
                    {
                        spa.Options.SourcePath = "ClientApp";
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                    else
                    {
                        spa.UseProxyToSpaDevelopmentServer(_configuration["ANGULAR_DEV_SERVER"]);
                    }
                });
            }
            else
            {
                Log.Logger.Information("Not enabling single page application hosting from Dotnet - using externally hosted static content.");
            }

        }

    }
}
