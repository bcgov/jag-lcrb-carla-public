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
                result.modifiedOn =  worker.Modifiedon;
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
            }
            return result;
        }


        public static void CopyValues(this MicrosoftDynamicsCRMadoxioWorker to, ViewModels.Worker from)
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
            } else
            {
                to.AdoxioGendercode = null;
            }
            if (from.Status != 0)
            {
                to.Statuscode = (int?)from.Status;
            } else
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
            to.AdoxioPhonenumber = from.phonenumber;
            to.AdoxioEmail = from.email;
            if (from.selfdisclosure != null)
            {
                to.AdoxioSelfdisclosure = from.selfdisclosure == true ? 1 : 0;
            }
            if (from.triggerphs != null)
            {
                to.AdoxioTriggerphs = from.triggerphs == true ? 1 : 0;
            }
            if (from.paymentReceived != null)
            {
                to.AdoxioPaymentreceived = from.paymentReceived == true ? 1 : 0;
            }
            to.AdoxioPaymentreceiveddate = from.paymentRecievedDate;
            to.AdoxioWorkerid = from.workerId;
            to.AdoxioCurrentaddressdatefrom = from.fromdate;
        }
    }
}
