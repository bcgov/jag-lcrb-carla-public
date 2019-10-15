using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Role Database Model
    /// </summary>
    public sealed partial class VoteOption : IEquatable<VoteOption>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Role" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a Role (required).</param>
        /// <param name="name">The name of the Role, as established by the user creating the role. (required).</param>
        /// <param name="description">A description of the role as set by the user creating&amp;#x2F;updating the role. (required).</param>
        /// <param name="rolePermissions">RolePermissions.</param>
        /// <param name="userRoles">UserRoles.</param>
        public VoteOption(Guid id, string question, int totalVotes, int displayOrder)
        {
            Id = id;
            Option = question;
            TotalVotes = totalVotes;
            DisplayOrder = displayOrder;
        }

        public VoteOption()
        {

        }

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
        [MaxLength(512)]
        public string Option { get; set; }

        public int TotalVotes { get; set; }

        public int DisplayOrder { get; set; }


        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class VoteOption {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Option: ").Append(Option).Append("\n");
            sb.Append("  TotalVotes: ").Append(TotalVotes).Append("\n");
            sb.Append("  DisplayOrder: ").Append(DisplayOrder).Append("\n");
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
            return obj.GetType() == GetType() && Equals((VoteOption)obj);
        }

        /// <summary>
        /// Returns true if Role instances are equal
        /// </summary>
        /// <param name="other">Instance of Role to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(VoteOption other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    Option == other.Option ||
                    Option != null &&
                    Option.Equals(other.Option)
                ) &&
                (
                    TotalVotes == other.TotalVotes ||
                    TotalVotes.Equals(other.TotalVotes)
                ) &&
                (
                    DisplayOrder == other.DisplayOrder ||
                    DisplayOrder.Equals(other.DisplayOrder)
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

                if (Option != null)
                {
                    hash = hash * 59 + Option.GetHashCode();
                }

                // TotalVotes is never null
                hash = hash * 59 + TotalVotes.GetHashCode();

                // DisplayOrder is never null, so no null check.
                hash = hash * 59 + DisplayOrder.GetHashCode();

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
        public static bool operator ==(VoteOption left, VoteOption right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(VoteOption left, VoteOption right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
