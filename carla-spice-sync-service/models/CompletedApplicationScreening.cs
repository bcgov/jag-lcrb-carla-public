using System.Collections.Generic;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedApplicationScreening
    {
        public string RecordIdentifier { get; set; }
        public string Result { get; set; }
        public List<Associate> Associates { get; set; }
    }
}
