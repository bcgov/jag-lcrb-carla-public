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

namespace Gov.Lclb.Cllb.OneStopService
{
    public class ReceiveFromHubService : IReceiveFromHubService
    {
        IDynamicsClient _dynamicsClient;

        public ILogger _logger{ get; }

        public ReceiveFromHubService(IDynamicsClient dynamicsClient, ILogger logger)
        {
            _dynamicsClient = dynamicsClient;
            _logger = logger;
        }

        public string receiveFromHub(string inputXML)
        {
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

                //TODO: Update dynamics

            }
            catch (Exception e)
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
