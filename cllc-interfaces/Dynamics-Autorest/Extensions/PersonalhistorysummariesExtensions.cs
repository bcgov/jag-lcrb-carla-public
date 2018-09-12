using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Lclb.Cllb.Interfaces
{
    static partial class PersonalhistorysummariesExtensions
    {
        public static MicrosoftDynamicsCRMadoxioPersonalhistorysummary GetByWorkerJobNumber (this IPersonalhistorysummaries operations, string workerJobNumber)
        {
            MicrosoftDynamicsCRMadoxioPersonalhistorysummary result = null;
            try
            {
                string jobNumberFilter = "adoxio_workerjobnumber eq '" + workerJobNumber + "'";
                IEnumerable<MicrosoftDynamicsCRMadoxioPersonalhistorysummary> dataRows = operations.Get(filter: jobNumberFilter).Value;
                result = dataRows.FirstOrDefault();
            }
            catch (OdataerrorException)
            {
                result = null;
            }

            return result;
        }
    }
}
