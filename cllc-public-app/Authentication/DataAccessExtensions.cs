using System;
using Gov.Lclb.Cllb.Public.Models;

namespace Gov.Lclb.Cllb.Public.Authentication
{
    /// <summary>
    /// Db Extension - Validates User Credential against the HETS Database
    /// </summary>
    public static class DataAccessExtensions
    {
        /// <summary>
        /// Load User from HETS database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static User LoadUser(this DataAccess context, string userId, string guid = null)
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
