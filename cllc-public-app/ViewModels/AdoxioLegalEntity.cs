using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioLegalEntity
    {
        public string id { get; set; }
        public string name { get; set; }

        public Account account { get; set; }
    }
}
