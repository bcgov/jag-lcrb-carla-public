using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace Watchdog
{
    public static class HCUIextensions {
        public static void AddHealthCheckEndpointIfExists(this HealthChecks.UI.Configuration.Settings setup, string name, string uri)
    {
        if (!string.IsNullOrEmpty(uri))
        {
            try
            {

                setup.AddHealthCheckEndpoint(name, uri);
            }
            catch (Exception)
            {
                
            }
            
        }
    }

        public static void AddWebHookIfExists(this HealthChecks.UI.Configuration.Settings setup, string name, string uri, string watchdogUri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                try
                {
                    setup.AddWebhookNotification(name, uri,
                        "{\r\n  \"@context\": \"http://schema.org/extensions\",\r\n  \"@type\": \"MessageCard\",\r\n  \"themeColor\": \"0072C6\",\r\n  \"title\": \"Watchdog Alert: [[LIVENESS]] has failed!\",\r\n  \"text\": \"[[FAILURE]] Click **Learn More** to go to the Watchdog Service\",\r\n  \"potentialAction\": [\r\n    {\r\n      \"@type\": \"OpenUri\",\r\n      \"name\": \"Learn More\",\r\n      \"targets\": [\r\n        { \"os\": \"default\", \"uri\": \"" + watchdogUri + "\" }\r\n      ]\r\n    }\r\n  ]\r\n}",
                        "{\"text\":\"Watchdog Alert: The HealthCheck [[LIVENESS]] is recovered. All is up and running\",\"channel\":\"#general\",\"link_names\": 1,\"username\":\"monkey-bot\",\"icon_emoji\":\":monkey_face\" }"
                        );
                    
                    
                }
                catch (Exception)
                {

                }

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
                string datasource = Configuration["DATASOURCE"];
                if (!string.IsNullOrEmpty(datasource))
                {
                    setup.SetHealthCheckDatabaseConnectionString($"Data Source={datasource}");
                }
                //
                setup.AddHealthCheckEndpointIfExists("Watchdog", Configuration["WATCHDOG_HEALTH_URI"]);

                setup.AddHealthCheckEndpointIfExists("Portal API", Configuration["PORTAL_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("PDF", Configuration["PDF_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("File Manager", Configuration["FILE_MANAGER_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("Federal Reporting", Configuration["FEDERAL_REPORTING_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("Geocoder", Configuration["GEOCODER_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("One Stop", Configuration["ONE_STOP_HEALTH_URI"]);
                setup.AddHealthCheckEndpointIfExists("Org Book", Configuration["ORG_BOOK_URI"]);
                setup.AddHealthCheckEndpointIfExists("SPICE/CARLA Sync", Configuration["SPICE_SYNC_URI"]);

                setup.AddWebHookIfExists("Teams", Configuration["TEAMS_WEB_HOOK"], Configuration["WATCHDOG_URI"]);

                setup.SetMinimumSecondsBetweenFailureNotifications(60 * 60); // re-notify every hour.

            });

           
            services.AddRazorPages()
                .AddNewtonsoftJson(opts =>
                {
                    opts.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    opts.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                    opts.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;

                    // ReferenceLoopHandling is set to Ignore to prevent JSON parser issues with the user / roles model.
                    opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
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

            app.UseStaticFiles();

            app.UseRouting()
            .UseEndpoints(endpoints =>
             {
                 endpoints.MapHealthChecksUI();
                 endpoints.MapRazorPages();
             });
        }
    }
}
