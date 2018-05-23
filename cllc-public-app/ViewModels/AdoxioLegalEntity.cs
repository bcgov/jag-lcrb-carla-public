using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioLegalEntity
    {
        public string adoxio_legalentityid { get; set; }
        public string adoxio_name { get; set; }

        public Account account { get; set; }
    }
}
