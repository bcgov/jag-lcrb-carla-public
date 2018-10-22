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
    }
}
