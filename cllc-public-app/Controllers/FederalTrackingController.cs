using System.Collections.Generic;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FederalTrackingController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IConfiguration _configuration;                

        public FederalTrackingController(IDynamicsClient dynamicsClient, IConfiguration configuration)
        {
            _dynamicsClient = dynamicsClient;
            _configuration = configuration;            
        }


        /// <summary>
        /// Get a csv with the federal tracking report for a given reporting period
        /// </summary>
        /// <returns></returns>
        [HttpGet("{month}/{year}")]
        public IActionResult GetFederalTrackingReport(int month, int year)
        {
            if(month < 1 || month > 12 || year < 2018)
            {
                return new BadRequestResult();
            }
            
            string filter = $"adoxio_reportingperiodmonth eq '{month}' and adoxio_reportingperiodyear eq '{year}'";
            CannabismonthlyreportsGetResponseModel resp = _dynamicsClient.Cannabismonthlyreports.Get(filter: filter);

            if(resp.Value.Count > 0)
            {
                return new JsonResult(new Dictionary<string, int>{
                    { "found", resp.Value.Count },
                    { "month", month },
                    { "year", year }
                });
            }
            return new NotFoundResult();
        }
    }
}
