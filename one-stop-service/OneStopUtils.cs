using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
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
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;

        private IOneStopRestClient _onestopRestClient;

        public OneStopUtils(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this._dynamics = OneStopUtils.SetupDynamics(Configuration);
            this._onestopRestClient = OneStopUtils.SetupOneStopClient(Configuration);
        }

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop.
        /// </summary>
        public async Task SendLicenceCreationMessage(PerformContext hangfireContext, string licenceGuid, string suffix)
        {
            hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");


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
                    throw;
                }
            }
            hangfireContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(output));

            hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
        }
        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop using REST.
        /// </summary>
        public async Task SendLicenceCreationMessageREST(PerformContext hangfireContext, string licenceGuid, string suffix)
        {
            hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");

            // prepare soap message
            var req = new ProgramAccountRequest();
            var innerXML = req.CreateXML(GetLicenceFromDynamics(hangfireContext, licenceGuid), suffix);
            // send message to Onestop hub
            var outputXML = await _onestopRestClient.receiveFromPartner(innerXML);

            hangfireContext.WriteLine(outputXML);
            hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
        }



        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessage(PerformContext hangfireContext, string licenceGuid)
        {
            hangfireContext.WriteLine("Starting OneStop SendLicenceCreationMessage Job.");


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
                    var innerXML = req.CreateXML(GetLicenceFromDynamics(hangfireContext, licenceGuid));
                    var request = new OneStopHubService.receiveFromPartnerRequest(innerXML, "out");
                    output = serviceClient.receiveFromPartnerAsync(request).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    hangfireContext.WriteLine("Error sending program account details broadcast:");
                    hangfireContext.WriteLine(ex.Message);
                    throw;
                }
            }
            hangfireContext.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(output));

            hangfireContext.WriteLine("End ofOneStop SendLicenceCreationMessage  Job.");
        }

        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessageREST(PerformContext hangfireContext, string licenceGuid)
        {
            hangfireContext.WriteLine("Starting OneStop REST SendLicenceCreationMessage Job.");

            //prepare soap content
            var req = new ProgramAccountDetailsBroadcast();
            var innerXML = req.CreateXML(GetLicenceFromDynamics(hangfireContext, licenceGuid));

            //send message to Onestop hub
            var outputXML = await _onestopRestClient.receiveFromPartner(innerXML);

            hangfireContext.WriteLine(outputXML);
            hangfireContext.WriteLine("End ofOneStop REST SendLicenceCreationMessage  Job.");
        }


        private MicrosoftDynamicsCRMadoxioLicences GetLicenceFromDynamics(PerformContext hangfireContext, string guid)
        {
            MicrosoftDynamicsCRMadoxioLicences result;
            try
            {
                string filter = $"adoxio_licencesid eq {guid}";
                var expand = new List<string> { "adoxio_Licencee", "adoxio_establishment" };
                result = _dynamics.Licenses.Get(filter: filter, expand: expand).Value.FirstOrDefault();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting Licence");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
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

        public static IOneStopRestClient SetupOneStopClient(IConfiguration Configuration)
        {
            //create authorization header 
            var byteArray = Encoding.ASCII.GetBytes($"{Configuration["ONESTOP_HUB_USERNAME"]}:{Configuration["ONESTOP_HUB_PASSWORD"]}");
            string authorization = "Basic " + Convert.ToBase64String(byteArray);
            
            //create client
            var client = new OneStopRestClient(new Uri(Configuration["ONESTOP_HUB_REST_URI"]), authorization);
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
