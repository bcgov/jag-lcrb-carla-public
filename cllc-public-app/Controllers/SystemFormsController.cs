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
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class SystemFormsController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public SystemFormsController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = distributedCache;
        }
        [HttpGet()]
        public async Task<JsonResult> GetSystemForms()
        {

            // get all of the forms.
            var systemForms = await _system.Systemforms.ExecuteAsync();
            List<ViewModels.Form> forms = new List<ViewModels.Form>();

            foreach (var systemForm in systemForms)
            {
                ViewModels.Form form = new ViewModels.Form();
                form.name = systemForm.Name;
                form.id = systemForm.Formidunique == null ? "" : systemForm.Formidunique.ToString();

                //adxForm.Adx_entityformid == null ? "" : adxForm.Adx_entityformid.ToString();
                //form.displayname = adxForm.Adx_formname;
                //form.entity = adxForm.Adx_entityname;
                form.entity = systemForm.Objecttypecode;
                // get the form fields.  
                string key = "SystemForm_FormXML_" + form.entity;
                form.formxml = systemForm.Formxml;

                forms.Add(form);
            }
            return Json(forms);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetSystemForm(string id)
        {
            string key = "SystemForm_" + id + "_FormXML";
            string formXml = await _distributedCache.GetStringAsync(key);
            ViewModels.Form form = new ViewModels.Form();
            form.id = id;
            form.formxml = formXml;
            form.tabs = new List<ViewModels.FormTab>();

            var tabs = XDocument.Parse(formXml).XPathSelectElements("form/tabs/tab");
            if (tabs != null)
            {
                foreach (var tab in tabs)
                {
                    var tabLabel = tab.XPathSelectElement("labels/label");
                    string description = tabLabel.Attribute("description").Value;
                    string tabId = tabLabel.Attribute("id").Value;
                    FormTab formTab = new FormTab();
                    formTab.name = description;
                    formTab.id = tabId;
                    

                    // get the sections
                    var sections = tab.XPathSelectElements("columns/column/sections/section");
                    foreach (var section in sections)
                    {
                        FormSection formSection = new FormSection();
                        formSection.name = section.Attribute("name").Value;

                        // get the fields.
                        var labels = tab.XPathSelectElements("labels");

                        foreach (var label in labels)
                        {
                            FormField formField = new FormField();
                            formField.name = label.Attribute("description").Value;
                            formSection.fields.Add(formField);
                        }

                        formTab.sections.Add(formSection);
                    }

                    form.tabs.Add(formTab);
                }
            }
            else // single tab form.
            {
                FormTab formTab = new FormTab();
                formTab.name = "";
                form.tabs.Add(formTab);
            }

            // extract the fields from the formxml.
            /*
            var sections = XDocument.Parse(formXml).XPathSelectElements("form/tabs/tab").Where(
                    tab => tab.XPathSelectElements("labels/label").Any(
                        label => label.Attributes("description").Any(description => description.Value == TabName))).SelectMany(tab => tab.XPathSelectElements("columns/column/sections/section"));
*/
            return Json(form);
        }
    }
}
