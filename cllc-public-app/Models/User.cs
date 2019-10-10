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
    /// User Database Model
    /// </summary>
    public partial class User : IEquatable<User>
    {
        /// <summary>
        /// User Database Model Constructor (required by entity framework)
        /// </summary>


        /// <summary>
        /// Initializes a new instance of the <see cref="User" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a User (required).</param>
        /// <param name="givenName">Given name of the user. (required).</param>
        /// <param name="surname">Surname of the user. (required).</param>
        /// <param name="active">A flag indicating the User is active in the system. Set false to remove access to the system for the user. (required).</param>
        /// <param name="initials">Initials of the user, to be presented where screen space is at a premium..</param>
        /// <param name="email">The email address of the user in the system..</param>
        /// <param name="smUserId">Security Manager User ID.</param>
        /// <param name="guid">The GUID unique to the user as provided by the authentication system. In this case, authentication is done by Siteminder and the GUID uniquely identifies the user within the user directories managed by Siteminder - e.g. IDIR and BCeID. The GUID is equivalent to the IDIR Id, but is guaranteed unique to a person, while the IDIR ID is not - IDIR IDs can be recycled..</param>
        /// <param name="userType">The user directory service used by Siteminder to authenticate the user - usually IDIR or BCeID..</param>
        /// <param name="userRoles">UserRoles.</param>
        /// <param name="district">The District that the User belongs to.</param>
        public User(Guid id, string givenName, string surname, bool active, string initials = null, string email = null,
            string smUserId = null, string accountId = null, string userType = null, List<UserRole> userRoles = null)
        {
            ContactId = id;
            GivenName = givenName;
            Surname = surname;
            Active = active;
            Initials = initials;
            Email = email;
            SmUserId = smUserId;
            if (accountId != null)
            {
                AccountId = Guid.Parse(accountId);
            }
            UserType = userType;
            UserRoles = userRoles;
        }

        public User()
        {

        }

        /// <summary>
        /// A system-generated unique identifier for a User
        /// </summary>
        /// <value>A system-generated unique identifier for a User</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ContactId { get; set; }

        /// <summary>
        /// Given name of the user.
        /// </summary>
        /// <value>Given name of the user.</value>
		[MaxLength(50)]
        public string GivenName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        /// <value>Surname of the user.</value>
        [MaxLength(50)]
        public string Surname { get; set; }

        /// <summary>
        /// A flag indicating the User is active in the system. Set false to remove access to the system for the user.
        /// </summary>
        /// <value>A flag indicating the User is active in the system. Set false to remove access to the system for the user.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Initials of the user, to be presented where screen space is at a premium.
        /// </summary>
        /// <value>Initials of the user, to be presented where screen space is at a premium.</value>
        [MaxLength(10)]
        public string Initials { get; set; }

        /// <summary>
        /// The email address of the user in the system.
        /// </summary>
        /// <value>The email address of the user in the system.</value>
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Security Manager User ID
        /// </summary>
        /// <value>Security Manager User ID</value>
        [MaxLength(255)]
        public string SmUserId { get; set; }

        /// <summary>
        /// The GUID for the Dynamics Account of the user
        /// </summary>
        /// <value></value>
        public Guid AccountId { get; set; }

        /// <summary>
        /// The siteminder user guid
        /// </summary>
        [MaxLength(255)]
        public string SiteMinderGuid { get; set; }

        /// <summary>
        /// The user directory service used by Siteminder to authenticate the user - usually IDIR or BCeID.
        /// </summary>
        /// <value>The user directory service used by Siteminder to authenticate the user - usually IDIR or BCeID.</value>
        [MaxLength(255)]
        public string UserType { get; set; }

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

            sb.Append("class User {\n");
            sb.Append("  Id: ").Append(ContactId).Append("\n");
            sb.Append("  GivenName: ").Append(GivenName).Append("\n");
            sb.Append("  Surname: ").Append(Surname).Append("\n");
            sb.Append("  Active: ").Append(Active).Append("\n");
            sb.Append("  Initials: ").Append(Initials).Append("\n");
            sb.Append("  Email: ").Append(Email).Append("\n");
            sb.Append("  SmUserId: ").Append(SmUserId).Append("\n");
            sb.Append("  Guid: ").Append(AccountId).Append("\n");
            sb.Append("  SmAuthorizationDirectory: ").Append(UserType).Append("\n");
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
            return obj.GetType() == GetType() && Equals((User)obj);
        }

        /// <summary>
        /// Returns true if User instances are equal
        /// </summary>
        /// <param name="other">Instance of User to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(User other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    ContactId == other.ContactId ||
                    ContactId.Equals(other.ContactId)
                ) &&
                (
                    GivenName == other.GivenName ||
                    GivenName != null &&
                    GivenName.Equals(other.GivenName)
                ) &&
                (
                    Surname == other.Surname ||
                    Surname != null &&
                    Surname.Equals(other.Surname)
                ) &&
                (
                    Active == other.Active ||
                    Active.Equals(other.Active)
                ) &&
                (
                    Initials == other.Initials ||
                    Initials != null &&
                    Initials.Equals(other.Initials)
                ) &&
                (
                    Email == other.Email ||
                    Email != null &&
                    Email.Equals(other.Email)
                ) &&
                (
                    SmUserId == other.SmUserId ||
                    SmUserId != null &&
                    SmUserId.Equals(other.SmUserId)
                ) &&
                (
                    AccountId == other.AccountId ||
                    AccountId != null &&
                    AccountId.Equals(other.AccountId)
                ) &&
                (
                    UserType == other.UserType ||
                    UserType != null &&
                    UserType.Equals(other.UserType)
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
                hash = hash * 59 + ContactId.GetHashCode();

                if (GivenName != null)
                {
                    hash = hash * 59 + GivenName.GetHashCode();
                }

                if (Surname != null)
                {
                    hash = hash * 59 + Surname.GetHashCode();
                }

                hash = hash * 59 + Active.GetHashCode();

                if (Initials != null)
                {
                    hash = hash * 59 + Initials.GetHashCode();
                }

                if (Email != null)
                {
                    hash = hash * 59 + Email.GetHashCode();
                }

                if (SmUserId != null)
                {
                    hash = hash * 59 + SmUserId.GetHashCode();
                }

                if (AccountId != null)
                {
                    hash = hash * 59 + AccountId.GetHashCode();
                }

                if (UserType != null)
                {
                    hash = hash * 59 + UserType.GetHashCode();
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
        public static bool operator ==(User left, User right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(User left, User right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
