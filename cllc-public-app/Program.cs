using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var log = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    log.LogInformation("Fetching the application's database context ...");
                    AppDbContext context = services.GetService<AppDbContext>();

                    log.LogInformation("Migrating the database ...");
                    context.Database.Migrate();
                    log.LogInformation("The database migration complete.");

                    // run the database seeders
                    log.LogInformation("Adding/Updating seed data ...");

                    IConfiguration Configuration = host.Services.GetService<IConfiguration>();
                    IHostingEnvironment hostingEnvironment = host.Services.GetService<IHostingEnvironment>();
                    ILoggerFactory loggerFactory = host.Services.GetService<ILoggerFactory>();

                    Seeders.SeedFactory<AppDbContext> seederFactory = new Seeders.SeedFactory<AppDbContext>(Configuration, hostingEnvironment, loggerFactory);
                    seederFactory.Seed((AppDbContext)context);
                    log.LogInformation("Seeding operations are complete.");
                }
                catch (Exception ex)
                {
                    StringBuilder msg = new StringBuilder();
                    msg.AppendLine("The database migration failed!");
                    msg.AppendLine("The database may not be available and the application will not function as expected.");
                    msg.AppendLine("Please ensure a database is available and the connection string is correct.");
                    msg.AppendLine("If you are running in a development environment, ensure your test database and server configuraiotn match the project's default connection string.");
                    log.LogCritical(new EventId(-1, "Database Migration Failed"), e, msg.ToString());
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
