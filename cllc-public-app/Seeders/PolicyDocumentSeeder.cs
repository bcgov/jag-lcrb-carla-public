using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Public.Models;
using System;
using System.IO;
using Gov.Lclb.Cllb.Public.Contexts;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class PolicyDocumentSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public PolicyDocumentSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory) 
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdatePolicyDocuments(context);
            
        }

        private void UpdatePolicyDocuments(AppDbContext context)
        {
            List<PolicyDocument> seedPolicyDocuments = GetSeedPolicyDocuments();

            foreach (PolicyDocument PolicyDocument in seedPolicyDocuments)
            {
                context.UpdateSeedPolicyDocumentInfo(PolicyDocument);                
            }

            AddInitialPolicyDocuments(context);            
        }

        private void AddInitialPolicyDocuments(AppDbContext context)
        {
            string PolicyDocumentInitializationFile = Configuration["PolicyDocumentInitializationFile"];
            if (string.IsNullOrEmpty(PolicyDocumentInitializationFile))
            {
                // default to sample data, which is stored in the "SeedData" directory.
                PolicyDocumentInitializationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData" + Path.DirectorySeparatorChar + "PolicyDocuments.json"); 
            }
            context.AddInitialPolicyDocumentsFromFile(PolicyDocumentInitializationFile);
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
