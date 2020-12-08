using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Gov.Lclb.Cllb.Public.Test
{
    #region snippet1
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<SepService.Startup>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                                .AddUserSecrets<SepService.Startup>()
                                .AddEnvironmentVariables()
                                .Build();

            builder
            .UseEnvironment("Staging")
            .UseConfiguration(Configuration)
            .UseStartup<SepService.Startup>();
        }
    }
    #endregion
}
