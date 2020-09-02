using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Mapping
{
    public class FieldMap
    {
        protected Dictionary<string, FieldMapping> fieldMap;

        public string GetViewModelKey (string dataModelKey)
        {
            string result = null;
            // convert from the data model to the view model.
            if (fieldMap != null && fieldMap.ContainsKey (dataModelKey))
            {
                result = fieldMap[dataModelKey].FieldName;
            }
            return result;
        }

        public bool GetRequired(string dataModelKey)
        {
            bool result = false;
            // convert from the data model to the view model.
            if (fieldMap != null && fieldMap.ContainsKey(dataModelKey))
            {
                result = fieldMap[dataModelKey].IsRequired;
            }
            return result;
        }


    }
}
