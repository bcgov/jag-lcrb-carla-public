using Gov.Lclb.Cllb.Public.Models;
using System;
using System.Security.Claims;

namespace Gov.Lclb.Cllb.Public.Authorization
{
    /// <summary>
    /// Calaims Principal Extension
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Check if the user has permission to execute the method
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public static bool HasPermissions(this ClaimsPrincipal user, params string[] permissions)
        {
            if (!user.HasClaim(c => c.Type == User.PermissionClaim))
                return false;

            bool hasRequiredPermissions = false;

            if (!user.HasClaim(c => c.Type == User.PermissionClaim))
                return false;

            if (user.HasClaim(c => c.Type == User.PermissionClaim))
            {
                bool hasPermissions = true;

                foreach (string permission in permissions)
                {
                    if (!user.HasClaim(User.PermissionClaim, permission))
                    {
                        hasPermissions = false;
                        break;
                    }
                }

                hasRequiredPermissions = hasPermissions;
            }

            return hasRequiredPermissions;
        }

        /// <summary>
        /// Check if the user is a member if the group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool IsInGroup(this ClaimsPrincipal user, string group)
        {
            return user.HasClaim(c => c.Type == ClaimTypes.GroupSid && c.Value.Equals(group, StringComparison.OrdinalIgnoreCase));
        }
    }
}
