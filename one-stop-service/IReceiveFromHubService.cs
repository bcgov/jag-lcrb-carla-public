using System.ServiceModel;
using Microsoft.Extensions.Caching.Memory;

namespace Gov.Jag.Lcrb.OneStopService
{
    [ServiceContract]
    public interface IReceiveFromHubService
    {
        [OperationContract]
        string receiveFromHub(string inputXML);

        void SetCache(IMemoryCache cache);
    }
}