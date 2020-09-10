using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Rest;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class ReceiveFromHubService : IReceiveFromHubService
    {
        IDynamicsClient _dynamicsClient;
        private IMemoryCache _cache;

        private readonly IConfiguration Configuration;
        private readonly IWebHostEnvironment _env;

        public ReceiveFromHubService(IDynamicsClient dynamicsClient, IConfiguration configuration, IWebHostEnvironment env)
        {
            _dynamicsClient = dynamicsClient;
            
            Configuration = configuration;
            _env = env;
        }

        public void SetCache (IMemoryCache cache)
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
                result = xmlDocument.ChildNodes[1].Name;
            }            
            return result;
        }

        private string HandleSBNCreateProgramAccountResponse(string inputXML)
        {
            Log.Logger.Information($"Reached HandleSBNCreateProgramAccountResponse");
            if (! _env.IsProduction())
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

            // Get licence from dynamics

            string businessProgramAccountNumber = "1";
            MicrosoftDynamicsCRMadoxioLicences licence = null;

            var filter = $"adoxio_licencenumber eq '{licenceNumber}'";
            try
            {
                licence = _dynamicsClient.Licenceses.Get(filter: filter).Value.FirstOrDefault();
                businessProgramAccountNumber = licenseData.body.businessProgramAccountNumber.businessProgramAccountReferenceNumber;
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
                Log.Logger.Information($"Licence record retrieved from Dynamics.");
                //save the program account number to dynamics
                
                int tempBpan = int.Parse(businessProgramAccountNumber);
                string sanitizedBpan = tempBpan.ToString();

                MicrosoftDynamicsCRMadoxioLicences pathLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioBusinessprogramaccountreferencenumber = sanitizedBpan,
                    AdoxioOnestopsent = true
                };
                Log.Logger.Information($"Sending update to Dynamics for BusinessProgramAccountNumber.");
                try
                {
                    _dynamicsClient.Licenceses.Update(licence.AdoxioLicencesid, pathLicence);
                    Log.Logger.Information($"ONESTOP Updated Licence {licenceNumber} record {licence.AdoxioLicencesid} to {businessProgramAccountNumber}");
                }
                catch (HttpOperationException odee)
                {
                    Log.Logger.Error(odee, "Error updating Licence {licence.AdoxioLicencesid}");
                    // fail if we can't get results.
                    throw (odee);
                }

                //Trigger the Send ProgramAccountDetailsBroadcast Message                
                BackgroundJob.Enqueue(() => new OneStopUtils(Configuration, _cache).SendProgramAccountDetailsBroadcastMessageREST(null, licence.AdoxioLicencesid));

                Log.Logger.Information("send program account details broadcast done.");
            }

            return httpStatusCode;
            
        }

        private string HandleSBNErrorNotification(string inputXML)
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
            if (errorNotification.body.validationErrors[0].errorMessageNumber.Equals("11845")) // Transaction not allowed - Duplicate Client event exists )
            {
                  
                Log.Logger.Error($"CRA has rejected the message due to an incorrect business number.  The business in question may have had multiple business numbers in the past and the number in the record is no longer valid.  Please correct the business number for record with partnernote of {errorNotification.header.partnerNote}");

            }
            else if (errorNotification.body.validationErrors[0].errorMessageNumber.Equals("11409")) // Old account number.               
            {
                Log.Logger.Information("Error is old account number is already associated with another account.  Retrying.");
                // retry the request with a higher increment.

                string licenceGuid = OneStopUtils.GetGuidFromPartnerNote(errorNotification.header.partnerNote);
                int currentSuffix = OneStopUtils.GetSuffixFromPartnerNote(errorNotification.header.partnerNote, Log.Logger);

                string cacheKey = "_BPAR_" + licenceGuid;
                int suffixLimit = 10;
                Log.Logger.Information($"Reading cache value for key {cacheKey}");

                _cache.TryGetValue(cacheKey, out suffixLimit);               
                
                // sanity check
                if (currentSuffix < suffixLimit)
                {
                    currentSuffix++;
                    Log.Logger.Information($"Starting resend of licence creation message, with new value of {currentSuffix}");

                    var patchRecord = new MicrosoftDynamicsCRMadoxioLicences()
                    {
                        AdoxioBusinessprogramaccountreferencenumber = currentSuffix.ToString()
                    };
                    _dynamicsClient.Licenceses.Update(licenceGuid, patchRecord);

                    BackgroundJob.Schedule(() => new OneStopUtils(Configuration, _cache).SendProgramAccountRequestREST(null, licenceGuid, currentSuffix.ToString("D3"))// zero pad 3 digit.
                    , TimeSpan.FromSeconds(30)); // Try again after 30 seconds
                }                
                else
                {
                    Log.Logger.Error($"Skipping resend of licence creation message as there have been too many tries({currentSuffix} - {suffixLimit}) Partner Note is partner note {errorNotification.header.partnerNote}");         
                }
            }
            else
            {
                Log.Logger.Error($"Received error notification for record with partner note {errorNotification.header.partnerNote} Error Code is  {errorNotification.body.validationErrors[0].errorMessageNumber}. Error Text is {errorNotification.body.validationErrors[0].errorMessageText} {inputXML}");
                
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
                        result = HandleSBNCreateProgramAccountResponse(inputXML);
                        break;
                    case "SBNErrorNotification":
                        result = HandleSBNErrorNotification(inputXML);
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


    [ServiceContract]
    public interface IReceiveFromHubService
    {
        [OperationContract]
        string receiveFromHub(string inputXML);

        void SetCache(IMemoryCache cache);
    }
}
