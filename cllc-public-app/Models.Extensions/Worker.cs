extern alias DV;
using System;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using adoxio_worker = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker;
using adoxio_generalyesno = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using adoxio_gender = DV::Gov.Lclb.Cllb.Interfaces.adoxio_gender;
using adoxio_worker_statuscode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker_statuscode;
using adoxio_worker_statecode = DV::Gov.Lclb.Cllb.Interfaces.adoxio_worker_statecode;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class WorkerExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>
        public static ViewModels.Worker ToViewModel(this MicrosoftDynamicsCRMadoxioWorker worker)
        {
            ViewModels.Worker result = null;
            if (worker != null)
            {
                result = new ViewModels.Worker();
                if (worker.AdoxioWorkerid != null)
                {
                    result.id = worker.AdoxioWorkerid;
                }
                if (worker.AdoxioIsldbworker != null)
                {
                    result.isldbworker = worker.AdoxioIsldbworker == 1;
                }
                result.firstname = worker.AdoxioFirstname;
                result.middlename = worker.AdoxioMiddlename;
                result.lastname = worker.AdoxioLastname;
                result.dateofbirth = worker.AdoxioDateofbirth;
                result.modifiedOn = worker.Modifiedon;
                if (worker.AdoxioGendercode != null)
                {
                    result.gender = (ViewModels.Gender)worker.AdoxioGendercode;
                }
                if (worker.Statuscode != null)
                {
                    result.Status = (ViewModels.StatusCode)worker.Statuscode;
                }
                if (worker.Statecode != null)
                {
                    result.StateCode = (ViewModels.StatusCode)worker.Statecode;
                }
                result.birthplace = worker.AdoxioBirthplace;
                result.driverslicencenumber = worker.AdoxioDriverslicencenumber;
                result.bcidcardnumber = worker.AdoxioBcidcardnumber;
                result.phonenumber = worker.AdoxioPhonenumber;
                result.email = worker.AdoxioEmail;
                if (worker.AdoxioSelfdisclosure != null)
                {
                    result.selfdisclosure = worker.AdoxioSelfdisclosure == 1;
                }
                if (worker.AdoxioTriggerphs != null)
                {
                    result.triggerphs = worker.AdoxioTriggerphs == 1;
                }
                if (worker.AdoxioContactId != null)
                {
                    result.contact = worker.AdoxioContactId.ToViewModel();
                }
                if (worker.AdoxioPaymentreceived != null)
                {
                    result.paymentReceived = worker.AdoxioPaymentreceived == 1;
                }
                result.paymentRecievedDate = worker.AdoxioPaymentreceiveddate;
                result.workerId = worker.AdoxioWorkerid;
                result.fromdate = worker.AdoxioCurrentaddressdatefrom;
                result.ConsentToSecurityScreening = worker.AdoxioConsenttosecurityscreening;
                result.CertifyInformationIsCorrect = worker.AdoxioCertifyinformationiscorrect;
                result.ElectronicSignature = worker.AdoxioElectronicsignature;
            }
            return result;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioWorker to, ViewModels.Worker from)
        {
            to.AdoxioPhonenumber = from.phonenumber;
            to.AdoxioEmail = from.email;
            to.CopyValuesNoEmailPhone(from);
        }

        public static void CopyValuesNoEmailPhone(this MicrosoftDynamicsCRMadoxioWorker to, ViewModels.Worker from)
        {
            if (from.isldbworker != null)
            {
                to.AdoxioIsldbworker = from.isldbworker == true ? 1 : 0;
            }
            to.AdoxioFirstname = from.firstname;
            to.AdoxioMiddlename = from.middlename;
            to.AdoxioLastname = from.lastname;
            to.AdoxioDateofbirth = from.dateofbirth;
            if (from.gender != 0)
            {
                to.AdoxioGendercode = (int?)from.gender;
            }
            else
            {
                to.AdoxioGendercode = null;
            }
            if (from.Status != 0)
            {
                to.Statuscode = (int?)from.Status;
            }
            else
            {
                to.Statuscode = null;
            }

            if (from.StateCode != 0)
            {
                to.Statecode = (int?)from.StateCode;
            }
            else
            {
                to.Statecode = null;
            }
            to.AdoxioBirthplace = from.birthplace;
            to.AdoxioDriverslicencenumber = from.driverslicencenumber;
            to.AdoxioBcidcardnumber = from.bcidcardnumber;
            if (from.selfdisclosure != null)
            {
                to.AdoxioSelfdisclosure = from.selfdisclosure == true ? 1 : 0;
            }
            if (from.triggerphs != null)
            {
                to.AdoxioTriggerphs = from.triggerphs == true ? 1 : 0;
            }
            
            to.AdoxioWorkerid = from.workerId;
            to.AdoxioCurrentaddressdatefrom = from.fromdate;
            to.AdoxioConsenttosecurityscreening = from.ConsentToSecurityScreening;
            to.AdoxioCertifyinformationiscorrect = from.CertifyInformationIsCorrect;
            to.AdoxioElectronicsignature = from.ElectronicSignature;
        }

        // ---- Xrm.Sdk adoxio_worker extensions ----

        public static ViewModels.Worker ToViewModel(this adoxio_worker worker)
        {
            if (worker == null) return null;
            var result = new ViewModels.Worker();
            result.id = worker.Id.ToString();
            result.workerId = worker.Id.ToString();
            if (worker.adoxio_IsLDBWorker.HasValue)
                result.isldbworker = worker.adoxio_IsLDBWorker == adoxio_generalyesno.Yes;
            result.firstname = worker.adoxio_FirstName;
            result.middlename = worker.adoxio_MiddleName;
            result.lastname = worker.adoxio_LastName;
            result.dateofbirth = worker.adoxio_DateofBirth.HasValue
                ? (DateTimeOffset?)worker.adoxio_DateofBirth.Value
                : null;
            result.modifiedOn = worker.ModifiedOn.HasValue
                ? (DateTimeOffset?)worker.ModifiedOn.Value
                : null;
            if (worker.adoxio_GenderCode.HasValue)
                result.gender = (ViewModels.Gender)(int)worker.adoxio_GenderCode.Value;
            if (worker.statuscode.HasValue)
                result.Status = (ViewModels.StatusCode)(int)worker.statuscode.Value;
            if (worker.statecode.HasValue)
                result.StateCode = (ViewModels.StatusCode)(int)worker.statecode.Value;
            result.birthplace = worker.adoxio_Birthplace;
            result.driverslicencenumber = worker.adoxio_DriversLicenceNumber;
            result.bcidcardnumber = worker.adoxio_BCIDCardNumber;
            result.phonenumber = worker.adoxio_PhoneNumber;
            result.email = worker.adoxio_Email;
            if (worker.adoxio_SelfDisclosure.HasValue)
                result.selfdisclosure = worker.adoxio_SelfDisclosure == adoxio_generalyesno.Yes;
            if (worker.adoxio_TriggerPHS.HasValue)
                result.triggerphs = worker.adoxio_TriggerPHS == adoxio_generalyesno.Yes;
            if (worker.adoxio_ContactId != null)
                result.contact = new ViewModels.Contact { id = worker.adoxio_ContactId.Id.ToString() };
            if (worker.adoxio_PaymentReceived.HasValue)
                result.paymentReceived = worker.adoxio_PaymentReceived == adoxio_generalyesno.Yes;
            result.paymentRecievedDate = worker.adoxio_PaymentReceivedDate.HasValue
                ? (DateTimeOffset?)worker.adoxio_PaymentReceivedDate.Value
                : null;
            result.fromdate = worker.adoxio_CurrentAddressDateFrom.HasValue
                ? (DateTimeOffset?)worker.adoxio_CurrentAddressDateFrom.Value
                : null;
            result.ConsentToSecurityScreening = worker.adoxio_ConsenttoSecurityScreening;
            result.CertifyInformationIsCorrect = worker.adoxio_CertifyInformationIsCorrect;
            result.ElectronicSignature = worker.adoxio_ElectronicSignature;
            return result;
        }

        public static void CopyValues(this adoxio_worker to, ViewModels.Worker from)
        {
            to.adoxio_PhoneNumber = from.phonenumber;
            to.adoxio_Email = from.email;
            to.CopyValuesNoEmailPhone(from);
        }

        public static void CopyValuesNoEmailPhone(this adoxio_worker to, ViewModels.Worker from)
        {
            if (from.isldbworker != null)
                to.adoxio_IsLDBWorker = from.isldbworker == true ? adoxio_generalyesno.Yes : adoxio_generalyesno.No;
            to.adoxio_FirstName = from.firstname;
            to.adoxio_MiddleName = from.middlename;
            to.adoxio_LastName = from.lastname;
            to.adoxio_DateofBirth = from.dateofbirth?.DateTime;
            if (from.gender != 0)
                to.adoxio_GenderCode = (adoxio_gender?)(int?)from.gender;
            else
                to.adoxio_GenderCode = null;
            if (from.Status != 0)
                to.statuscode = (adoxio_worker_statuscode?)(int?)from.Status;
            else
                to.statuscode = null;
            if (from.StateCode != 0)
                to.statecode = (adoxio_worker_statecode?)(int?)from.StateCode;
            else
                to.statecode = null;
            to.adoxio_Birthplace = from.birthplace;
            to.adoxio_DriversLicenceNumber = from.driverslicencenumber;
            to.adoxio_BCIDCardNumber = from.bcidcardnumber;
            if (from.selfdisclosure != null)
                to.adoxio_SelfDisclosure = from.selfdisclosure == true ? adoxio_generalyesno.Yes : adoxio_generalyesno.No;
            if (from.triggerphs != null)
                to.adoxio_TriggerPHS = from.triggerphs == true ? adoxio_generalyesno.Yes : adoxio_generalyesno.No;
            to.adoxio_CurrentAddressDateFrom = from.fromdate?.DateTime;
            to.adoxio_ConsenttoSecurityScreening = from.ConsentToSecurityScreening;
            to.adoxio_CertifyInformationIsCorrect = from.CertifyInformationIsCorrect;
            to.adoxio_ElectronicSignature = from.ElectronicSignature;
        }
    }
}

