using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using Microsoft.Extensions.Configuration;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Geocoder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeocoderController : ControllerBase
    {
        IConfiguration Configuration;
        private readonly ILogger _logger;

        public GeocoderController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this._logger = loggerFactory.CreateLogger(typeof(GeocoderController));
        }

        // Geocode a given establishment.
        [HttpGet("GeocodeEstablishment/{establishmentId}")]
        public ActionResult GeocodeEstablishment( string establishmentId )
        {
            _logger.LogInformation($"Geocoding establishment. EstablishmentId: {establishmentId}");
            BackgroundJob.Enqueue(() => new GeocodeUtils(Configuration, _logger).GeocodeEstablishment(null, establishmentId));
            return Ok();
        }

        // Geocode a given establishment.
        [HttpGet("GeocodeEstablishments")]
        public ActionResult GeocodeEstablishments()
        {
            _logger.LogInformation($"Geocoding establishments");
            BackgroundJob.Enqueue(() => new GeocodeUtils(Configuration, _logger).GeocodeEstablishments(null));
            return Ok();
        }

    }
}
