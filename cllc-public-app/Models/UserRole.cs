using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// User Role Database Model
    /// </summary>
    public sealed class UserRole : IEquatable<UserRole>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="UserRole" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a UserRole (required).</param>
        /// <param name="effectiveDate">The date on which the user was given the related role. (required).</param>
        /// <param name="expiryDate">The date on which a role previously assigned to a user was removed from that user..</param>
        /// <param name="role">A foreign key reference to the system-generated unique identifier for a Role.</param>
        public UserRole(Guid id, DateTime effectiveDate, DateTime? expiryDate = null, Role role = null)
        {
            Id = id;
            EffectiveDate = effectiveDate;
            ExpiryDate = expiryDate;
            Role = role;
        }

        public UserRole()
        {

        }

        /// <summary>
        /// A system-generated unique identifier for a UserRole
        /// </summary>
        /// <value>A system-generated unique identifier for a UserRole</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// The date on which the user was given the related role.
        /// </summary>
        /// <value>The date on which the user was given the related role.</value>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// The date on which a role previously assigned to a user was removed from that user.
        /// </summary>
        /// <value>The date on which a role previously assigned to a user was removed from that user.</value>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// A foreign key reference to the system-generated unique identifier for a Role
        /// </summary>
        /// <value>A foreign key reference to the system-generated unique identifier for a Role</value>
        public Role Role { get; set; }

        /// <summary>
        /// Foreign key for Role 
        /// </summary>   
        [ForeignKey("Role")]
        [JsonIgnore]
        public Guid? RoleId { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class UserRole {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  EffectiveDate: ").Append(EffectiveDate).Append("\n");
            sb.Append("  ExpiryDate: ").Append(ExpiryDate).Append("\n");
            sb.Append("  Role: ").Append(Role).Append("\n");
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
            return obj.GetType() == GetType() && Equals((UserRole)obj);
        }

        /// <summary>
        /// Returns true if UserRole instances are equal
        /// </summary>
        /// <param name="other">Instance of UserRole to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UserRole other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    EffectiveDate == other.EffectiveDate ||
                    EffectiveDate.Equals(other.EffectiveDate)
                ) &&
                (
                    ExpiryDate == other.ExpiryDate ||
                    ExpiryDate != null &&
                    ExpiryDate.Equals(other.ExpiryDate)
                ) &&
                (
                    Role == other.Role ||
                    Role != null &&
                    Role.Equals(other.Role)
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
                hash = hash * 59 + EffectiveDate.GetHashCode();

                if (ExpiryDate != null)
                {
                    hash = hash * 59 + ExpiryDate.GetHashCode();
                }

                if (Role != null)
                {
                    hash = hash * 59 + Role.GetHashCode();
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
        public static bool operator ==(UserRole left, UserRole right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(UserRole left, UserRole right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
