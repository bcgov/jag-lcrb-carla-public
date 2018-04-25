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
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class FormsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System context;
        private string encryptionKey;

        public FormsController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration)
        {
            Configuration = configuration;
            this.context = context;            
        }
        [HttpGet()]
        public async Task<JsonResult> GetForms()
        {
            var systemForms = await context.Systemforms.ExecuteAsync();
            Dictionary<string, string> fields = new Dictionary<string, string>();

            foreach (var item in systemForms)
            {
                if (item.Formid != null)
                {
                    if (!fields.ContainsKey(item.Objecttypecode))
                    {
                        fields.Add(item.Objecttypecode, item.Formxml);
                    }
                    
                }
                
            }
            
            // get all of the forms.
            var adxForms = await context.Adx_entityforms.ExecuteAsync();
            List<ViewModels.Form> forms = new List<ViewModels.Form>();

            foreach (var adxForm in adxForms)
            {                
                ViewModels.Form form = new ViewModels.Form();
                form.name = adxForm.Adx_name;
                form.id = adxForm.Adx_entityformid == null ? "" : adxForm.Adx_entityformid.ToString();
                form.displayname = adxForm.Adx_formname;
                form.entity = adxForm.Adx_entityname;

                // get the form fields.              
                if (adxForm.Adx_entityformid != null && fields.ContainsKey(form.entity))
                {
                    form.formxml = fields[form.entity];
                }
                
                forms.Add(form);
            }
            return Json(forms);
        }

        
    }
}
