using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Gov.Lclb.Cllb.Public
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();

                    // ConfigMaps mounted as volumes (not envFrom) so changes take effect live —
                    // Kubernetes updates the mounted files in place; KeyPerFile + reloadOnChange
                    // picks up the new values on the next read, no pod restart needed. Each
                    // top-level key in the ConfigMap becomes one file under the mount path.
                    config.AddKeyPerFile("/config/banner", optional: true, reloadOnChange: true);
                })
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
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
