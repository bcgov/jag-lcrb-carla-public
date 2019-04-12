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
    class ApplicationCleaner
    {
        public void Clean(DynamicsClient _dynamicsClient)
        {

            try
            {
                var select = new List<String> { "adoxio_applicationid" };
                // remove all Applications.
                var applications = _dynamicsClient.Applications.Get(select: select).Value;

                foreach (var application in applications)
                {
                    try
                    {
                        _dynamicsClient.Applications.Delete(application.AdoxioApplicationid);
                        Console.Out.WriteLine("Deleted Application " + application.AdoxioApplicationid);
                    }
                    catch (OdataerrorException odee)
                    {
                        Console.Out.WriteLine("Error deleting application");
                        Console.Out.WriteLine("Request:");
                        Console.Out.WriteLine(odee.Request.Content);
                        Console.Out.WriteLine("Response:");
                        Console.Out.WriteLine(odee.Response.Content);
                    }
                }
            }
            catch (OdataerrorException odee)
            {
                Console.Out.WriteLine("Error getting applications");
                Console.Out.WriteLine("Request:");
                Console.Out.WriteLine(odee.Request.Content);
                Console.Out.WriteLine("Response:");
                Console.Out.WriteLine(odee.Response.Content);
                throw (odee);
            }

            


        }

    }
}
