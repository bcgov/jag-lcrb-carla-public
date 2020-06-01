using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using System;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole(x =>
                {
                    x.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    x.IncludeScopes = true;
                });
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddDebug();
                logging.AddEventSourceLogger();
            })
            .UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                config.AddEnvironmentVariables();
            })
            
            .UseStartup<Startup>();
    }
}
