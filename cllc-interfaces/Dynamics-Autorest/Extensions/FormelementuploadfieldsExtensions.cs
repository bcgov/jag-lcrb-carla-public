using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.Interfaces
{
    static partial class FormelementuploadfieldsExtensions
    {
        public static IList<MicrosoftDynamicsCRMadoxioFormelementuploadfield> GetDocumentFieldsByForm(this IFormelementuploadfields operations, string formId)
        {
            IList<MicrosoftDynamicsCRMadoxioFormelementuploadfield> result = null;
            try
            {
                string formFilter = "adoxio_formguid eq '" + formId + "'";
                result = operations.Get(filter: formFilter).Value;                
            }
            catch (HttpOperationException)
            {
                result = null;
            }

            return result;
        }
    }
}
