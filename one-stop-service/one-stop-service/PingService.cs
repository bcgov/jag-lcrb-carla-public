using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class PingService : IPingService
    {
        public string Ping(string message)
        {
            return string.Join("", message.Reverse());
        }
    }

    [ServiceContract]
    public interface IPingService
    {
        [OperationContract]
        string Ping(string message);
    }
}
