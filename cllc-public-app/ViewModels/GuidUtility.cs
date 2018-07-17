using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public static class GuidUtility
    {
        /// <summary>
        /// Covert a string to a nullable guid.  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Guid? SafeNullableGuidConvert(string id)
        {
            Guid? result = null;
            if (! string.IsNullOrEmpty(id))
            {
                result = (Guid?)SafeGuidConvert(id);
            }
            return result;
        }

        /// <summary>
        /// Convert a string to a guid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Guid or Guid.empty if the string is invalid.</returns>
        public static Guid SafeGuidConvert(string id)
        {
            Guid result = new Guid();
            Guid.TryParse(id, out result);            
            return result;
        }
    }
}
