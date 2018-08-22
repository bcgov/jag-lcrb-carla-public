using Hangfire;
using Hangfire.Console;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using System.Text;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (!System.Diagnostics.Debugger.IsAttached)
                builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            // Other ConfigureServices() code...

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "JAG LCRB SPD Transfer Service", Version = "v1" });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

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
                    ValidIssuer = Configuration["ISSUER_TOKEN"],
                    ValidAudience = Configuration["ISSUER_TOKEN"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TOKEN_KEY"]))
                };                
            });

            services.AddHangfire(config =>
            {
                // Change this line if you wish to have Hangfire use persistent storage.
                config.UseMemoryStorage();
                // enable console logs for jobs
                config.UseConsole();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


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
                // enable Hangfire
                app.UseHangfireServer();

                // Modify this to change authentication for Hangfire.
                // For example, to restore default authorization (local access only), remove the Authorization = new [] ... line.
                DashboardOptions dashboardOptions = new DashboardOptions
                {
                    Authorization = new[] { new LocalHangfireAuthorizationFilter() }
                };

                dashboardOptions.AppPath = null;

                app.UseHangfireDashboard("/hangfire", dashboardOptions); 
            }

            // this should be set as an environment variable
            if (!string.IsNullOrEmpty(Configuration["ENABLE_HANGFIRE_INDEX"]))
            {
                SetupHangfireJobs(app, loggerFactory);
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB SPD Transfer Service");
            });
        }

        /// <summary>
        /// Setup the Hangfire jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="loggerFactory"></param>
        private void SetupHangfireJobs(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            ILogger log = loggerFactory.CreateLogger(typeof(Startup));
            log.LogInformation("Starting setup of Hangfire Document Index job ...");

            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    string accessToken = Configuration["SearchService:AccessToken"];
                    string baseUri = Configuration["SearchService:BaseUri"];
                    
                    log.LogInformation("Creating Hangfire job for SPD Daily Export ...");
                    // every 60 seconds we see if a file needs to be indexed.
                    RecurringJob.AddOrUpdate(() => SpdUtils.SendExportJob(baseUri, null), Cron.Daily);
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
