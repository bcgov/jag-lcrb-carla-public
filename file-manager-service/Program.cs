using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Gov.Lclb.Cllb.Services.FileManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddUserSecrets(Assembly.GetExecutingAssembly());
                        config.AddEnvironmentVariables();
                    }
                )
                .ConfigureLogging(
                    (hostingContext, logging) =>
                    {
                        logging.ClearProviders();
                        logging.SetMinimumLevel(LogLevel.Debug);
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                    }
                )
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseOpenShiftIntegration(_ => _.CertificateMountPoint = "/var/run/secrets/service-cert")
                        .UseStartup<Startup>()
                        .UseKestrel(options =>
                        {
                            options.Limits.MaxRequestBodySize = 512 * 1024 * 1024; // allow large transfers
                            // for macOS local dev but don't have env
                            // options.ListenLocalhost(5001, o => {
                            //     o.Protocols = HttpProtocols.Http2;
                            // });
                        });
                });
        }
    }
}
