using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Gov.Lclb.Cllb.Public.Test
{
    #region snippet1
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<Gov.Lclb.Cllb.Public.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseEnvironment("Staging")
                .UseConfiguration(new ConfigurationBuilder()
                    .AddUserSecrets<Gov.Lclb.Cllb.Public.Startup>()
                    .AddEnvironmentVariables()
                .Build())
                .UseStartup<Gov.Lclb.Cllb.Public.Startup>();
        }
    }
    #endregion
}
