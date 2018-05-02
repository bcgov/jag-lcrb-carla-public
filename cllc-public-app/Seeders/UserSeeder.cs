using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class UserSeeder : Seeder<AppDbContext>
    {
        private string[] ProfileTriggers = { AllProfiles };

        public UserSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache)
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles
        {
            get { return ProfileTriggers; }
        }
        

        protected override void Invoke(AppDbContext context)
        {
            UpdateUsers(context);
        }

        private void UpdateUsers(AppDbContext context)
        {
            List<User> seedUsers = GetSeedUsers(context);
            foreach (var user in seedUsers)
            {
                context.UpdateSeedUserInfo(user);
                context.SaveChanges();
            }

            AddInitialUsers(context);            
        }

        private void AddInitialUsers(AppDbContext context)
        {
            
        }

        private List<User> GetSeedUsers(AppDbContext context)
        {
            List<User> users = new List<User>(GetDefaultUsers(context));
            if (IsDevelopmentEnvironment)
                users.AddRange(GetDevUsers(context));
            if (IsTestEnvironment || IsStagingEnvironment)
                users.AddRange(GetTestUsers(context));
            if (IsProductionEnvironment)
                users.AddRange(GetProdUsers(context));

            return users;
        }

        /// <summary>
        /// Returns a list of users to be populated in all environments.
        /// </summary>
        private List<User> GetDefaultUsers(AppDbContext context)
        {
            return new List<User>();
        }

        /// <summary>
        /// Returns a list of users to be populated in the Development environment.
        /// </summary>
        private List<User> GetDevUsers(AppDbContext context)
        {
            return new List<User>
            {
                new User
                {
                    Active = true,
                    Email = "Testy.McTesterson@TestDomain.test",
                    GivenName = "Testy",
                    Guid = "2cbf7cb8d6b445f087fb82ad75566a9c",
                    Initials = "TT",
                    SmAuthorizationDirectory = "TEST",
                    SmUserId = "TMcTesterson",
                    Surname = "McTesterson",
                    UserRoles = new List<UserRole>
                    {
                        new UserRole
                        {
                            EffectiveDate = DateTime.UtcNow,
                            Role = context.GetRole("User")
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Returns a list of users to be populated in the Test environment.
        /// </summary>
        private List<User> GetTestUsers(AppDbContext context)
        {
            return new List<User>();
        }

        /// <summary>
        /// Returns a list of users to be populated in the Production environment.
        /// </summary>
        private List<User> GetProdUsers(AppDbContext context)
        {
            return new List<User>();
        }
    }
}
