using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Events;
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
            .UseSerilog()
            .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                config.AddEnvironmentVariables();
            })
            
            .UseStartup<Startup>();
    }
}
