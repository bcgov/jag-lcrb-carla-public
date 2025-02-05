using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Drawing.Printing;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    public class MaintenanceBannerModel
    {
        public bool bannerEnabled { get; set; }
        public string bannerText { get; set; }
        public string bannerStartDate { get; set; }
        public string bannerEndDate { get; set; }
    }

    [Route("api/banner")]
    [ApiController]
    [AllowAnonymous]

    public class MaintenanceBannerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public MaintenanceBannerController(IConfiguration configuration, ILogger<MaintenanceBannerController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult GetMaintenanceBanner()
        {
            try 
            {
            MaintenanceBannerModel banner = new MaintenanceBannerModel{
                bannerEnabled = _configuration.GetValue<bool>("BANNER_ENABLED", false),
                bannerText = _configuration.GetValue<string>("BANNER_TEXT", ""),
                bannerStartDate = _configuration.GetValue<string>("BANNER_START_DATE", ""),
                bannerEndDate = _configuration.GetValue<string>("BANNER_END_DATE", "")
            };
            return Ok(banner);

            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving maintenance banner configuration");
                return StatusCode(500, "Unable to retrieve maintenance banner configuration");
            }
        }
    }

}
