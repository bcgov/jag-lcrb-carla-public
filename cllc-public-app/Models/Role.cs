using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Role Database Model
    /// </summary>
    public sealed partial class Role : IEquatable<Role>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Role" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a Role (required).</param>
        /// <param name="name">The name of the Role, as established by the user creating the role. (required).</param>
        /// <param name="description">A description of the role as set by the user creating&amp;#x2F;updating the role. (required).</param>
        /// <param name="rolePermissions">RolePermissions.</param>
        /// <param name="userRoles">UserRoles.</param>
        public Role(Guid id, string name, string description, List<RolePermission> rolePermissions = null, List<UserRole> userRoles = null)
        {
            Id = id;
            Name = name;
            Description = description;
            RolePermissions = rolePermissions;
            UserRoles = userRoles;
        }

        public Role()
        { }

        /// <summary>
        /// A system-generated unique identifier for a Role
        /// </summary>
        /// <value>A system-generated unique identifier for a Role</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the Role, as established by the user creating the role.
        /// </summary>
        /// <value>The name of the Role, as established by the user creating the role.</value>
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// A description of the role as set by the user creating&#x2F;updating the role.
        /// </summary>
        /// <value>A description of the role as set by the user creating&#x2F;updating the role.</value>
        [MaxLength(2048)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or Sets RolePermissions
        /// </summary>
        public List<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Gets or Sets UserRoles
        /// </summary>
        public List<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class Role {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
            sb.Append("  RolePermissions: ").Append(RolePermissions).Append("\n");
            sb.Append("  UserRoles: ").Append(UserRoles).Append("\n");
            sb.Append("}\n");

            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Role)obj);
        }

        /// <summary>
        /// Returns true if Role instances are equal
        /// </summary>
        /// <param name="other">Instance of Role to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Role other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    Name == other.Name ||
                    Name != null &&
                    Name.Equals(other.Name)
                ) &&
                (
                    Description == other.Description ||
                    Description != null &&
                    Description.Equals(other.Description)
                ) &&
                (
                    RolePermissions == other.RolePermissions ||
                    RolePermissions != null &&
                    RolePermissions.SequenceEqual(other.RolePermissions)
                ) &&
                (
                    UserRoles == other.UserRoles ||
                    UserRoles != null &&
                    UserRoles.SequenceEqual(other.UserRoles)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;

                // Suitable nullity checks                                   
                hash = hash * 59 + Id.GetHashCode();

                if (Name != null)
                {
                    hash = hash * 59 + Name.GetHashCode();
                }

                if (Description != null)
                {
                    hash = hash * 59 + Description.GetHashCode();
                }

                if (RolePermissions != null)
                {
                    hash = hash * 59 + RolePermissions.GetHashCode();
                }

                if (UserRoles != null)
                {
                    hash = hash * 59 + UserRoles.GetHashCode();
                }

                return hash;
            }
        }

        #region Operators

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Role left, Role right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Role left, Role right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
