using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum ContentCategory
    {
        Preamble = 845280000,
        BeforeStarting = 845280001,
        NextSteps = 845280002,
    }
    public class ApplicationTypeContent
    {
        public string Id;
        public string Body { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ContentCategory Category { get; set; }

        public bool? Iscoop { get; set; }
        public bool? IsEstate { get; set; }
        public bool? IsGeneralPartnership { get; set; }
        public bool? IsIndigenousNation { get; set; }
        public bool? IsLimitedliabilityCorporation { get; set; }
        public bool? IsLimitedliabilityPartnership { get; set; }
        public bool? IsLimitedPartnership { get; set; }
        public bool? IsLocalGovernment { get; set; }
        public bool? IsPartnership { get; set; }
        public bool? IsPrivateCorporation { get; set; }
        public bool? IsPublicCorporation { get; set; }
        public bool? IsSociety { get; set; }
        public bool? IsSoleProprietorship { get; set; }
        public bool? IsTrust { get; set; }
        public bool? IsUniversity { get; set; }
        public bool? IsUnlimitedLiabilityCorporation { get; set; }

        public List<string> BusinessTypes { get
            {
                var businessTypes = new List<string>();


                if (Iscoop == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.Coop.ToString());
                }
                if (IsEstate == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.Estate.ToString());
                }
                if (IsGeneralPartnership == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.GeneralPartnership.ToString());
                }
                if (IsIndigenousNation == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.IndigenousNation.ToString());
                }
                if (IsLimitedliabilityCorporation == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.LimitedLiabilityCorporation.ToString());
                }
                if (IsLimitedliabilityPartnership == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.LimitedLiabilityPartnership.ToString());
                }
                if (IsLimitedPartnership == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.LimitedPartnership.ToString());
                }
                if (IsLocalGovernment == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.LocalGovernment.ToString());
                }
                if (IsPartnership == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.Partnership.ToString());
                }
                if (IsPrivateCorporation == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.PrivateCorporation.ToString());
                }
                if (IsPublicCorporation == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.PublicCorporation.ToString());
                }
                if (IsSociety == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.Society.ToString());
                }
                if (IsSoleProprietorship == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.SoleProprietor.ToString());
                }
                if (IsTrust == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.Trust.ToString());
                }
                if (IsUniversity == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.University.ToString());
                }
                if (IsUnlimitedLiabilityCorporation == true)
                {
                    businessTypes.Add(AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation.ToString());
                }

                return businessTypes;
            } }
    }
}
