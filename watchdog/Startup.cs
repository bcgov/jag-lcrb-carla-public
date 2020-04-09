using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Watchdog
{
    public static class HCUIextensions {
        public static void AddHealthCheckEndpointIfExists(this HealthChecks.UI.Configuration.Settings setup, string name, string uri)
    {
        if (!string.IsNullOrEmpty(uri))
        {
            setup.AddHealthCheckEndpoint(name, uri);
        }
    }
}
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
                setup.AddHealthCheckEndpoint("Watchdog", "/hc/ready");
                
                setup.AddHealthCheckEndpointIfExists("Portal API", Configuration["PORTAL_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("PDF", Configuration["PDF_HEALTH_URI"]);                
                setup.AddHealthCheckEndpointIfExists("File Manager", Configuration["FILE_MANAGER_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("Federal Reporting", Configuration["FEDERAL_REPORTING_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("Geocoder", Configuration["GEOCODER_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("One Stop", Configuration["ONE_STOP_HEALTH_URI"]);
                setup.AddHealthCheckEndpoint("Org Book", Configuration["ORG_BOOK_URI"]);
                
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

            app.UseHealthChecks("/hc/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/hc/live", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = (_) => false
            });

            app.UseRouting()
            .UseEndpoints(config =>
             {
                 config.MapHealthChecksUI(); 
             });
        }
    }
}
