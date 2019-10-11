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
            //toDynamics = fromVM.BusinessAccountType;
            toDynamics.AdoxioNumberofsharesnew = fromVM.NumberofSharesNew;
            toDynamics.AdoxioNumberofsharesold = fromVM.NumberofSharesOld;
            //toDynamics = fromVM.Statecode;
            //toDynamics = fromVM.Statuscode;
            toDynamics.AdoxioEmailnew = fromVM.EmailNew;
            toDynamics.AdoxioEmailold = fromVM.EmailOld;
            toDynamics.AdoxioFirstnamenew = fromVM.FirstNameNew;
            toDynamics.AdoxioFirstnameold = fromVM.FirstNameOld;
            //toDynamics = fromVM.JobNumber;
            toDynamics.AdoxioLastnamenew = fromVM.LastNameNew;
            toDynamics.AdoxioLastnameold = fromVM.LastNameOld;
            toDynamics.AdoxioName = fromVM.Name;
            toDynamics.AdoxioDateofbirthnew = fromVM.DateofBirthNew;
            toDynamics.AdoxioDateofbirthold = fromVM.DateofBirthOld;
            //toDynamics = fromVM.BusinessAccount;
            //toDynamics = fromVM.Contact;
            //toDynamics = fromVM.ParentBusinessAccount;
            //toDynamics = fromVM.ApplicationId;
            //toDynamics.le = fromVM.LegalEntityId;
            //toDynamics.pare = fromVM.ParentLinceseeChangeLogId;
        }

        public static LicenseeChangeLog ToViewModel(this MicrosoftDynamicsCRMadoxioLicenseechangelog changeLog)
        {
            var result = new LicenseeChangeLog()
            {
                Id = changeLog.AdoxioLicenseechangelogid,
                ChangeType = (LicenseeChangeType?)changeLog.AdoxioChangetype,
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
                //BusinessAccountType = changeLog,
                NumberofSharesNew = changeLog.AdoxioNumberofsharesnew,
                NumberofSharesOld = changeLog.AdoxioNumberofsharesold,
                //Statecode = changeLog,
                //Statuscode = changeLog,
                EmailNew = changeLog.AdoxioEmailnew,
                EmailOld = changeLog.AdoxioEmailold,
                FirstNameNew = changeLog.AdoxioFirstnamenew,
                FirstNameOld = changeLog.AdoxioFirstnameold,
                //JobNumber = changeLog,
                LastNameNew = changeLog.AdoxioLastnamenew,
                LastNameOld = changeLog.AdoxioLastnameold,
                Name = changeLog.AdoxioName,
                DateofBirthNew = changeLog.AdoxioDateofbirthnew,
                DateofBirthOld = changeLog.AdoxioDateofbirthold,

                //BusinessAccount = changeLog,
                //Contact = changeLog,
                //ParentBusinessAccount = changeLog,
                //ApplicationId = changeLog,
                LegalEntityId = changeLog._adoxioLegalentityValue,
                ParentLegalEntityId = changeLog._adoxioParentlegalentityidValue,
                ParentLinceseeChangeLogId = changeLog._adoxioParentlinceseechangelogidValue,
            };
            return result;
        }
    }
}
