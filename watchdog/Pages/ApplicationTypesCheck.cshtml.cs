extern alias DV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using DataverseClient = DV::Gov.Lclb.Cllb.Interfaces.DataverseClient;
using adoxio_applicationtype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype;

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

            allFieldClasses = new Dictionary<string, Dictionary<string, string>>();

            devAppTypes = new Dictionary<string, adoxio_applicationtype>();
            tstAppTypes = new Dictionary<string, adoxio_applicationtype>();
            prdAppTypes = new Dictionary<string, adoxio_applicationtype>();

            Parallel.Invoke(
                delegate() { GetAppTypes("DEV", Configuration, devAppTypes, devFieldNames, allKeys); },
                delegate() { GetAppTypes("TST", Configuration, tstAppTypes, tstFieldNames, allKeys); },
                delegate() { GetAppTypes("PRD", Configuration, prdAppTypes, prdFieldNames, allKeys); }
            );

            foreach (var item in allFieldNames.Keys)
            {
                var d = new Dictionary<string, string>();

                foreach (var field in allFieldNames[item])
                {
                    d.Add(field, GetRowClass(devFieldNames[item].Contains(field).ToString(), tstFieldNames[item].Contains(field).ToString(), prdFieldNames[item].Contains(field).ToString()));
                }

                allFieldClasses.Add(item, d);
            }

        }

        public List<string> allKeys;

        public Dictionary<string, adoxio_applicationtype> devAppTypes;
        public Dictionary<string, adoxio_applicationtype> tstAppTypes;
        public Dictionary<string, adoxio_applicationtype> prdAppTypes;

        public Dictionary<string, int> allRowSizes;

        public Dictionary<string, List<string>> allFieldNames;
        public Dictionary<string, List<string>> devFieldNames;
        public Dictionary<string, List<string>> tstFieldNames;
        public Dictionary<string, List<string>> prdFieldNames;

        public Dictionary<string, Dictionary<string, string>> allFieldClasses;

        // true if there is a difference
        public bool IsDifferent (string dev, string test, string prod)
        {
            bool result = true;
            if (dev == test && test == prod)
            {
                result = false;
            }
            return result;
        }

        public string GetRowClass (string dev, string test, string prod)
        {
            string result;
            if (IsDifferent(dev, test, prod))
            {
                result = "different";
            }
            else
            {
                result = "same";
            }
            return result;
        }

        private void GetAppTypes (string prefix, IConfigurationRoot configuration, Dictionary<string, adoxio_applicationtype> appTypesDict, Dictionary<string, List<string>> envFields, List<string> allKeys)
        {
            IConfigurationRoot config = CreateConfig(prefix, configuration);
            var client = new DataverseClient(config);

            var appTypes = client.GetApplicationTypesAsync().GetAwaiter().GetResult();

            foreach (var item in appTypes)
            {
                appTypesDict.Add(item.adoxio_name, item);
                if (! allKeys.Contains (item.adoxio_name))
                {
                    allKeys.Add(item.adoxio_name);
                }

                if (! string.IsNullOrEmpty (item.adoxio_FormReference))
                {
                    List<string> fields = new List<string>();

                    // add the form fields.
                    try
                    {
                        string formXml = client.GetSystemFormXmlByIdAsync(item.adoxio_FormReference).GetAwaiter().GetResult();

                        if (formXml != null)
                        {
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

                                            if (! allFieldNames.ContainsKey(item.adoxio_name))
                                            {
                                                allFieldNames.Add(item.adoxio_name, new List<string>());
                                            }
                                            if (! allFieldNames[item.adoxio_name].Contains (fieldName))
                                            {
                                                allFieldNames[item.adoxio_name].Add(fieldName);
                                            }

                                            // add for this environment.
                                            if (!envFields.ContainsKey(item.adoxio_name))
                                            {
                                                envFields.Add(item.adoxio_name, new List<string>());
                                            }
                                            if (!envFields[item.adoxio_name].Contains(fieldName))
                                            {
                                                envFields[item.adoxio_name].Add(fieldName);
                                            }


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

                int rowSize = 31;
                if (allFieldNames.ContainsKey(item.adoxio_name))
                {
                    rowSize += allFieldNames[item.adoxio_name].Count + 1;
                }

                if (!allRowSizes.ContainsKey(item.adoxio_name))
                {
                    allRowSizes.Add(item.adoxio_name, rowSize);
                }
                else
                {
                    if (allRowSizes[item.adoxio_name] < rowSize)
                    {
                        allRowSizes[item.adoxio_name] = rowSize;
                    }
                }
            }

        }

        private IConfigurationRoot CreateConfig (string prefix, IConfigurationRoot input)
        {
            var strings = new Dictionary<string, string>();

            strings.AddConfigItem("DYNAMICS_ODATA_URI", prefix, input);
            strings.AddConfigItem("DYNAMICS_AAD_TENANT_ID", prefix, input);
            strings.AddConfigItem("DYNAMICS_APP_REG_CLIENT_ID", prefix, input);
            strings.AddConfigItem("DYNAMICS_APP_REG_CLIENT_KEY", prefix, input);

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
