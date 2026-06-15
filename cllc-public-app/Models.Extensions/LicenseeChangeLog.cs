extern alias DV;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DvChangelog = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licenseechangelog;
using DvChangeType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licenseechangelog_adoxio_changetype;
using DvApplicantType = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicanttypecodes;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class LicenseeChangeLogExtension
    {
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioLicenseechangelog toDynamics, LicenseeChangeLog fromVM)
        {
            toDynamics.AdoxioChangetype = (int?)fromVM.ChangeType;
            toDynamics.AdoxioBusinesstype = (int?)fromVM.BusinessType;
            toDynamics.AdoxioIsdirectornew = fromVM.IsDirectorNew;
            toDynamics.AdoxioIsdirectorold = fromVM.IsDirectorOld;
            toDynamics.AdoxioIsmanagernew = fromVM.IsManagerNew;
            toDynamics.AdoxioIsmanagerold = fromVM.IsManagerOld;
            toDynamics.AdoxioIsofficernew = fromVM.IsOfficerNew;
            toDynamics.AdoxioIsofficerold = fromVM.IsOfficerOld;
            toDynamics.AdoxioIsownernew = fromVM.IsOwnerNew;
            toDynamics.AdoxioIsownerold = fromVM.IsOwnerOld;
            toDynamics.AdoxioIsshareholdernew = fromVM.IsShareholderNew;
            toDynamics.AdoxioIsshareholderold = fromVM.IsShareholderOld;
            toDynamics.AdoxioIstrusteenew = fromVM.IsTrusteeNew;
            toDynamics.AdoxioIstrusteeold = fromVM.IsTrusteeOld;
            toDynamics.AdoxioNumberofsharesnew = fromVM.NumberofSharesNew;
            toDynamics.AdoxioNumberofsharesold = fromVM.NumberofSharesOld;
            toDynamics.AdoxioNumberofnonvotingsharesnew = fromVM.NumberOfNonVotingSharesNew;
            toDynamics.AdoxioNumberofnonvotingsharesold = fromVM.NumberOfNonVotingSharesOld;
            toDynamics.AdoxioTotalsharesnew = fromVM.TotalSharesNew;
            toDynamics.AdoxioTotalsharesold = fromVM.NumberofSharesOld;
            toDynamics.AdoxioEmailnew = fromVM.EmailNew;
            toDynamics.AdoxioEmailold = fromVM.EmailOld;
            toDynamics.AdoxioFirstnamenew = fromVM.FirstNameNew;
            toDynamics.AdoxioFirstnameold = fromVM.FirstNameOld;
            toDynamics.AdoxioLastnamenew = fromVM.LastNameNew;
            toDynamics.AdoxioLastnameold = fromVM.LastNameOld;
            toDynamics.AdoxioBusinessnamenew = fromVM.BusinessNameNew;
            toDynamics.AdoxioBusinesnameold = fromVM.BusinessNameOld;
            toDynamics.AdoxioDateofbirthnew = fromVM.DateofBirthNew;
            toDynamics.AdoxioDateofbirthold = fromVM.DateofBirthOld;
            toDynamics.AdoxioInterestpercentagenew = fromVM.InterestPercentageNew;
            toDynamics.AdoxioInterestpercentageold = fromVM.InterestPercentageOld;
            toDynamics.AdoxioNumberofmembers = fromVM.NumberOfMembers;
            toDynamics.AdoxioAnnualmembershipfee = fromVM.AnnualMembershipFee;
            toDynamics.AdoxioTitlenew = fromVM.TitleNew;
            toDynamics.AdoxioTitleold = fromVM.TitleOld;
        }

        public static void CopyValues(this DvChangelog to, LicenseeChangeLog from)
        {
            if (from.ChangeType != null)
                to.adoxio_ChangeType = (DvChangeType?)(int)from.ChangeType;
            if (from.BusinessType != null)
                to.adoxio_BusinessType = (DvApplicantType?)(int)from.BusinessType;
            to.adoxio_IsDirectorNew = from.IsDirectorNew;
            to.adoxio_IsDirectorOld = from.IsDirectorOld;
            to.adoxio_IsManagerNew = from.IsManagerNew;
            to.adoxio_IsManagerOld = from.IsManagerOld;
            to.adoxio_IsOfficerNew = from.IsOfficerNew;
            to.adoxio_IsOfficerOld = from.IsOfficerOld;
            to.adoxio_IsOwnerNew = from.IsOwnerNew;
            to.adoxio_isownerold = from.IsOwnerOld;
            to.adoxio_IsShareholderNew = from.IsShareholderNew;
            to.adoxio_IsShareholderOld = from.IsShareholderOld;
            to.adoxio_IsTrusteeNew = from.IsTrusteeNew;
            to.adoxio_IsTrusteeOld = from.IsTrusteeOld;
            to.adoxio_NumberOfSharesNew = from.NumberofSharesNew;
            to.adoxio_NumberOfSharesOld = from.NumberofSharesOld;
            to.adoxio_NumberofNonVotingSharesNew = from.NumberOfNonVotingSharesNew;
            to.adoxio_NumberofNonVotingSharesOld = from.NumberOfNonVotingSharesOld;
            to.adoxio_TotalSharesNew = from.TotalSharesNew;
            to.adoxio_TotalSharesOld = from.NumberofSharesOld;
            to.adoxio_EmailNew = from.EmailNew;
            to.adoxio_EmailOld = from.EmailOld;
            to.adoxio_FirstNameNew = from.FirstNameNew;
            to.adoxio_FirstNameOld = from.FirstNameOld;
            to.adoxio_LastNameNew = from.LastNameNew;
            to.adoxio_LastNameOld = from.LastNameOld;
            to.adoxio_BusinessNameNew = from.BusinessNameNew;
            to.adoxio_BusinesNameOld = from.BusinessNameOld;
            to.adoxio_DateOfBirthNew = from.DateofBirthNew?.DateTime;
            to.adoxio_DateOfBirthOld = from.DateofBirthOld?.DateTime;
            to.adoxio_InterestPercentageNew = from.InterestPercentageNew;
            to.adoxio_InterestPercentageOld = from.InterestPercentageOld;
            to.adoxio_NumberOfMembers = from.NumberOfMembers;
            to.adoxio_AnnualMembershipFee = from.AnnualMembershipFee.HasValue ? new Money(from.AnnualMembershipFee.Value) : null;
            to.adoxio_TitleNew = from.TitleNew;
            to.adoxio_TitleOld = from.TitleOld;
        }

        public static LicenseeChangeLog ToViewModel(this DvChangelog changeLog)
        {
            var result = new LicenseeChangeLog
            {
                Id = changeLog.adoxio_licenseechangelogId?.ToString() ?? changeLog.Id.ToString(),
                ChangeType = changeLog.adoxio_ChangeType != null ? (LicenseeChangeType?)(int)changeLog.adoxio_ChangeType : null,
                BusinessType = changeLog.adoxio_BusinessType != null ? (AdoxioApplicantTypeCodes?)(int)changeLog.adoxio_BusinessType : null,
                IsDirectorNew = changeLog.adoxio_IsDirectorNew,
                IsDirectorOld = changeLog.adoxio_IsDirectorOld,
                IsManagerNew = changeLog.adoxio_IsManagerNew,
                IsManagerOld = changeLog.adoxio_IsManagerOld,
                IsOfficerNew = changeLog.adoxio_IsOfficerNew,
                IsOfficerOld = changeLog.adoxio_IsOfficerOld,
                IsOwnerNew = changeLog.adoxio_IsOwnerNew,
                IsOwnerOld = changeLog.adoxio_isownerold,
                IsShareholderNew = changeLog.adoxio_IsShareholderNew,
                IsShareholderOld = changeLog.adoxio_IsShareholderOld,
                IsTrusteeNew = changeLog.adoxio_IsTrusteeNew,
                IsTrusteeOld = changeLog.adoxio_IsTrusteeOld,
                NumberofSharesNew = changeLog.adoxio_NumberOfSharesNew,
                NumberofSharesOld = changeLog.adoxio_NumberOfSharesOld,
                NumberOfNonVotingSharesNew = changeLog.adoxio_NumberofNonVotingSharesNew,
                NumberOfNonVotingSharesOld = changeLog.adoxio_NumberofNonVotingSharesOld,
                TotalSharesNew = changeLog.adoxio_TotalSharesNew,
                TotalSharesOld = changeLog.adoxio_TotalSharesOld,
                EmailNew = changeLog.adoxio_EmailNew,
                EmailOld = changeLog.adoxio_EmailOld,
                FirstNameNew = changeLog.adoxio_FirstNameNew,
                FirstNameOld = changeLog.adoxio_FirstNameOld,
                LastNameNew = changeLog.adoxio_LastNameNew,
                LastNameOld = changeLog.adoxio_LastNameOld,
                BusinessNameNew = changeLog.adoxio_BusinessNameNew,
                BusinessNameOld = changeLog.adoxio_BusinesNameOld,
                InterestPercentageNew = changeLog.adoxio_InterestPercentageNew,
                InterestPercentageOld = changeLog.adoxio_InterestPercentageOld,
                DateofBirthNew = changeLog.adoxio_DateOfBirthNew.HasValue ? (DateTimeOffset?)changeLog.adoxio_DateOfBirthNew.Value : null,
                DateofBirthOld = changeLog.adoxio_DateOfBirthOld.HasValue ? (DateTimeOffset?)changeLog.adoxio_DateOfBirthOld.Value : null,
                TitleOld = changeLog.adoxio_TitleOld,
                TitleNew = changeLog.adoxio_TitleNew,
                LegalEntityId = changeLog.adoxio_LegalEntityId?.Id.ToString(),
                ParentLegalEntityId = changeLog.adoxio_ParentLegalEntityId?.Id.ToString(),
                ParentLicenseeChangeLogId = changeLog.adoxio_ParentLinceseeChangeLogId?.Id.ToString(),
                NumberOfMembers = changeLog.adoxio_NumberOfMembers,
                AnnualMembershipFee = changeLog.adoxio_AnnualMembershipFee?.Value,
            };
            return result;
        }

        public static LicenseeChangeLog ToViewModel(this MicrosoftDynamicsCRMadoxioLicenseechangelog changeLog)
        {
            var result = new LicenseeChangeLog
            {
                Id = changeLog.AdoxioLicenseechangelogid,
                ChangeType = (LicenseeChangeType?)changeLog.AdoxioChangetype,
                BusinessType = (AdoxioApplicantTypeCodes?)changeLog.AdoxioBusinesstype,
                IsDirectorNew = changeLog.AdoxioIsdirectornew,
                IsDirectorOld = changeLog.AdoxioIsdirectorold,
                IsManagerNew = changeLog.AdoxioIsmanagernew,
                IsManagerOld = changeLog.AdoxioIsmanagerold,
                IsOfficerNew = changeLog.AdoxioIsofficernew,
                IsOfficerOld = changeLog.AdoxioIsofficerold,
                IsOwnerNew = changeLog.AdoxioIsownernew,
                IsOwnerOld = changeLog.AdoxioIsownerold,
                IsShareholderNew = changeLog.AdoxioIsshareholdernew,
                IsShareholderOld = changeLog.AdoxioIsshareholderold,
                IsTrusteeNew = changeLog.AdoxioIstrusteenew,
                IsTrusteeOld = changeLog.AdoxioIstrusteeold,
                NumberofSharesNew = changeLog.AdoxioNumberofsharesnew,
                NumberofSharesOld = changeLog.AdoxioNumberofsharesold,
                NumberOfNonVotingSharesNew = changeLog.AdoxioNumberofnonvotingsharesnew,
                NumberOfNonVotingSharesOld = changeLog.AdoxioNumberofnonvotingsharesold,
                TotalSharesNew = changeLog.AdoxioTotalsharesnew,
                TotalSharesOld = changeLog.AdoxioTotalsharesold,
                EmailNew = changeLog.AdoxioEmailnew,
                EmailOld = changeLog.AdoxioEmailold,
                FirstNameNew = changeLog.AdoxioFirstnamenew,
                FirstNameOld = changeLog.AdoxioFirstnameold,
                LastNameNew = changeLog.AdoxioLastnamenew,
                LastNameOld = changeLog.AdoxioLastnameold,
                BusinessNameNew = changeLog.AdoxioBusinessnamenew,
                BusinessNameOld = changeLog.AdoxioBusinesnameold,
                InterestPercentageNew = changeLog.AdoxioInterestpercentagenew,
                InterestPercentageOld = changeLog.AdoxioInterestpercentageold,

                DateofBirthNew = changeLog.AdoxioDateofbirthnew,
                DateofBirthOld = changeLog.AdoxioDateofbirthold,
                TitleOld = changeLog.AdoxioTitleold,
                TitleNew = changeLog.AdoxioTitlenew,

                LegalEntityId = changeLog._adoxioLegalentityidValue,
                ParentLegalEntityId = changeLog._adoxioParentlegalentityidValue,
                ParentLicenseeChangeLogId = changeLog._adoxioParentlinceseechangelogidValue, // Dynamics has a typo for this
                NumberOfMembers = changeLog.AdoxioNumberofmembers,
                AnnualMembershipFee = changeLog.AdoxioAnnualmembershipfee,
            };
            return result;
        }
    }
}
