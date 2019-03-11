

using DataTool;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Newtonsoft.Json;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DemoTool
{
    /// <summary>
    /// Utility Program to backfill data in the event of a problem with BCeID
    /// This tool can also be used to generate an export of active users
    /// </summary>
    class Program
    {
        static Dictionary<string, string> ContactMap = new Dictionary<string, string>();
        static Dictionary<string, string> AccountMap = new Dictionary<string, string>();
        static Dictionary<string, string> WorkerMap = new Dictionary<string, string>();
        static Dictionary<string, string> AliasMap = new Dictionary<string, string>();
        static Dictionary<string, string> InvoiceMap = new Dictionary<string, string>();
        static Dictionary<string, string> LicenceMap = new Dictionary<string, string>();
        static Dictionary<string, string> ApplicationMap = new Dictionary<string, string>();
        static Dictionary<string, string> EstablishmentMap = new Dictionary<string, string>();
        static Dictionary<string, string> LegalEntityMap = new Dictionary<string, string>();
        static Dictionary<string, string> LocalgovindigenousnationMap = new Dictionary<string, string>();

        static DynamicsClient GetDynamicsConnection(IConfiguration Configuration)
        {
            string dynamicsOdataUri = Configuration["DYNAMICS_ODATA_URI"];
            string ssgUsername = Configuration["SSG_USERNAME"];
            string ssgPassword = Configuration["SSG_PASSWORD"];
            string dynamicsNativeOdataUri = Configuration["DYNAMICS_NATIVE_ODATA_URI"];

            string aadTenantId = Configuration["DYNAMICS_AAD_TENANT_ID"];
            string serverAppIdUri = Configuration["DYNAMICS_SERVER_APP_ID_URI"];
            string clientKey = Configuration["DYNAMICS_CLIENT_KEY"];
            string clientId = Configuration["DYNAMICS_CLIENT_ID"];

            ServiceClientCredentials serviceClientCredentials = null;

            if (string.IsNullOrEmpty(ssgUsername) || string.IsNullOrEmpty(ssgPassword))
            {
                var authenticationContext = new AuthenticationContext(
                "https://login.windows.net/" + aadTenantId);
                ClientCredential clientCredential = new ClientCredential(clientId, clientKey);
                var task = authenticationContext.AcquireTokenAsync(serverAppIdUri, clientCredential);
                task.Wait();
                var authenticationResult = task.Result;
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

            var _dynamicsClient = new DynamicsClient(new Uri(dynamicsOdataUri), serviceClientCredentials);
            return _dynamicsClient;
        }



        // the one parameter is the BCeID guid for an individual.
        static void Main(string[] args)
        {
            bool isObfuscate = false;

            bool isClean = false;

            bool isImport = true;

            bool isMove = true;
            

            // start by getting secrets.
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>();
            var Configuration = builder.Build();

            string basepath = Directory.GetParent(Directory.GetCurrentDirectory()).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath = Directory.GetParent(basepath).ToString();
            basepath += "\\data";
            string rawbase = basepath + "\\raw";
            string exportbase = basepath + "\\export";

            if (args.Length > 0)
            {
                string arg = args[0];
                if (!string.IsNullOrEmpty(arg))
                {
                    if (arg.ToLower().Equals("obfuscate"))
                    {
                        isObfuscate = true;
                        Console.Out.WriteLine("Data obfuscation enabled");
                    }
                    else if (arg.ToLower().Equals("import"))
                    {
                        isObfuscate = true;
                        Console.Out.WriteLine("Data import enabled");
                    }
                    else
                    {
                        Console.Out.WriteLine("USAGE - enter the obfuscate parameter to obfuscate data");
                    }
                }
            }


            if (isObfuscate)
            {
                var obfuscator = new Obfuscator(
                    ContactMap,
                    AccountMap,
                    WorkerMap,
                    AliasMap,
                    InvoiceMap,
                    LicenceMap,
                    ApplicationMap,
                    EstablishmentMap,
                    LegalEntityMap,
                    LocalgovindigenousnationMap
                    );

                string filename = $"{rawbase}\\accounts.json";
                List<MicrosoftDynamicsCRMaccount> accounts = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMaccount>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\contacts.json";
                List<MicrosoftDynamicsCRMcontact> contacts = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMcontact>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_aliases.json";
                List<MicrosoftDynamicsCRMadoxioAlias> adoxio_aliases = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioAlias>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_applications.json";
                List<MicrosoftDynamicsCRMadoxioApplication> adoxio_applications = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioApplication>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_establishments.json";
                List<MicrosoftDynamicsCRMadoxioEstablishment> adoxio_establishments = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioEstablishment>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_legalentities.json";
                List<MicrosoftDynamicsCRMadoxioLegalentity> adoxio_legalEntities = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioLegalentity>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_licences.json";
                List<MicrosoftDynamicsCRMadoxioLicences> adoxio_licences = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioLicences>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\adoxio_workers.json";
                List<MicrosoftDynamicsCRMadoxioWorker> adoxio_workers = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioWorker>>(File.ReadAllText(filename));

                filename = $"{rawbase}\\invoices.json";
                List<MicrosoftDynamicsCRMinvoice> invoices = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMinvoice>>(File.ReadAllText(filename));

                // obfuscate the data.

                var o_accounts = obfuscator.ObfuscateAccounts(accounts);
                var o_contacts = obfuscator.ObfuscateContacts(contacts);
                var o_aliases = obfuscator.ObfuscateAliases(adoxio_aliases);

                var o_invoices = obfuscator.ObfuscateInvoices(invoices);

                var o_establishments = obfuscator.ObfuscateEstablishments(adoxio_establishments);

                var o_applications = obfuscator.ObfuscateApplications(adoxio_applications);

                var o_workers = obfuscator.ObfuscateWorkers(adoxio_workers);

                var o_licences = obfuscator.ObfuscateLicences(adoxio_licences);

                var o_legalentities = obfuscator.ObfuscateLegalEntities(adoxio_legalEntities);

                // now save the data
                filename = $"{exportbase}\\accounts.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_accounts));
                filename = $"{exportbase}\\contacts.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_contacts));

                filename = $"{exportbase}\\adoxio_aliases.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_aliases));
                filename = $"{exportbase}\\adoxio_applications.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_applications));

                filename = $"{exportbase}\\adoxio_establishments.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_establishments));

                filename = $"{exportbase}\\adoxio_legalentities.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_legalentities));

                filename = $"{exportbase}\\adoxio_licences.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_licences));

                filename = $"{exportbase}\\adoxio_workers.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_workers));

                filename = $"{exportbase}\\invoices.json";
                File.WriteAllText(filename, JsonConvert.SerializeObject(o_invoices));


                /*
                // remove all BusinessContacts.
                var businessContacts = _dynamicsClient.Businesscontacts.Get().Value;

                foreach (var businessContact in businessContacts)
                {
                    try
                    {
                        _dynamicsClient.Businesscontacts.Delete(businessContact.BcgovBusinesscontactid);
                        Console.Out.WriteLine("Deleted BusinessContact " + businessContact.BcgovBusinesscontactid);
                    }
                    catch (OdataerrorException odee)
                    {
                        Console.Out.WriteLine("Error deleting business contact");
                        Console.Out.WriteLine("Request:");
                        Console.Out.WriteLine(odee.Request.Content);
                        Console.Out.WriteLine("Response:");
                        Console.Out.WriteLine(odee.Response.Content);
                    }
                }
            */
            }

            if (isClean)
            {
                var conn = GetDynamicsConnection(Configuration);
                Cleaner cleaner = new Cleaner();
                cleaner.Clean(conn);
            }

            if (isImport)
            {

                var importer = new Importer(
                    ContactMap,
                    AccountMap,
                    WorkerMap,
                    AliasMap,
                    InvoiceMap,
                    LicenceMap,
                    ApplicationMap,
                    EstablishmentMap,
                    LegalEntityMap,
                    LocalgovindigenousnationMap
                    );

                var conn = GetDynamicsConnection(Configuration);

                // read the exported data.
                string filename = $"{exportbase}\\accounts.json";
                List<MicrosoftDynamicsCRMaccount> accounts = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMaccount>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\contacts.json";
                List<MicrosoftDynamicsCRMcontact> contacts = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMcontact>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_aliases.json";
                List<MicrosoftDynamicsCRMadoxioAlias> aliases = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioAlias>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_applications.json";
                List<MicrosoftDynamicsCRMadoxioApplication> applications = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioApplication>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_establishments.json";
                List<MicrosoftDynamicsCRMadoxioEstablishment> establishments = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioEstablishment>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_legalentities.json";
                List<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioLegalentity>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_licences.json";
                List<MicrosoftDynamicsCRMadoxioLicences> licences = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioLicences>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_workers.json";
                List<MicrosoftDynamicsCRMadoxioWorker> workers = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioWorker>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\invoices.json";
                List<MicrosoftDynamicsCRMinvoice> invoices = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMinvoice>>(File.ReadAllText(filename));

                filename = $"{exportbase}\\adoxio_localgovindigenousnations.json";
                List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation> lgin = JsonConvert.DeserializeObject<List<MicrosoftDynamicsCRMadoxioLocalgovindigenousnation>>(File.ReadAllText(filename));

                // the order of import is important.

                Console.Out.WriteLine("Importing Accounts");

                importer.ImportAccounts(conn, accounts);

                Console.Out.WriteLine("Importing Contacts");

                importer.ImportContacts(conn, contacts);

                Console.Out.WriteLine("Importing Workers");

                importer.ImportWorkers(conn, workers );

                Console.Out.WriteLine("Importing Aliases");

                importer.ImportAliases(conn, aliases);

                Console.Out.WriteLine("Importing LG IN data");

                importer.ImportLocalGovIndigenousNations(conn, lgin);

                Console.Out.WriteLine("Importing Legal Entities");

                importer.ImportLegalEntities(conn, legalEntities);

                Console.Out.WriteLine("Importing Establishments");

                importer.ImportEstablishments(conn, establishments);

                Console.Out.WriteLine("Importing Invoices");

                importer.ImportInvoices(conn, invoices);

                Console.Out.WriteLine("Importing Licences");

                importer.ImportLicences(conn, licences);

                Console.Out.WriteLine("Importing Applications");

                importer.ImportApplications(conn, applications);
            }

            if (isMove)
            {
                var conn = GetDynamicsConnection(Configuration);
                Mover mover = new Mover();
                mover.Move(conn);
            }
   

        }
    }
}
