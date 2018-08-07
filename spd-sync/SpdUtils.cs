using Hangfire.Console;
using Hangfire.Server;
using System.Net.Http;
using System.Threading.Tasks;


namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpdUtils
    {
       
        private static HttpClient Client = new HttpClient();
        
        
        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        
        public static async Task SendExportJob(string baseUri, PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Export Job.");
            
            hangfireContext.WriteLine("Done.");
        }


        /// <summary>
        /// Hangfire job to receive an import from SPD.
        /// </summary>

        public static async Task ReceiveImportJob(string baseUri, PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Import Job.");

            hangfireContext.WriteLine("Done.");
        }


    }
}

