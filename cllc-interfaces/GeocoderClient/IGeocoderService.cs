using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public interface IGeocoderService
    {
        Task GeocodeEstablishment(string establishmentId, ILogger logger);
        Task<bool> TestAuthentication();
    }
}
