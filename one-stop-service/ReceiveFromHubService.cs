using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml.Serialization;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class ReceiveFromHubService : IReceiveFromHubService
    {
        IDynamicsClient _dynamicsClient;

        public ILogger _logger{ get; }
        private readonly IConfiguration Configuration;

        public ReceiveFromHubService(IDynamicsClient dynamicsClient, ILogger logger, IConfiguration configuration)
        {
            _dynamicsClient = dynamicsClient;
            _logger = logger;
            Configuration = configuration;
        }

        public string receiveFromHub(string inputXML)
        {
            _logger.LogInformation($">>>> Reached receiveFromHub method: { DateTime.Now.ToString() }");

            if (string.IsNullOrEmpty(inputXML))
            {
                _logger.LogInformation("inputXML is empty - returning 400.");
                return "400";
            }

            try
            {
                // deserialize the inputXML
                var serializer = new XmlSerializer(typeof(SBNCreateProgramAccountResponse1));
                SBNCreateProgramAccountResponse1 licenseData;

                // sanitize inputXML.
                inputXML = inputXML.Trim();

                using (TextReader reader = new StringReader(inputXML))
                {
                    licenseData = (SBNCreateProgramAccountResponse1)serializer.Deserialize(reader);
                    _logger.LogInformation(inputXML);
                }
                _logger.LogInformation("Getting licence with number of {licenseData.header.partnerNote}");
                var filter = $"adoxio_licencenumber eq '{licenseData.header.partnerNote}'";
                MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.Licenses.Get(filter: filter).Value.FirstOrDefault();
                if(licence == null)
                {
                    _logger.LogInformation("licence is null - returning 400.");
                    return "400";
                }

                //save the program account number to dynamics
                var businessProgramAccountNumber = licenseData.body.businessProgramAccountNumber.businessProgramAccountReferenceNumber;
                MicrosoftDynamicsCRMadoxioLicences pathLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioBusinessprogramaccountreferencenumber = businessProgramAccountNumber
                };

                try
                {
                    _dynamicsClient.Licenses.Update(licence.AdoxioLicencesid, pathLicence);
                    _logger.LogInformation($"Updated Licence record {licence.AdoxioLicencesid} to {businessProgramAccountNumber}");
                }
                catch (OdataerrorException odee)
                {
                    _logger.LogError("Error updating Licence");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    // fail if we can't get results.
                    throw (odee);
                }

                //Trigger the Send ProgramAccountDetailsBroadcast Message
                BackgroundJob.Enqueue(() => new OneStopUtils(Configuration).SendProgramAccountDetailsBroadcastMessageREST(null, licence.AdoxioLicencesid));
                _logger.LogInformation("Enqueued send program account details broadcast.");

            }
            catch (Exception ex)
            {
                _logger.LogError("Exception occured during processing of SOAP message");
                _logger.LogError(ex.Message);
                return "500";
            }

            return "200";

        }
    }


    [ServiceContract]
    public interface IReceiveFromHubService
    {
        [OperationContract]
        string receiveFromHub(string inputXML);
    }
}
