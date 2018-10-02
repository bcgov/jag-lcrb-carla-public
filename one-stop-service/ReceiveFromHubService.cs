using Gov.Lclb.Cllb.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Gov.Lclb.Cllb.Interfaces.Models;

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
                return "400";
            }

            try
            {
                // deserialize the inputXML
                var serializer = new XmlSerializer(typeof(SBNCreateProgramAccountResponse1));
                SBNCreateProgramAccountResponse1 licenseData;
                using (TextReader reader = new StringReader(inputXML))
                {
                    licenseData = (SBNCreateProgramAccountResponse1)serializer.Deserialize(reader);
                    _logger.LogInformation(inputXML);
                }

                var filter = $"adoxio_licencenumber eq '{licenseData.header.partnerNote}'";
                MicrosoftDynamicsCRMadoxioLicences licence = _dynamicsClient.Licenses.Get(filter: filter).Value.FirstOrDefault();
                if(licence == null)
                {
                    return "400";
                }

                //save the program account number to dynamics
                var businessProgramAccountNumber = licenseData.body.businessProgramAccountNumber.businessProgramAccountReferenceNumber;
                MicrosoftDynamicsCRMadoxioLicences pathLicence = new MicrosoftDynamicsCRMadoxioLicences()
                {
                    AdoxioBusinessprogramaccountreferencenumber = businessProgramAccountNumber
                };
                _dynamicsClient.Licenses.Update(licence.AdoxioLicencesid, pathLicence);

                //Trigger the Send ProgramAccountDetailsBroadcast Message
                BackgroundJob.Enqueue(() => new OneStopUtils(Configuration).SendProgramAccountDetailsBroadcastMessageREST(null, licence.AdoxioLicencesid));

            }
            catch (Exception ex)
            {
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
