extern alias DV;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Xrm.Sdk;
using System;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Business-User")]
    public class AnnualVolumeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IDataverseClient _dataverse;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnnualVolumeController(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IDataverseClient dataverse)
        {
            _configuration = configuration;
            _dataverse = dataverse;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("application/{applicationId}")]
        public async Task<IActionResult> UpdateAnnualVolumeForApplication(string applicationId, [FromBody] AnnualVolume volume)
        {
            var application = await _dataverse.GetApplicationByIdAsync(applicationId);
            if (application == null) return new NotFoundResult();

            if (!CurrentUserHasAccessToApplicationOwnedBy(application.adoxio_Applicant?.Id.ToString()))
                throw new Exception("User doesn't have an access the application");

            var existingVols = await _dataverse.GetAnnualVolumesByApplicationIdAsync(applicationId);
            foreach (var vol in existingVols)
            {
                try { await _dataverse.DeleteAnnualVolumeAsync(vol.adoxio_annualvolumeId?.ToString()); }
                catch { /* best effort */ }
            }

            var newVol = new adoxio_annualvolume
            {
                adoxio_VolumeDestroyed = volume.VolumeDestroyed,
                adoxio_VolumeProduced = volume.VolumeProduced,
                adoxio_CalendarYear = volume.CalendarYear,
                adoxio_Application = new EntityReference(adoxio_application.EntityLogicalName, Guid.Parse(applicationId)),
            };

            try
            {
                await _dataverse.CreateAnnualVolumeAsync(newVol);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private bool CurrentUserHasAccessToApplicationOwnedBy(string accountId)
        {
            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);
            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                return userSettings.AccountId == accountId;
            return false;
        }
    }
}
