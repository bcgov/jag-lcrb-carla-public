extern alias DV;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.GeoCoder;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Geocoder
{
    public class GeocodeUtils
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDataverseClient _dataverse;

        private IGeocoderClient _geocoder;

        private ILogger _logger;

        public GeocodeUtils(IConfiguration Configuration, IDataverseClient dataverse, ILogger logger)
        {
            this.Configuration = Configuration;
            _dataverse = dataverse;
            _logger = logger;
            _geocoder = GeocoderSetupUtil.SetupGeocoder(Configuration);
        }

        public string SanitizeStreetAddress(string address)
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

            var establishment = await _dataverse.GetEstablishmentByIdAsync(establishmentId);

            await GeocodeEstablishment(hangfireContext, establishment);
        }

        private async Task GeocodeEstablishment(PerformContext hangfireContext, adoxio_establishment establishment)
        {
            if (establishment != null && !string.IsNullOrEmpty(establishment.adoxio_AddressCity))
            {
                string streetAddress = SanitizeStreetAddress(establishment.adoxio_AddressStreet);
                string address = $"{establishment.adoxio_AddressStreet}, {establishment.adoxio_AddressCity}, BC";
                // output format can be xhtml, kml, csv, shpz, geojson, geojsonp, gml
                var output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: address);

                hangfireContext.WriteLine($"{address} returns {output.Features[0].Properties.Faults.Count} faults");

                // if there are any faults try a query based on the LGIN instead of the city.
                if (output.Features[0].Properties.Faults.Count > 1 && establishment.adoxio_LGIN != null)
                {
                    var lgin = await _dataverse.GetLginByIdAsync(establishment.adoxio_LGIN.Id.ToString());
                    if (lgin != null)
                    {
                        _logger.LogError($"Unable to find a good match for address {address}, using lgin of {lgin.adoxio_name}");
                        hangfireContext.WriteLine($"Unable to find a good match for address {address}, using lgin of {lgin.adoxio_name}");

                        string sanitizedLgin = lgin.adoxio_name;
                        if (sanitizedLgin.Contains("First Nation"))
                        {
                            sanitizedLgin = sanitizedLgin.Replace("First Nation", "").Trim();
                        }

                        address = $"{establishment.adoxio_AddressStreet}, {sanitizedLgin}, BC";
                        output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: address);
                        hangfireContext.WriteLine($"{address} returns {output.Features[0].Properties.Faults.Count} faults");
                    }
                }

                // if the LGIN did not provide a good match just default to the specified city.
                if (output.Features[0].Properties.Faults.Count > 3)
                {
                    _logger.LogError($"Unable to find a good match for address {address} with city {establishment.adoxio_LGIN?.Id}, defaulting to just {establishment.adoxio_AddressCity}");
                    hangfireContext.WriteLine($"Unable to find a good match for address {address} with city {establishment.adoxio_LGIN?.Id}, defaulting to just {establishment.adoxio_AddressCity}");
                    output = _geocoder.GeoCoderAPI.Sites(outputFormat: "json", addressString: $"{establishment.adoxio_AddressCity}, BC");
                }

                // get the lat and long for the pin.
                double? longData = output.Features[0].Geometry.Coordinates[0];
                double? latData = output.Features[0].Geometry.Coordinates[1];

                // update the establishment.
                var patchEstablishment = new adoxio_establishment()
                {
                    Id = establishment.Id,
                    adoxio_Longitude = longData,
                    adoxio_Latitude = latData
                };
                try
                {
                    await _dataverse.UpdateEstablishmentAsync(patchEstablishment);
                    _logger.LogInformation($"Updated establishment with address {address}");
                    hangfireContext.WriteLine($"Updated establishment with address {address}");
                }
                catch (Exception ex)
                {
                    if (hangfireContext != null)
                    {
                        _logger.LogError(ex, "Error updating establishment");
                        hangfireContext.WriteLine("Error updating establishment");
                        hangfireContext.WriteLine(ex.Message);
                    }

                    // fail if we can't update.
                    throw;
                }
            }
        }

        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task GeocodeEstablishments(PerformContext hangfireContext, bool redoGeocoded)
        {
            if (hangfireContext != null)
            {
                _logger.LogInformation("Starting GeocodeEstablishments job.");
                hangfireContext.WriteLine("Starting GeocodeEstablishments job.");
            }

            var crsType = await _dataverse.GetLicenceTypeByNameAsync("Cannabis Retail Store");
            var s119Type = await _dataverse.GetLicenceTypeByNameAsync("Section 119 Authorization");

            var typeIds = new List<string>();
            if (crsType != null) typeIds.Add(crsType.Id.ToString());
            if (s119Type != null) typeIds.Add(s119Type.Id.ToString());

            IList<adoxio_licences> licences = null;

            if (typeIds.Count > 0)
            {
                try
                {
                    licences = await _dataverse.GetActiveLicencesByTypeIdsAsync(typeIds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting licenses");
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine("Error getting licenses");
                        hangfireContext.WriteLine(ex.Message);
                    }

                    throw new Exception("Unable to get licences");
                }
            }

            if (licences != null)
            {
                foreach (var license in licences)
                {
                    var estRef = license.adoxio_establishment;
                    if (estRef != null)
                    {
                        var establishment = await _dataverse.GetEstablishmentByIdAsync(estRef.Id.ToString());

                        if (establishment != null && (redoGeocoded || establishment.adoxio_Latitude == null))
                        {
                            await GeocodeEstablishment(hangfireContext, establishment);
                        }
                    }
                }
            }

            // second pass to get BC Cannabis Stores.

            IList<adoxio_establishment> establishments = null;
            try
            {
                establishments = await _dataverse.GetEstablishmentsByNameAsync("BC Cannabis Store");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting establishments");
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine("Error getting establishments");
                    hangfireContext.WriteLine(ex.Message);
                }

                throw new Exception("Unable to get establishments");
            }

            if (establishments != null)
            {
                foreach (var establishment in establishments)
                {
                    if (establishment != null && (redoGeocoded || establishment.adoxio_Latitude == null))
                    {
                        await GeocodeEstablishment(hangfireContext, establishment);
                    }
                }
            }

            _logger.LogInformation("End of GeocodeEstablishments job.");
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("End of GeocodeEstablishments job.");
            }
        }

    }
}
