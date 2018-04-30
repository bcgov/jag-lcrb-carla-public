using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class FormsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public FormsController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }
        [HttpGet()]
        public async Task<JsonResult> GetForms()
        {
            
            
            // get all of the forms.
            var adxForms = await _system.Adx_entityforms.ExecuteAsync();
            List<ViewModels.Form> forms = new List<ViewModels.Form>();

            foreach (var adxForm in adxForms)
            {                
                ViewModels.Form form = new ViewModels.Form();
                form.name = adxForm.Adx_name;
                form.id = adxForm.Adx_entityformid == null ? "" : adxForm.Adx_entityformid.ToString();
                form.displayname = adxForm.Adx_formname;
                form.entity = adxForm.Adx_entityname;
                // get the form fields.  
                string key = "SystemForm_FormXML_" + form.entity;
                form.formxml = await _distributedCache.GetStringAsync(key);
                
                
                forms.Add(form);
            }
            return Json(forms);
        }

        
    }
}
