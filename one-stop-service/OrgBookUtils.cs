using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.OneStopService
{
    public class Attributes
    {
        public string registration_id { get; set; }
        public string licence_number { get; set; }
        public string establishment_name { get; set; }
        public DateTimeOffset issue_date { get; set; }
        public DateTimeOffset? effective_date { get; set; }
        public DateTimeOffset? expiry_date { get; set; }
        public string civic_address { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class Credential
    {
        public string schema { get; set; }
        public string version { get; set; }
        public Attributes attributes { get; set; }
    }

    public class OrgBookUtils
    {
        private static readonly HttpClient Client = new HttpClient();

        private IConfiguration Configuration { get; }

        private IDynamicsClient _dynamics;

        private ILogger _logger;

        public OrgBookUtils(IConfiguration Configuration, ILogger logger)
        {
            this.Configuration = Configuration;
            _dynamics = DynamicsSetupUtil.SetupDynamics(Configuration);
            _logger = logger;
        }

        /// <summary>
        /// Hangfire job to check for and send recent licences
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for new licences for orgbook job.");
            }
            IList<MicrosoftDynamicsCRMadoxioLicences> result = null;
            try
            {
                string filter = $"adoxio_businessprogramaccountreferencenumber eq null";
                result = _dynamics.Licenceses.Get(filter: filter).Value;
            }
            catch (OdataerrorException odee)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine("Error getting Licences");
                    hangfireContext.WriteLine("Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("Response:");
                    hangfireContext.WriteLine(odee.Response.Content);
                }

                // fail if we can't get results.
                throw (odee);
            }

            // now for each one process it.
            foreach (var item in result)
            {
                string licenceId = item.AdoxioLicencesid;
                await CreateLicenceCredential(hangfireContext, licenceId);
            }

            hangfireContext.WriteLine("End of check for new licences for orgbook job.");
        }

        /// <summary>
        /// Hangfire job to create licence credential in OrgBook.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CreateLicenceCredential(PerformContext hangfireContext, string licenceGuidRaw)
        {
            hangfireContext.WriteLine("Starting OrgBook CreateLicenceCredential Job.");
            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            hangfireContext.WriteLine($"Getting Licence {licenceGuid}");
            var licence = _dynamics.GetLicenceByIdWithChildren(licenceGuid);

            string schema = MapLicenceTypeToSchema(licence.AdoxioLicenceType.AdoxioName);

            Credential credential = new Credential()
            {
                attributes = new Attributes() {
                    //TODO Get real field in place
                    registration_id = Configuration["ORGBOOK_REGISTRATION_ID"],
                    licence_number = licence.AdoxioLicencenumber,
                    establishment_name = licence.AdoxioEstablishment?.AdoxioName,
                    issue_date = DateTime.UtcNow,
                    effective_date = licence.AdoxioEffectivedate,
                    expiry_date = licence.AdoxioExpirydate,
                    civic_address = licence.AdoxioEstablishmentaddressstreet,
                    city = licence.AdoxioEstablishmentaddresscity,
                    province = "BC",
                    postal_code = licence.AdoxioEstablishmentaddresspostalcode,
                    country = "Canada",
                },
                schema = schema,
                version = GetSchemaVersion(schema)
            };

            HttpResponseMessage response = await Client.PostAsJsonAsync(
                Configuration["AGENT_URL"] + "lcrb/issue-credential", new List<Credential>() { credential });

            if (!response.IsSuccessStatusCode)
            {
                string _responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
                hangfireContext.WriteLine($"Failed to create verifiable credential for licence {licence.AdoxioLicencenumber}");
            }

            _logger.LogInformation($"Successfully created verifiable credential for licence {licence.AdoxioLicencenumber}");
            hangfireContext.WriteLine($"Successfully created verifiable credential for licence {licence.AdoxioLicencenumber}");
        }

        private string MapLicenceTypeToSchema(string licenceType)
        {
            if (licenceType == "Cannabis Retail Store")
            {
                return "cannabis-retail-store-licence.lcrb";
            }
            else if (licenceType == "Marketing")
            {
                return "cannabis-marketing-licence.lcrb";
            }
            return null;
        }

        private string GetSchemaVersion(string schema)
        {
            if(schema == "cannabis-marketing-licence.lcrb")
            {
                return Configuration["MARKETING_SCHEMA_VERSION"];
            }
            else if(schema == "cannabis-retail-store-licence.lcrb")
            {
                return Configuration["RETAIL_STORE_SCHEMA_VERSION"];
            }
            return null;
        }
    }
}
