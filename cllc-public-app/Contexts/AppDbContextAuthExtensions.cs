using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class AppDbContextAuthExtensions
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

        /// <summary>
        /// Returns a user based on the guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static User GetUserByGuid(this AppDbContext context, string guid)
        {
            User user = context.Users.Where(x => x.Guid != null && x.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase))
                    .Include(u => u.UserRoles).ThenInclude(r => r.Role).ThenInclude(rp => rp.RolePermissions).ThenInclude(p => p.Permission)
                    .FirstOrDefault();

            return user;
        }

        /// <summary>
        /// Returns a user based on the account name
        /// </summary>
        /// <param name="context"></param>
        /// <param name="smUserId"></param>
        /// <returns></returns>
        public static User GetUserBySmUserId(this AppDbContext context, string smUserId)
        {
            User user = context.Users.Where(x => x.SmUserId != null && x.SmUserId.Equals(smUserId, StringComparison.OrdinalIgnoreCase))
                    .Include(u => u.UserRoles).ThenInclude(r => r.Role).ThenInclude(rp => rp.RolePermissions).ThenInclude(p => p.Permission)
                    .FirstOrDefault();

            return user;
        }

        public static void SaveUser(this AppDbContext context, User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        /// <summary>
        /// Load User from HETS database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static User LoadUser(this AppDbContext context, string userId, string guid = null)
        {
            User user = null;

            if (!string.IsNullOrEmpty(guid))
                user = context.GetUserByGuid(guid);

            if (user == null)
                user = context.GetUserBySmUserId(userId);

            if (user == null)
                return null;

            if (guid == null)
                return user;

            if (string.IsNullOrEmpty(user.Guid))
            {
                // self register (write the users Guid to thd db)
                user.Guid = guid;
                context.SaveUser(user);
            }
            else if (!user.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase))
            {
                // invalid account - guid doesn't match user credential
                return null;
            }

            return user;
        }

    }
}
