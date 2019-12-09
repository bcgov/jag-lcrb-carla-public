using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            toDynamics.AdoxioIsshareholdernew = fromVM.IsShareholderNew;
            toDynamics.AdoxioIsshareholderold = fromVM.IsShareholderOld;
            toDynamics.AdoxioIstrusteenew = fromVM.IsTrusteeNew;
            toDynamics.AdoxioIstrusteeold = fromVM.IsTrusteeOld;
            toDynamics.AdoxioNumberofsharesnew = fromVM.NumberofSharesNew;
            toDynamics.AdoxioNumberofsharesold = fromVM.NumberofSharesOld;
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
        }

        public static LicenseeChangeLog ToViewModel(this MicrosoftDynamicsCRMadoxioLicenseechangelog changeLog)
        {
            var result = new LicenseeChangeLog()
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
                IsShareholderNew = changeLog.AdoxioIsshareholdernew,
                IsShareholderOld = changeLog.AdoxioIsshareholderold,
                IsTrusteeNew = changeLog.AdoxioIstrusteenew,
                IsTrusteeOld = changeLog.AdoxioIstrusteeold,
                NumberofSharesNew = changeLog.AdoxioNumberofsharesnew,
                NumberofSharesOld = changeLog.AdoxioNumberofsharesold,
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

                LegalEntityId = changeLog._adoxioLegalentityidValue,
                ParentLegalEntityId = changeLog._adoxioParentlegalentityidValue,
                ParentLinceseeChangeLogId = changeLog._adoxioParentlinceseechangelogidValue,
            };
            return result;
        }
    }
}
