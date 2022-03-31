using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Linq;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {
        const string AccountDocumentListTitle = "account";

        private static string GetServerRelativeURL(string listTitle, string folderName)
        {
            string serverRelativeUrl = Uri.EscapeUriString(listTitle) + "/" + Uri.EscapeUriString(folderName);
            return serverRelativeUrl;
        }

        public static string GetServerUrl(this MicrosoftDynamicsCRMaccount account)
        {
            string result = "";
            // use the account document location if it exists.
            if (account.AccountSharepointDocumentLocation != null && account.AccountSharepointDocumentLocation.Count > 0)
            {
                var location = account.AccountSharepointDocumentLocation.FirstOrDefault();
                if (location != null)
                {
                    if (string.IsNullOrEmpty(location.Relativeurl))
                    {
                        if (!string.IsNullOrEmpty(location.Absoluteurl))
                        {
                            result = location.Absoluteurl;
                        }
                    }
                    else
                    {
                        string serverRelativeUrl = "";

                        serverRelativeUrl += "/" + GetServerRelativeURL(AccountDocumentListTitle, location.Relativeurl);

                        result = serverRelativeUrl;
                    }
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                string serverRelativeUrl = "";
                string accountIdCleaned = account.Accountid.ToUpper().Replace("-", "");
                string folderName = $"_{accountIdCleaned}";

                serverRelativeUrl += "/" + GetServerRelativeURL(AccountDocumentListTitle, folderName);

                result = serverRelativeUrl;

            }
            return result;
        }



        /// <summary>
        /// Copy values from a ViewModel to a Dynamics Account.
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// updateIfNull defaults to true
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMaccount toDynamics, ViewModels.Account fromVM, Boolean copyIfNull)
        {
            if (copyIfNull || (!copyIfNull && fromVM.name != null))
            {
                toDynamics.Name = fromVM.name;
            }
            if (copyIfNull || (!copyIfNull && fromVM.description != null))
            {
                toDynamics.Description = fromVM.description;
            }
            if (copyIfNull || (!copyIfNull && fromVM.externalId != null))
            {
                toDynamics.AdoxioExternalid = fromVM.externalId;
            }
            if (copyIfNull || (!copyIfNull && fromVM.bcIncorporationNumber != null))
            {
                toDynamics.AdoxioBcincorporationnumber = fromVM.bcIncorporationNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.dateOfIncorporationInBC != null))
            {
                toDynamics.AdoxioDateofincorporationinbc = fromVM.dateOfIncorporationInBC;
            }
            if (copyIfNull || (!copyIfNull && fromVM.businessNumber != null))
            {
                toDynamics.Accountnumber = fromVM.businessNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.pstNumber != null))
            {
                toDynamics.AdoxioPstnumber = fromVM.pstNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactEmail != null))
            {
                toDynamics.Emailaddress1 = fromVM.contactEmail;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactPhone != null))
            {
                toDynamics.Telephone1 = fromVM.contactPhone;
            }
            if (copyIfNull || (!copyIfNull && fromVM.TermsOfUseAccepted != null))
            {
                toDynamics.AdoxioTermsofuseaccepted = fromVM.TermsOfUseAccepted;
            }
            if (copyIfNull || (!copyIfNull && fromVM.TermsOfUseAcceptedDate != null))
            {
                toDynamics.AdoxioTermsofuseaccepteddate = fromVM.TermsOfUseAcceptedDate;
            }

            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressName != null))
            {
                toDynamics.Address2Name = fromVM.mailingAddressName;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressStreet != null))
            {
                toDynamics.Address2Line1 = fromVM.mailingAddressStreet;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressStreet2 != null))
            {
                toDynamics.Address2Line2 = fromVM.mailingAddressStreet2;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCity != null))
            {
                toDynamics.Address2City = fromVM.mailingAddressCity;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCountry != null))
            {
                toDynamics.Address2Country = fromVM.mailingAddressCountry;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressProvince != null))
            {
                toDynamics.Address2Stateorprovince = fromVM.mailingAddressProvince;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressPostalCode != null))
            {
                if (fromVM.mailingAddressPostalCode != null)
                {
                    toDynamics.Address2Postalcode = fromVM.mailingAddressPostalCode.Replace(" ", "");
                }
                else
                {
                    toDynamics.Address2Postalcode = null;
                }

            }

            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressName != null))
            {
                toDynamics.Address1Name = fromVM.physicalAddressName;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressStreet != null))
            {
                toDynamics.Address1Line1 = fromVM.physicalAddressStreet;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressStreet2 != null))
            {
                toDynamics.Address1Line2 = fromVM.physicalAddressStreet2;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressCity != null))
            {
                toDynamics.Address1City = fromVM.physicalAddressCity;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressCountry != null))
            {
                toDynamics.Address1Country = fromVM.physicalAddressCountry;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressProvince != null))
            {
                toDynamics.Address1Stateorprovince = fromVM.physicalAddressProvince;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressPostalCode != null))
            {
                if (fromVM.physicalAddressPostalCode != null)
                {
                    toDynamics.Address1Postalcode = fromVM.physicalAddressPostalCode.Replace(" ", "");
                }
                else
                {
                    toDynamics.Address1Postalcode = null;
                }

            }

            toDynamics.Websiteurl = fromVM.websiteUrl;

            // business type must be set only during creation, not in update (removed from copyValues() )
            //	toDynamics.AdoxioBusinesstype = (int)Enum.Parse(typeof(ViewModels.Adoxio_applicanttypecodes), fromVM.businessType, true);

            // SEP Police Review Limits
            if (copyIfNull || (!copyIfNull && fromVM.isLateHoursApproval != null))
            {
                toDynamics.AdoxioIslatehoursapproval = fromVM.isLateHoursApproval;
            }
            if (copyIfNull || (!copyIfNull && fromVM.maxGuestsForPublicEvents != null))
            {
                toDynamics.AdoxioMaxguestsforpublic = fromVM.maxGuestsForPublicEvents;
            }
            if (copyIfNull || (!copyIfNull && fromVM.maxGuestsForPrivateEvents != null))
            {
                toDynamics.AdoxioMaxguestsforprivate = fromVM.maxGuestsForPrivateEvents;
            }
            if (copyIfNull || (!copyIfNull && fromVM.maxGuestsForFamilyEvents != null))
            {
                toDynamics.AdoxioMaxguestsforfamily = fromVM.maxGuestsForFamilyEvents;
            }
        }

        /// <summary>
        /// Copy values from a ViewModel to a Dynamics Account.
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// updateIfNull defaults to true
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMaccount toDynamics, ViewModels.Account fromVM)
        {
            bool copyIfNull = true;
            toDynamics.CopyValues(fromVM, copyIfNull);
        }

        /// <summary>
        /// Create a new ViewModel from a Dynamics Account
        /// </summary>
        /// <param name="account"></param>
        public static ViewModels.Account ToViewModel(this MicrosoftDynamicsCRMaccount account)
        {
            ViewModels.Account accountVM = null;
            if (account != null)
            {
                accountVM = new ViewModels.Account();
                if (account.Accountid != null)
                {
                    accountVM.id = account.Accountid;
                }

                accountVM.name = account.Name;
                accountVM.description = account.Description;
                accountVM.externalId = account.AdoxioExternalid;
                accountVM.bcIncorporationNumber = account.AdoxioBcincorporationnumber;
                accountVM.dateOfIncorporationInBC = account.AdoxioDateofincorporationinbc;
                accountVM.businessNumber = account.Accountnumber;
                accountVM.pstNumber = account.AdoxioPstnumber;
                accountVM.contactEmail = account.Emailaddress1;
                accountVM.contactPhone = account.Telephone1;

                accountVM.mailingAddressName = account.Address2Name;
                accountVM.mailingAddressStreet = account.Address2Line1;
                accountVM.mailingAddressStreet2 = account.Address2Line2;
                accountVM.mailingAddressCity = account.Address2City;
                accountVM.mailingAddressCountry = account.Address2Country;
                accountVM.mailingAddressProvince = account.Address2Stateorprovince;


                accountVM.mailingAddressPostalCode = account.Address2Postalcode;

                accountVM.physicalAddressName = account.Address1Name;
                accountVM.physicalAddressStreet = account.Address1Line1;
                accountVM.physicalAddressStreet2 = account.Address1Line2;
                accountVM.physicalAddressCity = account.Address1City;
                accountVM.physicalAddressCountry = account.Address1Country;
                accountVM.physicalAddressProvince = account.Address1Stateorprovince;
                accountVM.physicalAddressPostalCode = account.Address1Postalcode;

                accountVM.TermsOfUseAccepted = account.AdoxioTermsofuseaccepted;
                accountVM.TermsOfUseAcceptedDate = account.AdoxioTermsofuseaccepteddate;

                accountVM.LocalGovernmentId = account._adoxioLginlinkidValue;

                accountVM.websiteUrl = account.Websiteurl;

                // SEP Police Review Limits
                accountVM.isLateHoursApproval = account.AdoxioIslatehoursapproval;
                accountVM.maxGuestsForPublicEvents = account.AdoxioMaxguestsforpublic;
                accountVM.maxGuestsForPrivateEvents = account.AdoxioMaxguestsforprivate;
                accountVM.maxGuestsForFamilyEvents = account.AdoxioMaxguestsforfamily;

                if (account.Primarycontactid != null)
                {
                    accountVM.primarycontact = account.Primarycontactid.ToViewModel();
                }

                if (account.AdoxioBusinesstype != null)
                {
                    accountVM.businessType = Enum.ToObject(typeof(ViewModels.AdoxioApplicantTypeCodes), account.AdoxioBusinesstype).ToString();
                }
            }
            return accountVM;
        }


    }
}
