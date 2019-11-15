using Gov.Lclb.Cllb.Interfaces;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using System.Net.Http;

namespace Gov.Lclb.Cllb.FederalReportingService
{
    public class Startup
    {
        private readonly ILoggerFactory _loggerFactory;
        public IConfiguration Configuration { get; }

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

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Microsoft.Extensions.Logging.ILogger>(_loggerFactory.CreateLogger("FederalReportingService"));

            services.AddHangfire(config =>
            {
                // Change this line if you wish to have Hangfire use persistent storage.
                config.UseMemoryStorage();
                // enable console logs for jobs
                config.UseConsole();
            });

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("Federal Reporting Service", () => HealthCheckResult.Healthy());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHealthChecks("/hc");
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

            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app, loggerFactory);
            }

            // enable Splunk logger using Serilog
            Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console()
                    .WriteTo.EventCollector( splunkHost: Configuration["SPLUNK_COLLECTOR_URL"],
                       sourceType: "manual", eventCollectorToken: Configuration["SPLUNK_TOKEN"], 
                       restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
#pragma warning disable CA2000 // Dispose objects before losing scope
                       messageHandler: new HttpClientHandler()
                       {
                           ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                       }
#pragma warning restore CA2000 // Dispose objects before losing scope
                     )                    
                    .CreateLogger();

                Serilog.Debugging.SelfLog.Enable(Console.Error);

        }

        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        private void SetupHangfireJobs(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            Microsoft.Extensions.Logging.ILogger log = loggerFactory.CreateLogger(typeof(Startup));
            log.LogInformation("Starting setup of Hangfire job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    log.LogInformation($"Creating Hangfire jobs for {typeof(Startup)} ...");

                    // Run 1 minute past midnight on the 15th of every month
                    RecurringJob.AddOrUpdate(() => new FederalReportingController(Configuration, loggerFactory).GenerateFederalTrackingReport(null), "1 8 15 * *");

                    log.LogInformation("Hangfire jobs setup.");
                }
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Failed to setup Hangfire job.");

                log.LogCritical(new EventId(-1, "Hangfire job setup failed"), e, msg.ToString());
            }
        }

    }
}
