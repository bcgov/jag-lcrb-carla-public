using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Authorization;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NWebsec.AspNetCore.Mvc;
using NWebsec.AspNetCore.Mvc.Csp;
using System;
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


        /// <summary>
        /// Logic required to generate a connection string.  If no environment variables exists, defaults to a local sql instance.
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            string result = "Server=";

            if (!string.IsNullOrEmpty(Configuration["DATABASE_SERVICE_NAME"]))
            {
                result += Configuration["DATABASE_SERVICE_NAME"];
            }
            else // default to a local connection.
            {
                result += "127.0.0.1";
            }

            result += ";Database=";

            result += GetDatabaseName();

            if (!string.IsNullOrEmpty(Configuration["DB_USER"]) && !string.IsNullOrEmpty(Configuration["DB_PASSWORD"]))
            {
                result += ";User Id=" + Configuration["DB_USER"] + ";Password=" + Configuration["DB_PASSWORD"] + ";";
            }
           
            return result;
        }

        /// <summary>
        /// Returns the name of the database, as set in the environment.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            string result = "";
            if (!string.IsNullOrEmpty(Configuration["DB_DATABASE"]))
            {
                result += Configuration["DB_DATABASE"];
            }
            else // default to a local connection.
            {
                result += "Surveys";
            }

            return result;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // for security reasons, the following headers are set.
            services.AddMvc(opts =>
            {
                opts.Filters.Add(typeof(NoCacheHttpHeadersAttribute));
                opts.Filters.Add(new XRobotsTagAttribute() { NoIndex = true, NoFollow = true });
                opts.Filters.Add(typeof(XContentTypeOptionsAttribute));
                opts.Filters.Add(typeof(XDownloadOptionsAttribute));
                opts.Filters.Add(typeof(XFrameOptionsAttribute));
                opts.Filters.Add(typeof(XXssProtectionAttribute));
                //CSP
                opts.Filters.Add(typeof(CspAttribute));
                opts.Filters.Add(new CspDefaultSrcAttribute { Self = true });
                opts.Filters.Add(new CspScriptSrcAttribute { Self = true });
                //CSPReportOnly
                opts.Filters.Add(typeof(CspReportOnlyAttribute));
                opts.Filters.Add(new CspScriptSrcReportOnlyAttribute { None = true });
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });


            // add a singleton for data access.

            string connectionString = GetConnectionString();
            string databaseName = GetDatabaseName();

            services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(connectionString));
            
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

            // allow for large files to be uploaded
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1073741824; // 1 GB
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseXContentTypeOptions();
            app.UseXfo(xfo => xfo.Deny());
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseNoCacheHttpHeaders();
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

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            ILogger log = loggerFactory.CreateLogger(typeof(Startup));

            try
            {                              
                using (IServiceScope serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    log.LogInformation("Fetching the application's database context ...");
                    AppDbContext context = serviceScope.ServiceProvider.GetService<AppDbContext>();
                    // run the database seeders
                    log.LogInformation("Adding/Updating seed data ...");
                    Seeders.SeedFactory<AppDbContext> seederFactory = new Seeders.SeedFactory<AppDbContext>(Configuration, env, loggerFactory);
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
                msg.AppendLine("If you are running in a development environment, ensure your test database and server configuraiotn match the project's default connection string.");
                log.LogCritical(new EventId(-1, "Database Migration Failed"), e, msg.ToString());
            }

        }
    }
}
