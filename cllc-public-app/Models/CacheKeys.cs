using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Models
{
    public static class CacheKeys
    {
        public static string PolicyDocumentPrefix { get { return "_PD_"; } }
        public static string PolicyDocumentCategoryPrefix { get { return "_PDC_"; } }
        public static string ApplicationPrefix { get { return "_APP_"; } }
        public static string ApplicationTypePrefix { get { return "_AT_"; } }
        public static string LicenceTypePrefix { get { return "_LT_"; } }
    }
}
