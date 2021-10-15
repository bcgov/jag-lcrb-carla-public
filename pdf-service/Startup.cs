using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using System;
using HealthChecks.UI.Client;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace Gov.Jag.Lcrb.PdfService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (!Debugger.IsAttached)
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
            services.AddControllers(config =>
            {
                config.EnableEndpointRouting = false;
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
                
                c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAG LCRB PDF Service", Version = "v1" });
                c.ParameterFilter<AutoRestParameterFilter>();
                // This is only required because Swashbuckle does not present enums correctly
                c.AddEnumsWithValuesFixFilters();
                string baseUri = Configuration["BASE_URI"];
                if (baseUri != null)
                {
                    // ensure baseUri is in the right format.
                    baseUri = baseUri.TrimEnd('/') + @"/";
                    c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri(baseUri + "api/authentication/redirect/" + Configuration["JWT_TOKEN_KEY"]),
                                Scopes = new Dictionary<string, string>
                                {
                                    {"openid", "oidc standard"}
                                }
                            }
                        }
                    });
                }

                c.OperationFilter<AuthenticationRequirementsOperationFilter>();
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders();

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
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
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        //    RequireExpirationTime = false,
                        ValidIssuer = Configuration["JWT_VALID_ISSUER"],
                        ValidAudience = Configuration["JWT_VALID_AUDIENCE"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT_TOKEN_KEY"]))
                    };
                });
            }

            // health checks.
            services.AddHealthChecks();


            if (!string.IsNullOrEmpty(Configuration["WKHTMLTOPDF_LOCATION"]))
            {
                string wkhtmltopdfLocation = Configuration["WKHTMLTOPDF_LOCATION"];
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            }
            else
            {
                services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.SerializeAsV2 = true;
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB PDF Service");
                });
            }

            if (!string.IsNullOrEmpty(Configuration["JWT_TOKEN_KEY"]))
            {
                app.UseAuthentication();
            }

            app.UseMvc();
            
            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }

        public class AuthenticationRequirementsOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (operation.Security == null)
                    operation.Security = new List<OpenApiSecurityRequirement>();


                var scheme = new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearer" } };
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [scheme] = new List<string>()
                });
            }
        }
    }
}

