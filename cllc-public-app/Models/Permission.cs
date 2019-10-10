using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Permission Database Model
    /// </summary>
    public sealed partial class Permission : IEquatable<Permission>
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="Permission" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a Permission (required).</param>
        /// <param name="code">The name of the permission referenced in the software of the application. (required).</param>
        /// <param name="name">The &amp;#39;user friendly&amp;#39; name of the permission exposed to the user selecting the permissions to be included in a Role. (required).</param>
        /// <param name="description">A description of the purpose of the permission and exposed to the user selecting the permissions to be included in a Role..</param>
        public Permission(Guid id, string code, string name, string description = null)
        {
            Id = id;
            Code = code;
            Name = name;
            Description = description;
        }

        public Permission(string code, string name, string description = null)
        {
            Code = code;
            Name = name;
            Description = description;
        }

        public Permission()
        {
        }

        /// <summary>
        /// A system-generated unique identifier for a Permission
        /// </summary>
        /// <value>A system-generated unique identifier for a Permission</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the permission referenced in the software of the application.
        /// </summary>
        /// <value>The name of the permission referenced in the software of the application.</value>
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// The &#39;user friendly&#39; name of the permission exposed to the user selecting the permissions to be included in a Role.
        /// </summary>
        /// <value>The &#39;user friendly&#39; name of the permission exposed to the user selecting the permissions to be included in a Role.</value>
        [MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// A description of the purpose of the permission and exposed to the user selecting the permissions to be included in a Role.
        /// </summary>
        /// <value>A description of the purpose of the permission and exposed to the user selecting the permissions to be included in a Role.</value>
        [MaxLength(2048)]
        public string Description { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class Permission {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Code: ").Append(Code).Append("\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Description: ").Append(Description).Append("\n");
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
            return obj.GetType() == GetType() && Equals((Permission)obj);
        }

        /// <summary>
        /// Returns true if Permission instances are equal
        /// </summary>
        /// <param name="other">Instance of Permission to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Permission other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    Code == other.Code ||
                    Code != null &&
                    Code.Equals(other.Code)
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

                if (Code != null)
                {
                    hash = hash * 59 + Code.GetHashCode();
                }

                if (Name != null)
                {
                    hash = hash * 59 + Name.GetHashCode();
                }

                if (Description != null)
                {
                    hash = hash * 59 + Description.GetHashCode();
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
        public static bool operator ==(Permission left, Permission right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Permission left, Permission right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
