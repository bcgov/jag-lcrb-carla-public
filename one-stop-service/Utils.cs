using System;

namespace Gov.Jag.Lcrb.OneStopService
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

        public static string GetTimeStamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-hh.mm.ss.000000");
        }
    }
}
