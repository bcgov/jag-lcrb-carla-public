using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NWebsec.AspNetCore.Mvc;
using NWebsec.AspNetCore.Mvc.Csp;

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
        /// Logic required to generate a connection string.  If no environment variables exists, defaults to a local mongo instance.
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString()
        {
            string result = "mongodb://";
            if (!string.IsNullOrEmpty(Configuration["MONGODB_USER"]) && !string.IsNullOrEmpty(Configuration["MONGODB_PASSWORD"]))
            {
                result += Configuration["MONGODB_USER"] + ":" + Configuration["MONGODB_PASSWORD"] + "@";
            }

            if (!string.IsNullOrEmpty(Configuration["DATABASE_SERVICE_NAME"]))
            {
                result += Configuration["DATABASE_SERVICE_NAME"];
            }
            else // default to a local connection.
            {
                result += "127.0.0.1";
            }

            result += ":27017";

            return result;
        }

        /// <summary>
        /// Returns the name of the database, as set in the environment.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
        {
            string result = "";
            if (!string.IsNullOrEmpty(Configuration["MONGODB_DATABASE"]))
            {
                result += Configuration["MONGODB_DATABASE"];
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

            services.AddSingleton<DataAccess>(new DataAccess(connectionString, databaseName));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

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
        }
    }
}
