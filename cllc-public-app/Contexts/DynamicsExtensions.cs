using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Mapping;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Gov.Lclb.Cllb.Interfaces
{
    public static class DynamicsExtensions
    {

        public static List<Public.ViewModels.ApplicationLicenseSummary> GetLicensesByLicencee(this IDynamicsClient _dynamicsClient, string licenceeId, IMemoryCache _cache)
        {

            var licences = _dynamicsClient.GetAllLicensesByLicencee(_cache, licenceeId).ToList();

            List<Public.ViewModels.ApplicationLicenseSummary> licenseSummaryList = new List<Public.ViewModels.ApplicationLicenseSummary>();

            if (licences != null)
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licenceeId);
                foreach (var licence in licences)
                {
                    var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();
                    licenseSummaryList.Add(licence.ToLicenseSummaryViewModel(applications, _dynamicsClient));
                }
            }

            return licenseSummaryList;
        }




        public static List<Public.ViewModels.ApplicationLicenseSummary> GetPaidLicensesOnTransfer(this IDynamicsClient _dynamicsClient, string licenceeId)
        {
            var applicationFilter = $"_adoxio_applicant_value eq {licenceeId} and adoxio_paymentrecieved eq true ";
            applicationFilter += " and _adoxio_assignedlicence_value ne null"; // avoid problems caused by applications that lack an assigned licence
            applicationFilter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
            applicationFilter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
            applicationFilter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Approved}";
            applicationFilter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused}";
            applicationFilter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

            var applicationType = _dynamicsClient.GetApplicationTypeByName("Liquor Licence Transfer");
            if (applicationType != null)
            {
                applicationFilter += $" and _adoxio_applicationtypeid_value eq {applicationType.AdoxioApplicationtypeid} ";
            }

            var licenceExpand = new List<string> {
                "adoxio_adoxio_licences_adoxio_application_AssignedLicence",
                "adoxio_LicenceType",
                "adoxio_establishment",
                "adoxio_ThirdPartyOperatorId"
            };

            List<MicrosoftDynamicsCRMadoxioLicences> licences = _dynamicsClient.Applications.Get(filter: applicationFilter).Value
                .Select(app => _dynamicsClient.Licenceses.GetByKey(app._adoxioAssignedlicenceValue, expand: licenceExpand))
                .ToList();

            List<Public.ViewModels.ApplicationLicenseSummary> licenseSummaryList = new List<Public.ViewModels.ApplicationLicenseSummary>();

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applicationsInProgress = _dynamicsClient.GetApplicationsForLicenceByApplicant(licenceeId);
            if (licences != null && applicationsInProgress != null)
            {
                foreach (var licence in licences)
                {
                    var applications = applicationsInProgress.Where(app => app._adoxioAssignedlicenceValue == licence.AdoxioLicencesid).ToList();
                    var result = licence.ToLicenseSummaryViewModel(applications, _dynamicsClient);
                    result.LicenceTypeName = "Transfer in Progress - " + result.LicenceTypeName;
                    licenseSummaryList.Add(result);
                }
            }

            return licenseSummaryList;
        }

        public static List<Public.ViewModels.LicenseeChangeLog> GetApplicationChangeLogs(this IDynamicsClient _dynamicsClient, string applicationId, ILogger _logger)
        {

            var result = new List<Public.ViewModels.LicenseeChangeLog>();
            var filter = "_adoxio_application_value eq " + applicationId;
            try
            {
                var response = _dynamicsClient.Licenseechangelogs.Get(filter: filter).Value.ToList();
                foreach (var item in response)
                {
                    result.Add(item.ToViewModel());
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error reading LegalEntityChangelog");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while reading LegalEntityChangelog");
            }
            return result;
        }

        public static Public.ViewModels.LegalEntity GetLegalEntityTree(this IDynamicsClient _dynamicsClient, string accountId, ILogger _logger, IConfiguration _configuration)
        {
            Public.ViewModels.LegalEntity result = null;
            var filter = "_adoxio_account_value eq " + accountId;
            filter += " and _adoxio_legalentityowned_value eq null";

            var expand = new List<string>{
                "adoxio_Contact"
            };

            var response = _dynamicsClient.Legalentities.Get(filter: filter, expand: expand);

            if (response != null && response.Value != null)
            {
                var legalEntity = response.Value.FirstOrDefault();
                if (legalEntity != null)
                {
                    result = legalEntity.ToViewModel();
                    if (!string.IsNullOrEmpty(result.contactId))
                    {
                        result.PhsLink = GetPhsLink(result.contactId, _configuration);
                        result.CasLink = GetCASLink(result.contactId, _configuration);
                    }
                    result.children = _dynamicsClient.GetLegalEntityChildren(result.id, _logger, _configuration);
                }
            }
            return result;
        }


        public static List<MicrosoftDynamicsCRMcontact> GetLEConnectionsForAccount(this IDynamicsClient _dynamicsClient,
            string accountId, ILogger _logger, IConfiguration _configuration, List<string> memo = null)
        {
            if (memo == null)
            {
                memo = new List<string>();
            }
            var result = new List<MicrosoftDynamicsCRMcontact>();
            if (memo.Contains(accountId))
            {
                return result; // accountId already processed
            }
            memo.Add(accountId);

            // select ACTIVE le-connections of the parent account
            var filter = $"statecode eq 0 and _adoxio_parentaccount_value eq {accountId}";

            var expand = new List<string>{
                "adoxio_ChildProfileName_contact",
                "adoxio_ChildProfileName_account"
            };
            MicrosoftDynamicsCRMadoxioLeconnectionCollection response = null;
            try
            {
                response = _dynamicsClient.Leconnections.Get(filter: filter, expand: expand);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error Getting LEConnections");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception getting LEConnections");
                throw;
            }

            var leConnections = response?.Value?.ToList();
            leConnections.ForEach(item =>
            {
                if (item.AdoxioIsindividual == true
                    && item.AdoxioSecurityscreeningrequired == true
                    && item.AdoxioChildProfileNameContact != null) // Individual Connection that requires screening
                {
                    item.AdoxioChildProfileNameContact.PhsLink = GetPhsLink(item.AdoxioChildProfileNameContact.Contactid, _configuration);
                    item.AdoxioChildProfileNameContact.CasLink = GetCASLink(item.AdoxioChildProfileNameContact.Contactid, _configuration);
                    result.Add(item.AdoxioChildProfileNameContact);
                }
                else if (item.AdoxioIsindividual != true && item.AdoxioChildProfileNameAccount != null)
                { //Business connection
                    result.AddRange(GetLEConnectionsForAccount(_dynamicsClient, item.AdoxioChildProfileNameAccount.Accountid, _logger, _configuration, memo));
                }
            });

            return result;
        }

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


        public static List<Public.ViewModels.LegalEntity> GetLegalEntityChildren(this IDynamicsClient _dynamicsClient, string parentLegalEntityId, ILogger _logger, IConfiguration _configuration, List<string> processedEntities = null)
        {
            List<Public.ViewModels.LegalEntity> result = new List<Public.ViewModels.LegalEntity>();
            MicrosoftDynamicsCRMadoxioLegalentityCollection response = null;
            var filter = "_adoxio_legalentityowned_value eq " + parentLegalEntityId;
            if (processedEntities == null)
            {
                processedEntities = new List<string>();
            }
            try
            {
                response = _dynamicsClient.Legalentities.Get(filter: filter);
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Error while patching legal entity");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unexpected Exception while patching legal entity");
            }

            if (response != null && response.Value != null)
            {
                var legalEntities = response.Value.ToList();

                foreach (var legalEntity in legalEntities)
                {
                    var viewModel = legalEntity.ToViewModel();
                    if (!String.IsNullOrEmpty(legalEntity.AdoxioLegalentityid) && !processedEntities.Contains(legalEntity.AdoxioLegalentityid))
                    {
                        processedEntities.Add(legalEntity.AdoxioLegalentityid);
                        viewModel.children = _dynamicsClient.GetLegalEntityChildren(legalEntity.AdoxioLegalentityid, _logger, _configuration, processedEntities);
                    }
                    if (!string.IsNullOrEmpty(viewModel.contactId))
                    {
                        viewModel.PhsLink = GetPhsLink(viewModel.contactId, _configuration);
                        viewModel.CasLink = GetCASLink(viewModel.contactId, _configuration);
                    }

                    result.Add(viewModel);
                }

            }
            return result;

        }

        public static int GetNotTerminatedCRSApplicationCount(this IDynamicsClient _dynamicsClient, string accountId)
        {
            int result = 0;
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = _dynamicsClient.GetApplicationListByApplicant(accountId);
            if (dynamicsApplicationList != null)
            {
                foreach (MicrosoftDynamicsCRMadoxioApplication dynamicsApplication in dynamicsApplicationList)
                {
                    if (!string.IsNullOrEmpty(dynamicsApplication._adoxioLicencetypeValue))
                    {
                        Guid adoxio_licencetypeId = Guid.Parse(dynamicsApplication._adoxioLicencetypeValue);
                        var adoxio_licencetype = _dynamicsClient.GetAdoxioLicencetypeById(adoxio_licencetypeId);
                        string licenseType = adoxio_licencetype.AdoxioName;

                        // hide terminated applications from view.
                        if (dynamicsApplication.Statuscode == null || (dynamicsApplication.Statuscode != (int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated
                            && dynamicsApplication.Statuscode != (int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused
                            && dynamicsApplication.Statuscode != (int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled
                            && dynamicsApplication.Statuscode != (int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded)
                            && licenseType == "Cannabis Retail Store"
                            )
                        {
                            result++;
                        }
                    }
                }

            }

            return result;

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
                        result = value.Substring(pos+1);
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
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMaccount GetActiveAccountByLegalName(this IDynamicsClient system, string legalName)
        {
            // ensure that the legal name is safe for a query.
            legalName.Replace("'", "''");

            MicrosoftDynamicsCRMaccount result = null;
            try
            {
                var accountResponse = system.Accounts.Get(filter: $"statecode eq 0 and name eq '{legalName}'");
                if (accountResponse.Value != null && accountResponse.Value.Count == 1)
                {
                    var firstAccount = accountResponse.Value.FirstOrDefault();
                    if (string.IsNullOrEmpty(firstAccount.AdoxioExternalid))
                    {
                        result = firstAccount;
                    }
                }
            }
            catch (Exception)
            {

                result = null;
            }

            // get the primary contact.
            if (result != null && result.Primarycontactid == null && result._primarycontactidValue != null)
            {
                result.Primarycontactid = system.GetContactById(Guid.Parse(result._primarycontactidValue)).GetAwaiter().GetResult();
            }

            return result;

        }


        /// <summary>
        /// Get a Account by their Guid
        /// </summary>
        /// <param name="system"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<MicrosoftDynamicsCRMaccount> GetActiveAccountBySiteminderBusinessGuid(this IDynamicsClient system, string siteminderId)
        {
            // ensure that the siteminderId does not have any dashes.
            string sanitizedSiteminderId = GuidUtility.SanitizeGuidString(siteminderId);

            MicrosoftDynamicsCRMaccount result = null;
            try
            {
                var accountResponse = await system.Accounts.GetAsync(filter: "statecode eq 0 and adoxio_externalid eq '" + sanitizedSiteminderId + "'");
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
        /// Get an Invoice by the Id
        /// </summary>
        /// <param name="system">Re</param>
        /// <param name="id"></param>
        /// <returns>The Invoice, or null if it does not exist</returns>
        public static async Task<MicrosoftDynamicsCRMinvoice> GetInvoiceById(this IDynamicsClient system, Guid id)
        {
            return await system.GetInvoiceById(id.ToString());
        }

        /// <summary>
        /// Get an Invoice by the Id
        /// </summary>
        /// <param name="system">Re</param>
        /// <param name="id"></param>
        /// <returns>The Invoice, or null if it does not exist</returns>
        public static async Task<MicrosoftDynamicsCRMinvoice> GetInvoiceById(this IDynamicsClient system, string id)
        {
            MicrosoftDynamicsCRMinvoice result;
            try
            {
                // fetch from Dynamics.
                result = await system.Invoices.GetByKeyAsync(id);
            }
            catch (HttpOperationException)
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
        public static MicrosoftDynamicsCRMinvoice GetInvoiceByIdWithApplications(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMinvoice result;
            var expand = new List<string> { "adoxio_invoice_adoxio_application_Invoice", "adoxio_invoice_adoxio_application_LicenceFeeInvoice" };
            try
            {
                // fetch from Dynamics.
                result = system.Invoices.GetByKey(invoiceid: id.ToString(), expand: expand);
            }
            catch (HttpOperationException)
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
            catch (HttpOperationException)
            {
                result = null;
            }
            return result;
        }

        public static List<MicrosoftDynamicsCRMadoxioLicences> GetLicensesByLicencee(this IDynamicsClient _dynamicsClient, IMemoryCache _cache, string licenceeId)
        {
            var expand = new List<string> { "adoxio_adoxio_licences_adoxio_application_AssignedLicence", "adoxio_LicenceType", "adoxio_establishment", "adoxio_ThirdPartyOperatorId" };

            List<MicrosoftDynamicsCRMadoxioLicences> licences = null;

            var filter = $"_adoxio_licencee_value eq {licenceeId}";

            try
            {
                List<MicrosoftDynamicsCRMadoxioLicences> data = (List<MicrosoftDynamicsCRMadoxioLicences>)_dynamicsClient.Licenceses.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                licences = data
                    .Where(licence =>
                    {
                        return licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Cancelled
                            && licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Inactive;

                    })
                    .Select(licence =>
                    {
                        licence.AdoxioLicenceType = Gov.Lclb.Cllb.Public.Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
                        return licence;
                    })
                    .ToList();
            }
            catch (HttpOperationException)
            {
                licences = null;
            }
            return licences;
        }

        public static IEnumerable<MicrosoftDynamicsCRMadoxioLicences> GetTransferLicensesByLicencee(this IDynamicsClient _dynamicsClient, IMemoryCache _cache, string licenceeId)
        {
            var expand = new List<string> { "adoxio_adoxio_licences_adoxio_application_AssignedLicence", "adoxio_LicenceType", "adoxio_establishment", "adoxio_ThirdPartyOperatorId" };

            IEnumerable<MicrosoftDynamicsCRMadoxioLicences> licences = null;

            var filter = $"_adoxio_proposedowner_value eq {licenceeId} and adoxio_ownershiptransferinprogress eq 845280001";

            try
            {
                licences = _dynamicsClient.Licenceses.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                licences = licences
                    .Where(licence =>
                    {
                        return licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Cancelled
                            && licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Inactive;

                    })
                    .Select(licence =>
                    {
                        licence.AdoxioLicenceType = Gov.Lclb.Cllb.Public.Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
                        return licence;
                    });
            }
            catch (HttpOperationException)
            {
                licences = null;
            }
            return licences;
        }

        public static IEnumerable<MicrosoftDynamicsCRMadoxioLicences> GetAllLicensesByLicencee(this IDynamicsClient _dynamicsClient, IMemoryCache _cache, string licenceeId)
        {
            var expand = new List<string> { "adoxio_adoxio_licences_adoxio_application_AssignedLicence", "adoxio_LicenceType", "adoxio_establishment", "adoxio_ThirdPartyOperatorId" };

            IEnumerable<MicrosoftDynamicsCRMadoxioLicences> licences = null;

            var filter = $"_adoxio_licencee_value eq { licenceeId} or (_adoxio_proposedowner_value eq {licenceeId} and adoxio_ownershiptransferinprogress eq 845280001)";

            try
            {
                licences = _dynamicsClient.Licenceses.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                licences = licences
                    .Where(licence =>
                    {
                        return licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Cancelled
                            && licence.Statuscode != (int)Public.ViewModels.LicenceStatusCodes.Inactive;

                    })
                    .Select(licence =>
                    {
                        licence.AdoxioLicenceType = Gov.Lclb.Cllb.Public.Models.ApplicationExtensions.GetCachedLicenceType(licence._adoxioLicencetypeValue, _dynamicsClient, _cache);
                        return licence;
                    });
            }
            catch (HttpOperationException)
            {
                licences = null;
            }
            return licences;
        }

        public static async Task<MicrosoftDynamicsCRMadoxioTiedhouseconnection> GetTiedHouseConnectionById(this IDynamicsClient system, Guid id)
        {
            MicrosoftDynamicsCRMadoxioTiedhouseconnection result;
            try
            {
                // fetch from Dynamics.
                result = await system.Tiedhouseconnections.GetByKeyAsync(id.ToString());
            }
            catch (HttpOperationException)
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
            catch (HttpOperationException)
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
                    result.AdoxioApplicant = await system.GetAccountByIdAsync(Guid.Parse(result._adoxioApplicantValue));
                }
            }
            catch (HttpOperationException)
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
                    result.AdoxioApplicant = await system.GetAccountByIdAsync(Guid.Parse(result._adoxioApplicantValue));
                }

                if (result.AdoxioAssignedLicence != null && result.AdoxioAssignedLicence._adoxioEstablishmentValue != null)
                {
                    result.AdoxioAssignedLicence.AdoxioEstablishment = system.GetEstablishmentById(Guid.Parse(result.AdoxioAssignedLicence._adoxioEstablishmentValue));
                }
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            return result;
        }

        // Does not return application of type "Licensee Changes"
        public static IEnumerable<MicrosoftDynamicsCRMadoxioApplication> GetApplicationListByApplicant(this IDynamicsClient _dynamicsClient, string applicantId)
        {
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_AssignedLicence", "adoxio_LicenceType", "adoxio_ApplicationTypeId", "adoxio_ApplicationExtension" };
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty(applicantId))
            {
                dynamicsApplicationList = _dynamicsClient.Applications.Get(expand: expand).Value;
            }
            else
            {
                var filter = $"_adoxio_applicant_value eq {applicantId} and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
                // Approved applications need to be passed to the client app
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
                filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

                var applicationType = _dynamicsClient.GetApplicationTypeByName("Licensee Changes");
                if (applicationType != null)
                {
                    filter += $" and _adoxio_applicationtypeid_value ne {applicationType.AdoxioApplicationtypeid} ";
                }

                try
                {
                    dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                }
                catch (HttpOperationException)
                {
                    dynamicsApplicationList = null;
                }
            }
            return dynamicsApplicationList;
        }

        public static IEnumerable<MicrosoftDynamicsCRMadoxioCannabisinventoryreport> GetInventoryReportsForMonthlyReport(this IDynamicsClient _dynamicsClient, string monthlyReportId)
        {
            IEnumerable<MicrosoftDynamicsCRMadoxioCannabisinventoryreport> inventoryReportsList = null;
            if (string.IsNullOrEmpty(monthlyReportId))
            {
                inventoryReportsList = _dynamicsClient.Cannabisinventoryreports.Get().Value;
            }
            else
            {
                var select = new List<string>() { "adoxio_cannabisinventoryreportid", "adoxio_openinginventory", "adoxio_qtyreceiveddomestic", "adoxio_qtyreceivedreturns", "adoxio_qtyreceivedother", "adoxio_qtyshippeddomestic", "adoxio_qtyshippedreturned", "adoxio_qtydestroyed", "adoxio_qtyloststolen", "adoxio_otherreductions", "adoxio_closinginventory", "adoxio_valueofclosinginventory", "adoxio_packagedunitsnumber", "adoxio_totalvalue", "adoxio_packagedunitsnumberretailer", "adoxio_totalvalueretailer", "adoxio_otherdescription", "adoxio_weightofclosinginventory", "adoxio_totalnumberseeds" };
                var expand = new List<string>() { "adoxio_ProductId($select=adoxio_cannabisproductadminid, adoxio_name, adoxio_description, adoxio_displayorder)" };
                //adoxioProductId
                var filter = $"_adoxio_monthlyreportid_value eq {monthlyReportId}";

                try
                {
                    inventoryReportsList = _dynamicsClient.Cannabisinventoryreports.Get(filter: filter, select: select, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
                }
                catch (HttpOperationException)
                {
                    inventoryReportsList = null;
                }
            }
            return inventoryReportsList;
        }

        public static IEnumerable<MicrosoftDynamicsCRMadoxioApplication> GetApplicationsForLicenceByApplicant(this IDynamicsClient _dynamicsClient, string applicantId)
        {
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_AssignedLicence", "adoxio_LicenceType", "adoxio_ApplicationTypeId" };
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty(applicantId))
            {
                throw new Exception("ERROR getting ApplicationsForLicenceByApplicant - Applicant cannot be null");
            }

            var filter = $"_adoxio_applicant_value eq {applicantId} and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

            try
            {
                dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
            }
            catch (HttpOperationException e)
            {
                dynamicsApplicationList = null;
            }
            return dynamicsApplicationList;
        }

        /// <summary>
        /// Gets all third party operator licences for the given applicantId
        /// </summary>
        /// <param name="_dynamicsClient"></param>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        public static IEnumerable<MicrosoftDynamicsCRMadoxioApplication> GetThirdPartyOperaotsLicences(this IDynamicsClient _dynamicsClient, string applicantId)
        {
            var expand = new List<string> { "adoxio_LicenceFeeInvoice", "adoxio_AssignedLicence", "adoxio_LicenceType", "adoxio_ApplicationTypeId" };
            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty(applicantId))
            {
                throw new Exception("ERROR getting ApplicationsForLicenceByApplicant - Applicant cannot be null");
            }

            var filter = $"_adoxio_applicant_value eq {applicantId} and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Terminated}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Refused}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.Cancelled}";
            filter += $" and statuscode ne {(int)Public.ViewModels.AdoxioApplicationStatusCodes.TerminatedAndRefunded}";

            try
            {
                dynamicsApplicationList = _dynamicsClient.Applications.Get(filter: filter, expand: expand, orderby: new List<string> { "modifiedon desc" }).Value;
            }
            catch (HttpOperationException)
            {
                dynamicsApplicationList = null;
            }
            return dynamicsApplicationList;
        }


        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static IList<MicrosoftDynamicsCRMcontact> GetActiveContactsByDetails(this IDynamicsClient system, string firstname, string middlename, string lastname, string emailaddress1)
        {
            // do not check Dynamics if we have no data to search for.
            if (firstname == null && lastname == null && emailaddress1 == null)
            {
                return null;
            }
            IList<MicrosoftDynamicsCRMcontact> result = null;
            string filter = "statecode eq 0";
            if (!string.IsNullOrEmpty(firstname))
            {
                firstname.Replace("'", "''");
                filter += $" and firstname eq '{firstname}'";
            }
            if (!string.IsNullOrEmpty(middlename))
            {

                middlename.Replace("'", "''");
                filter += $" and middlename eq '{middlename}'";
            }
            if (!string.IsNullOrEmpty(lastname))
            {
                lastname.Replace("'", "''");
                filter += $" and lastname eq '{lastname}'";

            }
            if (!string.IsNullOrEmpty(emailaddress1))
            {
                emailaddress1.Replace("'", "''");
                filter += $" and emailaddress1 eq '{emailaddress1}'";
            }

            if (!string.IsNullOrEmpty(filter))
            {
                try
                {
                    var contactsResponse = system.Contacts.Get(filter: filter);
                    result = contactsResponse.Value;
                }
                catch (HttpOperationException)
                {
                    result = null;
                }
                catch (Exception)
                {
                    result = null;
                }
            }


            return result;
        }

        public static int? GetLoginTypePicklistValue(this IDynamicsClient system, bool isServicesCard)
        {
            int? adoxioType;
            if (isServicesCard)
            {
                adoxioType = 845280000;
            }
            else
            {
                adoxioType = 845280001;
            }

            return adoxioType;
        }

        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMcontact GetActiveContactByExternalIdBridged(this IDynamicsClient dynamicsClient, bool isServicesCard, string siteminderId)
        {
            int? adoxioType = dynamicsClient.GetLoginTypePicklistValue(isServicesCard);

            string sanitizedSiteminderId = GuidUtility.SanitizeGuidString(siteminderId);
            MicrosoftDynamicsCRMcontact result = null;
            try
            {
                var bridgeQueryResults = dynamicsClient.Logins
                    .Get(filter: $"adoxio_type eq {adoxioType} and adoxio_externalid eq '{sanitizedSiteminderId}'").Value;
                foreach (var item in bridgeQueryResults)
                {
                    var temp = dynamicsClient.GetContactById(item._adoxioContactValue).GetAwaiter().GetResult();
                    if (temp.Statecode == 0) // only allow login from active accounts.
                    {
                        result = temp;
                        break;
                    }
                }
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Get a contact by their Siteminder ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMcontact GetActiveContactByExternalId(this IDynamicsClient system, string siteminderId)
        {
            string sanitizedSiteminderId = GuidUtility.SanitizeGuidString(siteminderId);
            MicrosoftDynamicsCRMcontact result = null;
            try
            {
                // 9-23-2020 - LCSD-3972 changed to only get active contacts.
                var contactsResponse = system.Contacts.Get(filter: "statecode eq 0 and adoxio_externalid eq '" + sanitizedSiteminderId + "'");
                result = contactsResponse.Value.FirstOrDefault();
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Get a list of active contacts by their account ID
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static List<MicrosoftDynamicsCRMcontact> GetActiveContactsByAccountId(this IDynamicsClient system, string accountId)
        {
            List<MicrosoftDynamicsCRMcontact> result = null;
            try
            {
                var contactsResponse = system.Contacts.Get(filter: $"statecode eq 0 and _accountid_value eq {accountId}");
                result = contactsResponse.Value.ToList();
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        /// <summary>
        /// Get a contact by their First Initial, Last Name and Birthdate
        /// </summary>
        /// <param name="system"></param>
        /// <param name="siteminderId"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMcontact GetContactByNameAndBirthdate(this IDynamicsClient system, string firstName, string lastName, string birthdate)
        {
            MicrosoftDynamicsCRMcontact result = null;
            try
            {
                string[] dateParts = birthdate.Split('-');
                var contactsResponse = system.Contacts.Get(filter: "lastname eq '" + lastName + "'");
                result = contactsResponse.Value
                    .Where(contact => contact.Firstname.Substring(0, 1).ToLower() == firstName.Substring(0, 1).ToLower()
                            && contact.Birthdate.Value.Year.ToString() == dateParts[0]
                            && contact.Birthdate.Value.Month.ToString() == dateParts[1]
                            && contact.Birthdate.Value.Day.ToString() == dateParts[2]).FirstOrDefault();
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static MicrosoftDynamicsCRMadoxioLegalentity GetAdoxioLegalentityByAccountId(this IDynamicsClient _dynamicsClient, Guid id)
        {
            MicrosoftDynamicsCRMadoxioLegalentity result = null;
            string accountFilter = "_adoxio_account_value eq " + id;
            IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = _dynamicsClient.Legalentities.Get(filter: accountFilter).Value;
            result = legalEntities.FirstOrDefault();
            return result;
        }


        public static MicrosoftDynamicsCRMadoxioLicencetype GetCachedLicenceTypeByName(this IDynamicsClient dynamicsClient, string name, IMemoryCache memoryCache)
        {
            string cacheKey = CacheKeys.LicenceTypePrefix + "_" + name;
            if (memoryCache == null || !memoryCache.TryGetValue(cacheKey, out MicrosoftDynamicsCRMadoxioLicencetype result))
            {
                // Key not in cache, so get data.
                result = dynamicsClient.GetAdoxioLicencetypeByName(name);

                if (memoryCache != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(365));
                    // Save data in cache.
                    memoryCache.Set(cacheKey, result, cacheEntryOptions);
                }
            }

            return result;
        }

        public static string GetCachedLicenceTypeIdByName(this IDynamicsClient dynamicsClient, string name, IMemoryCache memoryCache)
        {
            string result = null;
            if (!string.IsNullOrEmpty(name))
            {
                string cacheKey = CacheKeys.LicenceTypeIDByNamePrefix + name;
                if (memoryCache == null || !memoryCache.TryGetValue(cacheKey, out result))
                {
                    // Key not in cache, so get data.
                    var lt = dynamicsClient.GetAdoxioLicencetypeByName(name);
                    if (lt != null)
                    {
                        result = lt.AdoxioLicencetypeid;
                        if (memoryCache != null)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromDays(365));
                            // Save data in cache.
                            memoryCache.Set(cacheKey, result, cacheEntryOptions);
                        }
                    }

                }

            }

            return result;
        }



        public static MicrosoftDynamicsCRMadoxioLicencesubcategory GetAdoxioSubLicencetypeByName(this IDynamicsClient _dynamicsClient, string name)
        {
            MicrosoftDynamicsCRMadoxioLicencesubcategory result = null;
            string typeFilter = "adoxio_name eq '" + name + "'";

            IEnumerable<MicrosoftDynamicsCRMadoxioLicencesubcategory> licenceTypes = _dynamicsClient.Licencesubcategories.Get(filter: typeFilter).Value;

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

        public async static Task<MicrosoftDynamicsCRMadoxioApplicationtype> GetApplicationTypeById(this IDynamicsClient _dynamicsClient, string id)
        {
            MicrosoftDynamicsCRMadoxioApplicationtype result;
            try
            {
                // fetch from Dynamics.
                result = await _dynamicsClient.Applicationtypes.GetByKeyAsync(id);
            }
            catch (HttpOperationException)
            {
                result = null;
            }
            return result;
        }

        public static Public.ViewModels.Form GetSystemformViewModel(this IDynamicsClient _dynamicsClient, IMemoryCache _cache, ILogger _logger, string formid)
        {

            Public.ViewModels.Form form = null;

            // get the picklists.

            List<MicrosoftDynamicsCRMpicklistAttributeMetadata> picklistMetadata = null;

            try
            {
                picklistMetadata = _dynamicsClient.GetCachedApplicationPicklists(_cache);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ERROR getting accounts picklist metadata");

            }

            // get the application mapping.

            ApplicationMapping applicationMapping = new ApplicationMapping();

            try
            {
                var systemForm = _dynamicsClient.Systemforms.GetByKey(formid);

                /*
                string entityKey = "SystemForm_" + id + "_Entity";
                string nameKey = "SystemForm_" + id + "_Name";
                string xmlKey = "SystemForm_" + id + "_FormXML";
                string formXml = await _distributedCache.GetStringAsync(xmlKey);
                */

                string formXml = systemForm.Formxml;

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
                        Boolean tabShowLabel = tab.Attribute("showlabel").DynamicsAttributeToBoolean();
                        Boolean tabVisible = tab.Attribute("visible").DynamicsAttributeToBoolean();

                        Public.ViewModels.FormTab formTab = new Public.ViewModels.FormTab();
                        formTab.id = tabId;
                        formTab.name = description;
                        formTab.sections = new List<Public.ViewModels.FormSection>();
                        formTab.showlabel = tabShowLabel;
                        formTab.visible = tabVisible;

                        // get the sections
                        var sections = tab.XPathSelectElements("columns/column/sections/section");
                        foreach (var section in sections)
                        {
                            Boolean sectionShowLabel = section.Attribute("showlabel").DynamicsAttributeToBoolean();
                            Boolean sectionVisible = section.Attribute("visible").DynamicsAttributeToBoolean();
                            if (section.Attribute("visible") == null)
                            {
                                sectionVisible = true; // default visibility to true if it is not specified.
                            }


                            Public.ViewModels.FormSection formSection = new Public.ViewModels.FormSection();
                            formSection.fields = new List<Public.ViewModels.FormField>();
                            formSection.id = section.Attribute("id").Value;
                             formSection.name = section.Attribute("name").Value;
                            formSection.showlabel = sectionShowLabel;
                            formSection.visible = sectionVisible;

                            // get the fields.
                            var sectionLabels = section.XPathSelectElements("labels/label");

                            // the section label is the section name.
                            foreach (var sectionLabel in sectionLabels)
                            {
                                formSection.label = sectionLabel.Attribute("description").Value;
                            }
                            // get the cells.
                            var cells = section.XPathSelectElements("rows/row/cell");

                            foreach (var cell in cells)
                            {
                                Public.ViewModels.FormField formField = new Public.ViewModels.FormField();
                                // get cell visibility and showlabel
                                bool cellShowLabel = cell.Attribute("showlabel").DynamicsAttributeToBoolean();
                                bool cellVisible = cell.Attribute("visible").DynamicsAttributeToBoolean();

                                // set the cell to visible if it is not hidden.
                                if (cell.Attribute("visible") == null)
                                {
                                    cellVisible = true;
                                }

                                formField.showlabel = cellShowLabel;
                                formField.visible = cellVisible;
                                formField.name = cell.Attribute("name")?.Value ?? "";


                                // get the cell label.

                                if (formField.showlabel)
                                {
                                    var cellLabels = cell.XPathSelectElements("labels/label");
                                    foreach (var cellLabel in cellLabels)
                                    {
                                        formField.label = cellLabel.Attribute("description").Value;
                                    }
                                }
                                else
                                {
                                    // use the section name.
                                    formField.label = formSection.label;
                                    formSection.label = "";
                                }


                                // get the form field name.
                                var control = cell.XPathSelectElement("control");
                                if (!string.IsNullOrEmpty(formField.label) && control != null && control.Attribute("datafieldname") != null)
                                {
                                    formField.classid = control.Attribute("classid").Value;
                                    formField.controltype = formField.classid.DynamicsControlClassidToName();
                                    string datafieldname = control.Attribute("datafieldname").Value;
                                    // translate the data field name
                                    formField.datafieldname = applicationMapping.GetViewModelKey(datafieldname);

                                    formField.required = applicationMapping.GetRequired(datafieldname);

                                    if (formField.controltype.Equals("PicklistControl"))
                                    {
                                        // get the options.
                                        var metadata = picklistMetadata.FirstOrDefault(x => x.LogicalName == datafieldname);

                                        formField.options = new List<Public.ViewModels.OptionMetadata>();

                                        if (metadata == null)
                                        {
                                            formField.options.Add(new Public.ViewModels.OptionMetadata { label = "INVALID PICKLIST", value = 0 });
                                        }
                                        else
                                        {
                                            MicrosoftDynamicsCRMoptionSet optionSet = null;
                                            // could be an OptionSet or a globalOptionSet.
                                            if (metadata.OptionSet != null)
                                            {
                                                optionSet = metadata.OptionSet;
                                            }
                                            else
                                            {
                                                optionSet = metadata.GlobalOptionSet;
                                            }
                                            if (optionSet != null)
                                            {
                                                foreach (var option in optionSet.Options)
                                                {
                                                    int? value = option.Value;
                                                    string label = option.Label?.UserLocalizedLabel?.Label;
                                                    if (value == null || label == null)
                                                    {
                                                        formField.options.Add(new Public.ViewModels.OptionMetadata { label = "INVALID PICKLIST", value = 0 });
                                                    }
                                                    else
                                                    {
                                                        formField.options.Add(new Public.ViewModels.OptionMetadata { label = label, value = value.Value });
                                                    }

                                                }
                                            }

                                        }
                                    }
                                    if (formField.datafieldname != null)
                                    {
                                        formSection.fields.Add(formField);
                                    }
                                }

                            }

                            formTab.sections.Add(formSection);
                            form.sections.Add(formSection);
                        }

                        form.tabs.Add(formTab);
                    }
                }
                else // single tab form.
                {
                    Public.ViewModels.FormTab formTab = new Public.ViewModels.FormTab();
                    formTab.name = "";
                    form.tabs.Add(formTab);
                }
            }
            catch (HttpOperationException httpOperationException)
            {
                _logger.LogError(httpOperationException, "Unknown or invalid form reference - {formid}");
            }

            return form;
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
        /// Load User from database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="smGuid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<User> LoadUserLegacy(this IDynamicsClient _dynamicsClient, string smGuid, IHeaderDictionary Headers, ILogger _logger)
        {
            User user = null;
            MicrosoftDynamicsCRMcontact contact = null;
            Guid userGuid;

            _logger.LogDebug(">>>> LoadUserLegacy for BCEID.");
            if (Guid.TryParse(smGuid, out userGuid))
            {
                user = _dynamicsClient.GetActiveUserBySmGuid(smGuid);
                if (user == null)
                {
                    // try by other means.
                    var contactVM = new Public.ViewModels.Contact();
                    contactVM.CopyHeaderValues(Headers);
                    user = _dynamicsClient.GetUserByContactVmBlankSmGuid(contactVM);
                }
                if (user != null)
                {
                    _logger.LogDebug(">>>> LoadUserLegacy for BCEID: user != null");
                    // Update the contact with info from Siteminder
                    var contactVM = new Public.ViewModels.Contact();
                    contactVM.CopyHeaderValues(Headers);
                    _logger.LogDebug(">>>> After reading headers: " + JsonConvert.SerializeObject(contactVM));
                    MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
                    patchContact.CopyValues(contactVM);
                    try
                    {
                        _dynamicsClient.Contacts.Update(user.ContactId.ToString(), patchContact);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't create.
                        throw (httpOperationException);
                    }

                    // The account will be patched when we fetch data from bceid.


                }
            }
            else
            { //BC service card login

                _logger.LogDebug(">>>> LoadUserLegacy for BC Services Card.");
                string externalId = GetServiceCardID(smGuid);
                contact = _dynamicsClient.GetActiveContactByExternalId(externalId);

                if (contact == null)
                {
                    string firstInitial = Headers["smgov_givenname"];
                    string lastName = Headers["smgov_surname"];
                    string birthDate = Headers["smgov_birthdate"];
                    contact = _dynamicsClient.GetContactByNameAndBirthdate(firstInitial, lastName, birthDate);
                }

                if (contact != null)
                {
                    _logger.LogDebug(">>>> LoadUserLegacy for BC Services Card: contact != null");
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't update.
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Worker");
                    }

                }
            }

            return user;

        }

        /// <summary>
        /// Load User from database using their userId and guid
        /// </summary>
        /// <param name="context"></param>
        /// <param name="smGuid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static async Task<User> LoadUser(this IDynamicsClient _dynamicsClient, string smGuid, IHeaderDictionary Headers, ILogger _logger)
        {
            User user = null;
            MicrosoftDynamicsCRMcontact contact = null;
            Guid userGuid;

            _logger.LogDebug(">>>> LoadUser for BCEID.");
            if (Guid.TryParse(smGuid, out userGuid))
            {
                user = _dynamicsClient.GetActiveUserBySmGuid(smGuid);
                if (user == null)
                {
                    // try by other means.
                    var contactVM = new Public.ViewModels.Contact();
                    contactVM.CopyHeaderValues(Headers);
                    user = _dynamicsClient.GetUserByContactVmBlankSmGuid(contactVM);
                }
                if (user != null)
                {
                    _logger.LogDebug(">>>> LoadUser for BCEID: user != null");
                    // Update the contact with info from Siteminder
                    var contactVM = new Public.ViewModels.Contact();
                    contactVM.CopyHeaderValues(Headers);
                    _logger.LogDebug(">>>> After reading headers: " + JsonConvert.SerializeObject(contactVM));
                    MicrosoftDynamicsCRMcontact patchContact = new MicrosoftDynamicsCRMcontact();
                    patchContact.CopyValues(contactVM);
                    try
                    {
                        _dynamicsClient.Contacts.Update(user.ContactId.ToString(), patchContact);
                    }
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't create.
                        throw (httpOperationException);
                    }

                    // The account will be patched when we fetch data from bceid.


                }
            }
            else
            { //BC service card login

                _logger.LogDebug(">>>> LoadUser for BC Services Card.");
                string externalId = GetServiceCardID(smGuid);
                contact = _dynamicsClient.GetActiveContactByExternalId(externalId);

                if (contact == null)
                {
                    string firstInitial = Headers["smgov_givenname"];
                    string lastName = Headers["smgov_surname"];
                    string birthDate = Headers["smgov_birthdate"];
                    contact = _dynamicsClient.GetContactByNameAndBirthdate(firstInitial, lastName, birthDate);
                }

                if (contact != null)
                {
                    _logger.LogDebug(">>>> LoadUser for BC Services Card: contact != null");
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Contact");
                        // fail if we can't update.
                        throw (httpOperationException);
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
                    catch (HttpOperationException httpOperationException)
                    {
                        _logger.LogError(httpOperationException, "Error updating Worker");
                    }

                }
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
        public static User GetActiveUserBySmGuid(this IDynamicsClient _dynamicsClient, string guid)
        {
            Guid id = new Guid(guid);
            User user = null;
            // try the new method first

            // fallback

            var contact = _dynamicsClient.GetActiveContactByExternalId(id.ToString());
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
        public static User GetActiveUserBySmGuidBridged(this IDynamicsClient _dynamicsClient, bool isServicesCard, string guid)
        {
            User user = null;
            // try the new method first

            // fallback

            var contact = _dynamicsClient.GetActiveContactByExternalIdBridged(isServicesCard, guid);
            if (contact != null)
            {
                user = new User();
                user.FromContact(contact);
            }

            return user;
        }



        /// <summary>
        /// Returns a User based on certain contact details
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static Public.ViewModels.Contact GetUserAsViewModelContact(this IDynamicsClient _dynamicsClient, string userId)
        {
            Public.ViewModels.Contact result = null;
            try
            {
                var user = _dynamicsClient.Systemusers.GetByKey(userId);
                result = new Public.ViewModels.Contact()
                {
                    id = userId,
                    firstname = user.Firstname,
                    lastname = user.Lastname,
                    name = user.Firstname + " " + user.Lastname
                };
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Problem getting user.");
            }

            return result;
        }

        /// <summary>
        /// Returns a User based on certain contact details
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static User GetUserByContactVmBlankSmGuid(this IDynamicsClient _dynamicsClient, Public.ViewModels.Contact contact)
        {

            User result = null;
            var firstUser = _dynamicsClient.GetContactByContactVmBlankSmGuid(contact);
            if (firstUser != null && string.IsNullOrEmpty(firstUser.AdoxioExternalid))
            {
                result = new User();
                result.FromContact(firstUser);
            }

            return result;
        }

        /// <summary>
        /// Returns a Contact based that has an exact match on certain details
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMcontact GetContactByContactVmBlankSmGuid(this IDynamicsClient _dynamicsClient, Public.ViewModels.Contact contact)
        {

            MicrosoftDynamicsCRMcontact result = null;
            if (contact != null)
            {
                var users = _dynamicsClient.GetActiveContactsByDetails(contact.firstname, contact.middlename, contact.lastname, contact.emailaddress1);
                if (users != null && users.Count == 1)
                {
                    result = users.FirstOrDefault();
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
        public static List<MicrosoftDynamicsCRMadoxioPreviousaddress> GetPreviousAddressByContactId(this IDynamicsClient system, string guid)
        {
            List<MicrosoftDynamicsCRMadoxioPreviousaddress> result;
            try
            {
                // fetch from Dynamics.
                var filter = "_adoxio_contactid_value eq " + guid;
                result = system.Previousaddresses.Get(filter: filter).Value.ToList();
            }
            catch (HttpOperationException)
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
            catch (HttpOperationException)
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
            if (!string.IsNullOrEmpty(temp))
            {
                UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

                if (userSettings.AccountId != null && userSettings.AccountId.Length > 0)
                {
                    return userSettings.AccountId == accountId.ToString() || IsChildAccount(userSettings.AccountId, accountId.ToString(), _dynamicsClient);
                }
            }

            // if current user doesn't have an account they are probably not logged in
            return false;
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

        public static bool IsMostlyLiquor(List<MicrosoftDynamicsCRMadoxioLicences> licences)
        {
            bool result = false;
            int liquorCount = 0;
            // check the licence type's category field to determine how many licences are liquor related
            foreach (var licence in licences)
            {
                if (licence.AdoxioLicenceType?.AdoxioCategory != null &&
                    (Public.ViewModels.ApplicationTypeCategory)licence.AdoxioLicenceType?.AdoxioCategory == Public.ViewModels.ApplicationTypeCategory.Liquor)
                {
                    liquorCount++;
                }
            }

            // note that we specify floating point here, as otherwise the math will not work correctly.
            if (licences.Count > 0 && ((liquorCount * 1.0f) >= (licences.Count * 1.0f) / 2.0f))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="account"></param>
        /// <returns>True if the given account mostly has liquor licences.</returns>
        public static bool IsMostlyLiquor(this MicrosoftDynamicsCRMaccount account, IDynamicsClient dynamicsClient)
        {
            bool result = false;

            if (account != null)
            {
                // get the licences for the given account.

                var licences = dynamicsClient.GetAllLicensesByLicencee(null, account.Accountid).ToList();
                result = IsMostlyLiquor(licences);
            }

            return result;
        }

        public static PaymentType GetPaymentType(this MicrosoftDynamicsCRMadoxioApplication application, IDynamicsClient dynamicsClient)
        {
            PaymentType result;
            bool isLiquor = false;
            // determine if the application is for liquor.
            if (application != null && application.AdoxioApplicationTypeId != null && application.AdoxioApplicationTypeId.AdoxioCategory != null)
            {
                if (application.AdoxioApplicationTypeId.AdoxioName != null && application.AdoxioApplicationTypeId.AdoxioName == "Licensee Changes" && application.AdoxioApplicant != null)
                {
                    isLiquor = application.AdoxioApplicant.IsMostlyLiquor(dynamicsClient);
                }
                else
                {
                    isLiquor = (Public.ViewModels.ApplicationTypeCategory)application.AdoxioApplicationTypeId.AdoxioCategory == Public.ViewModels.ApplicationTypeCategory.Liquor;
                }

            }

            if (isLiquor)
            {
                result = PaymentType.LIQUOR;
            }
            else
            {
                result = PaymentType.CANNABIS;
            }

            // TODO - if this is a licencee changes application then check the account's licences.
            // If the account has more liquor licences then use liquor.

            return result;
        }

        public static void UpdateContactBridgeLogin(this IDynamicsClient dynamicsClient, string contactId, string externalGuid, string accountId, string externalBusinessGuid)
        {
            bool isServiceCard = externalBusinessGuid == null;
            // create a "Login" entity.
            MicrosoftDynamicsCRMadoxioLogin login = new MicrosoftDynamicsCRMadoxioLogin()
            {
                AdoxioType = dynamicsClient.GetLoginTypePicklistValue(isServiceCard),
                AdoxioExternalid = externalGuid,
                ContactODataBind = dynamicsClient.GetEntityURI("contacts", contactId)
            };
            if (accountId != null)
            {
                login.RelatedAccountODataBind = dynamicsClient.GetEntityURI("accounts", accountId);
            }

            try
            {
                dynamicsClient.Logins.Create(login);
            }
            catch (HttpOperationException h)
            {
                Log.Logger.Error(h,"Error creating Login record.");
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "Unexpected Error creating Login record.");
            }
        }
    }
}
