extern alias DV;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using adoxio_ldborder = DV::Gov.Lclb.Cllb.Interfaces.adoxio_ldborder;
using adoxio_licences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_licences;

using CsvHelper;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using LdbOrdersService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;
using Renci.SshNet;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LdbOrdersService
{
    public class LdbOrdersUtils
    {
        private IConfiguration Configuration { get; }
        private readonly IDataverseClient _dataverse;
        private bool _debugMode = false;

        public LdbOrdersUtils(IConfiguration configuration, IDataverseClient dataverse)
        {
            Configuration = configuration;
            _dataverse = dataverse;
            if (!string.IsNullOrEmpty(Configuration["DEBUG_MODE"]))
            {
                _debugMode = true;
            }
        }

        byte[] ScpGetData(PerformContext hangfireContext)
        {
            string ldbUrl = Configuration["LDB_URL"];
            string lbUsername = Configuration["LDB_USERNAME"];
            string ldbPassword = Configuration["LDB_PASSWORD"];
            if (string.IsNullOrEmpty(ldbUrl) ||
                string.IsNullOrEmpty(lbUsername) ||
                string.IsNullOrEmpty(ldbPassword))
            {
                return null;
            }

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

            return null;
        }

        private byte[] TestGetFile()
        {
            string testFileName = Configuration["TEST_FILE_NAME"];
            return File.ReadAllBytes(testFileName);
        }

        private List<LdbOrderCsv> GetOrderCsvs(byte[] data)
        {
            CsvHelper.Configuration.Configuration config = new CsvHelper.Configuration.Configuration();
            config.SanitizeForInjection = true;
            config.IgnoreBlankLines = true;
            config.HasHeaderRecord = false;
            config.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
            config.ShouldSkipRecord = record => { return record.All(string.IsNullOrEmpty); };
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
                        return csv.GetRecords<LdbOrderCsv>().ToList();
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
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for LDB sales");
            }

            byte[] data = TestGetFile(); //ScpGetData(hangfireContext);
            List<LdbOrderCsv> rows = GetOrderCsvs(data);

            foreach (var row in rows)
            {
                if (_debugMode && hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Licence {row.Licence} DateStart {row.DateStart} DateEnd {row.DateEnd} OrderTotal {row.OrderAmount}");
                }

                var licence = await _dataverse.GetLicenceByNumberAsync(row.Licence.ToString());
                if (licence != null)
                {
                    var ldbOrder = new adoxio_ldborder()
                    {
                        adoxio_LicenceId = new EntityReference(adoxio_licences.EntityLogicalName, licence.Id),
                        adoxio_MonthStart = row.DateStart,
                        adoxio_MonthEnd = row.DateEnd,
                        adoxio_Month = row.DateStart.Month,
                        adoxio_YearText = row.DateStart.Year.ToString(),
                        adoxio_TotalSales = row.OrderAmount
                    };
                    try
                    {
                        await _dataverse.CreateLdbOrderAsync(ldbOrder);
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

            hangfireContext.WriteLine("End of check for new OneStop queue items");
        }
    }
}
