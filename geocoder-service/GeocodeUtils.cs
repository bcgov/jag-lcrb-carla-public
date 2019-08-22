using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.GeoCoder;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gov.Lclb.Cllb.Geocoder
{
    public class GeocodeUtils
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        
        
        private IDynamicsClient _dynamics;

        private IGeocoderClient _geocoder;

        private ILogger _logger;

        public GeocodeUtils(IConfiguration Configuration, ILogger logger)
        {
            this.Configuration = Configuration;
            _dynamics = DynamicsSetupUtil.SetupDynamics(Configuration);
            _logger = logger;
            _geocoder = GeocoderSetupUtil.SetupGeocoder(Configuration);
        }

        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task GeocodeEstablishment(PerformContext hangfireContext, string establishmentId)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Geocoding an establishment");
            }

            var establishment = _dynamics.GetEstablishmentById(establishmentId);

            if (establishment != null)
            {
                
                string address = $"{establishment.AdoxioAddressstreet}, {establishment.AdoxioAddresscity}, BC";
                // output format can be xhtml, kml, csv, shpz, geojson, geojsonp, gml
                var output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: address);
                // get the lat and long for the pin.
                double? latData = output.Features[0].Geometry.Coordinates[0];
                double? longData = output.Features[0].Geometry.Coordinates[1];

                // update the establishment.

                var patchEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment()
                {
                    
                }

            }
        }

        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task GeocodeEstablishments(PerformContext hangfireContext)
        {
            if (hangfireContext != null)
            {
                _logger.LogInformation("Starting GeocodeEstablishments job.");
                hangfireContext.WriteLine("Starting GeocodeEstablishments job.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;
            try
            {
                var expand = new List<string> { "adoxio_Licencee" };
                string filter = $"adoxio_orgbookcredentialresult eq null";
                result = _dynamics.Licenceses.Get(filter: filter, expand: expand).Value;
            }
            catch (OdataerrorException odee)
            {
                if (hangfireContext != null)
                {
                    _logger.LogError("Error getting Licences");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string registrationId = item.AdoxioLicencee?.AdoxioBcincorporationnumber;
                string licenceId = item.AdoxioLicencesid;
            }

            _logger.LogInformation("End of GeocodeEstablishments job.");
            hangfireContext.WriteLine("End of GeocodeEstablishments job.");
        }

    }
}
