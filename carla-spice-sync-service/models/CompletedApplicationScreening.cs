using System.Collections.Generic;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class CompletedApplicationScreening
    {
        public string RecordIdentifier { get; set; }
        public SpiceApplicationStatus Result { get; set; }
        public List<Associate> Associates { get; set; }
    }
}
