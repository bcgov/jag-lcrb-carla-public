using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Console;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Hangfire.MemoryStorage;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Serilog.Exceptions;

namespace LdbOrdersService 
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
            services.AddLogging(configure => configure.AddSerilog(dispose: true));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LdbOrdersService ", Version = "v1" });
            });

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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

            Log.Logger.Information("LDB-Orders-Service Container Starting");

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
                SetupHangfireJobs(app);
            }

            app.UseAuthentication();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            // by positioning this after the health check, no need to filter out health checks from request logging.
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="cache"></param>
        private void SetupHangfireJobs(IApplicationBuilder app)
        {
            Log.Logger.Information("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    Log.Logger.Information("Creating Hangfire jobs for LDB Sales Reports");

                    RecurringJob.AddOrUpdate(() => new LdbOrdersUtils(Configuration).CheckForLdbSales(null), Cron.Daily());

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
