using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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
                    result.Id = worker.AdoxioWorkerid;
                }

                result.isldbworker = worker.AdoxioIsldbworker == 1;
                result.firstname = worker.AdoxioFirstname;
                result.middlename = worker.AdoxioMiddlename;
                result.lastname = worker.AdoxioLastname;
                result.dateofbirth = (DateTime)worker.AdoxioDateofbirth;
                //result.gender = worker.AdoxioGendercode;
                result.birthplace = worker.AdoxioBirthplace;
                result.driverslicensenumber = worker.AdoxioDriverslicencenumber;
                result.bcidcardnumber = worker.AdoxioBcidcardnumber;
                result.phonenumber = worker.AdoxioPhonenumber;
                result.email = worker.AdoxioEmail;
                result.selfdisclosure = worker.AdoxioSelfdisclosure == 1;
                result.triggerphs = worker.AdoxioTriggerphs == 1;
                result.bCIDCardNumber = worker.AdoxioBcidcardnumber;
                result.contactId = worker._adoxioContactidValue;
                result.paymentReceived = worker.AdoxioPaymentreceived == 1;
                result.paymentRecievedDate = (DateTime)worker.AdoxioPaymentreceiveddate;
                result.workerId = worker.AdoxioWorkerid;
            }
            return result;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioWorker to, ViewModels.Worker from)
        {
            to.AdoxioIsldbworker = from.isldbworker ? 1 : 0;
            to.AdoxioFirstname = from.firstname;
            to.AdoxioMiddlename = from.middlename;
            to.AdoxioLastname = from.lastname;
            to.AdoxioDateofbirth = from.dateofbirth;
            // to.AdoxioGendercode = from.gender;
            to.AdoxioBirthplace = from.birthplace;
            to.AdoxioDriverslicencenumber = from.driverslicensenumber;
            to.AdoxioBcidcardnumber = from.bcidcardnumber;
            to.AdoxioPhonenumber = from.phonenumber;
            to.AdoxioEmail = from.email;
            to.AdoxioSelfdisclosure = from.selfdisclosure ? 1 : 0;
            to.AdoxioTriggerphs = from.triggerphs ? 1 : 0;
            to.AdoxioBcidcardnumber = from.bCIDCardNumber;
            to._adoxioContactidValue = from.contactId;
            to.AdoxioPaymentreceived = from.paymentReceived ? 1 : 0;
            to.AdoxioPaymentreceiveddate = from.paymentRecievedDate;
            to.AdoxioWorkerid = from.workerId;
        }
    }
}
