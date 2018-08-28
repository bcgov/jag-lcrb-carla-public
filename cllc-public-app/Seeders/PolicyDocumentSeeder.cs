using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.Models;
using System;
using System.IO;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class PolicyDocumentSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public PolicyDocumentSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient) 
            : base(configuration, env, loggerFactory, dynamicsClient)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdatePolicyDocuments(_dynamicsClient);
            
        }

        private void UpdatePolicyDocuments(IDynamicsClient dynamicsClient)
        {
            AddInitialPolicyDocuments(dynamicsClient);            
        }

        private void AddInitialPolicyDocuments(IDynamicsClient dynamicsClient)
        {
            string PolicyDocumentInitializationFile = Configuration["PolicyDocumentInitializationFile"];
            if (string.IsNullOrEmpty(PolicyDocumentInitializationFile))
            {
                // default to sample data, which is stored in the "SeedData" directory.
                PolicyDocumentInitializationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData" + Path.DirectorySeparatorChar + "PolicyDocuments.json"); 
            }
            bool forceupdate = false;
            if (! string.IsNullOrEmpty(Configuration["FORCE_POLICY_UPDATE"]))
            {
                forceupdate = true;
            }
            dynamicsClient.AddInitialPolicyDocumentsFromFile(PolicyDocumentInitializationFile, forceupdate);
        }

        private List<PolicyDocument> GetSeedPolicyDocuments()
        {
            List<PolicyDocument> PolicyDocuments = new List<PolicyDocument>(GetDefaultPolicyDocuments());            

            if (IsProductionEnvironment)
            {
                PolicyDocuments.AddRange(GetProdPolicyDocuments());
            }
            else
            {
                PolicyDocuments.AddRange(GetDevPolicyDocuments());
            }                

            return PolicyDocuments;
        }

        /// <summary>
        /// Returns a list of users to be populated in all environments.
        /// </summary>
        private List<PolicyDocument> GetDefaultPolicyDocuments()
        {
            return new List<PolicyDocument>();
        }

        /// <summary>
        /// Returns a list of PolicyDocuments to be populated in the Development environment.
        /// </summary>
        private List<PolicyDocument> GetDevPolicyDocuments()
        {
            return new List<PolicyDocument>();            
        }

        /// <summary>
        /// Returns a list of PolicyDocuments to be populated in the Test environment.
        /// </summary>
        private List<PolicyDocument> GetTestPolicyDocuments()
        {
            return new List<PolicyDocument>();
        }

        /// <summary>
        /// Returns a list of PolicyDocuments to be populated in the Production environment.
        /// </summary>
        private List<PolicyDocument> GetProdPolicyDocuments()
        {
            return new List<PolicyDocument>();
        }
    }
}
