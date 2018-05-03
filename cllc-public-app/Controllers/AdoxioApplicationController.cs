using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioApplicationController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public AdoxioApplicationController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }

        [HttpPost()]
        public async Task<JsonResult> CreateApplication([FromBody] Contexts.Microsoft.Dynamics.CRM.Adoxio_application item)
        {
            // create a new contact.
            Contexts.Microsoft.Dynamics.CRM.Adoxio_application adoxioApplication = new Contexts.Microsoft.Dynamics.CRM.Adoxio_application();

            // create a DataServiceCollection to add the record
            DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_application> ContactCollection = new DataServiceCollection<Contexts.Microsoft.Dynamics.CRM.Adoxio_application>(_system);
            // add a new contact.
            ContactCollection.Add(adoxioApplication);

            // changes need to made after the add in order for them to be saved.
            // tab_general
            adoxioApplication.Adoxio_LicenceType = item.Adoxio_LicenceType; // Licence Type*
            // tab_businessinfo
            adoxioApplication.Adoxio_applicanttype = item.Adoxio_applicanttype; // Business Type*
            //  tab_personalhistory
            adoxioApplication.Adoxio_applicanttype = item.Adoxio_applicanttype; // Applicant Type*
            // tab_establishmentpart1
            adoxioApplication.Adoxio_registeredestablishment = item.Adoxio_registeredestablishment; // Are you applying for a previously registered establishment in your account?*

            adoxioApplication.Adoxio_Applicant = item.Adoxio_Applicant;
            adoxioApplication.Adoxio_applicationid = item.Adoxio_applicationid;

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);

            return Json(adoxioApplication);
        }
    }
}
