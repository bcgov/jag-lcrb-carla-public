
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class UserExtensions
    {

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userInfo"></param>
        public static void UpdateSeedUserInfo(this AppDbContext context, User userInfo)
        {
            User user = context.GetUserByGuid(userInfo.Guid);

            if (user == null)
            {
                context.Users.Add(userInfo);
            }
            else
            {
                user.Active = userInfo.Active;
                user.Email = userInfo.Email;
                user.GivenName = userInfo.GivenName;
                user.Initials = userInfo.Initials;
                user.SmAuthorizationDirectory = userInfo.SmAuthorizationDirectory;
                user.SmUserId = userInfo.SmUserId;
                user.Surname = userInfo.Surname;

                // Sync Roles
                if (user.UserRoles == null)
                {
                    user.UserRoles = new List<UserRole>();
                }

                foreach (UserRole item in user.UserRoles)
                {
                    context.Entry(item).State = EntityState.Deleted;
                }

                foreach (UserRole item in userInfo.UserRoles)
                {
                    user.UserRoles.Add(item);
                }                

            }
        }
    }
}
