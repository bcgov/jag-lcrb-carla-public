using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace sep_service.Controllers
{
    [ApiController]
    [Route("sol")]
    public class SolController : ControllerBase
    {
        private readonly ILogger<SolController> _logger;

        public SolController(ILogger<SolController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("echo")]
        public string Get([FromBody] string value)
        {
            return value;
        }

        /// <summary>
        /// Create a Sol record
        /// </summary>
        /// <param name="sol">New Sol Record</param>
        /// <returns></returns>
        [HttpPost]
        public string Create([FromBody] Sol sol)
        {
            return "Create Sol";
        }

        /// <summary>
        /// Cancel a Sol Event
        /// </summary>
        /// <param name="solId">Sol Id</param>
        /// <param name="cancelReason">Reason for cancellation</param>
        /// <returns></returns>
        [HttpPost("cancel/{solId}")]
        public string Cancel([FromRoute]string solId, [FromBody] string cancelReason)
        {
            return $"Cancel {solId} reason - {cancelReason}";
        }
    }
}
