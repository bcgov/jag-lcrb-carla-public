using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        public ILogger _logger { get; }

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamics;
        private static readonly HttpClient Client = new HttpClient();

        public SpdUtils(IConfiguration Configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(SpdUtils));
            _dynamics = SetupDynamics(Configuration);

        }

        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public void SendExportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Export Job.");
            _logger.LogError("Starting SPD Export Job.");

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

                _logger.LogError("Error getting SPD data rows");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }

            var batch = GetBatchNumber().ToString();
            Dictionary<string, string> countryCodeMap = GetCountryCodeMap();
            batch = AddZeroPadding(batch);

            if (result != null && result.Count > 0)
            {
                foreach (var row in result)
                {
                    var item = new List<string>();

                    foreach (var header in headerDefinition)
                    {
                        string newValue = "\"\"";
                        try
                        {
                            string value = row[header.Key].ToString();
                            if (value != null)
                            {
                                // replace country code with Country name
                                if (header.Key.ToLower().Contains("country") && countryCodeMap.ContainsKey(value))
                                {
                                    value = countryCodeMap[value];
                                }
                                newValue = $"\"{value.ToString()}\"";
                            }

                        }
                        catch (Exception e)
                        {
                            _logger.LogError("Exception occured during SPD export.");
                            _logger.LogError($"Field is {header.Key}");
                            _logger.LogError(e.Message);
                        }
                        item.Add(newValue);
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
                    _logger.LogError("Sent SPD email. Now updating Dynamics...");
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

                            _logger.LogError("Error updating application");
                            _logger.LogError("Request:");
                            _logger.LogError(odee.Request.Content);
                            _logger.LogError("Response:");
                            _logger.LogError(odee.Response.Content);
                            // fail if we can't create.
                            throw (odee);
                        }
                    });
                    hangfireContext.WriteLine("Dynamics update complete.");
                    _logger.LogError("Dynamics update complete.");
                }
                else
                {
                    hangfireContext.WriteLine("Error sending SPD email.");
                    _logger.LogError("Error sending SPD email.");
                }
            }
            else
            {
                hangfireContext.WriteLine("No data to send, sending a null email.");
                _logger.LogError("No data to send, sending a null email.");
                SendSPDNoResultsEmail(batch);
            }

            hangfireContext.WriteLine("End of SPD Export Job.");
            _logger.LogError("End of SPD Export Job.");
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
                        List<WorkerResponse> responses = WorkerResponseParser.ParseWorkerResponse(payload, _logger);
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

                                    _logger.LogError("Error updating worker personal history");
                                    _logger.LogError("Request:");
                                    _logger.LogError(odee.Request.Content);
                                    _logger.LogError("Response:");
                                    _logger.LogError(odee.Response.Content);
                                }
                            }
                        }
                    }
                }

                await pop3Client.DeleteAsync(message);
                hangfireContext.WriteLine("Deleted message:");
                _logger.LogError("Deleted message:");
            }
        }

        /// <summary>
        /// Hangfire job to receive an import from SPD.
        /// </summary>
        public void ReceiveImportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Import Job.");
            _logger.LogError("Starting SPD Import Job.");

            var runner = CheckMailBoxForImport(hangfireContext);

            runner.Wait();

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
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
                new KeyValuePair<string, string>("AdoxioPreviouspostalcodex", "PREV POSTCODE x"),
                new KeyValuePair<string, string>("AdoxioPreviouscountryx", "PREV COUNTRY x"),
            };
            return result;
        }

        private Dictionary<string, string> GetCountryCodeMap()
        {
            var map = new Dictionary<string, string> {
                {"AF", "Afghanistan"},
                {"AL", "Albania"},
                {"DZ", "Algeria"},
                {"AX", "Aland Islands"},
                {"AS", "American Samoa"},
                {"AI", "Anguilla"},
                {"AD", "Andorra"},
                {"AO", "Angola"},
                {"AN", "Antilles - Netherlands"},
                {"AG", "Antigua and Barbuda"},
                {"AQ", "Antarctica"},
                {"AR", "Argentina"},
                {"AM", "Armenia"},
                {"AU", "Australia"},
                {"AT", "Austria"},
                {"AW", "Aruba"},
                {"AZ", "Azerbaijan"},
                {"BA", "Bosnia and Herzegovina"},
                {"BB", "Barbados"},
                {"BD", "Bangladesh"},
                {"BE", "Belgium"},
                {"BF", "Burkina Faso"},
                {"BG", "Bulgaria"},
                {"BH", "Bahrain"},
                {"BI", "Burundi"},
                {"BJ", "Benin"},
                {"BM", "Bermuda"},
                {"BN", "Brunei Darussalam"},
                {"BO", "Bolivia"},
                {"BR", "Brazil"},
                {"BS", "Bahamas"},
                {"BT", "Bhutan"},
                {"BV", "Bouvet Island"},
                {"BW", "Botswana"},
                {"BV", "Belarus"},
                {"BZ", "Belize"},
                {"KH", "Cambodia"},
                {"CM", "Cameroon"},
                {"CA", "Canada"},
                {"CV", "Cape Verde"},
                {"CF", "Central African Republic"},
                {"TD", "Chad"},
                {"CL", "Chile"},
                {"CN", "China"},
                {"CX", "Christmas Island"},
                {"CC", "Cocos (Keeling) Islands"},
                {"CO", "Colombia"},
                {"CG", "Congo"},
                {"CI", "Cote D'Ivoire (Ivory Coast)"},
                {"CK", "Cook Islands"},
                {"CR", "Costa Rica"},
                {"HR", "Croatia (Hrvatska)"},
                {"CU", "Cuba"},
                {"CY", "Cyprus"},
                {"CZ", "Czech Republic"},
                {"CD", "Democratic Republic of the Congo"},
                {"DJ", "Djibouti"},
                {"DK", "Denmark"},
                {"DM", "Dominica"},
                {"DO", "Dominican Republic"},
                {"EC", "Ecuador"},
                {"EG", "Egypt"},
                {"SV", "El Salvador"},
                {"TP", "East Timor"},
                {"EE", "Estonia"},
                {"GQ", "Equatorial Guinea"},
                {"ER", "Eritrea"},
                {"ET", "Ethiopia"},
                {"FI", "Finland"},
                {"FJ", "Fiji"},
                {"FK", "Falkland Islands (Malvinas)"},
                {"FM", "Federated States of Micronesia"},
                {"FO", "Faroe Islands"},
                {"FR", "France"},
                {"FX", "France, Metropolitan"},
                {"GF", "French Guiana"},
                {"PF", "French Polynesia"},
                {"GA", "Gabon"},
                {"GM", "Gambia"},
                {"DE", "Germany"},
                {"GH", "Ghana"},
                {"GI", "Gibraltar"},
                {"GB", "Great Britain (UK)"},
                {"GD", "Grenada"},
                {"GE", "Georgia"},
                {"GR", "Greece"},
                {"GL", "Greenland"},
                {"GN", "Guinea"},
                {"GP", "Guadeloupe"},
                {"GS", "S. Georgia and S. Sandwich Islands"},
                {"GT", "Guatemala"},
                {"GU", "Guam"},
                {"GW", "Guinea-Bissau"},
                {"GY", "Guyana"},
                {"HK", "Hong Kong"},
                {"HM", "Heard Island and McDonald Islands"},
                {"HN", "Honduras"},
                {"HT", "Haiti"},
                {"HU", "Hungary"},
                {"ID", "Indonesia"},
                {"IE", "Ireland"},
                {"IL", "Israel"},
                {"IN", "India"},
                {"IO", "British Indian Ocean Territory"},
                {"IQ", "Iraq"},
                {"IR", "Iran"},
                {"IT", "Italy"},
                {"JM", "Jamaica"},
                {"JO", "Jordan"},
                {"JP", "Japan"},
                {"KE", "Kenya"},
                {"KG", "Kyrgyzstan"},
                {"KI", "Kiribati"},
                {"KM", "Comoros"},
                {"KN", "Saint Kitts and Nevis"},
                {"KP", "Korea (North)"},
                {"KR", "Korea (South)"},
                {"KW", "Kuwait"},
                {"KY", "Cayman Islands"},
                {"KZ", "Kazakhstan"},
                {"LA", "Laos"},
                {"LB", "Lebanon"},
                {"LC", "Saint Lucia"},
                {"LI", "Liechtenstein"},
                {"LK", "Sri Lanka"},
                {"LR", "Liberia"},
                {"LS", "Lesotho"},
                {"LT", "Lithuania"},
                {"LU", "Luxembourg"},
                {"LV", "Latvia"},
                {"LY", "Libya"},
                {"MK", "Macedonia"},
                {"MO", "Macao"},
                {"MG", "Madagascar"},
                {"MY", "Malaysia"},
                {"ML", "Mali"},
                {"MW", "Malawi"},
                {"MR", "Mauritania"},
                {"MH", "Marshall Islands"},
                {"MQ", "Martinique"},
                {"MU", "Mauritius"},
                {"YT", "Mayotte"},
                {"MT", "Malta"},
                {"MX", "Mexico"},
                {"MA", "Morocco"},
                {"MC", "Monaco"},
                {"MD", "Moldova"},
                {"MN", "Mongolia"},
                {"MM", "Myanmar"},
                {"MP", "Northern Mariana Islands"},
                {"MS", "Montserrat"},
                {"MV", "Maldives"},
                {"MZ", "Mozambique"},
                {"NA", "Namibia"},
                {"NC", "New Caledonia"},
                {"NE", "Niger"},
                {"NF", "Norfolk Island"},
                {"NG", "Nigeria"},
                {"NI", "Nicaragua"},
                {"NL", "Netherlands"},
                {"NO", "Norway"},
                {"NP", "Nepal"},
                {"NR", "Nauru"},
                {"NU", "Niue"},
                {"NZ", "New Zealand (Aotearoa)"},
                {"OM", "Oman"},
                {"PA", "Panama"},
                {"PE", "Peru"},
                {"PG", "Papua New Guinea"},
                {"PH", "Philippines"},
                {"PK", "Pakistan"},
                {"PL", "Poland"},
                {"PM", "Saint Pierre and Miquelon"},
                {"CS", "Serbia and Montenegro"},
                {"PN", "Pitcairn"},
                {"PR", "Puerto Rico"},
                {"PS", "Palestinian Territory"},
                {"PT", "Portugal"},
                {"PW", "Palau"},
                {"PY", "Paraguay"},
                {"QA", "Qatar"},
                {"RE", "Reunion"},
                {"RO", "Romania"},
                {"RU", "Russian Federation"},
                {"RW", "Rwanda"},
                {"SA", "Saudi Arabia"},
                {"WS", "Samoa"},
                {"SH", "Saint Helena"},
                {"VC", "Saint Vincent and the Grenadines"},
                {"SM", "San Marino"},
                {"ST", "Sao Tome and Principe"},
                {"SN", "Senegal"},
                {"SC", "Seychelles"},
                {"SL", "Sierra Leone"},
                {"SG", "Singapore"},
                {"SK", "Slovakia"},
                {"SI", "Slovenia"},
                {"SB", "Solomon Islands"},
                {"SO", "Somalia"},
                {"ZA", "South Africa"},
                {"ES", "Spain"},
                {"SD", "Sudan"},
                {"SR", "Suriname"},
                {"SJ", "Svalbard and Jan Mayen"},
                {"SE", "Sweden"},
                {"CH", "Switzerland"},
                {"SY", "Syria"},
                {"SU", "USSR (former)"},
                {"SZ", "Swaziland"},
                {"TW", "Taiwan"},
                {"TZ", "Tanzania"},
                {"TJ", "Tajikistan"},
                {"TH", "Thailand"},
                {"TL", "Timor-Leste"},
                {"TG", "Togo"},
                {"TK", "Tokelau"},
                {"TO", "Tonga"},
                {"TT", "Trinidad and Tobago"},
                {"TN", "Tunisia"},
                {"TR", "Turkey"},
                {"TM", "Turkmenistan"},
                {"TC", "Turks and Caicos Islands"},
                {"TV", "Tuvalu"},
                {"UA", "Ukraine"},
                {"UG", "Uganda"},
                {"AE", "United Arab Emirates"},
                {"UK", "United Kingdom"},
                {"US", "United States"},
                {"UM", "United States Minor Outlying Islands"},
                {"UY", "Uruguay"},
                {"UZ", "Uzbekistan"},
                {"VU", "Vanuatu"},
                {"VA", "Vatican City State"},
                {"VE", "Venezuela"},
                {"VG", "Virgin Islands (British)"},
                {"VI", "Virgin Islands (U.S.)"},
                {"VN", "Viet Nam"},
                {"WF", "Wallis and Futuna"},
                {"EH", "Western Sahara"},
                {"YE", "Yemen"},
                {"YU", "Yugoslavia (former)"},
                {"ZM", "Zambia"},
                {"ZR", "Zaire (former)"},
                {"ZW", "Zimbabwe"},
            };
            return map;
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
        public static SharePointFileManager SetupSharepoint(IConfiguration Configuration)
        {
            // add SharePoint.

            string sharePointServerAppIdUri = Configuration["SHAREPOINT_SERVER_APPID_URI"];
            string sharePointOdataUri = Configuration["SHAREPOINT_ODATA_URI"];
            string sharePointWebname = Configuration["SHAREPOINT_WEBNAME"];
            string sharePointAadTenantId = Configuration["SHAREPOINT_AAD_TENANTID"];
            string sharePointClientId = Configuration["SHAREPOINT_CLIENT_ID"];
            string sharePointCertFileName = Configuration["SHAREPOINT_CERTIFICATE_FILENAME"];
            string sharePointCertPassword = Configuration["SHAREPOINT_CERTIFICATE_PASSWORD"];
            string ssgUsername = Configuration["SSG_USERNAME"];
            string ssgPassword = Configuration["SSG_PASSWORD"];
            string sharePointNativeBaseURI = Configuration["SHAREPOINT_NATIVE_BASE_URI"];

            var manager = new SharePointFileManager
            (
                sharePointServerAppIdUri,
                sharePointOdataUri,
                sharePointWebname,
                sharePointAadTenantId,
                sharePointClientId,
                sharePointCertFileName,
                sharePointCertPassword,
                ssgUsername,
                ssgPassword,
                sharePointNativeBaseURI
            );
            return manager;
        }
    }
}
