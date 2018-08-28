using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    /// <summary>
    /// This class automattically loades all seeder classes defined in this assembly,
    /// and provides a simple interface for seeding the application and database with data.
    /// </summary>
    public class SeedFactory<T> where T : AppDbContext
    {
        private readonly IHostingEnvironment _env;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;

        private readonly List<Seeder<T>> _seederInstances = new List<Seeder<T>>();
        

        /// <summary>
        /// SeedFactory Constructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public SeedFactory(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient)
        {
            _env = env;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger(typeof(SeedFactory<T>));
            _configuration = configuration;
            _dynamicsClient = dynamicsClient;


            LoadSeeders();
            _seederInstances.Sort(new SeederComparer<T>());
        }

        private void LoadSeeders()
        {
            _logger.LogDebug("Loading seeders...");

            Assembly assembly = typeof(SeedFactory<T>).GetTypeInfo().Assembly;
            List<Type> types = assembly.GetTypes().Where(t => t.GetTypeInfo().IsSubclassOf(typeof(Seeder<T>))).ToList();
            foreach (Type type in types)
            {
                _logger.LogDebug($"\tCreating instance of {type.Name}...");
                _seederInstances.Add((Seeder<T>)Activator.CreateInstance(type, _configuration, _env, _loggerFactory, _dynamicsClient));
            }

            _logger.LogDebug($"\tA total of {types.Count} seeders loaded.");
        }

        /// <summary>
        /// Seed data instance
        /// </summary>
        /// <param name="context"></param>
        public void Seed(T context)
        {
            _seederInstances.ForEach(seeder =>
            {
                seeder.Seed(context);
            });
        }

        private class SeederComparer<TY> : Comparer<Seeder<TY>> where TY : AppDbContext
        {            
            public override int Compare(Seeder<TY> x, Seeder<TY> y)
            {
                // < 0 x is less than y
                // = 0 same
                // > 0 x greater than y
                int rtnValue = 0;
                if (y != null && x != null && (x.InvokeAfter == y.InvokeAfter))
                {
                    rtnValue = 0;
                }

                if (x != null &&  y != null && (x.InvokeAfter == null && y.InvokeAfter != null))
                {
                    rtnValue = -1;
                }

                if (y != null && x != null && (x.GetType() == y.InvokeAfter))
                {
                    rtnValue = -1;
                }

                if (y != null && x != null && (x.InvokeAfter == y.GetType()))
                {
                    rtnValue = 1;
                }

                return rtnValue;
            }            
        }
    }
}
