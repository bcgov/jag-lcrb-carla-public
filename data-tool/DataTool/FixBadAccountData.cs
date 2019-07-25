using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataTool
{
    class FixBadAccountData
    {
        public void Execute(IDynamicsClient _dynamicsClient, BCeIDBusinessQuery _bceidQuery)
        {
            // first find bad accounts
            string filter = "adoxio_externalid ne null and  (accountnumber eq '' or accountnumber eq null)";
            try
            {
                var badAccounts = _dynamicsClient.Accounts.Get(filter: filter).Value;

                Console.Out.WriteLine($"Found {badAccounts.Count} accounts missing the BN9 Account Number.");
                Console.Out.WriteLine($"Account ID - Account Name - Account Siteminder ID - BN9");
                foreach (var badAccount in badAccounts)
                {

                    // get the BN9.
                    string bn9 = "";
                    try
                    {
                        var result = _bceidQuery.ProcessBusinessQuery(badAccount.AdoxioExternalid).GetAwaiter().GetResult() ;
                        bn9 = result.businessNumber;

                    Console.Out.WriteLine($"{badAccount.Accountid} - {badAccount.Name} - {badAccount.AdoxioExternalid} - {bn9}");
                    }
                    catch (Exception e)
                    {
                        Console.Out.WriteLine("ERROR during get BCEID");
                        Console.Out.WriteLine(e.Message);
                    }
                    
                }

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Error getting bad accounts.");
                Console.Out.WriteLine(e.Message);
            }
            
        }

    }
}
