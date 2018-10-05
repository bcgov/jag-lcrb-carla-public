using CsvHelper;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SpdSync
{
    public class WorkerResponseParser
    {
        static public List<WorkerResponse> ParseWorkerResponse (string csvData)
        {
            TextReader textReader = new StringReader(csvData);
            var csv = new CsvReader(textReader);
            
            List<WorkerResponse> result = csv.GetRecords<WorkerResponse>().ToList();

            return result;
        }
    }
}
