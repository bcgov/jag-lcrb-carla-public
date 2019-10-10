using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Policy Database Model
    /// </summary>
    public sealed partial class PolicyDocument : IEquatable<PolicyDocument>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Policy" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a VoteQuestion (required).</param>
        /// <param name="question">The text of the voting question.</param>
        /// <param name="slug">A string that can be used to identify the question</param>
        public PolicyDocument(Guid id, string slug, string title, string menuText, string category, string body, int displayOrder)
        {
            Id = id;
            Slug = slug;
            Title = title;
            MenuText = menuText;
            Category = category;
            Body = body;
            DisplayOrder = displayOrder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoteQuestion" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a VoteQuestion (required).</param>
        /// <param name="question">The text of the voting question.</param>
        /// <param name="slug">A string that can be used to identify the question</param>
        public PolicyDocument(string slug, string title, string menuText, string category, string body, int displayOrder)
        {
            Slug = slug;
            Title = title;
            MenuText = menuText;
            Category = category;
            Body = body;
            DisplayOrder = displayOrder;
        }


        public PolicyDocument()
        {

        }


        /// <summary>
        /// A system-generated unique identifier for a Newsletter
        /// </summary>
        /// <value>A system-generated unique identifier for a Role</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string MenuText { get; set; }

        // category is used to build the side nav.
        public string Category { get; set; }

        public string Body { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        // string used to query the database to get a given question data.
        public string Slug { get; set; }

        public int DisplayOrder { get; set; }

        /// <summary>
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class PolicyDocument {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Title: ").Append(Title).Append("\n");
            sb.Append("  Description: ").Append(Title).Append("\n");
            sb.Append("  Slug: ").Append(Slug).Append("\n");
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
            return obj.GetType() == GetType() && Equals((Newsletter)obj);
        }

        /// <summary>
        /// Returns true if Role instances are equal
        /// </summary>
        /// <param name="other">Instance of Role to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(PolicyDocument other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    Title == other.Title ||
                    Title != null &&
                    Title.Equals(other.Title)
                ) &&
                (
                    Slug == other.Slug ||
                    Slug != null &&
                    Slug.Equals(other.Slug)
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

                if (Title != null)
                {
                    hash = hash * 59 + Title.GetHashCode();
                }

                if (Slug != null)
                {
                    hash = hash * 59 + Slug.GetHashCode();
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
        public static bool operator ==(PolicyDocument left, PolicyDocument right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PolicyDocument left, PolicyDocument right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
