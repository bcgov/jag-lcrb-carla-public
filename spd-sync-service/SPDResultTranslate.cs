using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync
{
    public static class SPDResultTranslate
    {
        const int SPD_RESULT_PASS = 845280000;
        const int SPD_RESULT_FAIL = 845280001;
        const int SPD_RESULT_WITHDRAWN = 845280003;

        public static int? GetTranslatedSecurityStatus (string spdResult)
        {
            // translate the text response into the Dynamics constant.
            int? result = null;

            if (!string.IsNullOrEmpty(spdResult))
            {
                string upper = spdResult.ToUpper();

                if (upper.Equals("PASS"))
                {
                    result = SPD_RESULT_PASS;
                }
                else if (upper.Equals("FAIL"))
                {
                    result = SPD_RESULT_FAIL;
                }
                else if (upper.Equals("WITHDRAWN"))
                {
                    result = SPD_RESULT_WITHDRAWN;
                }
            }

            return result;
        }
    }
}
