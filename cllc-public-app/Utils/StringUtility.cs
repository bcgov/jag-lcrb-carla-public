using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Utils
{
    public class StringUtility
    {
        /// <summary>
        /// Truncate a string to a field length
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Truncate(string data, int length)
        {
            string result = data;

            if (result != null && result.Length > length)
            {
                result = result.Substring(0, length);
            }

            return result;
        }
    }
}
