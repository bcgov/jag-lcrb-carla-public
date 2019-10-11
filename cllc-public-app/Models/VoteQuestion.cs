using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// Role Database Model
    /// </summary>
    public sealed partial class VoteQuestion : IEquatable<VoteQuestion>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="VoteQuestion" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a VoteQuestion (required).</param>
        /// <param name="question">The text of the voting question.</param>
        /// <param name="slug">A string that can be used to identify the question</param>
        public VoteQuestion(Guid id, string question, string slug, string title, ViewModels.VoteOption[] options)
        {
            Id = id;
            Question = question;
            Slug = slug;
            Title = title;

            AddOptions(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoteQuestion" /> class.
        /// </summary>
        /// <param name="id">A system-generated unique identifier for a VoteQuestion (required).</param>
        /// <param name="question">The text of the voting question.</param>
        /// <param name="slug">A string that can be used to identify the question</param>
        public VoteQuestion(string question, string slug, string title, ViewModels.VoteOption[] options)
        {
            Question = question;
            Slug = slug;
            Title = title;
            AddOptions(options);
        }


        public VoteQuestion()
        {

        }

        private void AddOptions(ViewModels.VoteOption[] options)
        {
            if (Options == null)
            {
                Options = new List<VoteOption>();
            }
            foreach (ViewModels.VoteOption option in options)
            {
                VoteOption voteOption = new VoteOption();
                voteOption.Option = option.option;
                voteOption.TotalVotes = option.totalVotes;
                voteOption.DisplayOrder = option.displayOrder;
                Options.Add(voteOption);
            }
        }

        /// <summary>
        /// A system-generated unique identifier for a Role
        /// </summary>
        /// <value>A system-generated unique identifier for a Role</value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(512)]
        public string Question { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }

        // string used to query the database to get a given question data.
        public string Slug { get; set; }

        public List<VoteOption> Options { get; set; }

        /// <summary>
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("class VoteQuestion {\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("  Question: ").Append(Question).Append("\n");
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
            return obj.GetType() == GetType() && Equals((VoteQuestion)obj);
        }

        /// <summary>
        /// Returns true if Role instances are equal
        /// </summary>
        /// <param name="other">Instance of Role to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(VoteQuestion other)
        {
            if (other is null) { return false; }
            if (ReferenceEquals(this, other)) { return true; }

            return
                (
                    Id == other.Id ||
                    Id.Equals(other.Id)
                ) &&
                (
                    Question == other.Question ||
                    Question != null &&
                    Question.Equals(other.Question)
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

                if (Question != null)
                {
                    hash = hash * 59 + Question.GetHashCode();
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
        public static bool operator ==(VoteQuestion left, VoteQuestion right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not Equals
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(VoteQuestion left, VoteQuestion right)
        {
            return !Equals(left, right);
        }

        #endregion Operators
    }
}
