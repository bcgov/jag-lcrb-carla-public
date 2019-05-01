
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utils;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gov.Lclb.Cllb.Interfaces
{
    public static class DynamicsExtensions
    {

        



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
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MicrosoftDynamicsCRMaccount> GetAccountBySiteminderBusinessGuid(this IDynamicsClient system, string siteminderId)
        {
            // ensure that the siteminderId does not have any dashes.
            string sanitizedSiteminderId = GuidUtility.SanitizeGuidString(siteminderId);

            MicrosoftDynamicsCRMaccount result = null;
            try
            {
                var accountResponse = await system.Accounts.GetAsync(filter: "adoxio_externalid eq '" + sanitizedSiteminderId + "'");
                result = accountResponse.Value.FirstOrDefault();
            }
            catch (Exception)
            {

                result = null;
            }

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
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
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
                catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
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
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
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
                result = await system.Legalentities.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
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
                result = await system.Tiedhouseconnections.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
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
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioWorker> GetWorkerById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioWorker result;
            try
            {
                // fetch from Dynamics.
                result = await system.Workers.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioAlias> GetAliasById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioAlias result;
            try
            {
                // fetch from Dynamics.
                result = await system.Aliases.GetByKeyAsync(id.ToString());
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
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
                    result.AdoxioLicenceType = system.GetAdoxioLicencetypeById(Guid.Parse(result._adoxioLicencetypeValue));
                }

                if (result._adoxioApplicantValue != null)
                {
                    result.AdoxioApplicant = await system.GetAccountById(Guid.Parse(result._adoxioApplicantValue));
                }
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioApplication> GetApplicationByIdWithChildren(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioApplication result;
            try
            {
                string[] expand = { "adoxio_localgovindigenousnationid", "adoxio_application_SharePointDocumentLocations", "adoxio_AssignedLicence", "adoxio_ApplicationTypeId" };

                // fetch from Dynamics.
                result = await system.Applications.GetByKeyAsync(id.ToString(), expand: expand);

                if (result._adoxioLicencetypeValue != null)
                {
                    string filter = $" eq {result._adoxioLicencetypeValue}";
                    result.AdoxioLicenceType = system.GetAdoxioLicencetypeById(Guid.Parse(result._adoxioLicencetypeValue));
                }

                if (result._adoxioApplicantValue != null)
                {
                    result.AdoxioApplicant = await system.GetAccountById(Guid.Parse(result._adoxioApplicantValue));
                }

                if (result.AdoxioAssignedLicence != null && result.AdoxioAssignedLicence._adoxioEstablishmentValue != null)
                {
                    result.AdoxioAssignedLicence.AdoxioEstablishment = system.GetEstablishmentById(Guid.Parse(result.AdoxioAssignedLicence._adoxioEstablishmentValue));
                }
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        public static IEnumerable<MicrosoftDynamicsCRMadoxioApplication> GetApplicationListByApplicant(this IDynamicsClient _dynamicsClient, string applicantId)
        {
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_AssignedLicence", "adoxio_LicenceType" };
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty(applicantId))
            {
                dynamicsApplicationList = _dynamicsClient.Applications.Get(expand: expand).Value;
            }
            else
            {
                var filter = $"_adoxio_applicant_value eq {applicantId} and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Denied}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                try
                {
                    dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                }
                catch (OdataerrorException)
                {
                    dynamicsApplicationList = null;
                }
            }
            return dynamicsApplicationList;
        }


        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMcontact GetContactByExternalId(this IDynamicsClient system, string siteminderId)
        {
            string sanitizedSiteminderId = GuidUtility.SanitizeGuidString(siteminderId);
            MicrosoftDynamicsCRMcontact result = null;
            var contactsResponse = system.Contacts.Get(filter: "adoxio_externalid eq '" + sanitizedSiteminderId + "'");
            result = contactsResponse.Value.FirstOrDefault();
            return result;
        }

        public static MicrosoftDynamicsCRMadoxioLegalentity GetAdoxioLegalentityByAccountId(this IDynamicsClient _dynamicsClient, Guid id)
        {
            MicrosoftDynamicsCRMadoxioLegalentity result = null;
            string accountFilter = "_adoxio_account_value eq " + id.ToString();
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = _dynamicsClient.Legalentities.Get(filter: accountFilter).Value;
            result = legalEntities.FirstOrDefault();
            return result;
        }


        public static MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(this IDynamicsClient _dynamicsClient, Guid id)
        {
            return _dynamicsClient.GetAdoxioLicencetypeById(id.ToString());
        }


        public static MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeById(this IDynamicsClient _dynamicsClient, string id)
        {
            MicrosoftDynamicsCRMadoxioLicencetype result = null;

            try
            {
                // EXPAND the list of application types for this licence type
                string[] expand = { "adoxio_licencetypes_applicationtypes" };
                result = _dynamicsClient.Licencetypes.GetByKey(adoxioLicencetypeid: id, expand: expand);
                
            }
            catch (OdataerrorException)
            {
                result = null;
            }

            // additional pass to populate the applicationtypes licencetype.
           /*
           if (result.AdoxioLicencetypesApplicationtypes != null)
           {
               foreach (var item in result.AdoxioLicencetypesApplicationtypes)
               {

                   if (item._adoxioLicencetypeidValue != null)
                   {
                       item.AdoxioLicenceTypeId = _dynamicsClient.GetAdoxioLicencetypeById(item._adoxioLicencetypeidValue);
                   }
               }
            }
           */


            return result;
        }


        public static MicrosoftDynamicsCRMadoxioEstablishment GetEstablishmentById(this IDynamicsClient _dynamicsClient, Guid id)
        {
            MicrosoftDynamicsCRMadoxioEstablishment result = null;

            try
            {
                result = _dynamicsClient.Establishments.GetByKey(id.ToString());
            }
            catch (OdataerrorException)
            {
                result = null;
            }

            return result;
        }


        public static MicrosoftDynamicsCRMadoxioLicencetype GetAdoxioLicencetypeByName(this IDynamicsClient _dynamicsClient, string name)
        {
            MicrosoftDynamicsCRMadoxioLicencetype result = null;
            string typeFilter = "adoxio_name eq '" + name + "'";

            IEnumerable<MicrosoftDynamicsCRMadoxioLicencetype> licenceTypes = _dynamicsClient.Licencetypes.Get(filter: typeFilter).Value;

            result = licenceTypes.FirstOrDefault();

            return result;
        }

        public static MicrosoftDynamicsCRMadoxioApplicationtype GetApplicationTypeByName(this IDynamicsClient _dynamicsClient, string name)
        {
            MicrosoftDynamicsCRMadoxioApplicationtype result = null;
            string typeFilter = "adoxio_name eq '" + name + "'";

            IEnumerable<MicrosoftDynamicsCRMadoxioApplicationtype> applicationTypes = _dynamicsClient.Applicationtypes.Get(filter: typeFilter).Value;

            result = applicationTypes.FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Convert a service card ID string into a format that is useful (and fits into Dynamics)
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static string GetServiceCardID (string raw)
        {
            string result = "";
            if (! string.IsNullOrEmpty(raw))
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
            result  = GuidUtility.SanitizeGuidString(result);
            return result;
        }

        /// <summary>
        /// Load User from database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<User> LoadUser(this IDynamicsClient _dynamicsClient, string userId, IHeaderDictionary Headers, ILogger _logger, string guid = null)
        {
            User user = null;
            MicrosoftDynamicsCRMcontact contact = null;
            Guid userGuid;

            if (!string.IsNullOrEmpty(guid))
            {
                user = await _dynamicsClient.GetUserByGuid(guid);
            }

            if (user == null)
            {
                _logger.LogInformation(">>>> LoadUser for BCEID.");
                if (Guid.TryParse(userId, out userGuid))
                {
                    user = _dynamicsClient.GetUserBySmUserId(userId);
                    if (user != null)
                    {
                        _logger.LogInformation(">>>> LoadUser for BCEID: user != null");
                        // Update the contact with info from Siteminder
                        var contactVM = new Public.ViewModels.Contact();
                        contactVM.CopyHeaderValues(Headers);
                        _logger.LogInformation(">>>> After reading headers: " + Newtonsoft.Json.JsonConvert.SerializeObject(contactVM));
                        MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
                        patchContact.CopyValues(contactVM);
                        try
                        {
                            _dynamicsClient.Contacts.Update(user.ContactId.ToString(), patchContact);
                        }
                        catch (OdataerrorException odee)
                        {
                            _logger.LogError("Error updating Contact");
                            _logger.LogError("Request:");
                            _logger.LogError(odee.Request.Content);
                            _logger.LogError("Response:");
                            _logger.LogError(odee.Response.Content);
                            // fail if we can't create.
                            throw (odee);
                        }
                    }
                }
                else
                { //BC service card login

                    _logger.LogInformation(">>>> LoadUser for BC Services Card.");
                    string externalId = GetServiceCardID(userId);
                    contact = _dynamicsClient.GetContactByExternalId(externalId);

                    if (contact != null)
                    {
                        _logger.LogInformation(">>>> LoadUser for BC Services Card: contact != null");
                        user = new User();
                        user.FromContact(contact);

                        // Update the contact and worker with info from Siteminder
                        var contactVM = new Public.ViewModels.Contact();
                        var workerVm = new Public.ViewModels.Worker();
                        contactVM.CopyHeaderValues(Headers);
                        workerVm.CopyHeaderValues(Headers);
                        MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
                        
                        patchContact.CopyValuesNoEmailPhone(contactVM);                        
                        try
                        {                            
                            _dynamicsClient.Contacts.Update(user.ContactId.ToString(), patchContact);
                        }
                        catch (OdataerrorException odee)
                        {
                            _logger.LogError("Error updating Contact");
                            _logger.LogError("Request:");
                            _logger.LogError(odee.Request.Content);
                            _logger.LogError("Response:");
                            _logger.LogError(odee.Response.Content);
                            // fail if we can't update.
                            throw (odee);
                        }

                        // update worker(s)
                        try
                        {
                            string filter = $"_adoxio_contactid_value eq {contact.Contactid}";
                            var workers = _dynamicsClient.Workers.Get(filter: filter).Value;
                            foreach (var item in workers)
                            {
                                MicrosoftDynamicsCRMadoxioWorker patchWorker = new MicrosoftDynamicsCRMadoxioWorker();
                                patchWorker.CopyValuesNoEmailPhone(workerVm);
                                _dynamicsClient.Workers.Update(item.AdoxioWorkerid, patchWorker);
                            }
                        }
                        catch (OdataerrorException odee)
                        {
                            _logger.LogError("Error updating Worker");
                            _logger.LogError("Request:");
                            _logger.LogError(odee.Request.Content);
                            _logger.LogError("Response:");
                            _logger.LogError(odee.Response.Content);
                        }

                    }
                }
            }

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
        public static async Task<User> GetUserByGuid(this IDynamicsClient _dynamicsClient, string guid)
        {
            Guid id = new Guid(guid);
            User user = null;
            var contact = await _dynamicsClient.GetContactById(id);
            if (contact != null)
            {
                user = new User();
                user.FromContact(contact);
            }

            return user;
        }



        /// <summary>
        /// Returns a User based on the guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static User GetUserBySmUserId(this IDynamicsClient _dynamicsClient, string guid)
        {
            Guid id = new Guid(guid);
            User user = null;
            var contact = _dynamicsClient.GetContactByExternalId(id.ToString());
            if (contact != null)
            {
                user = new User();
                user.FromContact(contact);
            }

            return user;
        }

        /// <summary>
        /// Get an Invoice by the Id
        /// </summary>
        /// <param name="system">Re</param>
        /// <param name="id"></param>
        /// <returns>The Invoice, or null if it does not exist</returns>
        public static List<MicrosoftDynamicsCRMadoxioPreviousaddress> GetPreviousAddressByContactId(this IDynamicsClient system, string guid)
        {
            List<MicrosoftDynamicsCRMadoxioPreviousaddress> result;
            try
            {
                // fetch from Dynamics.
                var filter = "_adoxio_contactid_value eq " + guid;
                result = system.Previousaddresses.Get(filter: filter).Value.ToList();
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Get an Invoice by the Id
        /// </summary>
        /// <param name="system">Re</param>
        /// <param name="id"></param>
        /// <returns>The Invoice, or null if it does not exist</returns>
        public static async Task<MicrosoftDynamicsCRMadoxioPreviousaddress> GetPreviousAddressById(this IDynamicsClient system, string guid)
        {
            MicrosoftDynamicsCRMadoxioPreviousaddress result;
            try
            {
                // fetch from Dynamics.
                result = await system.Previousaddresses.GetByKeyAsync(guid);
            }
            catch (Gov.Lclb.Cllb.Interfaces.Models.OdataerrorException)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Verify whether currently logged in user has access to this account id
        /// </summary>
        /// <returns>boolean</returns>
        public static bool CurrentUserHasAccessToAccount(Guid accountId, IHttpContextAccessor _httpContextAccessor, IDynamicsClient _dynamicsClient)
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
            {
                return userSettings.AccountId == accountId.ToString() || IsChildAccount(userSettings.AccountId, accountId.ToString(), _dynamicsClient);
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
        }

        private static bool IsChildAccount(String parentAccountId, String childAccountId, IDynamicsClient _dynamicsClient)
        {
            var filter = $"_adoxio_account_value eq {parentAccountId}";
            var result = false;

            var legalEntities = _dynamicsClient.Legalentities.Get(filter: filter).Value.ToList();
            if (legalEntities.Any(e => e._adoxioShareholderaccountidValue == childAccountId))
            {
                result = true;
            }
            else
            {
                legalEntities = legalEntities.Where(e => !String.IsNullOrEmpty(e._adoxioShareholderaccountidValue)).ToList();
                for (var i = 0; i < legalEntities.Count; i++)
                {
                    if (IsChildAccount(legalEntities[i]._adoxioShareholderaccountidValue, childAccountId, _dynamicsClient))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

    }
}
