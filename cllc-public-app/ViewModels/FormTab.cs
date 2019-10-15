using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class FormTab
    {
        public string id { get; set; }
        public string name { get; set; }

        public Boolean showlabel { get; set; }

        public Boolean visible { get; set; }

        public List<FormSection> sections { get; set; }
    }
}
