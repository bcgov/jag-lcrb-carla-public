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
    public class NewsletterSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public NewsletterSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient) 
            : base(configuration, env, loggerFactory, dynamicsClient)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdateNewsletters(context);
            
        }

        private void UpdateNewsletters(AppDbContext context)
        {
            List<Newsletter> seedNewsletters = GetSeedNewsletters();

            foreach (Newsletter Newsletter in seedNewsletters)
            {
                context.UpdateSeedNewsletterInfo(Newsletter);                
            }

            AddInitialNewsletters(context);            
        }

        private void AddInitialNewsletters(AppDbContext context)
        {
            string NewsletterInitializationFile = Configuration["NewsletterInitializationFile"];
            if (string.IsNullOrEmpty(NewsletterInitializationFile))
            {
                // default to sample data, which is stored in the "SeedData" directory.
                NewsletterInitializationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData" + Path.DirectorySeparatorChar + "Newsletters.json"); 
            }
            context.AddInitialNewslettersFromFile(NewsletterInitializationFile);
        }

        private List<Newsletter> GetSeedNewsletters()
        {
            List<Newsletter> Newsletters = new List<Newsletter>(GetDefaultNewsletters());

            if (IsProductionEnvironment)
            {
                Newsletters.AddRange(GetProdNewsletters());
            }
            else
            {
                Newsletters.AddRange(GetDevNewsletters());
            }
                

            return Newsletters;
        }

        /// <summary>
        /// Returns a list of users to be populated in all environments.
        /// </summary>
        private List<Newsletter> GetDefaultNewsletters()
        {
            return new List<Newsletter>();
        }

        /// <summary>
        /// Returns a list of Newsletters to be populated in the Development environment.
        /// </summary>
        private List<Newsletter> GetDevNewsletters()
        {
            return new List<Newsletter>();            
        }

        /// <summary>
        /// Returns a list of Newsletters to be populated in the Test environment.
        /// </summary>
        private List<Newsletter> GetTestNewsletters()
        {
            return new List<Newsletter>();
        }

        /// <summary>
        /// Returns a list of Newsletters to be populated in the Production environment.
        /// </summary>
        private List<Newsletter> GetProdNewsletters()
        {
            return new List<Newsletter>();
        }
    }
}
