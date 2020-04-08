using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EndorsementsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EndorsementsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Get a list of users endorsements
        /// </summary>
        /// <returns></returns>
        // [HttpGet()]
        // public IActionResult GetEndorsementsList()
        // {
        //     List<Endorsement> endorsements = new List<Endorsement>();

        //     return new JsonResult(features);
        // }

        // public HasEndorsement

    }
}
