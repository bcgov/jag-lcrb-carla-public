using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pdf
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

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Enable Node Services
            services.AddNodeServices();

            services.AddMvc(config =>
            {
					
                if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
                {
                    var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                    config.Filters.Add(new AuthorizeFilter(policy));
                }

            });

            // Other ConfigureServices() code...

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "JAG LCRB PDF Service", Version = "v1" });
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            //if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            //{
            //    // Configure JWT authentication
            //    services.AddAuthentication(o =>
            //    {
            //        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    }).AddJwtBearer(o =>
            //    {
            //        o.SaveToken = true;
            //        o.RequireHttpsMetadata = false;
            //        o.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            RequireExpirationTime = false,
            //            ValidIssuer = Configuration["JWT_VALID_ISSUER"],
            //            ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
            //        };
            //    });
            //}
            
            // health checks. 
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new
                    ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB PDF Service");
            });
        }

    }
}

