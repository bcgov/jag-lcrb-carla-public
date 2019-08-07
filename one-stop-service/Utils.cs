using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.OneStopService
{
    public static class Utils
    {
        public static string FormatPostalCode(string input)
        {
            string result = null;
            if (input != null)
            {
                result = input.ToUpper().Replace(" ", "");
            }
            return result;
        }

        public static string ParseGuid(string guid)
        {
            string result = null;
            Guid guidValue;
            if (Guid.TryParse(guid, out guidValue))
            {
                result = guidValue.ToString();
            }
            return result;
        }
    }
}
