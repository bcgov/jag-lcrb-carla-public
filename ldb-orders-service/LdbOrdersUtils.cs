
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


namespace ldb_orders_service
{
    public class LdbOrdersUtils
    {
        
        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private int maxLicencesPerInterval;

        private IConfiguration _configuration { get; }

        public LdbOrdersUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        


        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForLdbSales(PerformContext hangfireContext)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for LDB sales");
            }
            IList<MicrosoftDynamicsCRMadoxioOnestopmessageitem> result;

            hangfireContext.WriteLine("End of check for new OneStop queue items");
        }


    }
}
