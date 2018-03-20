using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class JurisdictionSeeder : Seeder<DataAccess>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public JurisdictionSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory) 
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(DataAccess context)
        {
            UpdateJurisdictions(context);
            
        }

        private void UpdateJurisdictions(DataAccess context)
        {
            List<Jurisdiction> seedJurisdictions = GetSeedJurisdictions();

            foreach (Jurisdiction jurisdiction in seedJurisdictions)
            {
                context.UpdateSeedJurisdictionInfo(jurisdiction);                
            }

            AddInitialJurisdictions(context);            
        }

        private void AddInitialJurisdictions(DataAccess context)
        {
            context.AddInitialJurisdictionsFromFile(Configuration["JurisdictionInitializationFile"]);
        }

        private List<Jurisdiction> GetSeedJurisdictions()
        {
            List<Jurisdiction> jurisdictions = new List<Jurisdiction>(GetDefaultJurisdictions());

            if (IsDevelopmentEnvironment)
                jurisdictions.AddRange(GetDevJurisdictions());

            if (IsTestEnvironment || IsStagingEnvironment)
                jurisdictions.AddRange(GetTestJurisdictions());

            if (IsProductionEnvironment)
                jurisdictions.AddRange(GetProdJurisdictions());

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
