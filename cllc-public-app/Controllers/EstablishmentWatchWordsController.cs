using Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EstablishmentWatchWordsController : ControllerBase
    {
        private readonly IDynamicsClient _dynamicsClient;

        public EstablishmentWatchWordsController(IDynamicsClient dynamicsClient)
        {
            _dynamicsClient = dynamicsClient;
        }

        [HttpGet()]
        public IActionResult GetEstablishmentWatchWords()
        {
            Dictionary<string, List<string>> returnVal = new Dictionary<string, List<string>>()
            {
                {"forbidden", new List<string>()},
                {"problematic", new List<string>()},
            };

            var watchWordsList = _dynamicsClient.Establishmentwatchwords.Get().Value;

            foreach (var word in watchWordsList)
            {
                if((bool)word.AdoxioForbidden)
                {
                    returnVal["forbidden"].Add(word.AdoxioName.ToLower());
                }
                else
                {
                    returnVal["problematic"].Add(word.AdoxioName.ToLower());
                }

            }
            return new JsonResult(returnVal);
        }
    }
}
