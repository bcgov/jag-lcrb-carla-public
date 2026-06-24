extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using DvLegalEntity = DV::Gov.Lclb.Cllb.Interfaces.adoxio_legalentity;
using DvGeneralYesNo = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LegalEntityExtensions
    {
        public static LegalEntity ToViewModel(this DvLegalEntity le)
        {
            if (le == null) return null;
            var result = new LegalEntity();
            result.id = le.adoxio_legalentityId?.ToString() ?? le.Id.ToString();
            result.accountId = le.adoxio_Account?.Id.ToString();
            result.shareholderAccountId = le.adoxio_ShareholderAccountID?.Id.ToString();
            result.parentLegalEntityId = le.adoxio_LegalEntityOwned?.Id.ToString();
            result.name = le.adoxio_name;
            result.firstname = le.adoxio_FirstName;
            result.lastname = le.adoxio_LastName;
            result.middlename = le.adoxio_MiddleName;
            result.email = le.adoxio_Email;
            result.commonnonvotingshares = le.adoxio_CommonNonVotingShares;
            result.commonvotingshares = le.adoxio_CommonVotingShares;
            result.preferrednonvotingshares = le.adoxio_PreferredNonVotingShares;
            result.preferredvotingshares = le.adoxio_PreferredVotingShares;
            result.dateofbirth = le.adoxio_DateofBirth;
            result.dateofappointment = le.adoxio_DateofAppointment;
            result.dateIssued = le.adoxio_DateofSharesIssued;
            result.securityAssessmentEmailSentOn = le.adoxio_DateEmailSent;
            result.jobTitle = le.adoxio_JobTitle;
            result.AnnualMembershipFee = le.adoxio_AnnualMembershipFee;
            result.NumberOfMembers = le.adoxio_NumberofMembers;
            result.TotalShares = le.adoxio_TotalShares;
            if (le.adoxio_InterestPercentage != null)
                result.interestpercentage = Convert.ToDecimal(le.adoxio_InterestPercentage);
            result.isindividual = le.adoxio_IsIndividual == DvGeneralYesNo.Yes;
            result.sameasapplyingperson = le.adoxio_SameAsApplyingPerson == DvGeneralYesNo.Yes;
            result.isApplicant = le.adoxio_IsApplicant == true;
            result.isPartner = le.adoxio_IsPartner == true;
            result.isShareholder = le.adoxio_IsShareholder == true;
            result.IsTrustee = le.adoxio_IsTrustee == true;
            result.isDirector = le.adoxio_IsDirector == true;
            result.isOfficer = le.adoxio_IsOfficer == true;
            result.isSeniorManagement = le.adoxio_IsSeniorManagement == true;
            result.isOwner = le.adoxio_IsOwner == true;
            result.isKeyPersonnel = le.adoxio_IsKeyPersonnel == true;
            if (le.adoxio_LegalEntityType != null)
                result.legalentitytype = (AdoxioApplicantTypeCodes)(int)le.adoxio_LegalEntityType;
            if (le.adoxio_PartnerType != null)
                result.partnerType = (AdoxioPartnerType)(int)le.adoxio_PartnerType;
            result.contactId = le.adoxio_Contact?.Id.ToString();
            return result;
        }

        public static void CopyValues(this DvLegalEntity to, LegalEntity from)
        {
            to.adoxio_name = from.name;
            to.adoxio_FirstName = from.firstname;
            to.adoxio_LastName = from.lastname;
            to.adoxio_MiddleName = from.middlename;
            to.adoxio_Email = from.email;
            to.adoxio_CommonNonVotingShares = from.commonnonvotingshares;
            to.adoxio_CommonVotingShares = from.commonvotingshares;
            to.adoxio_PreferredNonVotingShares = from.preferrednonvotingshares;
            to.adoxio_PreferredVotingShares = from.preferredvotingshares;
            to.adoxio_DateofBirth = from.dateofbirth?.DateTime;
            to.adoxio_DateofAppointment = from.dateofappointment?.DateTime;
            to.adoxio_DateofSharesIssued = from.dateIssued?.DateTime;
            to.adoxio_DateEmailSent = from.securityAssessmentEmailSentOn?.DateTime;
            to.adoxio_JobTitle = from.jobTitle;
            to.adoxio_AnnualMembershipFee = from.AnnualMembershipFee;
            to.adoxio_NumberofMembers = from.NumberOfMembers;
            to.adoxio_InterestPercentage = from.interestpercentage;
            to.adoxio_IsIndividual = (from.isindividual == true) ? DvGeneralYesNo.Yes : DvGeneralYesNo.No;
            to.adoxio_SameAsApplyingPerson = (from.sameasapplyingperson == true) ? DvGeneralYesNo.Yes : DvGeneralYesNo.No;
            to.adoxio_IsApplicant = from.isApplicant;
            to.adoxio_IsPartner = from.isPartner;
            to.adoxio_IsShareholder = from.isShareholder;
            to.adoxio_IsTrustee = from.IsTrustee;
            to.adoxio_IsDirector = from.isDirector;
            to.adoxio_IsOfficer = from.isOfficer;
            to.adoxio_IsSeniorManagement = from.isSeniorManagement;
            to.adoxio_IsOwner = from.isOwner;
            to.adoxio_IsKeyPersonnel = from.isKeyPersonnel;
            if (from.legalentitytype != null)
                to.adoxio_LegalEntityType = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes)(int)from.legalentitytype.Value;
            if (from.partnerType != null)
                to.adoxio_PartnerType = (DV::Gov.Lclb.Cllb.Interfaces.adoxio_partnertype)(int)from.partnerType.Value;
        }

    }

}
