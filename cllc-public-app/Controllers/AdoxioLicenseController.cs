using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OData.Client;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AdoxioLicenseController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public AdoxioLicenseController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }
        
        /// GET all licenses in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLicenses()
        {
            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences> LicenseCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences>(_system);

            // get all licenses in Dynamics
            var dynamicsLicenseList = await _system.Adoxio_licenceses.ExecuteAsync();

            List<ViewModels.AdoxioLicense> adoxioLicenses = new List<AdoxioLicense>();

            ViewModels.AdoxioLicense adoxioLicenseVM = null;

            if (dynamicsLicenseList != null)
            {
                foreach (var dynamicsLicense in dynamicsLicenseList)
                {
                    adoxioLicenseVM = await ToViewModel(dynamicsLicense);
                    adoxioLicenses.Add(adoxioLicenseVM);
                }
            }

            return Json(adoxioLicenses);
        }

        /// GET all licenses in Dynamics filtered by the GUID of the applying person
        [HttpGet("{id}")]
        public async Task<JsonResult> GetDynamicsLicenses(string id)
        {
            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences> LicenseCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences>(_system);

            // get all licenses in Dynamics filtered by the GUID of the applying person
            var filter = "_ownerid_value eq " + id;
            var dynamicsLicenseList = await _system.Adoxio_licenceses.AddQueryOption("$filter", filter).ExecuteAsync();

            List<ViewModels.AdoxioLicense> adoxioLicenses = new List<AdoxioLicense>();

            ViewModels.AdoxioLicense adoxioLicenseVM = null;

            if (dynamicsLicenseList != null)
            {
                foreach (var dynamicsLicense in dynamicsLicenseList)
                {
                    adoxioLicenseVM = await ToViewModel(dynamicsLicense);
                    adoxioLicenses.Add(adoxioLicenseVM);
                }
            }

            return Json(adoxioLicenses);
        }

        private async Task<AdoxioLicense> ToViewModel(Adoxio_licences dynamicsLicense)
        {
            AdoxioLicense adoxioLicenseVM = new ViewModels.AdoxioLicense();

            // fetch the establishment and get name and address
            Guid? adoxioEstablishmentId = dynamicsLicense._adoxio_establishment_value;
            if (adoxioEstablishmentId != null)
            {
                Contexts.Microsoft.Dynamics.CRM.Adoxio_establishment establishment = await _system.Adoxio_establishments.ByKey(adoxio_establishmentid: adoxioEstablishmentId).GetValueAsync();
                adoxioLicenseVM.establishmentName = establishment.Adoxio_name;
                adoxioLicenseVM.establishmentAddress = establishment.Adoxio_addressstreet
                                                    + ", " + establishment.Adoxio_addresscity
                                                    + " " + establishment.Adoxio_addresspostalcode;
            }

            // fetch the licence status
            int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
            if (adoxio_licenceStatusId != null)
            {
                adoxioLicenseVM.licenseStatus = dynamicsLicense.Statuscode.ToString();
            }

            // fetch the licence type
            Guid? adoxio_licencetypeId = dynamicsLicense._adoxio_licencetype_value;
            if (adoxio_licencetypeId != null)
            {
                Adoxio_licencetype adoxio_licencetype = await _system.Adoxio_licencetypes.ByKey(adoxio_licencetypeid: adoxio_licencetypeId).GetValueAsync();
                adoxioLicenseVM.licenseType = adoxio_licencetype.Adoxio_name;
            }

            // fetch license number
            adoxioLicenseVM.licenseNumber = dynamicsLicense.Adoxio_licencenumber;

            return adoxioLicenseVM;
        }

    }
}
