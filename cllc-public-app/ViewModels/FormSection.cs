using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class FormSection
    {
        public string name { get; set; }
        public string id { get; set; }
        public Boolean showlabel { get; set; }
        public Boolean visible { get; set; }
        public List<FormField> fields { get; set; }
    }
}
