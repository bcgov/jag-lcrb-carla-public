using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace Watchdog.Pages
{
    public static class AppTypeCheckerExtensions
    {
        public static void AddConfigItem(this Dictionary<string, string> obj, string name, string prefix, IConfigurationRoot input)
        {
            string sourceKey = $"{prefix}_{name}";
            string value = input[sourceKey];
            if (!string.IsNullOrEmpty(value))
            {
                obj.Add(name, value);
            }
        }

        public static Boolean DynamicsAttributeToBoolean(this XAttribute attribute)
        {
            Boolean result = false;
            if (attribute != null)
            {
                string value = attribute.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    result = (value.Equals("1") || value.ToLower().Equals("true"));
                }
            }

            return result;
        }
    }

    public class ApplicationTypesCheckModel : PageModel
    {
        private readonly IConfigurationRoot Configuration;
        

        public ApplicationTypesCheckModel(IConfiguration configuration)
        {
            Configuration = (ConfigurationRoot)configuration;
            allKeys = new List<string>();

            allRowSizes = new Dictionary<string, int>();
            allFieldNames = new Dictionary<string, List<string>>();
            devFieldNames = new Dictionary<string, List<string>>();
            tstFieldNames = new Dictionary<string, List<string>>();
            prdFieldNames = new Dictionary<string, List<string>>();

            devAppTypes = new Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype>();
            tstAppTypes = new Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype>();
            prdAppTypes = new Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype>();

            GetAppTypes ("DEV", Configuration, devAppTypes, devFieldNames, allKeys);
            GetAppTypes ("TST", Configuration, tstAppTypes, tstFieldNames, allKeys);
            GetAppTypes ("PRD", Configuration, prdAppTypes, prdFieldNames, allKeys);
            
        }

        public List<string> allKeys;

        public Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype> devAppTypes;
        public Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype> tstAppTypes;
        public Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype> prdAppTypes;

        public Dictionary<string, int> allRowSizes;

        public Dictionary<string, List<string>> allFieldNames;
        public Dictionary<string, List<string>> devFieldNames;
        public Dictionary<string, List<string>> tstFieldNames;
        public Dictionary<string, List<string>> prdFieldNames;


        private void GetAppTypes (string prefix, IConfigurationRoot configuration, Dictionary<string, MicrosoftDynamicsCRMadoxioApplicationtype> appTypesDict, Dictionary<string, List<string>> envFields, List<string> allKeys)
        {
            IConfigurationRoot config = CreateConfig(prefix, configuration);
            IDynamicsClient client = DynamicsSetupUtil.SetupDynamics(config);

            // get all of the app types.
            

            var appTypes = client.Applicationtypes.Get().Value;

            foreach (var item in appTypes)
            {
                appTypesDict.Add(item.AdoxioName, item);
                if (! allKeys.Contains (item.AdoxioName))
                {
                    allKeys.Add(item.AdoxioName);
                }

                if (! string.IsNullOrEmpty (item.AdoxioFormreference))
                {
                    List<string> fields = new List<string>();

                    // add the form fields.
                    try
                    {
                        var systemForm = client.Systemforms.GetByKey(item.AdoxioFormreference);

                        string formXml = systemForm.Formxml;

                        

                        var tabs = XDocument.Parse(formXml).XPathSelectElements("form/tabs/tab");
                        if (tabs != null)
                        {

                            foreach (var tab in tabs)
                            {
                                // get the sections
                                var sections = tab.XPathSelectElements("columns/column/sections/section");
                                foreach (var section in sections)
                                {
                                    var sectionLabels = section.XPathSelectElements("labels/label");

                                    string sectionName = "";
                                    foreach (var sectionLabel in sectionLabels)
                                    {
                                        sectionName = sectionLabel.Attribute("description").Value;
                                    }
                                    
                                    // get the cells.
                                    var cells = section.XPathSelectElements("rows/row/cell");

                                    foreach (var cell in cells)
                                    {
                                        bool cellShowLabel = cell.Attribute("showlabel").DynamicsAttributeToBoolean();
                                        var control = cell.XPathSelectElement("control");

                                        string fieldName = "";
                                        if (cellShowLabel)
                                        {
                                            var cellLabels = cell.XPathSelectElements("labels/label");
                                            foreach (var cellLabel in cellLabels)
                                            {
                                                fieldName = cellLabel.Attribute("description").Value;
                                            }

                                        }
                                        else // use section name
                                        {
                                            fieldName = sectionName;
                                        }

                                        if (!string.IsNullOrEmpty(fieldName) && control != null && control.Attribute("datafieldname") != null)
                                        {
                                            string datafieldname = control.Attribute("datafieldname").Value;
                                        }

                                        if (! allFieldNames.ContainsKey(item.AdoxioName))
                                        {
                                            allFieldNames.Add(item.AdoxioName, new List<string>());
                                        }
                                        if (! allFieldNames[item.AdoxioName].Contains (fieldName))
                                        {
                                            allFieldNames[item.AdoxioName].Add(fieldName);
                                        }

                                        // add for this environment.
                                        if (!envFields.ContainsKey(item.AdoxioName))
                                        {
                                            envFields.Add(item.AdoxioName, new List<string>());
                                        }
                                        if (!envFields[item.AdoxioName].Contains(fieldName))
                                        {
                                            envFields[item.AdoxioName].Add(fieldName);
                                        }


                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    

                }

                // now calculate the row height.

                int rowSize = 4;
                if (allFieldNames.ContainsKey(item.AdoxioName))
                {
                    rowSize += allFieldNames[item.AdoxioName].Count + 1;
                }

                if (!allRowSizes.ContainsKey(item.AdoxioName))
                {
                    allRowSizes.Add(item.AdoxioName, rowSize);
                }
                else
                {
                    if (allRowSizes[item.AdoxioName] < rowSize)
                    {
                        allRowSizes[item.AdoxioName] = rowSize;
                    }
                }
            }

        }

        


        private IConfigurationRoot CreateConfig (string prefix, IConfigurationRoot input)
        {
            var strings = new Dictionary<string, string>();

            strings.AddConfigItem("ADFS_OAUTH2_URI", prefix, input);
            strings.AddConfigItem("DYNAMICS_APP_GROUP_RESOURCE", prefix, input);
            strings.AddConfigItem("DYNAMICS_APP_GROUP_CLIENT_ID", prefix, input);
            strings.AddConfigItem("DYNAMICS_APP_GROUP_SECRET", prefix, input);
            strings.AddConfigItem("DYNAMICS_USERNAME", prefix, input);
            strings.AddConfigItem("DYNAMICS_PASSWORD", prefix, input);
            strings.AddConfigItem("DYNAMICS_ODATA_URI", prefix, input);
            strings.AddConfigItem("DYNAMICS_NATIVE_ODATA_URI", prefix, input);


            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            // Add defaultConfigurationStrings
            configurationBuilder.AddInMemoryCollection(strings);
            return (IConfigurationRoot) configurationBuilder.Build();
        }

        public void OnGet()
        {
            


        }
    }
}
