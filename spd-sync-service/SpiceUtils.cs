using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpdSync;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpiceUtils
    {
        public ILogger _logger { get; }

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamics;

        public SpiceUtils(IConfiguration Configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(SpdUtils));
            _dynamics = DynamicsUtil.SetupDynamics(Configuration);
        }

        /// <summary>
        /// Hangfire job to receive an import from SPICE.
        /// </summary>
        public void ReceiveImportJob(PerformContext hangfireContext, List<WorkerResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job.");
            _logger.LogError("Starting SPICE Import Job.");

            ImportResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Import responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private void ImportResponses(PerformContext hangfireContext, List<WorkerResponse> responses)
        {
            foreach (WorkerResponse workerResponse in responses)
            {
                // search for the Personal History Record.
                MicrosoftDynamicsCRMadoxioPersonalhistorysummary record = _dynamics.Personalhistorysummaries.GetByWorkerJobNumber(workerResponse.RecordIdentifier);

                if (record != null)
                {
                    // update the record.
                    MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchRecord = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary()
                    {
                        AdoxioSecuritystatus = SPDResultTranslate.GetTranslatedSecurityStatus(workerResponse.Result),
                        AdoxioCompletedon = workerResponse.DateProcessed
                    };

                    try
                    {
                        _dynamics.Personalhistorysummaries.Update(record.AdoxioPersonalhistorysummaryid, patchRecord);
                    }
                    catch (OdataerrorException odee)
                    {
                        hangfireContext.WriteLine("Error updating worker personal history");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        _logger.LogError("Error updating worker personal history");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                    }
                }
            }
        }
    }
}
