
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Microsoft.Extensions.Caching.Distributed;
using System.Xml.Linq;
using Microsoft.OData.Client;
using Gov.Lclb.Cllb.Interfaces.Models;

namespace Gov.Lclb.Cllb.Interfaces
{
    public static class DynamicsExtensions
    {

        public static string GetEntityURI(this IDynamicsClient client, string entityType, string id)
        {
            string result = "";
            result = client.NativeBaseUri + entityType + "(" + id.Trim() + ")";
            return result;
        }
		/// <summary>
        /// Utility method to call Dynamics <see langword="async"/>, with a delay to compensate for timing issues.
        /// </summary>
        /// <param name="system"></param>
        /// <returns>dsr</returns>
        public static DataServiceResponse SaveChangesSynchronous(this Microsoft.Dynamics.CRM.System system)
        {
            Task<DataServiceResponse> t = system.SaveChangesAsync();
            System.Threading.Thread.Sleep(5000);
            t.Wait();
            if (t.IsFaulted)
            {
                throw new Exception("Error save changes failed:" + t.Exception.Message);
            }
            else if (!t.IsCompletedSuccessfully)
            {
                throw new Exception("Error save changes failed with no message");
            }
            DataServiceResponse dsr = t.Result;
            return dsr;
        }

		/// <summary>
        /// Utility method to call Dynamics <see langword="async"/>, with a delay to compensate for timing issues.
        /// </summary>
		/// <param name="system"></param>
		/// <param name="options"></param>
        /// <returns>dsr</returns>
		public static DataServiceResponse SaveChangesSynchronous(this Microsoft.Dynamics.CRM.System system, SaveChangesOptions options)
		{
			Task<DataServiceResponse> t = system.SaveChangesAsync(options);
            System.Threading.Thread.Sleep(5000);
            t.Wait();
            if (t.IsFaulted)
            {
                throw new Exception("Error save changes failed:" + t.Exception.Message);
            }
            else if (!t.IsCompletedSuccessfully)
            {
                throw new Exception("Error save changes failed with no message");
            }
            DataServiceResponse dsr = t.Result;
			return dsr;
		}

        /// <summary>
        /// Return the ID assigned by dynamics, or NULL if none.
        /// </summary>
        /// <param name="dsr"></param>
        /// <returns></returns>
        public static Guid? GetAssignedId(this DataServiceResponse dsr)
        {
            Guid? result = null;
            if (dsr != null)
            {
				var ienum = dsr.GetEnumerator();
                while (ienum.MoveNext())
                {
                    ChangeOperationResponse cor = (ChangeOperationResponse)ienum.Current;
                    if (cor.Descriptor is EntityDescriptor)
                    {
                        EntityDescriptor ed = (EntityDescriptor)cor.Descriptor;
                        string identity = ed.Identity.ToString();
                        // convert the identity to a guid.
                        int endpos = identity.LastIndexOf(")");
                        int startpos = identity.LastIndexOf("(") + 1;
                        string guid = identity.Substring(startpos, endpos - startpos);
                        result = Guid.ParseExact(guid, "D");
						return result;
                    }
                }
            }            
            return result;
        }

		public static Guid? GetAssignedIdOfType(this DataServiceResponse dsr, string entityType)
		{
			Guid? result = null;
			if (dsr != null)
			{
				var ienum = dsr.GetEnumerator();
				while (ienum.MoveNext())
				{
					ChangeOperationResponse cor = (ChangeOperationResponse)ienum.Current;
					if (cor.Descriptor is EntityDescriptor)
					{
						EntityDescriptor ed = (EntityDescriptor)cor.Descriptor;
						string identity = ed.Identity.ToString();
						// convert the identity to a guid.
						if (identity.Contains(entityType))
						{
							int endpos = identity.LastIndexOf(")");
							int startpos = identity.LastIndexOf("(") + 1;
							string guid = identity.Substring(startpos, endpos - startpos);
							result = Guid.ParseExact(guid, "D");
							return result;
						}
					}
				}
			}
			return result;
		}

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

            
            if (!user.ContactId.ToString().Equals(guid, StringComparison.OrdinalIgnoreCase))
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
            Contact contact = await system.GetContactBySiteminderGuid(distributedCache, smUserId);
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
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Account> GetAccountBySiteminderBusinessGuid(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string siteminderId)
        {
            Account result = null;
            string key = "Account_" + siteminderId;
            // first look in the cache.
            string temp = null;
            if (distributedCache != null)
            {
                temp = distributedCache.GetString(key);
            }
            if (!string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<Account>(temp);
            }
            else
            {
                // fetch from Dynamics.
                try
                {
					var accounts = await system.Accounts.AddQueryOption("$filter", "adoxio_externalid eq '" + siteminderId + "'").ExecuteAsync();
					if (accounts != null)
                    {
                        List<Account> results = new List<Account>();
                        foreach (var accountEntity in accounts)
                        {
                            results.Add(accountEntity);
                        }
						if (results.Count == 0)
						{
							result = null;
						}
						else
						{
							result = results.First();
						}
                    }
                    else
                    {
                        result = null;
                    }
                }
                catch (DataServiceQueryException dsqe)
                {
					if (dsqe.Message.Contains("Does Not Exist"))
						result = null;
					else
						throw;
                }
                
                if (result != null && distributedCache != null)
                {
                    // store the contact data.
                    string cacheValue = JsonConvert.SerializeObject(result);
                    distributedCache.SetString(key, cacheValue);
                }
            }

            return result;
        }

        
        /// <summary>
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MicrosoftDynamicsCRMaccount> GetAccountBySiteminderBusinessGuid(this IDynamicsClient system, string siteminderId)
        {
            MicrosoftDynamicsCRMaccount result = null;
            var accountResponse = await system.Accounts.GetAsync(filter: "adoxio_externalid eq '" + siteminderId + "'");
            result = accountResponse.Value.FirstOrDefault();
            // get the primary contact.
            if (result != null && result.Primarycontactid == null && result._primarycontactidValue != null)
            {
                result.Primarycontactid = await system.GetContactById(Guid.Parse(result._primarycontactidValue));
            }

            return result;
            
        }


        /// <summary>
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<Account> GetAccountById(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, Guid id)
        {
            Account result = null;

            // fetch from Dynamics.
            try
            {
                result = await system.Accounts.ByKey(id).GetValueAsync();                
            }
            catch (DataServiceQueryException dsqe)
            {
				if (dsqe.Message.Contains("Does Not Exist") || dsqe.InnerException.Message.Contains("Does Not Exist"))
                    result = null;
                else
                    throw;
            }
            return result;
        }

        /// <summary>
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MicrosoftDynamicsCRMaccount> GetAccountById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMaccount result;
            try
            {
                // fetch from Dynamics.
                result = await system.Accounts.GetByKeyAsync(id.ToString());
            }            
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
            }

            // get the primary contact.
            if (result != null && result.Primarycontactid == null && result._primarycontactidValue != null)
            {
                try
                {
                    result.Primarycontactid = await system.GetContactById(Guid.Parse(result._primarycontactidValue));
                }
                catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
                {
                    result.Primarycontactid = null;
                }
            }
            return result;
        }


        /// <summary>
        /// Get an Invoice by the Id
        /// </summary>
        /// <param name="system">Re</param>
        /// <param name="id"></param>
        /// <returns>The Invoice, or null if it does not exist</returns>
        public static async Task<MicrosoftDynamicsCRMinvoice> GetInvoiceById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMinvoice result;
            try
            {
                // fetch from Dynamics.
                result = await system.Invoices.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
            }
            return result;
        }


        public static async Task<MicrosoftDynamicsCRMadoxioLegalentity> GetLegalEntityById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioLegalentity result;
            try
            {
                // fetch from Dynamics.
                result = await system.Adoxiolegalentities.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
            }
            return result;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioTiedhouseconnection> GetTiedHouseConnectionById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection result;
            try
            {
                // fetch from Dynamics.
                result = await system.AdoxioTiedhouseconnections.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
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
        public static async Task<Adoxio_licencetype> GetLicenceTypeById(this Microsoft.Dynamics.CRM.System system, Guid id)
        {
            Adoxio_licencetype result = null;
            
            // fetch from Dynamics.
            try
            {
                result = await system.Adoxio_licencetypes.ByKey(id).GetValueAsync();
            }
            catch (DataServiceQueryException dsqe)
            {
                if (dsqe.Message.Contains("Does Not Exist"))
                    result = null;
                else
                    throw;
            }

            return result;
        }


        public static async Task<MicrosoftDynamicsCRMcontact> GetContactById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMcontact result;
            try
            {
                // fetch from Dynamics.
                result = await system.Contacts.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
            }
            return result;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioApplication result;
            try
            {
                // fetch from Dynamics.
                result = await system.Applications.GetByKeyAsync(id.ToString());

                if (result._adoxioLicencetypeValue != null)
                {
                    result.AdoxioLicenceType = await system.GetAdoxioLicencetypeById(Guid.Parse(result._adoxioLicencetypeValue));                    
                }

                if (result._adoxioApplicantValue != null)
                {
                    result.AdoxioApplicant = await system.GetAccountById(Guid.Parse(result._adoxioApplicantValue));
                }
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException ex)
            {
                result = null;
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
            string temp = null;
            if (distributedCache != null)
            {
                temp = distributedCache.GetString(key);
            }

            if (!string.IsNullOrEmpty(temp))
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
					if (dsqe.Message.Contains("Does Not Exist"))
                        result = null;
                    else
                        throw;
                }

                if (result != null && distributedCache != null)
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
        public static async Task<MicrosoftDynamicsCRMcontact> GetContactBySiteminderGuid(this IDynamicsClient system, string siteminderId)
        {
            MicrosoftDynamicsCRMcontact result = null;
            var contactsResponse = await system.Contacts.GetAsync(filter: "adoxio_externalid eq '" + siteminderId + "'");
            result = contactsResponse.Value.FirstOrDefault();			
            return result;
        }


        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static async Task<Contact> GetContactBySiteminderGuid(this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string siteminderId)
        {
            Contact result = null;
            string key = "Contact_" + siteminderId;
            // first look in the cache.
            string temp = null;
            if (distributedCache != null)
            {
                temp = distributedCache.GetString(key);
            }

            if (!string.IsNullOrEmpty(temp))
            {
                Guid id = new Guid(temp);
                result = await system.GetContactById(distributedCache, id);
            }
            else
            {
                try
                {                    
                    var contacts = await system.Contacts.AddQueryOption("$filter", "adoxio_externalid eq '" + siteminderId + "'").ExecuteAsync();

                    result = contacts.FirstOrDefault();
                    if (result != null && distributedCache != null)
                    {
                        // store the contact data.
                        Guid id = (Guid)result.Contactid;
                        distributedCache.SetString(key, id.ToString());
                        // update the cache for the contact.
                        string contact_key = "Contact_" + id.ToString();
                        string cacheValue = JsonConvert.SerializeObject(result);
                        distributedCache.SetString(contact_key, cacheValue);
                    }
                }
                catch (DataServiceQueryException dsqe)
                {                    
                    result = null;                    
                }
            }

            return result;
        }

        public static MicrosoftDynamicsCRMadoxioLegalentity GetAdoxioLegalentityByAccountId(this IDynamicsClient _dynamicsClient, Guid id)
		{
            MicrosoftDynamicsCRMadoxioLegalentity result = null;
			string accountFilter = "_adoxio_account_value eq " + id.ToString();

            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = _dynamicsClient.Adoxiolegalentities.Get(filter: accountFilter).Value;

            result = legalEntities.FirstOrDefault();
            
			return result;
		}


        public static async Task<MicrosoftDynamicsCRMadoxioLicencetype> GetAdoxioLicencetypeById(this IDynamicsClient _dynamicsClient, Guid id)
        {
            MicrosoftDynamicsCRMadoxioLicencetype result = null;

            try
            {
                result = _dynamicsClient.AdoxioLicencetypes.GetByKey(id.ToString());
            }
            catch (DataServiceQueryException dsqe)
            {
                result = null;
            }

            return result;
        }

		public static MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeByName(this IDynamicsClient _dynamicsClient, string name)
        {
			MicrosoftDynamicsCRMadoxioLicencetype result = null;
			string typeFilter = "adoxio_name eq '" + name + "'";

			IEnumerable<MicrosoftDynamicsCRMadoxioLicencetype> licenceTypes = _dynamicsClient.AdoxioLicencetypes.Get(filter: typeFilter).Value;

			result = licenceTypes.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Get a dynamics application by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="distributedCache"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MicrosoftDynamicsCRMadoxioapplication> GetAdoxioApplicationById(this Microsoft.Dynamics.CRM.System system, Guid id)
        {
			MicrosoftDynamicsCRMadoxioapplication result = null;
            // fetch from Dynamics.
            try
            {
				result = await system.Adoxio_applications.ByKey(id).GetValueAsync();
            }
            catch (DataServiceQueryException dsqe)
            {
                result = null;
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
        public static async Task<List<Public.ViewModels.OptionMetadata>> GetPicklistOptions (this Microsoft.Dynamics.CRM.System system, IDistributedCache distributedCache, string datafield)
        {
            List<Public.ViewModels.OptionMetadata> result = null;
            string key = "GlobalOptionSetDefinition_" + datafield;
            // first look in the cache.
            string temp = null;
            if (distributedCache != null)
            {
                temp = distributedCache.GetString(key);
            }
            if (!string.IsNullOrEmpty(temp))
            {
                result = JsonConvert.DeserializeObject<List<Public.ViewModels.OptionMetadata>>(temp);
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
                    result = new List<Public.ViewModels.OptionMetadata>();
                    foreach (Microsoft.Dynamics.CRM.OptionMetadata option in optionSetMetadata.Options)
                    {
                        result.Add(option.ToViewModel());
                    }

                }
                if (distributedCache != null)
                {
                    // store the picklist data.
                    string cacheValue = JsonConvert.SerializeObject(result);
                    distributedCache.SetString(key, cacheValue);
                }
            }

            return result;
        }
    }
}
