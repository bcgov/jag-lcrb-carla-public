using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public abstract class Seeder<T> where T : AppDbContext
    {
        public const string AllProfiles = "all";

        private readonly IHostingEnvironment _env;
        protected ILogger Logger;
        protected IDynamicsClient _dynamicsClient;

        protected Seeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _env = env;
            Logger = loggerFactory.CreateLogger(typeof(Seeder<T>));
            Configuration = configuration;
            _dynamicsClient = dynamicsClient;
        }

        protected bool IsEnvironment(string environmentName)
        {
            return _env.IsEnvironment(environmentName);
        }

        protected bool IsDevelopmentEnvironment => _env.IsDevelopment();

        protected bool IsTestEnvironment => _env.IsEnvironment("Test");

        protected bool IsStagingEnvironment => _env.IsStaging();

        protected bool IsProductionEnvironment => _env.IsProduction();

        protected IConfiguration Configuration { get; set; }

        public virtual Type InvokeAfter => null;

        protected abstract IEnumerable<string> TriggerProfiles { get; }

        protected abstract void Invoke(T context);

        public void Seed(T context)
        {
            if (TriggerProfiles.Contains(_env.EnvironmentName, StringComparer.OrdinalIgnoreCase) || TriggerProfiles.Contains(AllProfiles, StringComparer.OrdinalIgnoreCase))
            {
                Logger.LogDebug("The trigger for {0} ({1}) matches the deployment profile ({2}); executing...", GetType().Name, string.Join(", ", TriggerProfiles), AllProfiles);
                Invoke(context);
            }
            else
            {
                Logger.LogDebug("Trigger profile(s) for {0} ({1}), do not match the deployment profile ({2}); skipping...", GetType().Name, string.Join(", ", TriggerProfiles), _env.EnvironmentName);
            }
        }
    }
}
