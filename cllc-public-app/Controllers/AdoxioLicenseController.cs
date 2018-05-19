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
        
        // GET: api/adoxiolicense
        [HttpGet]
        public async Task<JsonResult> GetDynamicsLicenses()
        {
            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences> LicenseCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_licences>(_system);

            // get all licenses in Dynamics filtered by the applying person
            //var dynamicsApplicationList = await _system.Adoxio_applications.AddQueryOption("$filter", "_adoxio_applyingperson_value eq 7d4a5b20-e352-e811-8140-480fcfeac941").ExecuteAsync();

            // get all licenses in Dynamics
            var dynamicsLicenseList = await _system.Adoxio_licenceses.ExecuteAsync();

            List<ViewModels.AdoxioLicense> adoxioLicenses = new List<AdoxioLicense>();

            ViewModels.AdoxioLicense adoxioLicense = null;

            if (dynamicsLicenseList != null)
            {
                foreach (var dynamicsLicense in dynamicsLicenseList)
                {
                    adoxioLicense = new ViewModels.AdoxioLicense();

                    // fetch the establishment
                    Guid? adoxioEstablishmentId = dynamicsLicense._adoxio_establishment_value;
                    if (adoxioEstablishmentId != null)
                    {
                        Contexts.Microsoft.Dynamics.CRM.Adoxio_establishment establishment = await _system.Adoxio_establishments.ByKey(adoxio_establishmentid: adoxioEstablishmentId).GetValueAsync();
                        adoxioLicense.establishmentName = establishment.Adoxio_name;
                        adoxioLicense.establishmentAddress = establishment.Adoxio_addressstreet + ", " + establishment.Adoxio_addresscity + " " + establishment.Adoxio_addresspostalcode;
                    }

                    // fetch the licence status
                    int? adoxio_licenceStatusId = dynamicsLicense.Statuscode;
                    if (adoxio_licenceStatusId != null)
                    {
                        adoxioLicense.licenseStatus = dynamicsLicense.Statuscode.ToString();
                    }

                    // fetch the licence type
                    Guid? adoxio_licencetypeId = dynamicsLicense._adoxio_licencetype_value;
                    if (adoxio_licencetypeId != null)
                    {
                        Adoxio_licencetype adoxio_licencetype = await _system.Adoxio_licencetypes.ByKey(adoxio_licencetypeid: adoxio_licencetypeId).GetValueAsync();
                        adoxioLicense.licenseType = adoxio_licencetype.Adoxio_name;
                    }

                    adoxioLicense.licenseNumber = dynamicsLicense.Adoxio_licencenumber;

                    adoxioLicenses.Add(adoxioLicense);
                }
            }

            return Json(adoxioLicenses);
        }

        // GET: api/AdoxioLicense/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
    }
}
