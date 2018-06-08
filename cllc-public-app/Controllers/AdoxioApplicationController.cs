using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Gov.Lclb.Cllb.Interfaces;


namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioApplicationController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;

        private readonly IHttpContextAccessor _httpContextAccessor;
  
        public AdoxioApplicationController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            Configuration = configuration;
            this._system = context;
            this._httpContextAccessor = httpContextAccessor;
            this._distributedCache = null; // distributedCache;
        }

        private async Task<List<ViewModels.AdoxioApplication>> GetApplicationsByAplicant(string applicantId)
        {
            List<ViewModels.AdoxioApplication> result = new List<ViewModels.AdoxioApplication>();
            IEnumerable<Adoxio_application> dynamicsApplicationList = null;
            if (string.IsNullOrEmpty (applicantId))
            {
                dynamicsApplicationList = await _system.Adoxio_applications.ExecuteAsync();
            }
            else
            {
                // Shareholders have an adoxio_position value of x.
                dynamicsApplicationList = await _system.Adoxio_applications
                    .AddQueryOption("$filter", "_adoxio_applicant_value eq " + applicantId) 
                    .ExecuteAsync();
            }

            if (dynamicsApplicationList != null)
            {
                foreach (Adoxio_application dynamicsApplication in dynamicsApplicationList)
                {
                    result.Add(await dynamicsApplication.ToViewModel(_system));
                }
            }
            return result;
        }

        /// GET all applications in Dynamics
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsApplications ()
        {
            // get all applications in Dynamics
            List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByAplicant(null);
            
            return Json(adoxioApplications);
        }

        /// GET all applications in Dynamics for the current user
        [HttpGet("current")]
        public async Task<JsonResult> GetCurrentUserDyanamicsApplications()
        {
            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // GET all applications in Dynamics by applicant using the account Id assigned to the user logged in
            List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByAplicant(userSettings.AccountId);

            // For Demo Only, hardcode the account id !!!
            //string accountId = "f3310e39-e352-e811-8140-480fcfeac941";
            //List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByAplicant(accountId);

            return Json(adoxioApplications);
        }

        /// GET all applications in Dynamics by applicant ID
        [HttpGet("{applicantId}")]
        public async Task<JsonResult> GetDynamicsApplications(string applicantId)
        {
            // get all applications in Dynamics
            List<ViewModels.AdoxioApplication> adoxioApplications = await GetApplicationsByAplicant(applicantId);

            return Json(adoxioApplications);
        }


        [HttpPost()]
		public async Task<IActionResult> CreateApplication([FromBody] ViewModels.AdoxioApplication item)
        {
			// create a new dynamics application object.
            //Interfaces.Microsoft.Dynamics.CRM.Adoxio_application adoxioApplication = new Interfaces.Microsoft.Dynamics.CRM.Adoxio_application();
			Adoxio_application adoxioApplication = await item.ToModel(_system);

			// create a DataServiceCollection to add the record
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_application> ApplicationCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_application>(_system);
            // add a new contact.
			ApplicationCollection.Add(adoxioApplication);

			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

			Interfaces.Microsoft.Dynamics.CRM.Account owningAccount = await _system.GetAccountById(_distributedCache, Guid.Parse(userSettings.AccountId));
			adoxioApplication.Adoxio_Applicant = owningAccount;

			// PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.
            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            
			var id = dsr.GetAssignedId();
			Adoxio_application application = await _system.GetAdoxioApplicationById(_distributedCache, (Guid)id);
			if (application == null) {
				return StatusCode(500, "Something bad happened");
			}
			application.Adoxio_applicationid = id;

			return Json(await application.ToViewModel(_system));

            /*
			// create a DataServiceCollection to add the record
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_application> ContactCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_application>(_system);
            // add a new contact.
            ContactCollection.Add(adoxioApplication);
            
            // changes need to made after the add in order for them to be saved.
            // tab_general
            adoxioApplication.Adoxio_LicenceType = item.Adoxio_LicenceType; // Licence Type*
            adoxioApplication.Adoxio_ApplyingPerson = item.Adoxio_ApplyingPerson; // Applying Person
            adoxioApplication.Adoxio_LastCompletedStep = item.Adoxio_LastCompletedStep; //Last Completed Step
            adoxioApplication.Adoxio_jobnumber = item.Adoxio_jobnumber; //Job Number
            adoxioApplication.Adoxio_Applicant = item.Adoxio_Applicant; // Applicant
            adoxioApplication.Adoxio_otherapplicanttype = item.Adoxio_otherapplicanttype; // Other Applicant Type
            adoxioApplication.Adoxio_MarkStepIncomplete = item.Adoxio_MarkStepIncomplete; // Mark Step Incomplete
            adoxioApplication.Adoxio_AssignedLicence = item.Adoxio_AssignedLicence; // Assigned Licence

            // tab_businessinfo
            adoxioApplication.Adoxio_applicanttype = item.Adoxio_applicanttype; // Business Type*
            adoxioApplication.Adoxio_businessnumber = item.Adoxio_businessnumber; // The Business Registration Number is
            adoxioApplication.Adoxio_businessnumber = item.Adoxio_businessnumber; // The Business Name is
            adoxioApplication.Adoxio_addressstreet = item.Adoxio_addressstreet; // Street
            adoxioApplication.Adoxio_addresscity = item.Adoxio_addresscity; // City
            adoxioApplication.Adoxio_addressprovince = item.Adoxio_addressprovince; // Province
            adoxioApplication.Adoxio_addresspostalcode = item.Adoxio_addresspostalcode; // Postal Code
            adoxioApplication.Adoxio_addresscountry = item.Adoxio_addresscountry; // Country

            // tab_contactperson
            adoxioApplication.Adoxio_areyouthemaincontactforapplication = item.Adoxio_areyouthemaincontactforapplication; // Are you the main person to contact for  this application?
            adoxioApplication.Adoxio_contactpersonfirstname = item.Adoxio_contactpersonfirstname; // First Name
            adoxioApplication.Adoxio_contactmiddlename = item.Adoxio_contactmiddlename; // Middle Name
            adoxioApplication.Adoxio_contactpersonlastname = item.Adoxio_contactpersonlastname; // Last Name
            adoxioApplication.Adoxio_email = item.Adoxio_email; // Email
            adoxioApplication.Adoxio_contactpersonphone = item.Adoxio_contactpersonphone; // Phone

            //  tab_personalhistory
            adoxioApplication.Adoxio_applicanttype = item.Adoxio_applicanttype; // Applicant Type*
            adoxioApplication.Adoxio_personalhistoryinstructionfield = item.Adoxio_personalhistoryinstructionfield; // Personal History Instruction Field

            // tab_establishmentpart1
            adoxioApplication.Adoxio_registeredestablishment = item.Adoxio_registeredestablishment; // Are you applying for a previously registered establishment in your account?*
            adoxioApplication.Adoxio_LicenceEstablishment = item.Adoxio_LicenceEstablishment; // Establishment
            adoxioApplication.Adoxio_establishmentpropsedname = item.Adoxio_establishmentpropsedname; // The Establishment Name is
            adoxioApplication.Adoxio_establishmentaddressstreet = item.Adoxio_establishmentaddressstreet; // Street
            adoxioApplication.Adoxio_establishmentaddresscity = item.Adoxio_establishmentaddresscity; // City
            adoxioApplication.Adoxio_establishmentaddresspostalcode = item.Adoxio_establishmentaddresspostalcode; // Postal Code
            adoxioApplication.Adoxio_establishmentaddresscountry = item.Adoxio_establishmentaddresscountry; // Country
            adoxioApplication.Adoxio_LocalGoverment = item.Adoxio_LocalGoverment; // Local Goverment
            adoxioApplication.Adoxio_Jurisdiction = item.Adoxio_Jurisdiction; // Jurisdiction
            adoxioApplication.Adoxio_establishmentparcelid = item.Adoxio_establishmentparcelid; // Parcel ID

            // tab_establishmentpart2
            adoxioApplication.Adoxio_establishmentcomplytozoningregulations = item.Adoxio_establishmentcomplytozoningregulations; // Does the Establishment location comply to all zoning regulations?
            adoxioApplication.Adoxio_establishmentcomplytoallbylaws = item.Adoxio_establishmentcomplytoallbylaws; // Does the Establishment location comply to all by-laws?

            // tab_establishmentpart3
            adoxioApplication.Adoxio_holdsotherlicencesoptionset = item.Adoxio_holdsotherlicencesoptionset; // Does the Establishment hold other licences?
            adoxioApplication.Adoxio_otherbusinessesatthesamelocation = item.Adoxio_otherbusinessesatthesamelocation; // Are there other businesses operating at the same location?
            adoxioApplication.Adoxio_establishmentotherbusinessname = item.Adoxio_establishmentotherbusinessname; // What is the Business Name?
            adoxioApplication.Adoxio_establishmentotherbusinessnature = item.Adoxio_establishmentotherbusinessnature; // In what nature this Business is operating?

            // tab_supportingdocuments
            adoxioApplication.Adoxio_uploadedevidenceofvalidinterest = item.Adoxio_uploadedevidenceofvalidinterest; // Have you attached and uploaded the Evidence of Valid Interest?
            adoxioApplication.Adoxio_uploadedfloorplans = item.Adoxio_uploadedfloorplans; // Have you attached and uploaded the Floor Plan?
            adoxioApplication.Adoxio_uploadedsitemap = item.Adoxio_uploadedsitemap; // Have you attached and uploaded the Site Map?
            adoxioApplication.Adoxio_uploadedimageofestablishment = item.Adoxio_uploadedimageofestablishment; // Have you attached and uploaded Images of the Establishment?

            // tab_declarationofsigningauthority
            adoxioApplication.Adoxio_signatureagreement = item.Adoxio_signatureagreement; // Signature Agreement
            adoxioApplication.Adoxio_signaturename = item.Adoxio_signaturename; // Signature Name
            adoxioApplication.Adoxio_signatureposition = item.Adoxio_signatureposition; // Signature Position
            adoxioApplication.Adoxio_signaturedate = item.Adoxio_signaturedate; // Signature Date

            // tab_payment
            adoxioApplication.Adoxio_paymentmethod = item.Adoxio_paymentmethod; // Payment Method
            adoxioApplication.Adoxio_Invoice = item.Adoxio_Invoice; // Invoice

            // tab_LicenceFeePayment
            adoxioApplication.Adoxio_licencefeeinvoicepaid = item.Adoxio_licencefeeinvoicepaid; // Licence Fee Invoice Paid
            adoxioApplication.Adoxio_LicenceFeeInvoice = item.Adoxio_LicenceFeeInvoice; // Licence Fee Invoice

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.
            DataServiceResponse dsr = await _system.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }

            return Json(adoxioApplication);
            */
        }
    }
}
