using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GetPdf(Dictionary<string, string> parameters, string template);
        Task<string> GetPdfHash(Dictionary<string, string> parameters, string template);
    }
}
