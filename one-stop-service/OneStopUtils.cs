using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WebApplicationSoap.OneStop;
using Microsoft.Extensions.Caching.Memory;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class OneStopUtils
    {
        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private const int MAX_LICENCES_PER_INTERVAL = 10;

        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration _configuration { get; }



        private IOneStopRestClient _onestopRestClient;

        private IMemoryCache _cache;

        public OneStopUtils(IConfiguration Configuration, IMemoryCache cache)
        {
            this._configuration = Configuration;
            _cache = cache;
            

            _onestopRestClient = OneStopUtils.SetupOneStopClient(Configuration, Log.Logger);

            
        }

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop using REST.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendProgramAccountRequestREST(PerformContext hangfireContext, string licenceGuidRaw, string suffix)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop ProgramAccountRequest Job.");
            }
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            // prepare soap message
            var req = new ProgramAccountRequest();

            if (hangfireContext != null)
            {
                hangfireContext.WriteLine($"Getting Licence {licenceGuid}");
            }

            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence {licenceGuid}.");
                }
            }
            else
            {
                // only send the request if Dynamics says the licence is not sent yet.
                if (licence.AdoxioOnestopsent == null || licence.AdoxioOnestopsent == false)
                {

                    var innerXML = req.CreateXML(licence, suffix);
                    // send message to Onestop hub
                    var outputXML = await _onestopRestClient.receiveFromPartner(innerXML);

                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine(outputXML);
                    }
                }
                else
                {
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Skipping ProgramAccountRequest for Licence {licence.AdoxioName} {licenceGuid} as the record is marked as sent to OneStop.");
                    }
                    if (Log.Logger != null)
                    {
                        Log.Logger.Error($"Skipping ProgramAccountRequest for Licence {licence.AdoxioName} {licenceGuid} as the record is marked as sent to OneStop.");
                    }                    
                }

            }


            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("End of OneStop ProgramAccountRequest  Job.");
            }
            

        }



        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessage(PerformContext hangfireContext, string licenceGuidRaw)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop ProgramAccountRequest Job.");
            }
            
            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            OneStopHubService.receiveFromPartnerResponse output;
            var serviceClient = new OneStopHubService.http___SOAP_BCPartnerPortTypeClient();
            serviceClient.ClientCredentials.UserName.UserName = _configuration["ONESTOP_HUB_USERNAME"];
            serviceClient.ClientCredentials.UserName.Password = _configuration["ONESTOP_HUB_PASSWORD"];
            var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            serviceClient.Endpoint.Binding = basicHttpBinding;

            using (new OperationContextScope(serviceClient.InnerChannel))
            {
                //Create message header containing the credentials
                var header = new OneStopServiceReference.SoapSecurityHeader("", _configuration["ONESTOP_HUB_USERNAME"],
                                                                            _configuration["ONESTOP_HUB_PASSWORD"], "");
                //Add the credentials message header to the outgoing request
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                try
                {
                    var req = new ProgramAccountDetailsBroadcast();
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Getting licence {licenceGuid}");
                    }
                        
                    MicrosoftDynamicsCRMadoxioLicences licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine("Got licence. Creating XML request");
                    }

                    var innerXML = req.CreateXML(licence);
                    hangfireContext.WriteLine("Sending request.");
                    var request = new OneStopHubService.receiveFromPartnerRequest(innerXML, "out");
                    output = serviceClient.receiveFromPartnerAsync(request).GetAwaiter().GetResult();
                    
                    

                    
                }
                catch (Exception ex)
                {
                    if (Log.Logger != null)
                    {
                        Log.Logger.Error(ex, "Error sending request.");
                    }

                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine("Error sending program account details broadcast:");
                        hangfireContext.WriteLine(ex.Message);
                    }
                    
                    throw;
                }
            }

            if (hangfireContext != null)
            {
                hangfireContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(output));
                hangfireContext.WriteLine("End of OneStop ProgramAccountDetails  Job.");
            }


        }

        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessageREST(PerformContext hangfireContext, string licenceGuidRaw)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST ProgramAccountDetailsBroadcast Job.");
            }
            
            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ProgramAccountDetailsBroadcast();
            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence {licenceGuid}.");
                }
            }
            else
            {
                var innerXML = req.CreateXML(licence);

                //send message to Onestop hub
                var outputXML = await _onestopRestClient.receiveFromPartner(innerXML);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End of OneStop REST ProgramAccountDetailsBroadcast  Job.");
                }
            }            
        }


        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for new licences for onestop job.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;

            /*
            try
            {
                string filter = $"adoxio_businessprogramaccountreferencenumber ne null";
                
                result = _dynamics.Licenceses.Get(filter: filter).Value;
            }
            catch (HttpOperationException odee)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            foreach (var item in result)
            {
                var patchRecord = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioOnestopsent = true
                };
                _dynamics.Licenceses.Update(item.AdoxioLicencesid, patchRecord);
            }
            */

                try
            {
                string filter = $"adoxio_onestopsent ne true and statuscode eq 1";
                string[] expand = { "adoxio_establishment" };
                result = dynamicsClient.Licenceses.Get(filter: filter, expand: expand).Value;                
            }
            catch (HttpOperationException odee)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            int currentItem = 0;
            // now for each one process it.
            foreach (var item in result)
            {
                if (item.AdoxioOnestopsent != true)
                {
                    // Do not attempt to send licence records that have no establishment (for example, Marketer Licence records)
                    if (item.AdoxioEstablishment != null)
                    {
                        string licenceId = item.AdoxioLicencesid;
                        string programAccountCode = "001";
                        if (item.AdoxioBusinessprogramaccountreferencenumber != null)
                        {
                            programAccountCode = item.AdoxioBusinessprogramaccountreferencenumber;
                        }

                        // set the maximum code.
                        string cacheKey = "_BPAR_" + item.AdoxioLicencesid;
                        int newNumber = 10;
                        string suffix = programAccountCode.TrimStart('0');
                        if (int.TryParse(suffix, out newNumber))
                        {
                            newNumber += 10; // 10 tries.                           
                        }
                        _cache.Set(cacheKey, newNumber);

                        if (hangfireContext != null)
                        {
                            hangfireContext.WriteLine($"SET key {cacheKey} to {newNumber}");
                        }
                            await SendProgramAccountRequestREST(hangfireContext, licenceId, suffix);
                        currentItem++;
                    }

                    if (currentItem > MAX_LICENCES_PER_INTERVAL)
                    {
                        break; // exit foreach    
                    }
                }
                
                                
            }

            hangfireContext.WriteLine("End of check for new licences for onestop job.");
        }




        public static IOneStopRestClient SetupOneStopClient(IConfiguration Configuration, ILogger logger)
        {
            //create authorization header 
            var byteArray = Encoding.ASCII.GetBytes($"{Configuration["ONESTOP_HUB_USERNAME"]}:{Configuration["ONESTOP_HUB_PASSWORD"]}");
            string authorization = "Basic " + Convert.ToBase64String(byteArray);
            
            //create client
            var client = new OneStopRestClient(new Uri(Configuration["ONESTOP_HUB_REST_URI"]), authorization, logger);
            return client;
        }
        
        /// <summary>
        /// Extract a guid from a partnerNote.
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <returns></returns>
        public static string GetGuidFromPartnerNote(string partnerNote)
        {
            string result = null;
            string[] parts = partnerNote.Split(",");
            result = parts[0];                
            
            return result;
        }

        /// <summary>
        /// Extract a suffix from a partnerNote
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <returns></returns>
        public static int GetSuffixFromPartnerNote(string partnerNote, ILogger logger)
        {
            int result = 0;
            int strPos = partnerNote.LastIndexOf("-");
            if (strPos > -1)
            {
                string suffix = partnerNote.Substring(strPos + 1);

                suffix = suffix.TrimStart('0');
                if (!int.TryParse(suffix, out result))
                {
                    logger.Error($"ERROR - unable to parse suffix of {suffix} in partner note {partnerNote}");
                }
            }
            
            return result;
        }

        /// <summary>
        /// Extract a Licence Number from a partnerNote.
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <returns></returns>
        public static string GetLicenceNumberFromPartnerNote(string partnerNote)
        {
            string result = null;
            string[] parts = partnerNote.Split(",");
            if (parts.Length > 1)
            {
                string secondString = parts[1];
                string[] secondParts = secondString.Split("-");
                result = secondParts[0];                
            }
            return result;
        }
    }
}
