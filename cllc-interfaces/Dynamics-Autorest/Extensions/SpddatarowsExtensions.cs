using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gov.Lclb.Cllb.Interfaces
{
    static partial class SpddatarowsExtensions
    {
        public static MicrosoftDynamicsCRMadoxioSpddatarow GetByWorkerJobId (this ISpddatarows operations, string workerJobId)
        {
            MicrosoftDynamicsCRMadoxioSpddatarow result = null;
            try
            {
                string accountFilter = "adoxio_lcrbworkerjobid eq '" + workerJobId + "'";
                IEnumerable<MicrosoftDynamicsCRMadoxioSpddatarow> dataRows = operations.Get(filter: accountFilter).Value;
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
