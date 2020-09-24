using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class AnnualVolumeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDynamicsClient _dynamicsClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        public AnnualVolumeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, Gov.Lclb.Cllb.Interfaces.IDynamicsClient dynamics)
        {
            _configuration = configuration;
            _dynamicsClient = dynamics;
            _httpContextAccessor = httpContextAccessor;
            _logger = Log.Logger;
        }

        [HttpPost("application/{applicationId}")]
        public async Task<IActionResult> UpdateAnnualVolumeForApplication(string applicationId, [FromBody] AnnualVolume volume)
        {
            var dynamicsApplication = await _dynamicsClient.GetApplicationByIdWithChildren(Guid.Parse(applicationId));

            if(!CurrentUserHasAccessToApplicationOwnedBy(dynamicsApplication._adoxioApplicantValue))
            {
                return NotFound();
            }

            // Remove other annual volumes that are attached to application
            string filter = $"_adoxio_application_value eq {applicationId}";
            try
            {
                IList<MicrosoftDynamicsCRMadoxioAnnualvolume> vols = _dynamicsClient.Annualvolumes.Get(filter: filter).Value;
                foreach (MicrosoftDynamicsCRMadoxioAnnualvolume vol in vols)
                {
                    try
                    {
                        _dynamicsClient.Annualvolumes.Delete(vol.AdoxioAnnualvolumeid);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.Error(httpOperationException, "Unexpected error deleting an annual volume.");
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, "Unexpected error deleting an annual volume.");
                    }
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Unexpected error getting annual volumes");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unexpected error getting annual volumes.");
            }

            MicrosoftDynamicsCRMadoxioAnnualvolume dynamicsVol = new MicrosoftDynamicsCRMadoxioAnnualvolume()
            {
                AdoxioVolumedestroyed = volume.VolumeDestroyed,
                AdoxioVolumeproduced = volume.VolumeProduced,
                AdoxioCalendaryear = volume.CalendarYear,
                AdoxioApplicationODataBind = _dynamicsClient.GetEntityURI("adoxio_applications", applicationId)
            };

            // create it!
            try
            {
                _dynamicsClient.Annualvolumes.Create(dynamicsVol);
                return Ok();
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.Error(httpOperationException, "Unexpected error creating annual volume");
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unexpected error creating annual volume");
                return BadRequest();
            }
        }

        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // For now, check if the account id matches the user's account.
            // TODO there may be some account relationships in the future
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }
    }
}
