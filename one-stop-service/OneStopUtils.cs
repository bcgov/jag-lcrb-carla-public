using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
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
            _dynamics = OneStopUtils.SetupDynamics(Configuration);
            _onestopRestClient = OneStopUtils.SetupOneStopClient(Configuration, logger);
            _logger = logger;
        }

        private string FormatGuidForDynamics(string guid)
        {
            string result = null;
            Guid guidValue;
            if (Guid.TryParse(guid, out guidValue))
            {
                result = guidValue.ToString();
            }
            return result;
        }
        

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop.
        /// </summary>
        public async Task SendLicenceCreationMessage(PerformContext hangfireContext, string licenceGuidRaw, string suffix)
        {
            hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");
            string licenceGuid = FormatGuidForDynamics(licenceGuidRaw);

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
                    var innerXML = req.CreateXML(GetLicenceFromDynamics(hangfireContext, licenceGuid), suffix);
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
        public async Task SendLicenceCreationMessageREST(PerformContext hangfireContext, string licenceGuidRaw, string suffix)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");
            }
            

            string licenceGuid = FormatGuidForDynamics(licenceGuidRaw);

            // prepare soap message
            var req = new ProgramAccountRequest();

            if (hangfireContext != null)
            {
                hangfireContext.WriteLine($"Getting Licence {licenceGuid}");
            }

            var licence = GetLicenceFromDynamics(hangfireContext, licenceGuid);

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
            
            string licenceGuid = FormatGuidForDynamics(licenceGuidRaw);

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
                        
                    MicrosoftDynamicsCRMadoxioLicences licence = GetLicenceFromDynamics(hangfireContext, licenceGuid);

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
            
            string licenceGuid = FormatGuidForDynamics(licenceGuidRaw);

            //prepare soap content
            var req = new ProgramAccountDetailsBroadcast();
            var licence = GetLicenceFromDynamics(hangfireContext, licenceGuid);

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
                result = _dynamics.Licenses.Get(filter: filter).Value;                
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


        private MicrosoftDynamicsCRMadoxioLicences GetLicenceFromDynamics(PerformContext hangfireContext, string licenceGuidRaw)
        {
            MicrosoftDynamicsCRMadoxioLicences result;
            string licenceGuid = FormatGuidForDynamics(licenceGuidRaw);
            try
            {
                string filter = $"adoxio_licencesid eq {licenceGuid}";
                // adoxio_Licencee,adoxio_establishment
                // Note that adoxio_Licencee is the Account linked to the licence
                var expand = new List<string> { "adoxio_Licencee", "adoxio_establishment" };
                result = _dynamics.Licenses.GetByKey(licenceGuid, expand: expand);
            }
            catch (OdataerrorException odee)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine("Error getting Licence");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }
                if (_logger != null)
                {
                    _logger.LogError("Error getting Licence");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                }
                // return null if we can't get results.
                result = null;
            }

            return result;
        }

        public static IDynamicsClient SetupDynamics(IConfiguration Configuration)
        {

            string dynamicsOdataUri = Configuration["DYNAMICS_ODATA_URI"];
            string aadTenantId = Configuration["DYNAMICS_AAD_TENANT_ID"];
            string serverAppIdUri = Configuration["DYNAMICS_SERVER_APP_ID_URI"];
            string clientKey = Configuration["DYNAMICS_CLIENT_KEY"];
            string clientId = Configuration["DYNAMICS_CLIENT_ID"];

            string ssgUsername = Configuration["SSG_USERNAME"];
            string ssgPassword = Configuration["SSG_PASSWORD"];

            AuthenticationResult authenticationResult = null;
            // authenticate using ADFS.
            if (string.IsNullOrEmpty(ssgUsername) || string.IsNullOrEmpty(ssgPassword))
            {
                var authenticationContext = new AuthenticationContext(
                    "https://login.windows.net/" + aadTenantId);
                ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
                var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
                task.Wait();
                authenticationResult = task.Result;
            }

            ServiceClientCredentials serviceClientCredentials = null;

            if (string.IsNullOrEmpty(ssgUsername) || string.IsNullOrEmpty(ssgPassword))
            {
                var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/" + aadTenantId);
                ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
                var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
                task.Wait();
                authenticationResult = task.Result;
                string token = authenticationResult.CreateAuthorizationHeader().Substring("Bearer ".Length);
                serviceClientCredentials = new TokenCredentials(token);
            }
            else
            {
                serviceClientCredentials = new BasicAuthenticationCredentials()
                {
                    UserName = ssgUsername,
                    Password = ssgPassword
                };
            }

            IDynamicsClient client = new DynamicsClient(new Uri(Configuration["DYNAMICS_ODATA_URI"]), serviceClientCredentials);

            // set the native client URI
            if (string.IsNullOrEmpty(Configuration["DYNAMICS_NATIVE_ODATA_URI"]))
            {
                client.NativeBaseUri = new Uri(Configuration["DYNAMICS_ODATA_URI"]);
            }
            else
            {
                client.NativeBaseUri = new Uri(Configuration["DYNAMICS_NATIVE_ODATA_URI"]);
            }

            return client;
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
        public static int GetSuffixFromPartnerNote(string partnerNote)
        {
            int result = 0;
            string[] parts = partnerNote.Split("-");
            if (parts.Length > 1)
            {
                string suffix = parts[1];
                int.TryParse(suffix, out result);
            }
            return result;
        }

        /// <summary>
        /// Extract a  from a partnerNote.
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
