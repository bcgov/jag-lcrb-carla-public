using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class LicenseType
    {
        public string id;
        public string code;
        public string name;

        public List<ApplicationType> AllowedActions;
    }
}
