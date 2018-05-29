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
    public class SystemFormController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Contexts.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        public SystemFormController(Contexts.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache)
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
                form.entity = systemForm.Objecttypecode;
                // get the form fields.  
                string key = "SystemForm_FormXML_" + form.entity;
                form.formxml = systemForm.Formxml;

                forms.Add(form);
            }
            return Json(forms);
        }

        private string TranslateDatafieldname (string entity, string dataFieldName)
        {
            Dictionary<string, Dictionary<string, string>> fieldMaps = new Dictionary<string, Dictionary<string, string>>
            {
                { "account", new Dictionary<string, string>
                    {
                        { "accountid", "id" },
                        { "name","name"},
                        { "description", "description"}
                    }
                },
                {
                  "contact", new Dictionary<string,string>
                  {
                      { "fullname", "name" },
                      { "address1_city", "address1_city" },
                      { "address1_line1", "address1_line1" },
                      { "address1_postalcode", "address1_postalcode" },
                      { "address1_stateorprovince", "address1_stateorprovince" },
                      { "adoxio_stateprovince", "address1_stateorprovince" },
                      { "adoxio_canattendcompliancemeetings", "adoxio_canattendcompliancemeetings" },
                      { "adoxio_canobtainlicenceinfofrombranch", "adoxio_canobtainlicenceinfofrombranch" },
                      { "adoxio_canrepresentlicenseeathearings", "adoxio_canrepresentlicenseeathearings" },
                      { "adoxio_cansigngrocerystoreproofofsalesrevenue", "adoxio_cansigngrocerystoreproofofsalesrevenue"},
                      { "adoxio_cansignpermanentchangeapplications", "adoxio_cansignpermanentchangeapplications" },
                      { "adoxio_cansigntemporarychangeapplications", "adoxio_cansigntemporarychangeapplications" },
                      { "emailaddress1", "emailaddress1" },
                      { "firstname", "firstname" },
                      { "lastname", "lastname" },
                      { "telephone1", "telephone1" }
                  }
                }
            };

            Dictionary<string, string> entityFieldMap = fieldMaps[entity];
            string result = null;
            if (entityFieldMap.ContainsKey(dataFieldName))
            {
                result = entityFieldMap[dataFieldName];
            }
            return result;
        }


        [HttpGet("{id}")]
        public async Task<JsonResult> GetSystemForm(string id)
        {
            string entityKey = "SystemForm_" + id + "_Entity";
            string nameKey = "SystemForm_" + id + "_Name";
            string xmlKey = "SystemForm_" + id + "_FormXML";
            string formXml = await _distributedCache.GetStringAsync(xmlKey);
            ViewModels.Form form = new ViewModels.Form();
            form.id = id;
            form.entity = await _distributedCache.GetStringAsync(entityKey);
            form.name = await _distributedCache.GetStringAsync(nameKey);
            form.formxml = formXml;
            form.tabs = new List<ViewModels.FormTab>();

            var tabs = XDocument.Parse(formXml).XPathSelectElements("form/tabs/tab");
            if (tabs != null)
            {
                foreach (var tab in tabs)
                {
                    var tabLabel = tab.XPathSelectElement("labels/label");
                    string description = tabLabel.Attribute("description").Value;
                    string tabId = tabLabel.Attribute("id") == null ? "" : tabLabel.Attribute("id").Value;
                    Boolean tabShowLabel = tab.Attribute("showlabel").DynamicsAttributeToBoolean();
                    Boolean tabVisible = tab.Attribute("visible").DynamicsAttributeToBoolean();

                    FormTab formTab = new FormTab();
                    formTab.id = tabId;
                    formTab.name = description;
                    formTab.sections = new List<FormSection>();
                    formTab.showlabel = tabShowLabel;
                    formTab.visible = tabVisible;

                    // get the sections
                    var sections = tab.XPathSelectElements("columns/column/sections/section");
                    foreach (var section in sections)
                    {
                        Boolean sectionShowLabel = section.Attribute("showlabel").DynamicsAttributeToBoolean();
                        Boolean sectionVisible = section.Attribute("visible").DynamicsAttributeToBoolean();

                        FormSection formSection = new FormSection();
                        formSection.fields = new List<FormField>();
                        formSection.id = section.Attribute("id").Value;
                        formSection.showlabel = sectionShowLabel;
                        formSection.visible = sectionVisible;

                        // get the fields.
                        var sectionLabels = section.XPathSelectElements("labels/label");

                        // the section label is the section name.
                        foreach (var sectionLabel in sectionLabels)
                        {
                            formSection.name = sectionLabel.Attribute("description").Value;                            
                        }
                        // get the cells.
                        var cells = section.XPathSelectElements("rows/row/cell");
                        
                        foreach (var cell in cells)
                        {
                            FormField formField = new FormField();
                            // get cell visibility and showlabel
                            Boolean cellShowLabel = cell.Attribute("showlabel").DynamicsAttributeToBoolean();
                            Boolean cellVisible = cell.Attribute("visible").DynamicsAttributeToBoolean();
                            

                            formField.showlabel = cellShowLabel;
                            formField.visible = cellVisible;

                            // get the cell label. 


                            var cellLabels = cell.XPathSelectElements("labels/label");
                            foreach (var cellLabel in cellLabels)
                            {
                                formField.name = cellLabel.Attribute("description").Value;
                            }
                            // get the form field name.
                            var control = cell.XPathSelectElement("control");
                            if ( !string.IsNullOrEmpty(formField.name) && control != null && control.Attribute("datafieldname") != null)
                            {
                                // datafieldname has to be translated.
                                string originalFieldName = control.Attribute("datafieldname").Value;
                                formField.datafieldname = TranslateDatafieldname(form.entity, originalFieldName);
                                if (formField.datafieldname != null)
                                {
                                    formField.classid = control.Attribute("classid").Value;
                                    formField.controltype = formField.classid.DynamicsControlClassidToName();
                                    formField.required = control.Attribute("isrequired").DynamicsAttributeToBoolean();
                                    // special case for picklists.
                                    if (formField.controltype != null && formField.controltype.Equals("PicklistControl"))
                                    {
                                        // get the options for the picklist and add it to the field.
                                        List<ViewModels.OptionMetadata> options = await _system.GetPicklistOptions(_distributedCache, originalFieldName);
                                        formField.options = options;
                                    }

                                    formSection.fields.Add(formField);
                                }
                            }                            
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

            return Json(form);
        }
    }
}
