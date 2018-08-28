using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Gov.Lclb.Cllb.Public.Models;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Interfaces;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    internal class PermissionSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public PermissionSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDynamicsClient dynamicsClient) 
            : base(configuration, env, loggerFactory, dynamicsClient)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        protected override void Invoke(AppDbContext context)
        {
            UpdatePermissions(context);
            context.SaveChanges();

            Logger.LogDebug("Listing permissions ...");
            foreach (var p in context.Permissions.ToList())
            {
                Logger.LogDebug($"{p.Code}");
            }
        }

        private void UpdatePermissions(AppDbContext context)
        {
            var permissions = Permission.AllPermissions;

            Logger.LogDebug("Updating permissions ...");

            foreach (Permission permission in permissions)
            {
                Logger.LogDebug($"Looking up {permission.Code} ...");

                Permission p = context.Permissions.FirstOrDefault(x => x.Code == permission.Code);

                if (p == null)
                {
                    Logger.LogDebug($"{permission.Code} does not exist, adding it ...");
                    context.Permissions.Add(permission);
                }
                else
                {
                    Logger.LogDebug($"Updating the fields for {permission.Code} ...");
                    p.Description = permission.Description;
                    p.Name = permission.Name;
                }
            }            
        }
    }
}
