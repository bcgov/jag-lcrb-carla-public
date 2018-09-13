using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Pop3;
using SpdSync;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpdUtils
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;

        public SpdUtils(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this._dynamics = SetupDynamics();
        }

        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public void SendExportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Export Job.");

            Type type = typeof(MicrosoftDynamicsCRMadoxioSpddatarow);

            var csvList = new List<List<string>>();
            var headers = new List<string>();
            var headerDefinition = GetExportHeadersWorker();
            headerDefinition.ForEach(h =>
            {
                headers.Add($"\"{h.Value}\"");
            });

            csvList.Add(headers);

            string filter = $"adoxio_isexport eq true and adoxio_exporteddate eq null";
            List<MicrosoftDynamicsCRMadoxioSpddatarow> result = null;

            try
            {
                result = _dynamics.Spddatarows.Get(filter: filter).Value.ToList();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting SPD data rows");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }

            var batch = GetBatchNumber().ToString();
            batch = AddZeroPadding(batch);

            if (result != null && result.Count > 0)
            {
                foreach (var row in result)
                {
                    var item = new List<string>();

                    foreach (var h in headerDefinition)
                    {
                        try
                        {
                            object value = row[h.Key];
                            if (value != null)
                            {
                                item.Add($"\"{value.ToString()}\"");
                            }
                            else
                            {
                                item.Add("\"\"");
                            }

                        }
                        catch (Exception)
                        {
                            item.Add("\"\""); ;
                        }
                    }
                    csvList.Add(item);

                }

                var csv = new StringBuilder();
                csvList.ForEach(row =>
                {
                    var line = String.Join(",", row);
                    csv.AppendLine(line);
                });
                var datePart = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                
                var attachmentName = $@"{batch}_Request_Worker_{datePart}.csv";
                //File.WriteAllText($@".\{attachmentName}", csv.ToString());

                if (SendSPDEmail(csv.ToString(), attachmentName))
                {
                    hangfireContext.WriteLine("Sent SPD email. Now updating Dynamics...");
                    //update exporteddate in dynamics
                    var exportDate = DateTime.Now;
                    result.ForEach(row =>
                    {
                        var patchApplication = new MicrosoftDynamicsCRMadoxioSpddatarow()
                        {
                            AdoxioExporteddate = exportDate
                        };
                        try
                        {
                            if (row.AdoxioSpddatarowid != null) // skip test data
                            {
                                _dynamics.Spddatarows.Update(row.AdoxioSpddatarowid, patchApplication);
                            }
                        }
                        catch (OdataerrorException odee)
                        {
                            hangfireContext.WriteLine("Error updating application");
                            hangfireContext.WriteLine("Request:");
                            hangfireContext.WriteLine(odee.Request.Content);
                            hangfireContext.WriteLine("Response:");
                            hangfireContext.WriteLine(odee.Response.Content);
                            // fail if we can't create.
                            throw (odee);
                        }
                    });
                    hangfireContext.WriteLine("Dynamics update complete.");
                }
                else
                {
                    hangfireContext.WriteLine("Error sending SPD email.");
                }
            }
            else
            {
                hangfireContext.WriteLine("No data to send, sending a null email.");
                SendSPDNoResultsEmail(batch);
            }

            hangfireContext.WriteLine("End of SPD Export Job.");
        }

        private long GetBatchNumber()
        {
            string filter = $"adoxio_isexport eq true and adoxio_exporteddate ne null";
            var select = new List<string>() { "adoxio_exporteddate" };
            var batchesRun = _dynamics.Spddatarows.Get(filter: filter, select: select)
                                .Value.ToList()
                                .Select(r => r.AdoxioExporteddate)
                                .Distinct()
                                .Count();

            return batchesRun + 1;
        }

        private string AddZeroPadding(string input, int maxLength = 8)
        {
            while (input.Length < maxLength)
            {
                input = "0" + input;
            }
            return input;
        }

        /// <summary>
        /// Check the import mailbox.  Returns the first CSV file in the mailbox.
        /// </summary>
        /// <returns></returns>
        public async Task CheckMailBoxForImport(PerformContext hangfireContext)
        {            
            Pop3Client pop3Client = new Pop3Client();
            await pop3Client.ConnectAsync(Configuration["SPD_IMPORT_POP3_SERVER"], 
                                          Configuration["SPD_IMPORT_POP3_USERNAME"], 
                                          Configuration["SPD_IMPORT_POP3_PASSWORD"], true);
            List<Pop3Message> messages = (await pop3Client.ListAndRetrieveAsync()).ToList();

            foreach (Pop3Message message in messages)
            {                
                var attachments = message.Attachments.ToList();
                if (attachments.Count > 0)
                {
                    // string payload = null; // File.ReadAllText("C:\\tmp\\testimport.csv");

                    string payload = Encoding.Default.GetString(attachments[0].GetData());
                    if (payload != null) // parse the payload
                    {
                        List<WorkerResponse> responses = WorkerResponseParser.ParseWorkerResponse(payload);
                        foreach (WorkerResponse workerResponse in responses)
                        {
                            // search for the Personal History Record.
                            MicrosoftDynamicsCRMadoxioPersonalhistorysummary record = _dynamics.Personalhistorysummaries.GetByWorkerJobNumber(workerResponse.RecordIdentifier);

                            if (record != null)
                            {
                                // update the record.
                                MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchRecord = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary()
                                {
                                    AdoxioSecuritystatus = SPDResultTranslate.GetTranslatedSecurityStatus(workerResponse.Result),
                                    AdoxioCompletedon = workerResponse.DateProcessed
                                };

                                try
                                {
                                    _dynamics.Personalhistorysummaries.Update(record.AdoxioPersonalhistorysummaryid, patchRecord);
                                }
                                catch (OdataerrorException odee)
                                {
                                    hangfireContext.WriteLine("Error updating worker personal history");
                                    hangfireContext.WriteLine("Request:");
                                    hangfireContext.WriteLine(odee.Request.Content);
                                    hangfireContext.WriteLine("Response:");
                                    hangfireContext.WriteLine(odee.Response.Content);
                                }
                            }
                        }
                    }
                }

                await pop3Client.DeleteAsync(message);
                hangfireContext.WriteLine("Deleted message:");
            }            
        }

        /// <summary>
        /// Hangfire job to receive an import from SPD.
        /// </summary>
        public void ReceiveImportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Import Job.");

            var runner = CheckMailBoxForImport(hangfireContext);

            runner.Wait();

            hangfireContext.WriteLine("Done.");
        }

        private bool SendSPDEmail(string attachmentContent, string attachmentName)
        {
            var emailSentSuccessfully = false;
            var datePart = DateTime.Now.ToString().Replace('/', '-').Replace(':', '_');
            var email = Configuration["SPD_EXPORT_EMAIL"];
            string body = $@"";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))    // using UTF-8 encoding by default
            using (var mailClient = new SmtpClient(Configuration["SMTP_HOST"]))
            using (var message = new MailMessage("no-reply@gov.bc.ca", email))
            {
                writer.WriteLine(attachmentContent);
                writer.Flush();
                stream.Position = 0;     // read from the start of what was written

                message.Subject = $"{attachmentName}";
                message.Body = body;
                message.IsBodyHtml = true;

                message.Attachments.Add(new Attachment(stream, attachmentName, "text/csv"));

                try
                {
                    mailClient.Send(message);
                    emailSentSuccessfully = true;
                }
                catch (Exception)
                {
                    emailSentSuccessfully = false;
                }

            }
            return emailSentSuccessfully;
        }

        private bool SendSPDNoResultsEmail(string batchNumber)
        {
            var emailSentSuccessfully = false;
            var datePart = DateTime.Now.ToString().Replace('/', '-').Replace(':', '_');
            var email = Configuration["SPD_EXPORT_EMAIL"];
            string body = "";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))    // using UTF-8 encoding by default
            using (var mailClient = new SmtpClient(Configuration["SMTP_HOST"]))
            using (var message = new MailMessage("no-reply@gov.bc.ca", email))
            {
                writer.WriteLine($"No data exists on this batch request({ batchNumber})");
                writer.Flush();
                stream.Position = 0;     // read from the start of what was written

                message.Subject = $"No data exists on this batch request({ batchNumber})";
                message.Body = body;
                message.IsBodyHtml = true;


                try
                {
                    mailClient.Send(message);
                    emailSentSuccessfully = true;
                }
                catch (Exception)
                {
                    emailSentSuccessfully = false;
                }

            }
            return emailSentSuccessfully;
        }

        private List<KeyValuePair<string, string>> GetExportHeadersWorker()
        {
            var result = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("AdoxioLcrbbusinessjobid", "LCRB BUSINESS JOB ID"),                
                // 9-12-18 Substituted AdoxioLcrbassociatejobid for AdoxioLcrbworkerjobid to fix blank fields in export.
                new KeyValuePair<string, string>("AdoxioLcrbworkerjobid", "LCRB ASSOCIATE JOB ID"),
                // 9-12-18 - BC Registries number is no longer required for individuals (only business records, which are not yet part of the export)
                // new KeyValuePair<string, string>("AdoxioBcregistriesnumber", "Bcregistriesnumber"),
                new KeyValuePair<string, string>("AdoxioSelfdisclosure", "SELF-DISCLOSURE YN"),
                new KeyValuePair<string, string>("AdoxioLegalsurname", "SURNAME"),
                new KeyValuePair<string, string>("AdoxioLegalfirstname", "FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioLegalmiddlename", "SECOND NAME"),
                new KeyValuePair<string, string>("AdoxioBirthdate", "BIRTHDATE"),
                new KeyValuePair<string, string>("AdoxioGendermf", "GENDER"),
                new KeyValuePair<string, string>("AdoxioBirthplacecity", "BIRTHPLACE"),
                new KeyValuePair<string, string>("AdoxioDriverslicence", "DRIVERS LICENCE"),
                new KeyValuePair<string, string>("AdoxioBcidentificationcardnumber", "BC ID CARD NUMBER"),
                new KeyValuePair<string, string>("AdoxioContactphone", "CONTACT PHONE"),
                new KeyValuePair<string, string>("AdoxioPersonalemailaddress", "EMAIL ADDRESS"),
                new KeyValuePair<string, string>("AdoxioAddressline1", "STREET"),
                new KeyValuePair<string, string>("AdoxioAddresscity", "CITY"),
                new KeyValuePair<string, string>("AdoxioAddressprovstate", "PROVINCE"),
                new KeyValuePair<string, string>("AdoxioAddresscountry", "COUNTRY"),
                new KeyValuePair<string, string>("AdoxioAddresspostalcode", "POSTAL CODE"),
                new KeyValuePair<string, string>("AdoxioAlias1surname", "ALIAS 1 SURNAME"),
                new KeyValuePair<string, string>("AdoxioAlias1firstname", "ALIAS 1 FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioAlias1middlename", "ALIAS 1 SECOND NAME"),
                new KeyValuePair<string, string>("AdoxioAlias2surname", "ALIAS 2 SURNAME"),
                new KeyValuePair<string, string>("AdoxioAlias2firstname", "ALIAS 2 FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioAlias2middlename", "ALIAS 2 MIDDLE NAME"),
                new KeyValuePair<string, string>("AdoxioAlias3surname", "ALIAS 3 SURNAME"),
                new KeyValuePair<string, string>("AdoxioAlias3firstname", "ALIAS 3 FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioAlias3middlename", "ALIAS 3 MIDDLE NAME"),
                new KeyValuePair<string, string>("AdoxioAlias4surname", "ALIAS 4 SURNAME"),
                new KeyValuePair<string, string>("AdoxioAlias4firstname", "ALIAS 4 FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioAlias4middlename", "ALIAS 4 MIDDLE NAME"),
                new KeyValuePair<string, string>("AdoxioAlias5surname", "ALIAS 5 SURNAME"),
                new KeyValuePair<string, string>("AdoxioAlias5firstname", "ALIAS 5 FIRST NAME"),
                new KeyValuePair<string, string>("AdoxioAlias5middlename", "ALIAS 5 MIDDLE NAME"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress1", "PREV STREET 1"),
                new KeyValuePair<string, string>("AdoxioPreviouscity1", "PREV CITY 1"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate1", "PREV PROVINCE 1"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry1", "PREV COUNTRY 1"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode1", "PREV POSTCODE1"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress2", "PREV STREET 2"),
                new KeyValuePair<string, string>("AdoxioPreviouscity2", "PREV CITY 2"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate2", "PREV PROVINCE 2"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry2", "PREV COUNTRY 2"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode2", "PREV POSTCODE2"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress3", "PREV STREET 3"),
                new KeyValuePair<string, string>("AdoxioPreviouscity3", "PREV CITY 3"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate3", "PREV PROVINCE 3"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry3", "PREV COUNTRY 3"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode3", "PREV POSTCODE3"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress4", "PREV STREET 4"),
                new KeyValuePair<string, string>("AdoxioPreviouscity4", "PREV CITY 4"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate4", "PREV PROVINCE 4"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry4", "PREV COUNTRY 4"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode4", "PREV POSTCODE4"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress5", "PREV STREET 5"),
                new KeyValuePair<string, string>("AdoxioPreviouscity5", "PREV CITY 5"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate5", "PREV PROVINCE 5"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry5", "PREV COUNTRY 5"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode5", "PREV POSTCODE5"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress6", "PREV STREET 6"),
                new KeyValuePair<string, string>("AdoxioPreviouscity6", "PREV CITY 6"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate6", "PREV PROVINCE 6"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry6", "PREV COUNTRY 6"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode6", "PREV POSTCODE6"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress7", "PREV STREET 7"),
                new KeyValuePair<string, string>("AdoxioPreviouscity7", "PREV CITY 7"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate7", "PREV PROVINCE 7"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry7", "PREV COUNTRY 7"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode7", "PREV POSTCODE7"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress8", "PREV STREET 8"),
                new KeyValuePair<string, string>("AdoxioPreviouscity8", "PREV CITY 8"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate8", "PREV PROVINCE 8"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry8", "PREV COUNTRY 8"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode8", "PREV POSTCODE8"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddress9", "PREV STREET 9"),
                new KeyValuePair<string, string>("AdoxioPreviouscity9", "PREV CITY 9"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstate9", "PREV PROVINCE 9"),
                new KeyValuePair<string, string>("AdoxioPreviouscountry9", "PREV COUNTRY 9"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcode9", "PREV POSTCODE9"),
                new KeyValuePair<string, string>("AdoxioPreviousstreetaddressx", "PREV STREET x"),
                new KeyValuePair<string, string>("AdoxioPreviouscityx", "PREV CITY x"),
                new KeyValuePair<string, string>("AdoxioPreviousprovstatex", "PREV PROVINCE x"),
                new KeyValuePair<string, string>("AdoxioPreviouspostalcodex", "PREV POSTCODEx"),
                new KeyValuePair<string, string>("AdoxioPreviouscountryx", "PREV COUNTRY x"),
            };
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




