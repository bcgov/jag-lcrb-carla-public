using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Gov.Lclb.Cllb.Public.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts;
using Microsoft.Extensions.Caching.Distributed;

namespace Gov.Lclb.Cllb.Public.Seeders
{
    public class RoleSeeder : Seeder<AppDbContext>
    {
        private readonly string[] _profileTriggers = { AllProfiles };

        public RoleSeeder(IConfiguration configuration, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache distributedCache) 
            : base(configuration, env, loggerFactory)
        { }

        protected override IEnumerable<string> TriggerProfiles => _profileTriggers;

        public override Type InvokeAfter => typeof(PermissionSeeder);

        protected override void Invoke(AppDbContext context)
        {
            UpdateRoles(context);
            context.SaveChanges();
        }

        private void UpdateRoles(AppDbContext context)
        {
            var permissions = context.Permissions.ToList();

            // Load the roles
            var roles = new List<Role>
            {
                new Role
                {
                    Name = "Unregistered user",
                    Description = "New user in process of creating an account.",
                    RolePermissions = permissions.Where(p =>
                        new[]
                            {
                                Permission.NewUserRegistration
                            }
                        .Contains(p.Code))
                        .Select(p => new RolePermission
                        {
                            Permission = p
                        })
                        .ToList()
                },
                new Role
                {
                    Name = "User",
                    Description = "Registered user",
                    RolePermissions = permissions.Where(p =>
                        new[]
                            {
                                Permission.Login
                            }
                        .Contains(p.Code))
                        .Select(p => new RolePermission
                        {
                            Permission = p
                        })
                        .ToList()
                },
                new Role
                {
                    Name = "Administrator",
                    Description = "System Administrator; full access to the whole system",
                    RolePermissions = permissions.Where(p =>
                            new[]
                                {
                                    Permission.Login,
                                    Permission.DistrictCodeTableManagement,
                                    Permission.CodeTableManagement,
                                    Permission.UserManagement,
                                    Permission.RolesAndPermissions,
                                    Permission.Admin                                    
                                }
                                .Contains(p.Code))
                        .Select(p => new RolePermission
                        {
                            Permission = p
                        })
                        .ToList()
                }
            };

            Logger.LogDebug("Updating roles ...");
            foreach (var role in roles)
            {
                var r = context.GetRole(role.Name);
                if (r == null)
                {
                    Logger.LogDebug($"Adding role; {role.Name} ...");
                    context.Roles.Add(role);
                }
                else
                {
                    Logger.LogDebug($"Updating role; {r.Name} ...");
                    r.Description = role.Description;
                }
            }
        }
    }
}
