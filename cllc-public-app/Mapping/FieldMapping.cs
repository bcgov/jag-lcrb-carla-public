using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Mapping
{
    public class FieldMapping
    {
        public FieldMapping (string fieldName, bool isRequired)
        {
            FieldName = fieldName;
            IsRequired = isRequired;
        }
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
    }
}
