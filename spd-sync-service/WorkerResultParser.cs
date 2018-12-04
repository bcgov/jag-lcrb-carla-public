using CsvHelper;
using Microsoft.Extensions.Logging;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpdSync
{
    public class WorkerResponseParser
    {
        static public List<WorkerResponse> ParseWorkerResponse (string csvData, ILogger _logger)
        {
            CsvHelper.Configuration.Configuration config = new CsvHelper.Configuration.Configuration();
            config.SanitizeForInjection = true;
            config.IgnoreBlankLines = true;
            
            config.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;
            config.ShouldSkipRecord = record =>
            {
                return record.All(string.IsNullOrEmpty);
            };

            // fix for unexpected spaces in header
            config.PrepareHeaderForMatch =
                header => header = header.Trim();

            TextReader textReader = new StringReader(csvData);
            var csv = new CsvReader(textReader, config);

            try
            {
                List<WorkerResponse> result = csv.GetRecords<WorkerResponse>().ToList();
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError("Error parsing worker response.");
                _logger.LogError("Message:");
                _logger.LogError(e.Message);
                // return an empty list so we continue processing other files.
               return new List<WorkerResponse>();
            }
        }
    }
}
