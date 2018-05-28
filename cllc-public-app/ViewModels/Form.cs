using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class Form
    {
        public string id { get; set; }
        public string name { get; set; }
        public string displayname { get; set; }

        public string entity { get; set; }

        public string formxml { get; set; }

        public List<FormTab> tabs { get; set; }
        
    }
}
