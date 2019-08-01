using System;
using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class DynamicsUtils
    {
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

        public static MicrosoftDynamicsCRMadoxioLicences GetLicenceFromDynamics(PerformContext hangfireContext, IDynamicsClient dynamicsClient, string licenceGuidRaw, ILogger logger)
        {
            MicrosoftDynamicsCRMadoxioLicences result;
            string licenceGuid = DynamicsUtils.FormatGuidForDynamics(licenceGuidRaw);
            try
            {
                string filter = $"adoxio_licencesid eq {licenceGuid}";
                // adoxio_Licencee,adoxio_establishment
                // Note that adoxio_Licencee is the Account linked to the licence
                var expand = new List<string> { "adoxio_Licencee", "adoxio_establishment", "adoxio_LicenceType" };
                result = dynamicsClient.Licenceses.GetByKey(licenceGuid, expand: expand);
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
                if (logger != null)
                {
                    logger.LogError("Error getting Licence");
                    logger.LogError("Request:");
                    logger.LogError(odee.Request.Content);
                    logger.LogError("Response:");
                    logger.LogError(odee.Response.Content);
                }
                // return null if we can't get results.
                result = null;
            }

            if (result != null && result.AdoxioLicencee != null)
            {
                if (!string.IsNullOrEmpty(result.AdoxioLicencee._primarycontactidValue))
                {
                    // get the contact.
                    var runner = dynamicsClient.GetContactById(Guid.Parse(result.AdoxioLicencee._primarycontactidValue));
                    runner.Wait();
                    result.AdoxioLicencee.Primarycontactid = runner.Result;
                }
            }

            return result;
        }

        public static string FormatGuidForDynamics(string guid)
        {
            string result = null;
            Guid guidValue;
            if (Guid.TryParse(guid, out guidValue))
            {
                result = guidValue.ToString();
            }
            return result;
        }
    }
}
