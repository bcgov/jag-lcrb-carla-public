using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Authorization
{
    /// <summary>
    /// Permission Requirements
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// All required permissions
        /// </summary>
        public IEnumerable<string> RequiredPermissions { get; }

        /// <summary>
        /// Set required permissions
        /// </summary>
        /// <param name="permissions"></param>
        public PermissionRequirement(params string[] permissions)
        {
            RequiredPermissions = permissions;
        }
    }
}
