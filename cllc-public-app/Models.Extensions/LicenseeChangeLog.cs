extern alias DV;
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

    }
}
