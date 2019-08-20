using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Rest;
using Splunk;
using Splunk.Configurations;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

[assembly: ApiController]
namespace Gov.Lclb.Cllb.SpdSync
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddMvc(config =>
            {                
                if (!string.IsNullOrEmpty(_configuration["JWT_TOKEN_KEY"]))
                {
                     var policy = new AuthorizationPolicyBuilder()
                                  .RequireAuthenticatedUser()
                                  .Build();
                     config.Filters.Add(new AuthorizeFilter(policy));
                }
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Other ConfigureServices() code...

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "JAG LCRB SPD Transfer Service", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.SchemaFilter<EnumTypeSchemaFilter>();
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            if (!string.IsNullOrEmpty(_configuration["JWT_TOKEN_KEY"]))
            {
                // Configure JWT authentication
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(o =>
                {
                    o.SaveToken = true;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = false,
                        ValidIssuer = _configuration["JWT_VALID_ISSUER"],
                        ValidAudience = _configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_TOKEN_KEY"]))
                    };
                });
            }

            // determine if we wire up SharePoint.
            if (!string.IsNullOrEmpty(_configuration["SHAREPOINT_ODATA_URI"]))
            {
                SetupSharePoint(services);
            }

            // determine if we wire up the SPICE service
            if (!string.IsNullOrEmpty(_configuration["SPICE_URI"]))
            {
                SetupSpice(services);
            }

            services.AddHangfire(config =>
            {
                // Change this line if you wish to have Hangfire use persistent storage.
                config.UseMemoryStorage();
                // enable console logs for jobs
                config.UseConsole();
            });

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("carla-spice-sync", () => HealthCheckResult.Healthy());
        }

        private void SetupSharePoint(IServiceCollection services)
        {
            string ssgUsername = _configuration["SSG_USERNAME"];
            string ssgPassword = _configuration["SSG_PASSWORD"];

            // add SharePoint.
            
            services.AddTransient<SharePointFileManager>(_ => new SharePointFileManager(_configuration));
        }

        private void SetupSpice(IServiceCollection services)
        {
            string spiceSsgUsername = _configuration["SPICE_SSG_USERNAME"];
            string spiceSsgPassword = _configuration["SPICE_SSG_PASSWORD"];
            string spiceURI = _configuration["SPICE_URI"];
            string token = _configuration["SPICE_JWT_TOKEN"];


            // create JWT credentials
            TokenCredentials credentials = new TokenCredentials(token);

            services.AddSingleton(_ => new SpiceClient(new Uri( spiceURI ), credentials));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {            

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

            if (!string.IsNullOrEmpty(_configuration["ENABLE_HANGFIRE_JOBS"]))
            {
                SetupHangfireJobs(app, loggerFactory);
            }

            app.UseHealthChecks("/hc");

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB SPD Transfer Service");
            });

            // enable Splunk logger
            if (!string.IsNullOrEmpty(_configuration["SPLUNK_COLLECTOR_URL"]))
            {
                var splunkLoggerConfiguration = GetSplunkLoggerConfiguration(app);

                //Append Http Json logger
                loggerFactory.AddHECJsonSplunkLogger(splunkLoggerConfiguration);
            }
        }

        SplunkLoggerConfiguration GetSplunkLoggerConfiguration(IApplicationBuilder app)
        {
            SplunkLoggerConfiguration result = null;
            string splunkCollectorUrl = _configuration["SPLUNK_COLLECTOR_URL"];
            if (!string.IsNullOrEmpty(splunkCollectorUrl))
            {
                string splunkToken = _configuration["SPLUNK_TOKEN"];
                if (!string.IsNullOrEmpty(splunkToken))
                {
                    result = new SplunkLoggerConfiguration()
                    {
                        HecConfiguration = new HECConfiguration()
                        {
                            BatchIntervalInMilliseconds = 5000,
                            BatchSizeCount = 10,
                            ChannelIdType = HECConfiguration.ChannelIdOption.None,
                            DefaultTimeoutInMilliseconds = 10000,

                            SplunkCollectorUrl = splunkCollectorUrl,
                            Token = splunkToken,
                            UseAuthTokenAsQueryString = false
                        }
                    };
                }
            }
            return result;
        }

        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        private void SetupHangfireJobs(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            ILogger log = loggerFactory.CreateLogger(typeof(Startup));
            log.LogInformation("Starting setup of Hangfire jobs ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {                    
                    log.LogInformation("Creating Hangfire jobs for SPD Export ...");
                    RecurringJob.AddOrUpdate(() => new SpiceUtils(_configuration, loggerFactory).SendFoundApplications(null), Cron.MinuteInterval(15));
                    // RecurringJob.AddOrUpdate(() => new SpiceUtils(_configuration, loggerFactory).SendFoundWorkers(null), Cron.MinuteInterval(15));
                    log.LogInformation("Hangfire Send Export job done.");

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
