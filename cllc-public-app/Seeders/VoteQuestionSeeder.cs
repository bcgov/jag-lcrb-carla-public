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
    public class VoteQuestionSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public VoteQuestionSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache distributedCache) 
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdateVoteQuestions(context);
            
        }

        private void UpdateVoteQuestions(AppDbContext context)
        {
            List<VoteQuestion> seedVoteQuestions = GetSeedVoteQuestions();

            foreach (VoteQuestion voteQuestion in seedVoteQuestions)
            {
                context.UpdateSeedVoteQuestionInfo(voteQuestion.ToViewModel());                
            }

            AddInitialVoteQuestions(context);            
        }

        private void AddInitialVoteQuestions(AppDbContext context)
        {
            string VoteQuestionInitializationFile = Configuration["VoteQuestionInitializationFile"];
            if (string.IsNullOrEmpty(VoteQuestionInitializationFile))
            {
                // default to sample data, which is stored in the "SeedData" directory.
                VoteQuestionInitializationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SeedData" + Path.DirectorySeparatorChar + "VoteQuestions.json"); 
            }
            context.AddInitialVoteQuestionsFromFile(VoteQuestionInitializationFile);
        }

        private List<VoteQuestion> GetSeedVoteQuestions()
        {
            List<VoteQuestion> VoteQuestions = new List<VoteQuestion>(GetDefaultVoteQuestions());

            if (IsProductionEnvironment)
            {
                VoteQuestions.AddRange(GetProdVoteQuestions());
            }
            else
            {
                VoteQuestions.AddRange(GetDevVoteQuestions());
            }
                

            return VoteQuestions;
        }

        /// <summary>
        /// Returns a list of users to be populated in all environments.
        /// </summary>
        private List<VoteQuestion> GetDefaultVoteQuestions()
        {
            return new List<VoteQuestion>();
        }

        /// <summary>
        /// Returns a list of VoteQuestions to be populated in the Development environment.
        /// </summary>
        private List<VoteQuestion> GetDevVoteQuestions()
        {
            return new List<VoteQuestion>();            
        }

        /// <summary>
        /// Returns a list of VoteQuestions to be populated in the Test environment.
        /// </summary>
        private List<VoteQuestion> GetTestVoteQuestions()
        {
            return new List<VoteQuestion>();
        }

        /// <summary>
        /// Returns a list of VoteQuestions to be populated in the Production environment.
        /// </summary>
        private List<VoteQuestion> GetProdVoteQuestions()
        {
            return new List<VoteQuestion>();
        }
    }
}
