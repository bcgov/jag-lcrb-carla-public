using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;

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
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioLegalentity to, ViewModels.AdoxioLegalEntity from)
        {

            to.AdoxioCommonnonvotingshares = from.commonnonvotingshares;
            to.AdoxioCommonvotingshares = from.commonvotingshares;
            to.AdoxioDateofbirth = from.dateofbirth;
            to.AdoxioFirstname = from.firstname;
            to.AdoxioInterestpercentage = (double?)from.interestpercentage;
            to.AdoxioIsindividual = (from.isindividual != null && (bool)from.isindividual) ? 1 : 0;
            to.AdoxioLastname = from.lastname;
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
            to.AdoxioPreferrednonvotingshares = from.preferrednonvotingshares;
            to.AdoxioPreferredvotingshares = from.preferredvotingshares;
            to.AdoxioSameasapplyingperson = (from.sameasapplyingperson != null && (bool)from.sameasapplyingperson) ? 1 : 0;
            to.AdoxioEmail = from.email;
            to.AdoxioDateofappointment = from.dateofappointment;
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
        public static ViewModels.AdoxioLegalEntity ToViewModel(this MicrosoftDynamicsCRMadoxioLegalentity adoxio_legalentity)
        {
            ViewModels.AdoxioLegalEntity result = null;
            if (adoxio_legalentity != null)
            {
                result = new ViewModels.AdoxioLegalEntity();
                if (adoxio_legalentity.AdoxioLegalentityid != null)
                {
                    result.id = adoxio_legalentity.AdoxioLegalentityid.ToString();
                }

                if (adoxio_legalentity._adoxioAccountValue != null)
                {
                    result.accountId = adoxio_legalentity._adoxioAccountValue;
                }
                if (adoxio_legalentity._adoxioShareholderaccountidValue != null)
                {
                    result.shareholderAccountId = adoxio_legalentity._adoxioShareholderaccountidValue;
                }

                result.parentLegalEntityId = adoxio_legalentity._adoxioLegalentityownedValue;

                result.commonnonvotingshares = adoxio_legalentity.AdoxioCommonnonvotingshares;
                result.commonvotingshares = adoxio_legalentity.AdoxioCommonvotingshares;
                result.dateofbirth = adoxio_legalentity.AdoxioDateofbirth;
                result.firstname = adoxio_legalentity.AdoxioFirstname;
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
                // result.isTrustee =  adoxio_legalentity.AdoxioIstrustee;
                result.isDirector = (adoxio_legalentity.AdoxioIsdirector == true);
                result.isOfficer = (adoxio_legalentity.AdoxioIsofficer == true);
                result.isSeniorManagement = (adoxio_legalentity.AdoxioIsseniormanagement == true);
                result.isOwner = (adoxio_legalentity.AdoxioIsowner == true);

                result.preferrednonvotingshares = adoxio_legalentity.AdoxioPreferrednonvotingshares;
                result.preferredvotingshares = adoxio_legalentity.AdoxioPreferredvotingshares;
                // convert from int to bool.
                result.sameasapplyingperson = (adoxio_legalentity.AdoxioSameasapplyingperson != null && adoxio_legalentity.AdoxioSameasapplyingperson != 0);
                result.dateofappointment = adoxio_legalentity.AdoxioDateofappointment;
                result.securityAssessmentEmailSentOn = adoxio_legalentity.AdoxioDateemailsent;

                // populate the account.
                if (adoxio_legalentity.AdoxioAccount != null)
                {
                    result.account = adoxio_legalentity.AdoxioAccount.ToViewModel();
                }

            }
            return result;
        }

    }

}
