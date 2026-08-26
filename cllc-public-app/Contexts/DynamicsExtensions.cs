extern alias DV;
using Gov.Lclb.Cllb.Interfaces;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Mapping;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Gov.Lclb.Cllb.Interfaces
{
    public static class DynamicsExtensions
    {

        public static string GetPhsLink(string contactId, IConfiguration _configuration)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/personal-history-summary/";

            string encryptionKey = _configuration["ENCRYPTION_KEY"];
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, encryptionKey));
            return result;
        }

        public static string GetCASLink(string contactId, IConfiguration _configuration)
        {
            string result = _configuration["BASE_URI"] + _configuration["BASE_PATH"] + "/cannabis-associate-screening/";
            //var ba = Guid.Parse(contactId).ToByteArray();
            string encryptionKey = _configuration["ENCRYPTION_KEY"];
            result += HttpUtility.UrlEncode(EncryptionUtility.EncryptStringHex(contactId, encryptionKey));
            return result;
        }


        public static async Task<List<Public.ViewModels.LicenseeChangeLog>> GetApplicationChangeLogsAsync(
            IDataverseClient dataverse, string applicationId, ILogger logger)
        {
            var result = new List<Public.ViewModels.LicenseeChangeLog>();
            try
            {
                var logs = await dataverse.GetLicenseeChangelogsByApplicationIdAsync(applicationId);
                foreach (var item in logs)
                    result.Add(item.ToViewModel());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error reading LegalEntityChangelog");
            }
            return result;
        }

        public static async Task<List<Public.ViewModels.LegalEntity>> GetLegalEntityChildrenAsync(
            IDataverseClient dataverse, string parentLegalEntityId, IConfiguration configuration,
            List<string>? processedEntities = null)
        {
            var result = new List<Public.ViewModels.LegalEntity>();
            processedEntities ??= new List<string>();
            var entities = await dataverse.GetLegalEntitiesByParentEntityIdAsync(parentLegalEntityId);
            foreach (var le in entities)
            {
                var vm = le.ToViewModel();
                if (!string.IsNullOrEmpty(vm.id) && !processedEntities.Contains(vm.id))
                {
                    processedEntities.Add(vm.id);
                    vm.children = await GetLegalEntityChildrenAsync(dataverse, vm.id, configuration, processedEntities);
                }
                if (!string.IsNullOrEmpty(vm.contactId))
                {
                    vm.PhsLink = GetPhsLink(vm.contactId, configuration);
                    vm.CasLink = GetCASLink(vm.contactId, configuration);
                }
                result.Add(vm);
            }
            return result;
        }

        public static async Task<Public.ViewModels.LegalEntity?> GetLegalEntityTreeAsync(
            IDataverseClient dataverse, string accountId, IConfiguration configuration)
        {
            var entities = await dataverse.GetLegalEntitiesByAccountIdAsync(accountId);
            var root = entities.FirstOrDefault(le => le.adoxio_LegalEntityOwned == null);
            if (root == null) return null;
            var result = root.ToViewModel();
            if (!string.IsNullOrEmpty(result.contactId))
            {
                result.PhsLink = GetPhsLink(result.contactId, configuration);
                result.CasLink = GetCASLink(result.contactId, configuration);
            }
            result.children = await GetLegalEntityChildrenAsync(dataverse, result.id, configuration);
            return result;
        }

        public static async Task<int> GetNotTerminatedCRSApplicationCountAsync(
            IDataverseClient dataverse, string accountId)
        {
            var crsType = await dataverse.GetLicenceTypeByNameAsync("Cannabis Retail Store");
            if (crsType == null) return 0;
            var excludeStatuses = new List<int>
            {
                (int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated,
                (int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused,
                (int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled,
                (int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded
            };
            var apps = await dataverse.GetApplicationsByApplicantAndTypeAsync(accountId, null, excludeStatuses);
            return apps.Count(app => app.adoxio_LicenceType?.Id == crsType.Id);
        }

        /// <summary>
        /// Convert a Dynamics attribute to boolean
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get the first name from
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetFirstName(this string value)
        {
            string result = "";
            if (value != null)
            {
                int pos = value.IndexOf(",");
                if (pos > -1)
                {
                    // last name, first
                    result = value.Substring(pos + 1);
                }
                else
                {
                    pos = value.IndexOf(" ");
                    if (pos > -1)
                    {
                        result = value.Substring(0, pos);
                    }
                    else
                    {
                        result = "";
                    }
                }
            }
            return result;
        }

        public static string GetLastName(this string value)
        {
            string result = "";
            if (value != null)
            {
                int pos = value.IndexOf(",");
                if (pos > -1)
                {
                    // last name, first
                    result = value.Substring(0, pos);
                }
                else
                {
                    pos = value.IndexOf(" "); // For example, Basic BCeID is Firstname<space>Lastname
                    if (pos > -1)
                    {
                        result = value.Substring(pos + 1);
                    }
                    else
                    {
                        result = "";
                    }
                }
            }
            return result;
        }

        public static string DynamicsControlClassidToName(this string value)
        {
            string result = "Unknown";
            // source for mappings:  https://msdn.microsoft.com/en-us/library/gg334472.aspx
            Dictionary<string, string> classidMap = new Dictionary<string, string>
            {
                { "{F93A31B2-99AC-4084-8EC2-D4027C31369A}","AccessPrivilegeControl" },
                { "{3F4E2A56-F102-4B4D-AB9C-F1574CA5BFDA}","AccessTeamEntityPicker" },
                { "{C72511AB-84E5-4FB7-A543-25B4FC01E83E}","ActivitiesContainerControl" },
                { "{6636847D-B74D-4994-B55A-A6FAF97ECEA2}","ActivitiesWallControl" },
                { "{F02EF977-2564-4B9A-B2F0-DF083D8A019B}","ArticleContentControl" },
                { "{00AD73DA-BD4D-49C6-88A8-2F4F4CAD4A20}","ButtonControl" },
                { "{B0C6723A-8503-4FD7-BB28-C8A06AC933C2}","CheckBoxControl" },
                { "{DB1284EF-9FFC-4E99-B382-0CC082FE2364}","CompositionLinkControl" },
                { "{3246F906-1F71-45F7-B11F-D7BE0F9D04C9}","ConnectionControl" },
                { "{821ACF1A-7E46-4A0C-965D-FE14A57D78C7}","ConnectionRoleObjectTypeListControl" },
                { "{4168A05C-D857-46AF-8457-5BB47EB04EA1}","CoverPagePicklistControl" },
                { "{F9A8A302-114E-466A-B582-6771B2AE0D92}","CustomControl" },
                { "{5B773807-9FB2-42DB-97C3-7A91EFF8ADFF}","DateTimeControl" },
                { "{C3EFE0C3-0EC6-42BE-8349-CBD9079DFD8E}","DecimalControl" },
                { "{AA987274-CE4E-4271-A803-66164311A958}","DurationControl" },
                { "{6896F004-B17A-4202-861E-8B7EA2080E0B}","DynamicPropertyListControl" },
                { "{ADA2203E-B4CD-49BE-9DDF-234642B43B52}","EmailAddressControl" },
                { "{6F3FB987-393B-4D2D-859F-9D0F0349B6AD}","EmailBodyControl" },
                { "{F4C16ECA-CA81-4E39-9448-834B8378721E}","ErrorStatusControl" },
                { "{0D2C745A-E5A8-4C8F-BA63-C6D3BB604660}","FloatControl" },
                { "{FD2A7985-3187-444E-908D-6624B21F69C0}","FrameControl" },
                { "{E7A81278-8635-4D9E-8D4D-59480B391C5B}","GridControl" },
                { "{5546E6CD-394C-4BEE-94A8-4425E17EF6C6}","HiddenInputControl" },
                { "{C6D124CA-7EDA-4A60-AEA9-7FB8D318B68F}","IntegerControl" },
                { "{A62B6FA9-169E-406C-B1AA-EAB828CB6026}","KBViewerControl" },
                { "{5635c4df-1453-413e-b213-e81b65411150}","LabelControl" },
                { "{671A9387-CA5A-4D1E-8AB7-06E39DDCF6B5}","LanguagePicker" },
                { "{DFDF1CDE-837B-4AC9-98CF-AC74361FD89D}","LinkControl" },
                { "{270BD3DB-D9AF-4782-9025-509E298DEC0A}","LookupControl" },
                { "{B634828E-C390-444A-AFE6-E07315D9D970}","MailMergeLanguagePicker" },
                { "{91DC0675-C8B9-4421-B1E0-261CEBF02BAC}","MapLinkControl" },
                { "{62B0DF79-0464-470F-8AF7-4483CFEA0C7D}","MapsControl" },
                { "{533B9E00-756B-4312-95A0-DC888637AC78}","MoneyControl" },
                { "{06375649-C143-495E-A496-C962E5B4488}","NotesControl" },
                { "{CBFB742C-14E7-4A17-96BB-1A13F7F64AA2}","PartyListControl" },
                { "{8C10015A-B339-4982-9474-A95FE05631A5}","PhoneNumberControl" },
                { "{3EF39988-22BB-4F0B-BBBE-64B5A3748AEE}","PicklistControl" },
                { "{2305E33A-BAD3-4022-9E15-1856CF218333}","PicklistLookupControl" },
                { "{5D68B988-0661-4DB2-BC3E-17598AD3BE6C}","PicklistStatusControl" },
                { "{06E9F7AF-1F54-4681-8EEC-1E21A1CEB465}","ProcessControl" },
                { "{5C5600E0-1D6E-4205-A272-BE80DA87FD42}","QuickFormCollectionControl" },
                { "{69AF7DCA-2E3B-4EE7-9201-0DA731DD2413}","QuickFormControl" },
                { "{67FAC785-CD58-4F9F-ABB3-4B7DDC6ED5ED}","RadioControl" },
                { "{F3015350-44A2-4AA0-97B5-00166532B5E9}","RegardingControl" },
                { "{163B90A6-EB64-49D2-9DF8-3C84A4F0A0F8}","RelatedInformationControl" },
                { "{5F986642-5961-4D9F-AB5E-643D71E231E9}","RelationshipRolePicklist" },
                { "{A28F441B-916C-4865-87FD-0C5D53BD59C9}","ReportControl" },
                { "{E616A57F-20E0-4534-8662-A101B5DDF4E0}","SearchWidget" },
                { "{86B9E25E-695E-4FEF-AC69-F05CFA96739C}","SocialInsightControl" },
                { "{E0DECE4B-6FC8-4A8F-A065-082708572369}","TextAreaControl" },
                { "{4273EDBD-AC1D-40D3-9FB2-095C621B552D}","TextBoxControl" },
                { "{1E1FC551-F7A8-43AF-AC34-A8DC35C7B6D4}","TickerControl" },
                { "{9C5CA0A1-AB4D-4781-BE7E-8DFBE867B87E}","TimerControl" },
                { "{7C624A0B-F59E-493D-9583-638D34759266}","TimeZonePicklistControl" },
                { "{71716B6C-711E-476C-8AB8-5D11542BFB47}","UrlControl" },
                { "{9FDF5F91-88B1-47F4-AD53-C11EFC01A01D}","WebResourceHtmlControl" },
                { "{587CDF98-C1D5-4BDE-8473-14A0BC7644A7}","WebResourceImageControl" },
                { "{080677DB-86EC-4544-AC42-F927E74B491F}","WebResourceSilverlightControl" }
            };
            if (value != null && classidMap.ContainsKey(value.ToUpper()))
            {
                result = classidMap[value.ToUpper()];
            }
            return result;
        }


        /// <summary>
        /// Convert a service card ID string into a format that is useful (and fits into Dynamics)
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string GetServiceCardID(string raw)
        {
            string result = "";
            if (!string.IsNullOrEmpty(raw))
            {
                var tokens = raw.Split('|');
                if (tokens.Length > 0)
                {
                    result = tokens[0];
                }

                if (!string.IsNullOrEmpty(result))
                {
                    tokens = result.Split(':');
                    result = tokens[tokens.Length - 1];
                }
            }
            result = GuidUtility.SanitizeGuidString(result);
            return result;
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        public static bool CurrentUserIsContact(string contactId, IHttpContextAccessor _httpContextAccessor)
        {
            // get the current user.

            UserSettings userSettings = UserSettings.CreateFromHttpContext(_httpContextAccessor);

            if (userSettings.ContactId != null && userSettings.ContactId.Length > 0)
            {
                return userSettings.ContactId == contactId;
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        public static async Task<bool> CurrentUserHasAccessToAccountAsync(Guid accountId, IHttpContextAccessor _httpContextAccessor, IDataverseClient _dataverse)
        {
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            if (!string.IsNullOrEmpty(temp))
            {
                var userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
                if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                    return userSettings.AccountId == accountId.ToString() || await IsChildAccountAsync(userSettings.AccountId, accountId.ToString(), _dataverse);
            }
            return false;
        }

        private static async Task<bool> IsChildAccountAsync(string parentAccountId, string childAccountId, IDataverseClient _dataverse)
        {
            var legalEntities = await _dataverse.GetLegalEntitiesByAccountIdAsync(parentAccountId);
            if (legalEntities.Any(e => e.adoxio_ShareholderAccountID?.Id.ToString() == childAccountId))
                return true;
            var withShareholders = legalEntities.Where(e => e.adoxio_ShareholderAccountID != null).ToList();
            foreach (var le in withShareholders)
            {
                if (await IsChildAccountAsync(le.adoxio_ShareholderAccountID.Id.ToString(), childAccountId, _dataverse))
                    return true;
            }
            return false;
        }

        public static async Task<Public.ViewModels.Form> GetSystemformViewModelAsync(this IDataverseClient dataverse, IMemoryCache cache, ILogger logger, string formid)
        {
            Public.ViewModels.Form form = null;

            IList<DV::Gov.Lclb.Cllb.Interfaces.DynamicsPicklistAttributeMetadata> picklistMetadata = null;
            try
            {
                string cacheKey = CacheKeys.PicklistTypePrefix + "Application";
                if (cache == null || !cache.TryGetValue(cacheKey, out picklistMetadata))
                {
                    picklistMetadata = await dataverse.GetApplicationPicklistsAsync("adoxio_application");
                    if (cache != null && picklistMetadata != null)
                        cache.Set(cacheKey, picklistMetadata, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(365)));
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "ERROR getting application picklist metadata");
            }

            var applicationMapping = new ApplicationMapping();

            try
            {
                string formXml = await dataverse.GetSystemFormXmlByIdAsync(formid);
                if (formXml == null) return null;

                form = new Public.ViewModels.Form();
                form.id = formid;
                form.tabs = new List<Public.ViewModels.FormTab>();
                form.sections = new List<Public.ViewModels.FormSection>();

                var tabs = XDocument.Parse(formXml).XPathSelectElements("form/tabs/tab");
                if (tabs != null)
                {
                    foreach (var tab in tabs)
                    {
                        var tabLabel = tab.XPathSelectElement("labels/label");
                        string description = tabLabel.Attribute("description").Value;
                        string tabId = tabLabel.Attribute("id") == null ? "" : tabLabel.Attribute("id").Value;
                        bool tabShowLabel = tab.Attribute("showlabel").DynamicsAttributeToBoolean();
                        bool tabVisible = tab.Attribute("visible").DynamicsAttributeToBoolean();

                        var formTab = new Public.ViewModels.FormTab
                        {
                            id = tabId,
                            name = description,
                            sections = new List<Public.ViewModels.FormSection>(),
                            showlabel = tabShowLabel,
                            visible = tabVisible
                        };

                        var sections = tab.XPathSelectElements("columns/column/sections/section");
                        foreach (var section in sections)
                        {
                            bool sectionShowLabel = section.Attribute("showlabel").DynamicsAttributeToBoolean();
                            bool sectionVisible = section.Attribute("visible") == null || section.Attribute("visible").DynamicsAttributeToBoolean();

                            var formSection = new Public.ViewModels.FormSection
                            {
                                fields = new List<Public.ViewModels.FormField>(),
                                id = section.Attribute("id").Value,
                                name = section.Attribute("name").Value,
                                showlabel = sectionShowLabel,
                                visible = sectionVisible
                            };

                            foreach (var sectionLabel in section.XPathSelectElements("labels/label"))
                                formSection.label = sectionLabel.Attribute("description").Value;

                            foreach (var cell in section.XPathSelectElements("rows/row/cell"))
                            {
                                var formField = new Public.ViewModels.FormField
                                {
                                    showlabel = cell.Attribute("showlabel").DynamicsAttributeToBoolean(),
                                    visible = cell.Attribute("visible") == null || cell.Attribute("visible").DynamicsAttributeToBoolean(),
                                    name = cell.Attribute("name")?.Value ?? ""
                                };

                                if (formField.showlabel)
                                {
                                    foreach (var cellLabel in cell.XPathSelectElements("labels/label"))
                                        formField.label = cellLabel.Attribute("description").Value;
                                }
                                else
                                {
                                    formField.label = formSection.label;
                                    formSection.label = "";
                                }

                                var control = cell.XPathSelectElement("control");
                                if (!string.IsNullOrEmpty(formField.label) && control != null && control.Attribute("datafieldname") != null)
                                {
                                    formField.classid = control.Attribute("classid").Value;
                                    formField.controltype = formField.classid.DynamicsControlClassidToName();
                                    string datafieldname = control.Attribute("datafieldname").Value;
                                    formField.datafieldname = applicationMapping.GetViewModelKey(datafieldname);
                                    formField.required = applicationMapping.GetRequired(datafieldname);

                                    if (formField.controltype.Equals("PicklistControl"))
                                    {
                                        formField.options = new List<Public.ViewModels.OptionMetadata>();
                                        var metadata = picklistMetadata?.FirstOrDefault(x => x.LogicalName == datafieldname);
                                        if (metadata == null)
                                        {
                                            formField.options.Add(new Public.ViewModels.OptionMetadata { label = "INVALID PICKLIST", value = 0 });
                                        }
                                        else
                                        {
                                            var optionSet = metadata.OptionSet ?? metadata.GlobalOptionSet;
                                            if (optionSet != null)
                                            {
                                                foreach (var option in optionSet.Options)
                                                {
                                                    int? value = option.Value;
                                                    string label = option.Label?.UserLocalizedLabel?.Label;
                                                    formField.options.Add(value == null || label == null
                                                        ? new Public.ViewModels.OptionMetadata { label = "INVALID PICKLIST", value = 0 }
                                                        : new Public.ViewModels.OptionMetadata { label = label, value = value.Value });
                                                }
                                            }
                                        }
                                    }
                                    if (formField.datafieldname != null)
                                        formSection.fields.Add(formField);
                                }
                            }

                            formTab.sections.Add(formSection);
                            form.sections.Add(formSection);
                        }

                        form.tabs.Add(formTab);
                    }
                }
                else
                {
                    form.tabs.Add(new Public.ViewModels.FormTab { name = "" });
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unknown or invalid form reference - {formid}", formid);
            }

            return form;
        }

        public static bool IsMostlyLiquor(IList<Public.ViewModels.ApplicationTypeCategory?> categories)
        {
            if (categories == null || categories.Count == 0) return false;
            int liquorCount = categories.Count(c => c == Public.ViewModels.ApplicationTypeCategory.Liquor);
            return (liquorCount * 1.0f) >= (categories.Count * 1.0f) / 2.0f;
        }

        public static async Task<PaymentType> GetPaymentTypeAsync(
            this DV::Gov.Lclb.Cllb.Interfaces.adoxio_application application,
            IDataverseClient dataverse)
        {
            if (application?.adoxio_ApplicationTypeId == null) return PaymentType.CANNABIS;

            var appType = await dataverse.GetApplicationTypeByIdAsync(
                application.adoxio_ApplicationTypeId.Id.ToString());
            if (appType == null) return PaymentType.CANNABIS;

            bool isLiquor;
            if (appType.adoxio_name == "Licensee Changes" && application.adoxio_Applicant?.Id is Guid accountId)
            {
                var licences = await dataverse.GetLicencesByAccountIdAsync(accountId.ToString());
                int liquorCount = 0;
                foreach (var lic in licences)
                {
                    if (lic.adoxio_LicenceType?.Id is Guid ltId)
                    {
                        var lt = await dataverse.GetLicenceTypeByIdAsync(ltId.ToString());
                        if (lt?.adoxio_Category == DV::Gov.Lclb.Cllb.Interfaces.adoxio_licencetype_adoxio_category.Liquor)
                            liquorCount++;
                    }
                }
                isLiquor = licences.Count > 0
                    && (liquorCount * 1.0f) >= (licences.Count * 1.0f) / 2.0f;
            }
            else
            {
                isLiquor = appType.adoxio_Category ==
                    DV::Gov.Lclb.Cllb.Interfaces.adoxio_applicationtype_adoxio_category.Liquor;
            }

            return isLiquor ? PaymentType.LIQUOR : PaymentType.CANNABIS;
        }
    }
}
