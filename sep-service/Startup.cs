using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using HealthChecks.UI.Client;
using Newtonsoft.Json;
using System.Net.Mime;
using Gov.Lclb.Cllb.Interfaces;
using System.Text.Json.Serialization;

namespace SepService
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
            services.AddControllers()
                .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.HttpMethod}");
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "JAG LCRB SEP Transfer Service", Version = "v1" });
                c.ParameterFilter<AutoRestParameterFilter>();
            });


            services.AddHttpClient<IDynamicsClient, DynamicsClient>();

            // health checks. 
            services.AddHealthChecks()
                .AddCheck("sep-service", () => HealthCheckResult.Healthy());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

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

            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "JAG LCRB SPD Transfer Service");
            });

            

            // enable Splunk logger using Serilog
            if (!string.IsNullOrEmpty(Configuration["SPLUNK_COLLECTOR_URL"]) &&
                !string.IsNullOrEmpty(Configuration["SPLUNK_TOKEN"])
            )
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level} {Properties} {Message}{NewLine}{Exception}") // one of the logger pipeline elements for writing out the log message
                    .WriteTo.EventCollector(splunkHost: Configuration["SPLUNK_COLLECTOR_URL"],
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
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .Enrich.WithExceptionDetails()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level} {Properties} {Message}{NewLine}{Exception}") // one of the logger pipeline elements for writing out the log message
                    .CreateLogger();
            }
        }
    }
}
