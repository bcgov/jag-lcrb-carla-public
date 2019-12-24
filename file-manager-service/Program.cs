using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using System;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args)
                .ConfigureKestrel(options =>
                {
                    // Setup a HTTP/2 endpoint without TLS.
                    options.ListenLocalhost(8080, o => o.Protocols =
                        HttpProtocols.Http1AndHttp2);
                })
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {

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
                .UseStartup<Startup>();
    }
}
