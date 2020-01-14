using System;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;
using Serilog;
using Serilog.Exceptions;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using Newtonsoft.Json;
using System.Linq;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public ILoggerFactory _loggerFactory;

        public Startup(IWebHostEnvironment environment, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (!System.Diagnostics.Debugger.IsAttached)
                builder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();
            
            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        RequireExpirationTime = false,
                        ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                        ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
                    };
                });
            }
            
            services.AddAuthorization();
            services.AddGrpc();

            services.AddHangfire(config => 
            {
                config.UseMemoryStorage();
                config.UseConsole();
            });

            services.AddHealthChecks()
                 .AddCheck("orgbook-service", () => HealthCheckResult.Healthy("OK"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            var healthCheckOptions = new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonConvert.SerializeObject(
                       new
                       {
                           checks = r.Entries.Select(e =>
                      new {
                          description = e.Key,
                          status = e.Value.Status.ToString(),
                          responseTime = e.Value.Duration.TotalMilliseconds
                      }),
                           totalResponseTime = r.TotalDuration.TotalMilliseconds
                       });
                    await c.Response.WriteAsync(result);
                }
            };

            app.UseHealthChecks("/hc/ready", healthCheckOptions);

            app.UseHealthChecks("/hc/live", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = (_) => false
            });

            bool startHangfire = true;
#if DEBUG
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
                app.UseHangfireServer();
                app.UseHangfireDashboard("/hangfire", new DashboardOptions { AppPath = null });
            }

            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app);
            }

            app.UseEndpoints(endpoints => 
            {
                endpoints.MapGrpcService<OrgBookController>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });

            if (!string.IsNullOrEmpty(Configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(Configuration["SPLUNK_TOKEN"]))
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.EventCollector(Configuration["SPLUNK_COLLECTOR_URL"],
                        Configuration["SPLUNK_TOKEN"], restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                    .CreateLogger();
            }
        }

        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        private void SetupHangfireJobs(IApplicationBuilder app)
        {
            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    // _logger.LogInformation("Creating Hangfire jobs for License issuance check ...");
                    GenericRequest id = new GenericRequest() {
                        Id = Guid.NewGuid().ToString()
                    };
                    RecurringJob.AddOrUpdate(() => new OrgBookController(Configuration, _loggerFactory).SyncLicencesToOrgbook(id, null), Cron.Hourly());
                    RecurringJob.AddOrUpdate(() => new OrgBookController(Configuration, _loggerFactory).SyncOrgbookToLicences(id, null), Cron.Hourly());
                    RecurringJob.AddOrUpdate(() => new OrgBookController(Configuration, _loggerFactory).SyncOrgbookToAccounts(id, null), Cron.Daily());
                }
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("Failed to setup Hangfire job.");

                // _logger.LogCritical(new EventId(-1, "Hangfire job setup failed"), e, msg.ToString());
            }
        }
    }
}
