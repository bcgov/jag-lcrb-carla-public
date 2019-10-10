using System.Collections.Generic;

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
