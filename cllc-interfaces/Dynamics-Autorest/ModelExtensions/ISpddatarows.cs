using Gov.Lclb.Cllb.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicsAutorest
{
    public partial interface ISpddatarows
    {
        MicrosoftDynamicsCRMadoxioSpddatarow GetByWorkerJobId(string workerJobId);        
    }
}
