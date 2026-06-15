extern alias DV;
using Gov.Lclb.Cllb.Interfaces.Models;
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

        /// <summary>
        /// Copy values from View Model to Dynamics legal entity
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioLegalentity to, LegalEntity from)
        {

            to.AdoxioCommonnonvotingshares = from.commonnonvotingshares;
            to.AdoxioCommonvotingshares = from.commonvotingshares;
            to.AdoxioDateofbirth = from.dateofbirth;
            to.AdoxioFirstname = from.firstname;
            to.AdoxioInterestpercentage = from.interestpercentage;
            to.AdoxioIsindividual = (from.isindividual != null && (bool)from.isindividual) ? 1 : 0;
            to.AdoxioLastname = from.lastname;
            to.AdoxioIstrustee = from.IsTrustee;
            if (from.legalentitytype != null)
            {
                to.AdoxioLegalentitytype = (int?)from.legalentitytype;
            }
            if (from.partnerType != null)
            {
                to.AdoxioPartnertype = (int?)from.partnerType;
            }
            to.AdoxioMiddlename = from.middlename;
            to.AdoxioName = from.name;
            to.AdoxioIspartner = from.isPartner;
            to.AdoxioIsshareholder = from.isShareholder;
            to.AdoxioIstrustee = false;
            to.AdoxioIsdirector = from.isDirector;
            to.AdoxioIsofficer = from.isOfficer;
            to.AdoxioIsseniormanagement = from.isSeniorManagement;
            to.AdoxioIsowner = from.isOwner;
            to.AdoxioIskeypersonnel = from.isKeyPersonnel;
            to.AdoxioPreferrednonvotingshares = from.preferrednonvotingshares;
            to.AdoxioPreferredvotingshares = from.preferredvotingshares;
            to.AdoxioSameasapplyingperson = (from.sameasapplyingperson != null && (bool)from.sameasapplyingperson) ? 1 : 0;
            to.AdoxioEmail = from.email;
            to.AdoxioDateofappointment = from.dateofappointment;
            to.AdoxioDateofsharesissued = from.dateIssued;
            to.AdoxioJobtitle = from.jobTitle;
            to.AdoxioNumberofmembers = from.NumberOfMembers;
            to.AdoxioAnnualmembershipfee = from.AnnualMembershipFee;
            to.AdoxioTotalshares = from.TotalShares;
            // Assigning the account this way throws exception:
            // System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            //if (from.account.id != null)
            //{
            //    // fetch the account from Dynamics.
            //    var getAccountTask = _system.GetAccountById(null, Guid.Parse(from.account.id));
            //    getAccountTask.Wait();
            //    to.Adoxio_Account= getAccountTask.Result;
            //}
            to.AdoxioDateemailsent = from.securityAssessmentEmailSentOn;
        }



        /// <summary>
        /// Convert a Dynamics Legal Entity to a ViewModel
        /// </summary>        
        public static LegalEntity ToViewModel(this MicrosoftDynamicsCRMadoxioLegalentity adoxio_legalentity)
        {
            LegalEntity result = null;
            if (adoxio_legalentity != null)
            {
                result = new LegalEntity();
                if (adoxio_legalentity.AdoxioLegalentityid != null)
                {
                    result.id = adoxio_legalentity.AdoxioLegalentityid;
                }

                if (adoxio_legalentity._adoxioAccountValue != null)
                {
                    result.accountId = adoxio_legalentity._adoxioAccountValue;
                }
                if (adoxio_legalentity._adoxioShareholderaccountidValue != null)
                {
                    result.shareholderAccountId = adoxio_legalentity._adoxioShareholderaccountidValue;
                }

                if (adoxio_legalentity.AdoxioContact != null)
                {
                    result.isContactComplete = (GeneralYesNo?)adoxio_legalentity.AdoxioContact.AdoxioPhscomplete;
                }

                result.parentLegalEntityId = adoxio_legalentity._adoxioLegalentityownedValue;

                result.commonnonvotingshares = adoxio_legalentity.AdoxioCommonnonvotingshares;
                result.commonvotingshares = adoxio_legalentity.AdoxioCommonvotingshares;
                result.dateofbirth = adoxio_legalentity.AdoxioDateofbirth;
                result.firstname = adoxio_legalentity.AdoxioFirstname;
                result.contactId = adoxio_legalentity._adoxioContactValue;
                if (adoxio_legalentity.AdoxioInterestpercentage != null)
                {
                    result.interestpercentage = Convert.ToDecimal(adoxio_legalentity.AdoxioInterestpercentage);
                }

                // convert from int to bool.
                result.isindividual = (adoxio_legalentity.AdoxioIsindividual != null && adoxio_legalentity.AdoxioIsindividual != 0);
                result.lastname = adoxio_legalentity.AdoxioLastname;
                if (adoxio_legalentity.AdoxioLegalentitytype != null)
                {
                    result.legalentitytype = (AdoxioApplicantTypeCodes)adoxio_legalentity.AdoxioLegalentitytype;
                }
                if (adoxio_legalentity.AdoxioPartnertype != null)
                {
                    result.partnerType = (AdoxioPartnerType)adoxio_legalentity.AdoxioPartnertype;
                }

                result.middlename = adoxio_legalentity.AdoxioMiddlename;
                result.name = adoxio_legalentity.AdoxioName;
                result.email = adoxio_legalentity.AdoxioEmail;
                result.isPartner = (adoxio_legalentity.AdoxioIspartner == true);
                result.isApplicant = (adoxio_legalentity.AdoxioIsapplicant == true);
                result.isShareholder = (adoxio_legalentity.AdoxioIsshareholder == true);
                result.IsTrustee =  (adoxio_legalentity.AdoxioIstrustee == true);
                result.isDirector = (adoxio_legalentity.AdoxioIsdirector == true);
                result.isOfficer = (adoxio_legalentity.AdoxioIsofficer == true);
                result.isSeniorManagement = (adoxio_legalentity.AdoxioIsseniormanagement == true);
                result.isOwner = (adoxio_legalentity.AdoxioIsowner == true);
                result.isKeyPersonnel = (adoxio_legalentity.AdoxioIskeypersonnel == true);

                result.preferrednonvotingshares = adoxio_legalentity.AdoxioPreferrednonvotingshares;
                result.preferredvotingshares = adoxio_legalentity.AdoxioPreferredvotingshares;
                // convert from int to bool.
                result.sameasapplyingperson = (adoxio_legalentity.AdoxioSameasapplyingperson != null && adoxio_legalentity.AdoxioSameasapplyingperson != 0);
                result.dateofappointment = adoxio_legalentity.AdoxioDateofappointment;
                result.dateIssued = adoxio_legalentity.AdoxioDateofsharesissued;
                result.securityAssessmentEmailSentOn = adoxio_legalentity.AdoxioDateemailsent;
                result.jobTitle = adoxio_legalentity.AdoxioJobtitle;

                result.AnnualMembershipFee = adoxio_legalentity.AdoxioAnnualmembershipfee;
                result.NumberOfMembers  = adoxio_legalentity.AdoxioNumberofmembers;
                result.TotalShares = adoxio_legalentity.AdoxioTotalshares;

                // populate the account.
                if (adoxio_legalentity.AdoxioAccount != null)
                {
                    result.account = adoxio_legalentity.AdoxioAccount.ToViewModel();
                }

            }
            return result;
        }

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
