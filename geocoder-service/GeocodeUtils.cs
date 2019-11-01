using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.GeoCoder;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
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

        public string SanitizeStreetAddress (string address)
        {
            string result = null;
            if (address != null)
            {
                // check for spaces between unit number and street address.
                Regex regex = new Regex(@"\s*(\d+)\s*-\s*(\d+)\s*(.*)");
                Match match = regex.Match(address);
                if (match.Success)
                {
                    // Groups is indexed at 1.
                    result = $"{match.Groups[1].Value}-{match.Groups[2].Value} {match.Groups[3].Value}";
                }
                else
                {
                    result = address;
                }
            }            

            return result;
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

            await GeocodeEstablishment(hangfireContext, establishment);
        }

        private async Task GeocodeEstablishment(PerformContext hangfireContext, MicrosoftDynamicsCRMadoxioEstablishment establishment)
        {
            if (establishment != null && ! string.IsNullOrEmpty(establishment.AdoxioAddresscity) )
            {
                string streetAddress = SanitizeStreetAddress(establishment.AdoxioAddressstreet);
                string address = $"{establishment.AdoxioAddressstreet}, {establishment.AdoxioAddresscity}, BC";
                // output format can be xhtml, kml, csv, shpz, geojson, geojsonp, gml
                var output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: address);
                               
                // if there are any faults try a query based on the LGIN instead of the city.
                if (output.Features[0].Properties.Faults.Count > 0  && establishment._adoxioLginValue != null) 
                {
                    _logger.LogError($"Unable to find a good match for address {address}, using lgin of  {establishment._adoxioLginValue}");
                    hangfireContext.WriteLine($"Unable to find a good match for address {address}, using lgin of  {establishment._adoxioLginValue}");

                    var lgin = _dynamics.GetLginById(establishment._adoxioLginValue);
                    address = $"{establishment.AdoxioAddressstreet}, {lgin.AdoxioName}, BC";
                    output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: address);                    
                }

                // if the LGIN did not provide a good match just default to the specified city.
                if (output.Features[0].Properties.Faults.Count > 1)
                {
                    _logger.LogError($"Unable to find a good match for address {address} with city {establishment._adoxioLginValue}, defaulting to just {establishment.AdoxioAddresscity}");
                    hangfireContext.WriteLine($"Unable to find a good match for address {address} with city {establishment._adoxioLginValue}, defaulting to just {establishment.AdoxioAddresscity}");
                    output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: $"{establishment.AdoxioAddresscity}, BC");
                }
                    

                // get the lat and long for the pin.
                double? longData = output.Features[0].Geometry.Coordinates[0];
                double? latData = output.Features[0].Geometry.Coordinates[1];

                // update the establishment.

                var patchEstablishment = new MicrosoftDynamicsCRMadoxioEstablishment()
                {
                    AdoxioLongitude = (decimal?)longData,
                    AdoxioLatitude = (decimal?)latData
                };
                try
                {
                    _dynamics.Establishments.Update(establishment.AdoxioEstablishmentid, patchEstablishment);
                    _logger.LogInformation($"Updated establishment with address {address}");
                    hangfireContext.WriteLine($"Updated establishment with address {address}");
                }
                catch (HttpOperationException odee)
                {
                    if (hangfireContext != null)
                    {
                        _logger.LogError("Error updating establishment");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                        hangfireContext.WriteLine("Error updating establishment");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);
                    }
                    
                    // fail if we can't update.
                    throw (odee);
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
            IList<MicrosoftDynamicsCRMadoxioEstablishment> result;

            try
            {
                string filter = $"adoxio_longitude eq null";
                result = _dynamics.Establishments.Get().Value;
            }
            catch (HttpOperationException odee)
            {
                if (hangfireContext != null)
                {
                    _logger.LogError("Error getting Establishments");
                    _logger.LogError("Request:");
                    _logger.LogError(odee.Request.Content);
                    _logger.LogError("Response:");
                    _logger.LogError(odee.Response.Content);
                    hangfireContext.WriteLine("Error getting Establishments");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            // now for each one process it.
            foreach (var establishment in result)
            {                 
                await GeocodeEstablishment(hangfireContext, establishment);
            }

            _logger.LogInformation("End of GeocodeEstablishments job.");
            hangfireContext.WriteLine("End of GeocodeEstablishments job.");
        }

    }
}
