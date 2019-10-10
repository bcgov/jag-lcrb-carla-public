using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class FormField
    {
        public string name { get; set; }
        public string datafieldname { get; set; }
        public Boolean showlabel { get; set; }
        public Boolean visible { get; set; }
        public string classid { get; set; }
        public string controltype { get; set; }
        public Boolean required { get; set; }
        public List<OptionMetadata> options { get; set; }
    }
}
