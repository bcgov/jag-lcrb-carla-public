using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class LdbExport
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;


        public LdbExport(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            this._dynamics = OneStopUtils.SetupDynamics(Configuration);
            
        }

        /// <summary>
        /// Hangfire job to send the daily export to OneStop
        /// </summary>
        public async Task SendLdbExport(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting OneStop SendLdbExport Job.");

            // Get data

            List<MicrosoftDynamicsCRMadoxioLicences> result = null;
            string filter = $"adoxio_businessprogramaccountreferencenumber eq null";
            var expand = new List<string> { "adoxio_Licencee", "adoxio_establishment" };
            try
            {
                result = _dynamics.Licenses.Get(filter: filter, expand: expand).Value.ToList();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting Licence data rows");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }

            if (result == null || result.Count == 0)
            {
                bool sent = SendNoResultsEmail();
                if (sent)
                {
                    hangfireContext.WriteLine("Sent No Results Email.");
                }
                else
                {
                    hangfireContext.WriteLine("Unable to send No Results Email.");
                }
            }
            else
            {
                // Create the CSV for LDB
                var csvList = new List<List<string>>();
                var headers = new List<string>();
                var headerDefinition = GetExportHeaders();
                headerDefinition.ForEach(h =>
                {
                    headers.Add($"\"{h.Value}\"");
                });

                csvList.Add(headers);

                

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
                    
                }

                var csv = new StringBuilder();
                csvList.ForEach(row =>
                {
                    var line = String.Join(",", row);
                    csv.AppendLine(line);
                });

                var datePart = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                var attachmentName = $@"LdbExport_{datePart}.csv";

                // Send as email
                bool emailSuccess = SendCsvEmail(csv.ToString(), attachmentName);
                // Upload to SharePoint
                bool uploadSuccess = true;

                if (emailSuccess || uploadSuccess)
                {
                    hangfireContext.WriteLine("Sent CSV export");
                }

            }

            hangfireContext.WriteLine("End ofOneStop SendLdbExport  Job.");
        }


        private List<KeyValuePair<string, string>> GetExportHeaders()
        {
            var result = new List<KeyValuePair<string, string>>
            {                
                new KeyValuePair<string, string>("AdoxioLicenceprintname", "Licence Type"),
                new KeyValuePair<string, string>("AdoxioLicencee.Name", "Legal Name"),
                new KeyValuePair<string, string>("AdoxioLicencenumber", "Licence Number"),
                new KeyValuePair<string, string>("AdoxioLicencee.Accountnumber", "Business Number"),
                new KeyValuePair<string, string>("AdoxioEffectivedate", "Effective Date"),
                new KeyValuePair<string, string>("AdoxioExpirydate", "Expiry Date"),
                new KeyValuePair<string, string>("AdoxioEstablishment.AdoxioName", "Establishment Name"),
                new KeyValuePair<string, string>("AdoxioEstablishment.AdoxioAddressstreet", "Street"),
                new KeyValuePair<string, string>("AdoxioEstablishment.AdoxioAddresscity", "City"),
                new KeyValuePair<string, string>("AdoxioEstablishment.AdoxioAddresspostalcode", "Postal Code")
            };
            return result;
        }

        private bool SendCsvEmail(string attachmentContent, string attachmentName)
        {
            var emailSentSuccessfully = false;
            var datePart = DateTime.Now.ToString().Replace('/', '-').Replace(':', '_');
            var email = Configuration["LDB_EXPORT_EMAIL"];
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

        private bool SendNoResultsEmail()
        {
            var emailSentSuccessfully = false;
            var datePart = DateTime.Now.ToString().Replace('/', '-').Replace(':', '_');
            var email = Configuration["LDB_EXPORT_EMAIL"];
            string body = $"No data exists on this LCRB batch request({ datePart})";

            using (var mailClient = new SmtpClient(Configuration["SMTP_HOST"]))
            using (var message = new MailMessage("no-reply@gov.bc.ca", email))
            {
                message.Subject = body;
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

    }
}
