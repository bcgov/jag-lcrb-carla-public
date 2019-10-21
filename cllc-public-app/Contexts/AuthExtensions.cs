using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class AuthExtensions
    {

        /// <summary>
        /// Returns a role based on the role name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Role GetRole(this AppDbContext context, string name)
        {
            Role role = context.Roles.Where(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .Include(r => r.RolePermissions).ThenInclude(p => p.Permission)
                    .FirstOrDefault();

            return role;
        }

    }
}
