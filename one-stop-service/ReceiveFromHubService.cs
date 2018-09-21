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
