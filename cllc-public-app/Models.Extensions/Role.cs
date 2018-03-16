using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Role Database Model Extension
    /// </summary>
    public sealed partial class Role
    {
        /// <summary>
        /// Adds a permission to this role instance.
        /// </summary>
        /// <param name="permission">The permission to add.</param>
        public void AddPermission(Permission permission)
        {
            var rolePermission = new RolePermission
            {
                Permission = permission,
                Role = this
            };
            RolePermissions.Add(rolePermission);
        }

        /// <summary>
        /// Removes a permission from this role instance.
        /// </summary>
        /// <param name="permission">The permission to remove.</param>
        public void RemovePermission(Permission permission)
        {
            var permissionToRemove = RolePermissions.FirstOrDefault(x => x.Permission.Code == permission.Code);
            if (permissionToRemove != null)
            {
                RolePermissions.Remove(permissionToRemove);
            }
        }
    }
}
