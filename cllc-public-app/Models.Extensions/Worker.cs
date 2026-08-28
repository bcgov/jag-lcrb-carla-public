extern alias DV;
using System;
using Gov.Lclb.Cllb.Interfaces;
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

