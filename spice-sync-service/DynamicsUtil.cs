using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace Gov.Lclb.Cllb.SpdSync
{
    public static class DynamicsUtil
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
    }
}
