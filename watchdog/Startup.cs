using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Watchdog
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

            services.AddHealthChecks();

            // Registers required services for health checks
            services.AddHealthChecksUI(setupSettings: setup =>
            {
                //setup.SetHealthCheckDatabaseConnectionString("Data Source=./healthchecksdb");
                setup.AddHealthCheckEndpoint("Watchdog", "/healthcheck");

                string portalHealthCheck = Configuration["PORTAL_HEALTH_URI"];
                //setup.AddHealthCheckEndpoint("Portal", portalHealthCheck);
                
                setup.AddHealthCheckEndpoint("PDF", Configuration["PDF_HEALTH_URI"]);
                /*
                //setup.AddHealthCheckEndpoint("File Manager", Configuration["FILE_MANAGER_HEALTH_URI"]);
                setup.AddHealthCheckEndpoint("Federal Reporting", Configuration["FEDERAL_REPORTING_HEALTH_URI"]);
                setup.AddHealthCheckEndpoint("Geocoder", Configuration["GEOCODER_HEALTH_URI"]);
                setup.AddHealthCheckEndpoint("One Stop", Configuration["ONE_STOP_HEALTH_URI"]);
                setup.AddHealthCheckEndpoint("Redis", Configuration["REDIS_HEALTH_URI"]);
                */
                //setup.AddWebhookNotification("webhook1", uri: "http://httpbin.org/status/200?code=ax3rt56s", payload: "{...}");
            });
            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseRouting()
            .UseEndpoints(config =>
             {
                 config.MapHealthChecksUI(); 
             });
        }
    }
}
