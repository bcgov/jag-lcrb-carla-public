extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DV::Gov.Lclb.Cllb.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Gov.Jag.Lcrb.OneStopService
{
    public class ReceiveFromHubService : IReceiveFromHubService
    {

        private IMemoryCache _cache;

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IDataverseClient _dataverse;

        public ReceiveFromHubService(IConfiguration configuration, IWebHostEnvironment env, IDataverseClient dataverse)
        {
            _configuration = configuration;
            _env = env;
            _dataverse = dataverse;
        }

        public void SetCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Get the name of the root node.
        /// </summary>
        /// <param name="inputXML"></param>
        /// <returns></returns>
        private string GetRootNodeName(string inputXML)
        {
            string result = null;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(inputXML);
            // node 0 is going to be "xml", so get the next node.
            if (xmlDocument.ChildNodes.Count > 1)
            {
                result = xmlDocument.ChildNodes[1]?.Name;
            }
            return result;
        }

        private async Task ClearProgramAccountDetailsAsync(string licenceId, string payload)
        {
            System.Collections.Generic.IList<adoxio_onestopmessageitem> result;
            try
            {
                result = await _dataverse.GetOneStopMessagesByLicenceIdAsync(licenceId);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"ERROR getting related queue items for licence {licenceId}");
                return;
            }

            foreach (var item in result)
            {
                // Only update issued and transfer complete
                switch ((OneStopHubStatusChange?)item.adoxio_StatusChangeDescription)
                {
                    case OneStopHubStatusChange.Issued:
                    case OneStopHubStatusChange.TransferComplete:
                        var patch = new adoxio_onestopmessageitem();
                        patch.Id = item.Id;
                        patch.adoxio_DateAcknowledgementReceived = DateTime.UtcNow;
                        patch.adoxio_AcknowledgementStatus = payload;
                        try
                        {
                            await _dataverse.UpdateOneStopMessageItemAsync(patch);
                        }
                        catch (Exception e)
                        {
                            Log.Logger.Error(e, $"ERROR updating queue items for licence {licenceId}");
                        }
                        break;
                }
            }
        }

        private async Task<string> HandleResponseAsync(string inputXML)
        {
            Log.Logger.Information("Reached HandleResponse");
            if (!_env.IsProduction())
            {
                Log.Logger.Information($"InputXML is: {inputXML}");
            }

            string httpStatusCode = "200";

            // deserialize the inputXML
            var serializer = new XmlSerializer(typeof(SBNCreateProgramAccountResponse1));
            SBNCreateProgramAccountResponse1 licenseData;
            using (TextReader reader = new StringReader(inputXML))
            {
                licenseData = (SBNCreateProgramAccountResponse1)serializer.Deserialize(reader);
            }

            string licenceNumber = OneStopUtils.GetLicenceNumberFromPartnerNote(licenseData.header.partnerNote);
            Log.Logger.Information($"Getting licence with number of {licenceNumber}");

            string businessProgramAccountNumber = "1";
            adoxio_licences licence = null;

            try
            {
                licence = await _dataverse.GetLicenceByNumberAsync(licenceNumber);
                if (licence != null)
                {
                    businessProgramAccountNumber = licenseData.body.businessProgramAccountNumber.businessProgramAccountReferenceNumber;
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Unable to get licence data for licence number {licenceNumber} {e.Message}");
                licence = null;
            }

            if (licence == null)
            {
                Log.Logger.Information("licence is null - returning 400.");
                httpStatusCode = "400";
            }
            else
            {
                Log.Logger.Information("Licence record retrieved from Dynamics.");
                //save the program account number to dynamics

                int tempBpan = int.Parse(businessProgramAccountNumber);
                string sanitizedBpan = tempBpan.ToString();

                var patch = new adoxio_licences();
                patch.Id = licence.Id;
                patch.adoxio_BusinessProgramAccountReferenceNumber = sanitizedBpan;

                Log.Logger.Information("Sending update to Dynamics for BusinessProgramAccountNumber.");
                try
                {
                    await _dataverse.UpdateLicenceAsync(patch);
                    Log.Logger.Information($"ONESTOP Updated Licence {licenceNumber} record {licence.Id} to {businessProgramAccountNumber}");
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Error updating Licence {licence.Id}");
                    throw;
                }
                // now clear out the cache item.
                await ClearProgramAccountDetailsAsync(licence.Id.ToString(), inputXML);

                //Trigger the Send ProgramAccountDetailsBroadcast Message
                BackgroundJob.Enqueue<OneStopUtils>(utils => utils.SendProgramAccountDetailsBroadcastMessageRest(null, licence.Id.ToString()));

                Log.Logger.Information("send program account details broadcast done.");
            }

            return httpStatusCode;
        }

        private async Task<string> HandleSBNErrorNotificationAsync(string inputXML)
        {
            string result = "200";
            // deserialize the inputXML
            var serializer = new XmlSerializer(typeof(SBNErrorNotification1));
            SBNErrorNotification1 errorNotification;

            using (TextReader reader = new StringReader(inputXML))
            {
                errorNotification = (SBNErrorNotification1)serializer.Deserialize(reader);
            }

            // check to see if it is simply a problem with an old account number.
            if (errorNotification?.body?.validationErrors != null &&
                errorNotification.body.validationErrors.Length > 0 &&
                errorNotification.body.validationErrors[0].errorMessageNumber != null &&
                errorNotification.body.validationErrors[0].errorMessageNumber.Equals("11845")) // Transaction not allowed - Duplicate Client event exists )
            {
                Log.Logger.Error($"CRA has rejected the message due to an incorrect business number.  The business in question may have had multiple business numbers in the past and the number in the record is no longer valid.  Please correct the business number for record with partnernote of {errorNotification.header.partnerNote}");
            }
            else if (errorNotification?.body?.validationErrors != null &&
                     errorNotification.body.validationErrors.Length > 0 &&
                     errorNotification.body.validationErrors[0].errorMessageNumber != null &&
                     errorNotification.body.validationErrors[0].errorMessageNumber.Equals("11409")) // Old account number.
            {
                Log.Logger.Information("Error is old account number is already associated with another account.  Retrying.");
                // retry the request with a higher increment.

                string licenceGuid = OneStopUtils.GetGuidFromPartnerNote(errorNotification.header.partnerNote);
                int currentSuffix = OneStopUtils.GetSuffixFromPartnerNote(errorNotification.header.partnerNote, Log.Logger);

                string cacheKey = "_BPAR_" + licenceGuid;

                Log.Logger.Information($"Reading cache value for key {cacheKey}");

                if (!_cache.TryGetValue(cacheKey, out int suffixLimit))
                {
                    suffixLimit = 10;
                }

                // sanity check
                if (currentSuffix < suffixLimit)
                {
                    currentSuffix++;
                    Log.Logger.Information($"Starting resend of send program account request message, with new value of {currentSuffix}");

                    var patch = new adoxio_licences();
                    patch.Id = Guid.Parse(licenceGuid);
                    patch.adoxio_BusinessProgramAccountReferenceNumber = currentSuffix.ToString();
                    await _dataverse.UpdateLicenceAsync(patch);

                    BackgroundJob.Schedule<OneStopUtils>(
                        utils => utils.SendProgramAccountRequestREST(null, licenceGuid, currentSuffix.ToString("D3"), null),
                        TimeSpan.FromSeconds(30)); // Try again after 30 seconds
                }
                else
                {
                    Log.Logger.Error($"Skipping resend of send program account request message as there have been too many tries({currentSuffix} - {suffixLimit}) Partner Note is partner note {errorNotification.header.partnerNote}");
                }
            }
            else
            {
                Log.Logger.Error($"Received error notification Error Text is {inputXML}");
            }

            return result;
        }


        public string receiveFromHub(string inputXML)
        {
            string result = "200";
            Log.Logger.Information($">>>> Reached receiveFromHub method: { DateTime.Now.ToString() }");

            if (string.IsNullOrEmpty(inputXML))
            {
                Log.Logger.Information("inputXML is empty - returning 400.");
                return "400";
            }

            try
            {
                // sanitize inputXML.
                inputXML = inputXML.Trim();

                // determine the type of XML.
                string rootNodeName = GetRootNodeName(inputXML);

                Log.Logger.Information($"ONESTOP ReceiveFromHub Message {rootNodeName}");
                if (!_env.IsProduction())
                {
                    Log.Logger.Information($"ReceiveFromHub InputXML is: {inputXML}");
                }

                switch (rootNodeName)
                {
                    case "SBNCreateProgramAccountResponse":
                        result = HandleResponseAsync(inputXML).GetAwaiter().GetResult();
                        break;
                    case "SBNErrorNotification":
                        result = HandleSBNErrorNotificationAsync(inputXML).GetAwaiter().GetResult();
                        break;
                    default:
                        Log.Logger.Information($"Unknown Root Node encountered: {rootNodeName}");
                        break;
                }


            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception occured during processing of SOAP message");
                return "500";
            }

            return result;

        }
    }
}
