extern alias DV;
using System;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using DvAdoxioApplicantTypeCodes = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes;
using DvAdoxioAccountType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_accounttype;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {
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
