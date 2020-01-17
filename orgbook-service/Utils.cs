using System;

namespace Gov.Lclb.Cllb.OrgbookService
{
    public static class Utils
    {
        public static string ParseGuid(string guid)
        {
            string result = null;
            if (Guid.TryParse(guid, out Guid guidValue))
            {
                result = guidValue.ToString();
            }
            return result;
        }
    }
}
