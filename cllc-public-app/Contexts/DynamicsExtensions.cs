
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Caching.Distributed;
using System.Xml.Linq;
using Microsoft.OData.Client;


namespace Gov.Lclb.Cllb.Public.Contexts
{
    public static class DynamicsExtensions
    {

        /// <summary>
        /// Load User from database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<User> LoadUser(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string userId, string guid = null)
        {
            User user = null;

            if (!string.IsNullOrEmpty(guid))
                user = await system.GetUserByGuid(distributedCache, guid);

            if (user == null)
                user = await system.GetUserBySmUserId(distributedCache, userId);

            if (user == null)
                return null;

            if (guid == null)
                return user;

            
            if (!user.Guid.Equals(guid, StringComparison.OrdinalIgnoreCase))
            {
                // invalid account - guid doesn't match user credential
                return null;
            }

            return user;
        }

        /// <summary>
        /// Returns a User based on the guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<User> GetUserByGuid(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string guid)
        {
            Guid id = new Guid(guid);
            User user = null;
            Contact contact = await system.GetContactById(distributedCache, id);
            if (contact != null)
            {
                user.FromContact(contact);
            }

            return user;
        }

        /// <summary>
        /// Returns a User based on the Siteminder ID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="smUserId"></param>
        /// <returns></returns>
        public static async Task<User> GetUserBySmUserId(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string smUserId)
        {
            User user = null;
            Contact contact = await system.GetContactBySiteminderId(distributedCache, smUserId);
            if (contact != null)
            {
                user = new User();
                user.FromContact(contact);
            }

            return user;
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
                    result = value.Substring(0, pos);
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
                    pos = value.IndexOf(" ");
                    result = value.Substring(pos + 1);
                }
            }
            return result;
        }

        public static string DynamicsControlClassidToName(this string value)
        {
            string result = "Unknown";
            // source for mappings:  https://msdn.microsoft.com/en-us/library/gg334472.aspx
            Dictionary<string, string> classidMap = new Dictionary<string, string>()
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
        /// Get a contact by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Account> GetAccountById(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, Guid id)
        {
            Account result = null;
            string key = "Account_" + id.ToString();
            // first look in the cache.
            string temp = distributedCache.GetString(key);
            if (!string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<Account>(temp);
            }
            else
            {
                // fetch from Dynamics.
                try
                {
                    result = await system.Accounts.ByKey(id).GetValueAsync();
                }
                catch (DataServiceQueryException dsqe)
                {
                    result = null;
                }
                
                if (result != null)
                {
                    // store the contact data.
                    string cacheValue = JsonConvert.SerializeObject(result);
                    distributedCache.SetString(key, cacheValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a contact by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Contact> GetContactById(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, Guid id)
        {
            Contact result = null;
            string key = "Contact_" + id.ToString();
            // first look in the cache.
            string temp = distributedCache.GetString(key);
            if (0 == 1 && !string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<Contact>(temp);
            }
            else
            {
                // fetch from Dynamics.
                try
                {
                    result = await system.Contacts.ByKey(id).GetValueAsync();
                }
                catch (DataServiceQueryException dsqe)
                {
                    result = null;
                }

                if (result != null)
                {
                    // store the contact data.
                    string cacheValue = JsonConvert.SerializeObject(result);
                    distributedCache.SetString(key, cacheValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static async Task<Contact> GetContactBySiteminderId(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string siteminderId)
        {
            Contact result = null;
            string key = "Contact_" + siteminderId;
            // first look in the cache.
            string temp = distributedCache.GetString(key);
            if (! string.IsNullOrEmpty(temp))
            {
                Guid id = new Guid(temp);
                result = await system.GetContactById(distributedCache, id);
            }
            else
            {
                // fetch from Dynamics.

                var contacts = await system.Contacts.AddQueryOption("$filter", "employeeid eq '" + siteminderId + "'").ExecuteAsync();

                result = contacts.FirstOrDefault();
                if (result != null)
                {
                    // store the contact data.
                    Guid id = (Guid) result.Contactid;
                    distributedCache.SetString(key, id.ToString());
                    // update the cache for the contact.
                    string contact_key = "Contact_" + id.ToString();
                    string cacheValue = JsonConvert.SerializeObject(result);
                    distributedCache.SetString(contact_key, cacheValue);
                }                
            }

            return result;
        }

        /// <summary>
        /// Get picklist options for a given field
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="datafield"></param>
        /// <returns></returns>
        public static async Task<List<ViewModels.OptionMetadata>> GetPicklistOptions (this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string datafield)
        {
            List<ViewModels.OptionMetadata> result = null;
            string key = "GlobalOptionSetDefinition_" + datafield;
            // first look in the cache.

            string temp = distributedCache.GetString(key);
            if (!string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<List<ViewModels.OptionMetadata>>(temp);
            }
            else
            {
                // fetch from dynamics
                var source = system.GlobalOptionSetDefinitions;
                OptionSetMetadataBaseSingle optionSetMetadataBaseSingle = new OptionSetMetadataBaseSingle (source.Context, source.GetKeyPath("Name='"+datafield+"'"));

                OptionSetMetadata optionSetMetadata = (OptionSetMetadata) await optionSetMetadataBaseSingle.GetValueAsync();

                if (optionSetMetadata != null)
                {
                    // convert it to a list.
                    result = new List<ViewModels.OptionMetadata>();
                    foreach (Microsoft.Dynamics.CRM.OptionMetadata option in optionSetMetadata.Options)
                    {
                        result.Add(option.ToViewModel());
                    }
                }
                // store the picklist data.
                string cacheValue = JsonConvert.SerializeObject(result);
                distributedCache.SetString(key, cacheValue);
            }

            return result;
        }
    }
}
