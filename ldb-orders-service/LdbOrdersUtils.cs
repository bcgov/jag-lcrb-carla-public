
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Renci.SshNet;
using System.IO;
using ldb_orders_service.Models;
using System.Linq;
using CsvHelper;

namespace ldb_orders_service
{
    public class LdbOrdersUtils
    {
        
        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private int maxLicencesPerInterval;

        private IConfiguration Configuration { get; }
        private bool _debugMode = false;
        public LdbOrdersUtils(IConfiguration configuration)
        {
            Configuration = configuration;
            if (!String.IsNullOrEmpty(Configuration["DEBUG_MODE"]))
            {
                _debugMode = true; // enable debugging info.
            }
        }


        byte[] ScpGetData(PerformContext hangfireContext)
        {
            string ldbUrl = Configuration["LDB_URL"];
            string lbUsername = Configuration["LDB_USERNAME"];
            string ldbPassword = Configuration["LDB_PASSWORD"];
            // check that configuration is valid.
            if (string.IsNullOrEmpty(ldbUrl) ||
                string.IsNullOrEmpty(lbUsername) ||
                string.IsNullOrEmpty(ldbPassword))
            {
                return null;
            }
            byte[]  result;

            // login to the scp service.
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine($"Connecting to SCP server {ldbUrl}");
            }

            using (var sftp = new SftpClient(ldbUrl, lbUsername, ldbPassword))
            {
                sftp.Connect();
                if (sftp.IsConnected)
                {
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Connected to SCP server {ldbUrl}");
                    }

                    var status = sftp.GetStatus(".");
                    var files = sftp.ListDirectory("");
                    foreach (var file in files)
                    {
                        if (hangfireContext != null)
                        {
                            hangfireContext.WriteLine($"Found file {file.FullName}");
                        }
                        
                    }
                    
                }
                sftp.Disconnect();
            }

            result = null;
            return result;
        }

        // For test purposes read a file into a byte array.
        private byte[] TestGetFile()
        {
            string testFileName = Configuration["TEST_FILE_NAME"];
            byte[] result = File.ReadAllBytes(testFileName);
            return result;
        }

        private List<LdbOrderCsv> GetOrderCsvs(byte[] data)
        {
            CsvHelper.Configuration.Configuration config = new CsvHelper.Configuration.Configuration();
            config.SanitizeForInjection = true;
            config.IgnoreBlankLines = true;
            config.HasHeaderRecord = false;
            config.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
            config.ShouldSkipRecord = record => { return record.All(string.IsNullOrEmpty); };

            // fix for unexpected spaces in header
            config.PrepareHeaderForMatch =
                (string header, int index) => header = header.Trim();

            using (var ms = new MemoryStream(data))
            {
                using (var reader = new StreamReader(ms, true))
                {
                    var csv = new CsvReader(reader, config);

                    try
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        List<LdbOrderCsv> result = csv.GetRecords<LdbOrderCsv>().ToList();
                        return result;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Error parsing LDB Orders");

                        return null;
                    }
                }
            }
        }

    

        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForLdbSales(PerformContext hangfireContext)
        {
            IDynamicsClient dynamicsClient = null;
            if (!string.IsNullOrEmpty(Configuration["DYNAMICS_ODATA_URI"]))
            {
                dynamicsClient = DynamicsSetupUtil.SetupDynamics(Configuration);
            }
            
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for LDB sales");
            }

            byte[] data = TestGetFile(); //ScpGetData(hangfireContext);

            // parse the data.

            List<LdbOrderCsv> rows = GetOrderCsvs(data);

            foreach (var row in rows)
            {
                if (_debugMode && hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Licence {row.Licence} DateStart {row.DateStart} DateEnd {row.DateEnd} OrderTotal {row.OrderAmount}");
                }
                // lookup the licence.
                if (dynamicsClient != null)
                {
                    var licence = dynamicsClient.GetLicenceByNumber(row.Licence.ToString());
                    if (licence != null)
                    {
                        // create a row for the ldb orders.
                        MicrosoftDynamicsCRMadoxioLdborder ldbOrder = new MicrosoftDynamicsCRMadoxioLdborder()
                        {
                            LicenceIdODataBind = dynamicsClient.GetEntityURI("adoxio_licenceses",licence.AdoxioLicencesid),
                            AdoxioMonthstart = row.DateStart,
                            AdoxioMonthend = row.DateEnd,
                            AdoxioMonth = row.DateStart.Month,
                            AdoxioYeartext = row.DateStart.Year.ToString(),
                            AdoxioTotalsales = row.OrderAmount
                        };
                        try
                        {
                            dynamicsClient.Ldborders.Create(ldbOrder);
                            if (hangfireContext != null)
                            {
                                hangfireContext.WriteLine($"Added Order data for Licence {row.Licence} DateStart {row.DateStart} DateEnd {row.DateEnd}");
                            }
                        }
                        catch (Exception e)
                        {
                            if (hangfireContext != null)
                            {
                                hangfireContext.WriteLine($"Error adding Order data for Licence {row.Licence} DateStart {row.DateStart} DateEnd {row.DateEnd}");
                            }

                            Log.Error(e,
                                $"Error adding Order data for Licence {row.Licence} DateStart {row.DateStart} DateEnd {row.DateEnd}");
                        }
                        
                    }
                }
            }
            

            hangfireContext.WriteLine("End of check for new OneStop queue items");
        }


    }
}
