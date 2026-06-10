extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Linq;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using DvAdoxioApplicantTypeCodes = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes;
using DvAdoxioAccountType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_accounttype;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {
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

                        serverRelativeUrl += "/" + GetServerRelativeURL(SharePointConstants.AccountFolderInternalName, location.Relativeurl);

                        result = serverRelativeUrl;
                    }
                }
            }
            if (string.IsNullOrEmpty(result))
            {
                string serverRelativeUrl = "";
                string accountIdCleaned = account.Accountid.ToUpper().Replace("-", "");
                string folderName = $"_{accountIdCleaned}";

                serverRelativeUrl += "/" + GetServerRelativeURL(SharePointConstants.AccountFolderInternalName, folderName);

                result = serverRelativeUrl;

            }
            return result;
        }



        /// <summary>
        /// Copy values from a ViewModel to a new Dynamics Account entity.
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull">`true` if null values should be copied, `false` otherwise.</param>
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
            if (copyIfNull || (!copyIfNull && fromVM.accountUrls != null))
            {
                toDynamics.AdoxioAccounturls = fromVM.accountUrls;
            }

            // toDynamics.Websiteurl = fromVM.websiteUrl;

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
        /// Copy values from a Dynamics Account entity to a new ViewModel.
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
                accountVM.accountUrls = account.AdoxioAccounturls;
                
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

        public static ViewModels.Account ToViewModel(this DvAccount account)
        {
            if (account == null) return null;
            var vm = new ViewModels.Account();
            vm.id = account.AccountId?.ToString() ?? account.Id.ToString();
            vm.name = account.Name;
            vm.description = account.Description;
            vm.externalId = account.adoxio_ExternalID;
            vm.bcIncorporationNumber = account.adoxio_BCIncorporationNumber;
            vm.dateOfIncorporationInBC = account.adoxio_DateofIncorporationinBC;
            vm.businessNumber = account.AccountNumber;
            vm.pstNumber = account.adoxio_PSTNumber;
            vm.contactEmail = account.EMailAddress1;
            vm.contactPhone = account.Telephone1;
            vm.mailingAddressName = account.Address2_Name;
            vm.mailingAddressStreet = account.Address2_Line1;
            vm.mailingAddressStreet2 = account.Address2_Line2;
            vm.mailingAddressCity = account.Address2_City;
            vm.mailingAddressCountry = account.Address2_Country;
            vm.mailingAddressProvince = account.Address2_StateOrProvince;
            vm.mailingAddressPostalCode = account.Address2_PostalCode;
            vm.physicalAddressName = account.Address1_Name;
            vm.physicalAddressStreet = account.Address1_Line1;
            vm.physicalAddressStreet2 = account.Address1_Line2;
            vm.physicalAddressCity = account.Address1_City;
            vm.physicalAddressCountry = account.Address1_Country;
            vm.physicalAddressProvince = account.Address1_StateOrProvince;
            vm.physicalAddressPostalCode = account.Address1_PostalCode;
            vm.TermsOfUseAccepted = account.adoxio_TermsofUseAccepted;
            vm.TermsOfUseAcceptedDate = account.adoxio_TermsofUseAcceptedDate;
            vm.LocalGovernmentId = account.adoxio_LGINLinkId?.Id.ToString();
            vm.websiteUrl = account.WebSiteURL;
            vm.accountUrls = account.adoxio_AccountURLs;
            vm.isLateHoursApproval = account.adoxio_IsLateHoursApproval;
            vm.maxGuestsForPublicEvents = account.adoxio_MaxGuestsforPublic;
            vm.maxGuestsForPrivateEvents = account.adoxio_MaxGuestsforPrivate;
            vm.maxGuestsForFamilyEvents = account.adoxio_MaxGuestsforFamily;
            if (account.adoxio_BusinessType != null)
                vm.businessType = Enum.ToObject(typeof(ViewModels.AdoxioApplicantTypeCodes), (int)account.adoxio_BusinessType).ToString();
            return vm;
        }

        public static void CopyValues(this DvAccount to, ViewModels.Account from, bool copyIfNull)
        {
            if (copyIfNull || from.name != null) to.Name = from.name;
            if (copyIfNull || from.description != null) to.Description = from.description;
            if (copyIfNull || from.externalId != null) to.adoxio_ExternalID = from.externalId;
            if (copyIfNull || from.bcIncorporationNumber != null) to.adoxio_BCIncorporationNumber = from.bcIncorporationNumber;
            if (copyIfNull || from.dateOfIncorporationInBC != null) to.adoxio_DateofIncorporationinBC = from.dateOfIncorporationInBC?.UtcDateTime;
            if (copyIfNull || from.pstNumber != null) to.adoxio_PSTNumber = from.pstNumber;
            if (copyIfNull || from.contactEmail != null) to.EMailAddress1 = from.contactEmail;
            if (copyIfNull || from.contactPhone != null) to.Telephone1 = from.contactPhone;
            if (copyIfNull || from.TermsOfUseAccepted != null) to.adoxio_TermsofUseAccepted = from.TermsOfUseAccepted;
            if (copyIfNull || from.TermsOfUseAcceptedDate != null) to.adoxio_TermsofUseAcceptedDate = from.TermsOfUseAcceptedDate?.UtcDateTime;
            if (copyIfNull || from.mailingAddressName != null) to.Address2_Name = from.mailingAddressName;
            if (copyIfNull || from.mailingAddressStreet != null) to.Address2_Line1 = from.mailingAddressStreet;
            if (copyIfNull || from.mailingAddressStreet2 != null) to.Address2_Line2 = from.mailingAddressStreet2;
            if (copyIfNull || from.mailingAddressCity != null) to.Address2_City = from.mailingAddressCity;
            if (copyIfNull || from.mailingAddressCountry != null) to.Address2_Country = from.mailingAddressCountry;
            if (copyIfNull || from.mailingAddressProvince != null) to.Address2_StateOrProvince = from.mailingAddressProvince;
            if (copyIfNull || from.mailingAddressPostalCode != null)
                to.Address2_PostalCode = from.mailingAddressPostalCode?.Replace(" ", "");
            if (copyIfNull || from.physicalAddressName != null) to.Address1_Name = from.physicalAddressName;
            if (copyIfNull || from.physicalAddressStreet != null) to.Address1_Line1 = from.physicalAddressStreet;
            if (copyIfNull || from.physicalAddressStreet2 != null) to.Address1_Line2 = from.physicalAddressStreet2;
            if (copyIfNull || from.physicalAddressCity != null) to.Address1_City = from.physicalAddressCity;
            if (copyIfNull || from.physicalAddressCountry != null) to.Address1_Country = from.physicalAddressCountry;
            if (copyIfNull || from.physicalAddressProvince != null) to.Address1_StateOrProvince = from.physicalAddressProvince;
            if (copyIfNull || from.physicalAddressPostalCode != null)
                to.Address1_PostalCode = from.physicalAddressPostalCode?.Replace(" ", "");
            if (copyIfNull || from.accountUrls != null) to.adoxio_AccountURLs = from.accountUrls;
            if (copyIfNull || from.isLateHoursApproval != null) to.adoxio_IsLateHoursApproval = from.isLateHoursApproval;
            if (copyIfNull || from.maxGuestsForPublicEvents != null) to.adoxio_MaxGuestsforPublic = from.maxGuestsForPublicEvents;
            if (copyIfNull || from.maxGuestsForPrivateEvents != null) to.adoxio_MaxGuestsforPrivate = from.maxGuestsForPrivateEvents;
            if (copyIfNull || from.maxGuestsForFamilyEvents != null) to.adoxio_MaxGuestsforFamily = from.maxGuestsForFamilyEvents;
        }


    }
}
