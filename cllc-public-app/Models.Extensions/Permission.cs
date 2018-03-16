using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Permission Database Model Extension
    /// </summary>
    public sealed partial class Permission
    {
        /// <summary>
        /// Login (UI) Permission
        /// </summary>
        public const string Login = "Login";

        /// <summary>
        /// User Management Permission
        /// </summary>
        public const string UserManagement = "UserManagement";

        /// <summary>
        /// Roles and Permissions Permission
        /// </summary>
        public const string RolesAndPermissions = "RolesAndPermissions";

        /// <summary>
        /// Admin Perission
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Import Legacy Data Permission Permission
        /// </summary>
        public const string ImportData = "ImportData";

        /// <summary>
        /// Code Table Management Permission
        /// </summary>
        public const string CodeTableManagement = "CodeTableManagement";

        /// <summary>
        /// District Code Table Management Permission
        /// </summary>
        public const string DistrictCodeTableManagement = "DistrictCodeTableManagement";

        /// <summary>
        /// Business Login Permission
        /// </summary>
        public const string BusinessLogin = "BusinessLogin";

        /// <summary>
        /// All Permissions List
        /// </summary>
        public static readonly IEnumerable<Permission> AllPermissions = new List<Permission>
        {
            new Permission
            {
                Code = Login,
                Name = "Login",
                Description = "Permission to login to the application and perform all Clerk functions within their designated District"
            },
            new Permission
            {
                Code = UserManagement,
                Name = "User Management",
                Description = "Gives the user access to the User Management screens"
            },
            new Permission
            {
                Code = RolesAndPermissions,
                Name = "Roles and Permissions",
                Description = "Gives the user access to the Roles and Permissions screens"
            },
            new Permission
            {
                Code = Admin,
                Name = "Admin",
                Description = "Allows the user to perform special administrative tasks"
            },
            new Permission
            {
                Code = ImportData,
                Name = "Import Data",
                Description = "Enables the user to import data from the legacy system"
            },
            new Permission
            {
                Code = CodeTableManagement,
                Name = "Code Table Management",
                Description = "Gives the user access to the Code Table Management screens"
            },
            new Permission
            {
                Code = DistrictCodeTableManagement,
                Name = "District Code Table Management",
                Description = "Gives the user access to the District Code Table Management screens"
            },
            new Permission
            {
                Code = BusinessLogin,
                Name = "Business Login",
                Description = "Permission to login to the business or owner facing application"
            }
        };
    }
}
