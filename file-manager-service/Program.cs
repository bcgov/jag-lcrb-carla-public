using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using System;
using System.Net;
using System.Reflection;


namespace Gov.Lclb.Cllb.Services.FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)                
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddUserSecrets(Assembly.GetExecutingAssembly());
                    config.AddEnvironmentVariables();
                    
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Debug);
                    logging.AddDebug();
                    logging.AddEventSourceLogger();
                })
                .UseSerilog()
                .UseOpenShiftIntegration(_ => _.CertificateMountPoint = "/var/run/secrets/service-cert")
                .UseStartup<Startup>();
    }
}
