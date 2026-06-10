extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class EstablishmentWatchWordsController : ControllerBase
    {
        private readonly IDataverseClient _dataverse;

        public EstablishmentWatchWordsController(IDataverseClient dataverse)
        {
            _dataverse = dataverse;
        }

        [HttpGet]
        public async Task<IActionResult> GetEstablishmentWatchWords()
        {
            var returnVal = new Dictionary<string, List<string>>
            {
                { "forbidden", new List<string>() },
                { "problematic", new List<string>() },
            };

            var watchWordsList = await _dataverse.GetEstablishmentWatchWordsAsync();

            foreach (var word in watchWordsList)
            {
                if (word.adoxio_Forbidden == true)
                {
                    returnVal["forbidden"].Add(word.adoxio_name?.ToLower());
                }
                else
                {
                    returnVal["problematic"].Add(word.adoxio_name?.ToLower());
                }
            }
            return new JsonResult(returnVal);
        }
    }
}
