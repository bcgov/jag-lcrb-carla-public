using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DataTool
{
    class DetectBadAccountData
    {
        public enum AdoxioApplicationStatusCodes
        {
            Active = 1,
            Cancelled = 2,
            [EnumMember(Value = "In progress")]
            InProgress = 845280000,
            Intake = 845280001,
            Incomplete = 845280002,
            [EnumMember(Value = "Under Review")]
            UnderReview = 845280003,
            Approved = 845280004,
            [EnumMember(Value = "Pending for LG/FN/Police Feedback")]
            PendingForLGFNPFeedback = 845280006,
            Refused = 845280005,
            [EnumMember(Value = "Pending for Licence Fee")]
            PendingForLicenceFee = 845280007,
            Denied = 845280005,
            [EnumMember(Value = "Approved In Principle")]
            ApprovedInPrinciple = 845280008,
            Terminated = 845280009,
            [EnumMember(Value = "Terminated and refunded")]
            TerminatedAndRefunded = 845280010
        }

        public void Execute(IDynamicsClient _dynamicsClient)
        {
            // first find Cantest1
            string filter = "adoxio_externalid ne null and  (accountnumber eq '' or accountnumber eq null)";
            try
            {
                var badAccounts = _dynamicsClient.Accounts.Get(filter: filter).Value;

                Console.Out.WriteLine($"Found {badAccounts.Count} accounts missing the BN9 Account Number.");
                Console.Out.WriteLine($"Account ID - Account Name - Account Siteminder ID");

                int hasApplications = 0;

                foreach (var badAccount in badAccounts)
                {
                    Console.Out.WriteLine($"{badAccount.Accountid} - {badAccount.Name} - {badAccount.AdoxioExternalid}");

                    // now check to see if there are any related applications.
                    int appCount = 0;
                    string appFilter = $"_adoxio_applicant_value  eq {badAccount.Accountid} and statuscode ne {(int)AdoxioApplicationStatusCodes.InProgress} and statuscode ne {(int)AdoxioApplicationStatusCodes.Terminated}";
                    filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Denied}";
                    filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.Cancelled}";
                    filter += $" and statuscode ne {(int)AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                    try
                    {
                        var result = _dynamicsClient.Applications.Get(filter: appFilter).Value;
                        appCount = result.Count;
                    }
                    catch (Exception)
                    {

                    }

                    Console.WriteLine($"{appCount} related applications");

                    if (appCount > 0)
                    {
                        hasApplications++;
                    }
                }

                Console.Out.WriteLine($"Total with applications: {hasApplications}");

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error getting bad accounts.");
                Console.Out.WriteLine(e.Message);
            }
            
        }

    }
}
