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
    class Mover
    {
        public void Move(DynamicsClient _dynamicsClient)
        {
            // first find Cantest1
            string filter = "name eq 'XYZ Cannabis Sales Test'";
            try
            {
                var cantest1 = _dynamicsClient.Accounts.Get(filter: filter).Value.FirstOrDefault();

                if (cantest1 != null)
                {
                    var applications = _dynamicsClient.Applications.Get().Value;

                    foreach (var application in applications)
                    {
                        try
                        {
                            MicrosoftDynamicsCRMadoxioApplication newItem = new MicrosoftDynamicsCRMadoxioApplication()
                            {
                                AdoxioApplicantODataBind = _dynamicsClient.GetEntityURI("accounts", cantest1.Accountid)
                            };
                            _dynamicsClient.Applications.Update(application.AdoxioApplicationid, newItem);
                        }
                        catch (OdataerrorException odee)
                        {
                            Console.Out.WriteLine("Error updating application");
                            Console.Out.WriteLine("Request:");
                            Console.Out.WriteLine(odee.Request.Content);
                            Console.Out.WriteLine("Response:");
                            Console.Out.WriteLine(odee.Response.Content);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Unable to find Cantest1.");
            }
            




            

        }

    }
}
