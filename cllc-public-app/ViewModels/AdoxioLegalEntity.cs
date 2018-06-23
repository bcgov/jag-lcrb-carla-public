using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum Adoxio_applicanttypecodes
    {
        [Display(Name = "Private Corporation")]
        PrivateCorporation = 845280000,
        Partnership,
        [Display(Name = "Sole Proprietor")]
        SoleProprietor,
        [Display(Name = "Public Corporation")]
        PublicCorporation,
        Society,
        Other
    }

    public enum PositionOptions
    {
        Partner,
        Shareholder,
        Trustee,
        Director,
        Officer,
        Owner
    }

    public class AdoxioLegalEntity
    {
        // string form of the guid.
        public string id { get; set; }
        public string name { get; set; }
        public bool? isindividual { get; set; }
        public bool? sameasapplyingperson { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Adoxio_applicanttypecodes legalentitytype { get; set; }
        public string otherlegalentitytype { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PositionOptions position;
        public DateTimeOffset? dateofbirth { get; set; }
        public decimal? interestpercentage { get; set; }
        public int? commonvotingshares { get; set; }
        public int? preferredvotingshares { get; set; }
        public int? commonnonvotingshares { get; set; }
        public int? preferrednonvotingshares { get; set; }
        public Account account { get; set; }
        List<AdoxioLegalEntity> relatedentities { get; set; }
        public string email { get; set; }
        public DateTimeOffset? dateofappointment { get; set; }
    }
}
