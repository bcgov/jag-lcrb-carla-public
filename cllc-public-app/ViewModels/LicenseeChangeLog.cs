using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum LicenseeChangeType
    {
        unchanged = 0, // never save this kind of change to dynamics
        addLeadership = 845280003,
        updateLeadership = 845280004,
        removeLeadership = 845280005,
        addBusinessShareholder = 845280006,
        updateBusinessShareholder = 845280007,
        removeBusinessShareholder = 845280008,
        addIndividualShareholder = 845280009,
        updateIndividualShareholder = 845280010,
        removeIndividualShareholder = 845280011,
    }

    public class LicenseeChangeLog
    {
        public string Id { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LicenseeChangeType? ChangeType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes? BusinessType { get; set; }
        public bool? IsDirectorNew { get; set; }
        public bool? IsDirectorOld { get; set; }
        public bool? IsManagerNew { get; set; }
        public bool? IsManagerOld { get; set; }
        public bool? IsOfficerNew { get; set; }
        public bool? IsOfficerOld { get; set; }
        public bool? IsOwnerNew { get; set; }
        public bool? IsOwnerOld { get; set; }
        public bool? IsShareholderNew { get; set; }
        public bool? IsShareholderOld { get; set; }
        public bool? IsTrusteeNew { get; set; }
        public bool? IsTrusteeOld { get; set; }
        public bool? IsIndividual { get; set; }
        public int? NumberofSharesNew { get; set; }
        public int? NumberofSharesOld { get; set; }
        public int? NumberOfNonVotingSharesNew { get; set; }
        public int? NumberOfNonVotingSharesOld { get; set; }
        public int? TotalSharesNew { get; set; }
        public int? TotalSharesOld { get; set; }
        public int? Statecode { get; set; }
        public int? Statuscode { get; set; }
        public string EmailNew { get; set; }
        public string EmailOld { get; set; }
        public string FirstNameNew { get; set; }
        public string FirstNameOld { get; set; }
        public string JobNumber { get; set; }
        public string LastNameNew { get; set; }
        public string LastNameOld { get; set; }
        public string BusinessNameNew { get; set; }
        public string BusinessNameOld { get; set; }
        public string TitleNew { get; set; }
        public string TitleOld { get; set; }
        public System.DateTimeOffset? DateofBirthNew { get; set; }
        public System.DateTimeOffset? DateofBirthOld { get; set; }

        public Decimal? AnnualMembershipFee { get; set; }
        public int? NumberOfMembers { get; set; }

        public Account BusinessAccount { get; set; }
        public Contact Contact { get; set; }
        public Account ParentBusinessAccount { get; set; }
        public string ParentBusinessAccountId { get; set; }
        public string ApplicationId { get; set; }
        public string BusinessAccountId { get; set; }
        public string ParentLegalEntityId { get; set; }
        public string LegalEntityId { get; set; }
        public string ParentLicenseeChangeLogId { get; set; }
        public IList<LicenseeChangeLog> Children { get; set; } = new List<LicenseeChangeLog>();

        public decimal? InterestPercentageOld { get; set; }
        public decimal? InterestPercentageNew { get; set; }

        public string NameOld { get; set; }

        public string PhsLink { get; set; }

        public bool? IsShareholderIndividual { get; set; }
        public bool? IsLeadershipIndividual { get; set; }


        public LicenseeChangeLog ParentLicenseeChangeLog { get; set; }

        public LicenseeChangeLog()
        {

        }

        public LicenseeChangeLog (LicenseeChangeLog copy)
        {
            if (copy != null)
            {
                Id = copy.Id;
                ParentLicenseeChangeLogId = copy.ParentLicenseeChangeLogId;
                LegalEntityId = copy.LegalEntityId;
                BusinessAccountId = copy.BusinessAccountId;
                BusinessType = copy.BusinessType;
                IsIndividual = copy.IsIndividual;
                ParentLegalEntityId = copy.ParentLegalEntityId;
                ChangeType = copy.ChangeType;
                IsDirectorNew = copy.IsDirectorNew;
                IsDirectorOld = copy.IsDirectorOld;
                IsManagerNew = copy.IsManagerNew;
                IsManagerOld = copy.IsManagerOld;
                IsOfficerNew = copy.IsOfficerNew;
                IsOfficerOld = copy.IsOfficerOld;
                IsOwnerNew = copy.IsOwnerNew;
                IsOwnerOld = copy.IsOwnerOld;
                IsShareholderNew = copy.IsShareholderNew;
                IsShareholderOld = copy.IsShareholderOld;
                IsTrusteeNew = copy.IsTrusteeNew;
                IsTrusteeOld = copy.IsTrusteeOld;
                ParentBusinessAccountId = copy.ParentBusinessAccountId;
                BusinessAccountId = copy.BusinessAccountId;

                NumberofSharesNew = copy.NumberofSharesNew;
                NumberofSharesOld = copy.NumberofSharesOld;
                EmailNew = copy.EmailNew;
                EmailOld = copy.EmailOld;
                FirstNameNew = copy.FirstNameNew;
                FirstNameOld = copy.FirstNameOld;
                LastNameNew = copy.LastNameNew;
                LastNameOld = copy.LastNameOld;
                BusinessNameNew = copy.BusinessNameNew;
                NameOld = copy.NameOld;
                DateofBirthNew = copy.DateofBirthNew;
                DateofBirthOld = copy.DateofBirthOld;
                TitleNew = copy.TitleNew;
                TitleOld = copy.TitleOld;
                PhsLink = copy.PhsLink;
                NumberOfMembers = copy.NumberOfMembers;
                AnnualMembershipFee = copy.AnnualMembershipFee;
                TotalSharesOld = copy.TotalSharesOld;
                TotalSharesNew = copy.TotalSharesNew;
                InterestPercentageOld = copy.InterestPercentageOld;
                InterestPercentageNew = copy.InterestPercentageNew;
            }
        }

        public void UpdateValues(LicenseeChangeLog update){
             if (update != null)
            {
                Id = update.Id;
                ParentLicenseeChangeLogId = update.ParentLicenseeChangeLogId;
                LegalEntityId = update.LegalEntityId;
                BusinessAccountId = update.BusinessAccountId;
                BusinessType = update.BusinessType;
                IsIndividual = update.IsIndividual;
                ParentLegalEntityId = update.ParentLegalEntityId;
                ChangeType = update.ChangeType;
                IsDirectorNew = update.IsDirectorNew;
                IsDirectorOld = update.IsDirectorOld;
                IsManagerNew = update.IsManagerNew;
                IsManagerOld = update.IsManagerOld;
                IsOfficerNew = update.IsOfficerNew;
                IsOfficerOld = update.IsOfficerOld;
                IsOwnerNew = update.IsOwnerNew;
                IsOwnerOld = update.IsOwnerOld;
                IsShareholderNew = update.IsShareholderNew;
                IsShareholderOld = update.IsShareholderOld;
                IsTrusteeNew = update.IsTrusteeNew;
                IsTrusteeOld = update.IsTrusteeOld;
                ParentBusinessAccountId = update.ParentBusinessAccountId;
                BusinessAccountId = update.BusinessAccountId;

                NumberofSharesNew = update.NumberofSharesNew;
                NumberofSharesOld = update.NumberofSharesOld;
                EmailNew = update.EmailNew;
                EmailOld = update.EmailOld;
                FirstNameNew = update.FirstNameNew;
                FirstNameOld = update.FirstNameOld;
                LastNameNew = update.LastNameNew;
                LastNameOld = update.LastNameOld;
                BusinessNameNew = update.BusinessNameNew;
                NameOld = update.NameOld;
                DateofBirthNew = update.DateofBirthNew;
                DateofBirthOld = update.DateofBirthOld;
                TitleNew = update.TitleNew;
                TitleOld = update.TitleOld;
                // PhsLink = update.PhsLink;
                NumberOfMembers = update.NumberOfMembers;
                AnnualMembershipFee = update.AnnualMembershipFee;
                TotalSharesOld = update.TotalSharesOld;
                TotalSharesNew = update.TotalSharesNew;
                InterestPercentageOld = update.InterestPercentageOld;
                InterestPercentageNew = update.InterestPercentageNew;
            }
        }

        public LicenseeChangeLog(LegalEntity legalEntity)
        {
            if (legalEntity != null)
            {
                LegalEntityId = legalEntity.id;
                BusinessAccountId = legalEntity.shareholderAccountId;
                BusinessType = legalEntity.legalentitytype;
                IsIndividual = legalEntity.isindividual;

            
                ParentLegalEntityId = legalEntity.parentLegalEntityId;
                ChangeType = LicenseeChangeType.unchanged;
                
                IsDirectorNew = legalEntity.isDirector;
                IsDirectorOld = legalEntity.isDirector;
                IsManagerNew = legalEntity.isSeniorManagement;
                IsManagerOld = legalEntity.isSeniorManagement;
                IsOfficerNew = legalEntity.isOfficer;
                IsOfficerOld = legalEntity.isOfficer;
                IsOwnerNew = legalEntity.isOwner;
                IsOwnerOld = legalEntity.isOwner;
                IsShareholderNew = legalEntity.isShareholder;
                IsShareholderOld = legalEntity.isShareholder;
                IsTrusteeNew = legalEntity.IsTrustee;
                IsTrusteeOld = legalEntity.IsTrustee;
                if (legalEntity.isApplicant)
                {
                    BusinessAccountId = legalEntity.accountId;
                }
                else
                {
                    ParentBusinessAccountId = legalEntity.accountId;
                    BusinessAccountId = legalEntity.shareholderAccountId;
                }
                NumberofSharesNew = legalEntity.commonvotingshares;
                NumberofSharesOld = legalEntity.commonvotingshares;
                NumberOfNonVotingSharesNew = legalEntity.commonnonvotingshares;
                NumberOfNonVotingSharesOld = legalEntity.commonnonvotingshares;
                EmailNew = legalEntity.email;
                EmailOld = legalEntity.email;
                FirstNameNew = legalEntity.firstname;
                FirstNameOld = legalEntity.firstname;
                LastNameNew = legalEntity.lastname;
                LastNameOld = legalEntity.lastname;
                BusinessNameNew = legalEntity.name;
                NameOld = legalEntity.name;
                DateofBirthNew = legalEntity.dateofbirth;
                DateofBirthOld = legalEntity.dateofbirth;
                TitleNew = legalEntity.jobTitle;
                TitleOld = legalEntity.jobTitle;
                PhsLink = legalEntity.PhsLink;
                NumberOfMembers = legalEntity.NumberOfMembers;
                AnnualMembershipFee = legalEntity.AnnualMembershipFee;
                TotalSharesOld = legalEntity.TotalShares;
                TotalSharesNew = legalEntity.TotalShares;
                InterestPercentageOld = legalEntity.interestpercentage;
                InterestPercentageNew = legalEntity.interestpercentage;
            }
        }
    
        public LicenseeChangeLog FindNodeByLegalEntityId(string id){
            LicenseeChangeLog result = null;
            if(id == LegalEntityId){
                result = this;
            } else if(Children != null && Children.Count > 0){
                foreach (var item in Children)
                {
                    var found = item.FindNodeByLegalEntityId(id);
                    if(found != null){
                        result = found;
                        break;
                    }
                }
            }
            return result;
        }
        public LicenseeChangeLog FindNodeByParentChangeLogId(string id){
            LicenseeChangeLog result = null;
            if(id == Id){
                result = this;
            } else if(Children != null && Children.Count > 0){
                foreach (var item in Children)
                {
                    var found = item.FindNodeByParentChangeLogId(id);
                    if(found != null){
                        result = found;
                        break;
                    }
                }
            }
            return result;
        }
    }
}
