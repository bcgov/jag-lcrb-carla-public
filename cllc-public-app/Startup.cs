using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Authorization;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Net.Http.Headers;
using NWebsec.AspNetCore.Mvc;
using NWebsec.AspNetCore.Mvc.Csp;
using System;
using System.Data.SqlClient;
using System.Text;

namespace Gov.Lclb.Cllb.Public
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }        

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add a singleton for data access.

            string connectionString = DatabaseTools.GetConnectionString(Configuration);
            string databaseName = DatabaseTools.GetDatabaseName(Configuration);

            DatabaseTools.CreateDatabaseIfNotExists(Configuration);

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connectionString));

            // add singleton to allow Controllers to query the Request object
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // determine if we wire up Dynamics.
            if (!string.IsNullOrEmpty(Configuration["DYNAMICS_ODATA_URI"]))
            {
                SetupDynamics(services);
            }



            // for security reasons, the following headers are set.
            services.AddMvc(opts =>
            {
                // default deny
                var policy = new AuthorizationPolicyBuilder()
                 .RequireAuthenticatedUser()
                 .Build();
                opts.Filters.Add(new AuthorizeFilter(policy));

                opts.Filters.Add(typeof(NoCacheHttpHeadersAttribute));
                opts.Filters.Add(new XRobotsTagAttribute() { NoIndex = true, NoFollow = true });
                opts.Filters.Add(typeof(XContentTypeOptionsAttribute));
                opts.Filters.Add(typeof(XDownloadOptionsAttribute));
                opts.Filters.Add(typeof(XFrameOptionsAttribute));
                opts.Filters.Add(typeof(XXssProtectionAttribute));
                opts.Filters.Add(typeof(CspAttribute));
                opts.Filters.Add(new CspDefaultSrcAttribute { Self = true });
                opts.Filters.Add(new CspScriptSrcAttribute { Self = true });
                //CSPReportOnly
                opts.Filters.Add(typeof(CspReportOnlyAttribute));
                opts.Filters.Add(new CspScriptSrcReportOnlyAttribute { None = true });
            })
                .AddJsonOptions(
                    opts => {
                        opts.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                        opts.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                        opts.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

                        // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
                        opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    });


            // setup siteminder authentication (core 2.0)
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SiteMinderAuthOptions.AuthenticationSchemeName;
                options.DefaultChallengeScheme = SiteMinderAuthOptions.AuthenticationSchemeName;
            }).AddSiteminderAuth(options =>
            {

            });

            // setup authorization
            services.AddAuthorization();
            services.RegisterPermissionHandler();

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


            // health checks
            services.AddHealthChecks(checks =>
            {
                checks.AddSqlCheck(DatabaseTools.GetDatabaseName(Configuration), DatabaseTools.GetConnectionString(Configuration));
            });

            services.AddSession();

        }

        private void SetupDynamics(IServiceCollection services)
        {
            string redisServer = Configuration["REDIS_SERVER"];

            string dynamicsOdataUri = Configuration["DYNAMICS_ODATA_URI"];
            string aadTenantId = Configuration["DYNAMICS_AAD_TENANT_ID"];
            string serverAppIdUri = Configuration["DYNAMICS_SERVER_APP_ID_URI"];
            string clientKey = Configuration["DYNAMICS_CLIENT_KEY"];
            string clientId = Configuration["DYNAMICS_CLIENT_ID"];

            if (string.IsNullOrEmpty(redisServer))            
            {
                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisServer;
                });
            }

            var authenticationContext = new AuthenticationContext(
               "https://login.windows.net/" + aadTenantId);
            ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
            var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
            task.Wait();
            AuthenticationResult authenticationResult = task.Result; 

            Interfaces.Microsoft.Dynamics.CRM.System context = new Interfaces.Microsoft.Dynamics.CRM.System (new Uri(Configuration["DYNAMICS_ODATA_URI"]));
            context.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add("Authorization", authenticationResult.CreateAuthorizationHeader());            
            services.AddSingleton<Interfaces.Microsoft.Dynamics.CRM.System>(context);

			//services.AddTransient<Interfaces.Microsoft.Dynamics.CRM.System>(_ =>
            //{
            //    var context = new Interfaces.Microsoft.Dynamics.CRM.System(new Uri(Configuration["DYNAMICS_ODATA_URI"]));
            //    context.BuildingRequest += (sender, eventArgs) => eventArgs.Headers.Add("Authorization", authenticationResult.CreateAuthorizationHeader());
            //    return context;
            //});

            // add SharePoint.
            string sharePointServerAppIdUri = Configuration["SHAREPOINT_SERVER_APPID_URI"];
            string sharePointWebname = Configuration["SHAREPOINT_WEBNAME"];
            string sharePointAadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
            string sharePointClientId = Configuration["SHAREPOINT_CLIENT_ID"];
            string sharePointCertFileName = Configuration["SHAREPOINT_CERTIFICATE_FILENAME"];
            string sharePointCertPassword = Configuration["SHAREPOINT_CERTIFICATE_PASSWORD"];

            // SharePointFileManager sharePointFileManager = ;
            services.AddTransient<SharePointFileManager>(_ => new SharePointFileManager(sharePointServerAppIdUri, sharePointWebname, sharePointAadTenantId, sharePointClientId, sharePointCertFileName, sharePointCertPassword));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var log = loggerFactory.CreateLogger("Startup");

            string connectionString = "unknown.";
            try
            {
                using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    log.LogInformation("Fetching the application's database context ...");
                    AppDbContext context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                    IDistributedCache distributedCache = serviceScope.ServiceProvider.GetService<IDistributedCache>();
                    Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.System system = serviceScope.ServiceProvider.GetService<Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM.System>();

                    connectionString = context.Database.GetDbConnection().ConnectionString;

                    log.LogInformation("Migrating the database ...");
                    context.Database.Migrate();
                    log.LogInformation("The database migration complete.");

                    // run the database seeders
                    log.LogInformation("Adding/Updating seed data ...");

                    Seeders.SeedFactory<AppDbContext> seederFactory = new Seeders.SeedFactory<AppDbContext>(Configuration, env, loggerFactory, system, distributedCache);
                    seederFactory.Seed((AppDbContext)context);
                    log.LogInformation("Seeding operations are complete.");
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

            
            string pathBase = Configuration["BASE_PATH"];
            if (! string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }            
            if (! env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseCsp(options => options
                .DefaultSources(s => s.Self())
                .ScriptSources(s => s.Self())
                .StyleSources(s => s.Self()));
                app.UseExceptionHandler("/Home/Error");
            }
            
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
                ctx.Context.Response.Headers["Content-Security-Policy"] = "script-src 'self' https://apis.google.com";
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
            

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                // Only run the angular CLI Server in Development mode (not staging or test.)
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });                        

        }
    }
}
