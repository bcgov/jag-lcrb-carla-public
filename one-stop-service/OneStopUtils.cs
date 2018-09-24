using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Pop3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
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

        public OneStopUtils(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this._dynamics = SetupDynamics();
        }

        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public async Task SendLicenceCreationMessage(PerformContext hangfireContext, string licenceGuild)
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
                    var innerXML = req.CreateXML(GetLicenceFromDynamics(hangfireContext));
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

        private MicrosoftDynamicsCRMadoxioLicences GetLicenceFromDynamics(PerformContext hangfireContext, string guid = "2287f8c8-0853-e811-8140-480fcfeac941")
        {
            MicrosoftDynamicsCRMadoxioApplication result;
            try
            {
                string filter = $"adoxio_applicationid eq {guid}";
                result = _dynamics.Applications.Get(filter: filter).Value.FirstOrDefault();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting Application");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }
            return result;
        }

        private IDynamicsClient SetupDynamics()
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


    }
}
