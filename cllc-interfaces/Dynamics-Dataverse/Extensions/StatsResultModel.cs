using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gov.Lclb.Cllb.Interfaces;

public enum CommRegions
{
    [EnumMember(Value = "Vancouver Island / Powell River / Gulf Islands")]
    VIPG = 845280001,
    [EnumMember(Value = "Greater Vancouver/Sunshine Coast")]
    Vancouver = 845280002,
    [EnumMember(Value = "Surrey /Fraser Valley")]
    Surrey = 845280003,
    [EnumMember(Value = "Interior and the North")]
    North = 845280004,
    [EnumMember(Value = "Location Not Yet Specified")]
    Unknown = 845280005
}

public class StatsResultModel
{
    public string adoxio_name { get; set; }
    public string adoxio_establishmentpropsedname { get; set; }
    public string adoxio_establishmentaddressstreet { get; set; }
    public string adoxio_establishmentaddresspostalcode { get; set; }
    public string adoxio_establishmentaddresscity { get; set; }
    public string adoxio_applicationid { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public CommRegions commregion { get; set; }
}

public class StatsResultResponse
{
    [JsonProperty(PropertyName = "value")]
    public List<Dictionary<string, string>> Value { get; set; }
}
