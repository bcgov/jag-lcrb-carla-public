using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public enum WorkerSecurityStatus
    {
        Pass = 845280000,
        Fail = 845280001,
        Withdrawn = 845280003
    }
}
