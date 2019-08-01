using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

namespace Gov.Lclb.Cllb.OneStopService
{
    public class OneStopUtils
    {
        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private const int MAX_LICENCES_PER_INTERVAL = 2;

        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;

        private IOneStopRestClient _onestopRestClient;

        private ILogger _logger;


        public OneStopUtils(IConfiguration Configuration, ILogger logger)
        {
            this.Configuration = Configuration;
            _dynamics = DynamicsUtils.SetupDynamics(Configuration);
            _onestopRestClient = OneStopUtils.SetupOneStopClient(Configuration, logger);
            _logger = logger;
        }

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop.
        /// </summary>
        public async Task SendLicenceCreationMessage(PerformContext hangfireContext, string licenceGuidRaw, string suffix)
        {
            hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");
            string licenceGuid = DynamicsUtils.FormatGuidForDynamics(licenceGuidRaw);

            OneStopHubService.receiveFromPartnerResponse output;
            var serviceClient = new OneStopHubService.http___SOAP_BCPartnerPortTypeClient();
            serviceClient.ClientCredentials.UserName.UserName = Configuration["ONESTOP_HUB_USERNAME"];
            serviceClient.ClientCredentials.UserName.Password = Configuration["ONESTOP_HUB_PASSWORD"];
            var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            serviceClient.Endpoint.Binding = basicHttpBinding;

            using (new OperationContextScope(serviceClient.InnerChannel))
            {
                //Create message header containing the credentials
                var header = new OneStopServiceReference.SoapSecurityHeader("", Configuration["ONESTOP_HUB_USERNAME"],
                                                                            Configuration["ONESTOP_HUB_PASSWORD"], "");
                //Add the credentials message header to the outgoing request
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                try
                {
                    var req = new ProgramAccountRequest();
                    var innerXML = req.CreateXML(DynamicsUtils.GetLicenceFromDynamics(hangfireContext, _dynamics, licenceGuid, _logger), suffix);
                    var request = new OneStopHubService.receiveFromPartnerRequest(innerXML, "out");
                    output = serviceClient.receiveFromPartnerAsync(request).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    hangfireContext.WriteLine($"Exception occured. {ex.Message}");
                    hangfireContext.WriteLine($"Cancelling");

                    if (_logger != null)
                    {
                        _logger.LogError(ex.Message);
                        _logger.LogError(ex.StackTrace);
                    }

                    throw;
                }
            }
            hangfireContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(output));

            hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
        }
        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop using REST.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendLicenceCreationMessageREST(PerformContext hangfireContext, string licenceGuidRaw, string suffix)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");
            }
            

            string licenceGuid = DynamicsUtils.FormatGuidForDynamics(licenceGuidRaw);

            // prepare soap message
            var req = new ProgramAccountRequest();

            if (hangfireContext != null)
            {
                hangfireContext.WriteLine($"Getting Licence {licenceGuid}");
            }

            var licence = DynamicsUtils.GetLicenceFromDynamics(hangfireContext, _dynamics, licenceGuid, _logger);

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

                if (_logger != null)
                {
                    _logger.LogError($"Unable to get licence {licenceGuid}.");
                }
            }
            else
            {
                var innerXML = req.CreateXML(licence, suffix);
                // send message to Onestop hub
                var outputXML = await _onestopRestClient.receiveFromPartner(innerXML);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
                }
            }
        }



        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessage(PerformContext hangfireContext, string licenceGuidRaw)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");
            }
            
            string licenceGuid = DynamicsUtils.FormatGuidForDynamics(licenceGuidRaw);

            OneStopHubService.receiveFromPartnerResponse output;
            var serviceClient = new OneStopHubService.http___SOAP_BCPartnerPortTypeClient();
            serviceClient.ClientCredentials.UserName.UserName = Configuration["ONESTOP_HUB_USERNAME"];
            serviceClient.ClientCredentials.UserName.Password = Configuration["ONESTOP_HUB_PASSWORD"];
            var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            serviceClient.Endpoint.Binding = basicHttpBinding;

            using (new OperationContextScope(serviceClient.InnerChannel))
            {
                //Create message header containing the credentials
                var header = new OneStopServiceReference.SoapSecurityHeader("", Configuration["ONESTOP_HUB_USERNAME"],
                                                                            Configuration["ONESTOP_HUB_PASSWORD"], "");
                //Add the credentials message header to the outgoing request
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                try
                {
                    var req = new ProgramAccountDetailsBroadcast();
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Getting licence {licenceGuid}");
                    }
                        
                    MicrosoftDynamicsCRMadoxioLicences licence = DynamicsUtils.GetLicenceFromDynamics(hangfireContext, _dynamics, licenceGuid, _logger);

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
                    if (_logger != null)
                    {
                        _logger.LogError(ex.Message);
                        _logger.LogError(ex.StackTrace);
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
                hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
            }


        }

        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessageREST(PerformContext hangfireContext, string licenceGuidRaw)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST SendLicenceCreationMessage Job.");
            }
            
            string licenceGuid = DynamicsUtils.FormatGuidForDynamics(licenceGuidRaw);

            //prepare soap content
            var req = new ProgramAccountDetailsBroadcast();
            var licence = DynamicsUtils.GetLicenceFromDynamics(hangfireContext, _dynamics, licenceGuid, _logger);

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

                if (_logger != null)
                {
                    _logger.LogError($"Unable to get licence {licenceGuid}.");
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
                    hangfireContext.WriteLine("End ofOneStop REST SendLicenceCreationMessage  Job.");
                }
            }            
        }


        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for new licences job.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;
            try
            {
                string filter = $"adoxio_businessprogramaccountreferencenumber eq null";
                result = _dynamics.Licenceses.Get(filter: filter).Value;                
            }
            catch (OdataerrorException odee)
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
                if (currentItem < MAX_LICENCES_PER_INTERVAL)
                {
                    string licenceId = item.AdoxioLicencesid;
                    await SendLicenceCreationMessageREST(hangfireContext, licenceId, "001");
                    currentItem++;
                }
                else
                {
                    break; // exit foreach.
                }
                
            }

            hangfireContext.WriteLine("End of check for new licences job.");
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
                    logger.LogError($"ERROR - unable to parse suffix of {suffix} in partner note {partnerNote}");
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
