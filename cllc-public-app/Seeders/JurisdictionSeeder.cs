using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.Models;
using System;
using System.IO;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.Extensions.Caching.Distributed;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class JurisdictionSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public JurisdictionSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory  loggerFactory, IDistributedCache distributedCache) 
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdateJurisdictions(context);
            
        }

        private void UpdateJurisdictions(AppDbContext context)
        {
            List<Jurisdiction> seedJurisdictions = GetSeedJurisdictions();

            foreach (Jurisdiction jurisdiction in seedJurisdictions)
            {
                context.UpdateSeedJurisdictionInfo(jurisdiction);                
            }

            AddInitialJurisdictions(context);            
        }

        private void AddInitialJurisdictions(AppDbContext context)
        {
            string jurisdictionInitializationFile = Configuration["JurisdictionInitializationFile"];
            if (string.IsNullOrEmpty(jurisdictionInitializationFile))
            {
                // default to sample data, which is stored in the "SeedData" directory.
                jurisdictionInitializationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData" + Path.DirectorySeparatorChar + "Jurisdictions.json"); 
            }
            context.AddInitialJurisdictionsFromFile(jurisdictionInitializationFile);
        }

        private List<Jurisdiction> GetSeedJurisdictions()
        {
            List<Jurisdiction> jurisdictions = new List<Jurisdiction>(GetDefaultJurisdictions());
                
            if (IsProductionEnvironment)
            {
                jurisdictions.AddRange(GetProdJurisdictions());
            }
            else
            {
                jurisdictions.AddRange(GetDevJurisdictions());
            }

                

            return jurisdictions;
        }

        /// <summary>
        /// Returns a list of users to be populated in all environments.
        /// </summary>
        private List<Jurisdiction> GetDefaultJurisdictions()
        {
            return new List<Jurisdiction>();
        }

        /// <summary>
        /// Returns a list of jurisdictions to be populated in the Development environment.
        /// </summary>
        private List<Jurisdiction> GetDevJurisdictions()
        {
            return new List<Jurisdiction>();            
        }

        /// <summary>
        /// Returns a list of jurisdictions to be populated in the Test environment.
        /// </summary>
        private List<Jurisdiction> GetTestJurisdictions()
        {
            return new List<Jurisdiction>();
        }

        /// <summary>
        /// Returns a list of jurisdictions to be populated in the Production environment.
        /// </summary>
        private List<Jurisdiction> GetProdJurisdictions()
        {
            return new List<Jurisdiction>();
        }
    }
}
