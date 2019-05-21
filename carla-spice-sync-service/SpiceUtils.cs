using CarlaSpiceSync.models;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using SpdSync;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpiceUtils
    {
        public ILogger _logger { get; }

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamicsClient;
        public ISpiceClient SpiceClient;

        public SpiceUtils(IConfiguration Configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(SpiceUtils));
            _dynamicsClient = DynamicsUtil.SetupDynamics(Configuration);

            // TODO - move this into a seperate routine.

            string spiceURI = Configuration["SPICE_URI"];
            string token = Configuration["SPICE_JWT_TOKEN"];

            // create JWT credentials
            TokenCredentials credentials = new TokenCredentials(token);

            SpiceClient = new SpiceClient(new Uri(spiceURI), credentials);
        }


        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public void SendWorkerExportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Export Job.");
            _logger.LogError("Starting SPD Export Job.");

            Type type = typeof(MicrosoftDynamicsCRMadoxioSpddatarow);

            string filter = $"adoxio_isexport eq true and adoxio_exporteddate eq null";
            List<MicrosoftDynamicsCRMadoxioSpddatarow> result = null;

            try
            {
                result = _dynamicsClient.Spddatarows.Get(filter: filter).Value.ToList();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting SPD data rows");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);

                _logger.LogError("Error getting SPD data rows");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }
            

            if (result != null && result.Count > 0)
            {
                List<Interfaces.Spice.Models.WorkerScreeningRequest> payload = new List<Interfaces.Spice.Models.WorkerScreeningRequest>();

                foreach (var row in result)
                {
                    var runner = GenerateWorkerScreeningRequest(row.AdoxioLcrbworkerjobid, _logger);
                    runner.Wait();
                    var workerRequest = runner.Result;
                    payload.Add(workerRequest);
                }

                // send to spice.

                var spiceRunner = SpiceClient.ReceiveWorkerScreeningsWithHttpMessagesAsync(payload);
                spiceRunner.Wait();
                var spiceResult = spiceRunner.Result;

                hangfireContext.WriteLine("Response code was");
                hangfireContext.WriteLine(spiceResult.Response.StatusCode.ToString());

                _logger.LogError("Response code was");
                _logger.LogError(spiceResult.Response.StatusCode.ToString());
           }

            hangfireContext.WriteLine("End of SPD Export Job.");
            _logger.LogError("End of SPD Export Job.");
        }

        /// <summary>
        /// Hangfire job to receive an application screening import from SPICE.
        /// </summary>
        public void ReceiveApplicationImportJob(PerformContext hangfireContext, List<ApplicationScreeningResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            _logger.LogError("Starting SPICE Import Job for Application Screening..");

            ImportApplicationResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Hangfire job to receive an import from SPICE.
        /// </summary>
        public void ReceiveWorkerImportJob(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Worker Screening.");
            _logger.LogError("Starting SPICE Import Job for Worker Screening.");

            ImportWorkerResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Import responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private void ImportWorkerResponses(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
        {
            foreach (WorkerScreeningResponse workerResponse in responses)
            {
                // search for the Personal History Record.
                MicrosoftDynamicsCRMadoxioPersonalhistorysummary record = _dynamicsClient.Personalhistorysummaries.GetByWorkerJobNumber(workerResponse.RecordIdentifier);

                if (record != null)
                {
                    // update the record.
                    MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchRecord = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary()
                    {
                        AdoxioSecuritystatus = SPDResultTranslate.GetTranslatedSecurityStatus(workerResponse.Result),
                        AdoxioCompletedon = workerResponse.DateProcessed
                    };

                    try
                    {
                        _dynamicsClient.Personalhistorysummaries.Update(record.AdoxioPersonalhistorysummaryid, patchRecord);
                    }
                    catch (OdataerrorException odee)
                    {
                        hangfireContext.WriteLine("Error updating worker personal history");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        _logger.LogError("Error updating worker personal history");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                    }
                }
            }
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private async void ImportApplicationResponses(PerformContext hangfireContext, List<ApplicationScreeningResponse> responses)
        {
            foreach (ApplicationScreeningResponse applicationResponse in responses)
            {
                MicrosoftDynamicsCRMadoxioApplication applicationRecord = await _dynamicsClient.GetApplicationByIdWithChildren(applicationResponse.RecordIdentifier);

                if (applicationRecord != null)
                {
                    // update the record.
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {

                    };

                    try
                    {
                        _dynamicsClient.Applications.Update(applicationRecord.AdoxioJobnumber, patchRecord);
                    }
                    catch (OdataerrorException odee)
                    {
                        hangfireContext.WriteLine("Error updating application");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                    }
                }
            }
        }

        /// <summary>
        /// Generate an application screening request
        /// </summary>
        /// <returns></returns>
        public Interfaces.Spice.Models.ApplicationScreeningRequest GenerateApplicationScreeningRequest(string applicationId)
        {
            string appFilter = "adoxio_applicationid eq " + applicationId;
            string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
            var application = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value[0];

            var screeningRequest = CreateApplicationScreeningRequest(application);
            var associates = CreateApplicationAssociatesScreeningRequest(application._adoxioApplicantValue, screeningRequest.Associates);
            screeningRequest.Associates = screeningRequest.Associates.Concat(associates).ToList();

            return screeningRequest;
        }

        /// <summary>
        /// Sends the application screening request to spice.
        /// </summary>
        /// <returns>The application screening request success boolean.</returns>
        /// <param name="applicationRequest">Application request.</param>
        public async Task<bool> SendApplicationScreeningRequest(Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest applicationRequest)
        {
            List<Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest> payload = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest>
            {
                applicationRequest
            };

            _logger.LogInformation($"Sending Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");
            _logger.LogInformation($"Application has {applicationRequest.Associates.Count} associates");

            var result = await SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload);

            _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
            _logger.LogInformation($"Done Send Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

            return result.Response.StatusCode.ToString() == "OK";
        }

        /// <summary>
        /// Sends the worker screening request to spice.
        /// </summary>
        /// <returns>The worker screening request success boolean.</returns>
        /// <param name="workerScreeningRequest">Worker screening request.</param>
        public async Task<bool> SendWorkerScreeningRequest(Gov.Lclb.Cllb.Interfaces.Spice.Models.WorkerScreeningRequest workerScreeningRequest, ILogger logger)
        {
            // send the data
            List<Interfaces.Spice.Models.WorkerScreeningRequest> payload = new List<Interfaces.Spice.Models.WorkerScreeningRequest>
            {
                workerScreeningRequest
            };

            logger.LogInformation($"Sending Worker {workerScreeningRequest.Contact.ContactId} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

            var result = await SpiceClient.ReceiveWorkerScreeningsWithHttpMessagesAsync(payload);

            logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
            logger.LogInformation($"Done Send Worker {workerScreeningRequest.Contact.ContactId} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

            return result.Response.StatusCode.ToString() == "OK";
        }

        public async Task<Interfaces.Spice.Models.WorkerScreeningRequest> GenerateWorkerScreeningRequest(string WorkerId, ILogger logger)
        {
            // Query Dynamics for application data
            var worker = await _dynamicsClient.GetWorkerById(WorkerId);

            /* Create application */
            Interfaces.Spice.Models.WorkerScreeningRequest request = new Interfaces.Spice.Models.WorkerScreeningRequest()
            {
                RecordIdentifier = worker.AdoxioWorkerid,
                Name = worker.AdoxioName,
                BirthDate = worker.AdoxioDateofbirth,
                SelfDisclosure = ((GeneralYesNo)worker.AdoxioSelfdisclosure).ToString(),
                Gender = ((AdoxioGenderCode)worker.AdoxioGendercode).ToString(),
                Birthplace = worker.AdoxioBirthplace,
                BcIdCardNumber = worker.AdoxioBcidcardnumber,
                DriversLicence = worker.AdoxioDriverslicencenumber
            };

            /* Add applicant details */
            if (worker.AdoxioContactId != null)
            {
                request.Contact = new Interfaces.Spice.Models.Contact()
                {
                    ContactId = worker.AdoxioContactId.Contactid,
                    FirstName = worker.AdoxioContactId.Firstname,
                    LastName = worker.AdoxioContactId.Lastname,
                    MiddleName = worker.AdoxioContactId.Middlename,
                    Email = worker.AdoxioContactId.Emailaddress1,
                    PhoneNumber = worker.AdoxioContactId.Telephone1,
                    Address = new Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                        AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                        AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                        City = worker.AdoxioContactId.Address1City,
                        StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                        Country = worker.AdoxioContactId.Address1Country
                    }
                };
            }

            // TODO - add aliases, previous addresses.
            logger.LogInformation("Finished building Model");
            return request;
        }

        private Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest CreateApplicationScreeningRequest(MicrosoftDynamicsCRMadoxioApplication application)
        {
            var screeningRequest = new Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest()
            {
                Name = application.AdoxioName,
                RecordIdentifier = application.AdoxioJobnumber,
                UrgentPriority = false,
                ApplicantType = Gov.Lclb.Cllb.Interfaces.Spice.Models.SpiceApplicantType.Cannabis,
                DateSent = DateTimeOffset.Now,
                BCeIDNumber = application.AdoxioBusinessnumber,
                ApplicantName = application.AdoxioNameofapplicant,
                BusinessAddress = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                {
                    AddressStreet1 = application.AdoxioAddressstreet,
                    City = application.AdoxioAddresscity,
                    StateProvince = application.AdoxioAddressprovince,
                    Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.AdoxioAddresspostalcode)) ? application.AdoxioAddresspostalcode : null,
                    Country = application.AdoxioAddresscountry
                },
                ContactPerson = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    FirstName = application.AdoxioContactpersonfirstname,
                    LastName = application.AdoxioContactpersonlastname,
                    MiddleName = application.AdoxioContactmiddlename,
                    Email = application.AdoxioEmail,
                    PhoneNumber = application.AdoxioContactpersonphone
                }
            };
            if (application.AdoxioApplyingPerson != null)
            {
                string companyName = null;
                if (application.AdoxioApplyingPerson._parentcustomeridValue != null) {
                    MicrosoftDynamicsCRMaccount company = _dynamicsClient.Accounts.Get(filter: "accountid eq " + application.AdoxioApplyingPerson._parentcustomeridValue).Value[0];
                    companyName = company.Name;
                }
                screeningRequest.ApplyingPerson = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    ContactId = application.AdoxioApplyingPerson.Contactid,
                    FirstName = application.AdoxioApplyingPerson.Firstname,
                    CompanyName = companyName,
                    MiddleName = application.AdoxioApplyingPerson.Middlename,
                    LastName = application.AdoxioApplyingPerson.Lastname,
                    Email = application.AdoxioApplyingPerson.Emailaddress1
                };
            }
            /* Add applicant details */
            if (application.AdoxioApplicant != null)
            {
                screeningRequest.ApplicantAccount = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                {
                    AccountId = application.AdoxioApplicant.Accountid,
                    Name = application.AdoxioApplicant.Name
                };
            }

            /* Add establishment */
            if (application.AdoxioEstablishment != null)
            {
                screeningRequest.Establishment = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Establishment()
                {
                    Name = application.AdoxioEstablishmentpropsedname,
                    PrimaryPhone = application.AdoxioEstablishmentphone,
                    PrimaryEmail = application.AdoxioEstablishmentemail,
                    ParcelId = application.AdoxioEstablishmentparcelid,
                    Address = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = application.AdoxioEstablishmentaddressstreet,
                        City = application.AdoxioEstablishmentaddresscity,
                        StateProvince = "BC",
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.AdoxioEstablishmentaddresspostalcode)) ? application.AdoxioEstablishmentaddresspostalcode : null,
                        Country = "Canada"
                    }
                };
            }

            /* Add key personnel */
            screeningRequest.Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>();
            string keypersonnelfilter = "_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_iskeypersonnel eq true and adoxio_isindividual eq 1";
            string[] expand = { "adoxio_Contact" };
            var keyPersonnel = _dynamicsClient.Legalentities.Get(filter: keypersonnelfilter, expand: expand).Value;
            if (keyPersonnel != null)
            {
                foreach (var legalEntity in keyPersonnel)
                {
                    Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity person = CreateAssociate(legalEntity);
                    screeningRequest.Associates.Add(person);
                }
            }
            return screeningRequest;
        }

        private List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> CreateApplicationAssociatesScreeningRequest(string accountId, IList<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> foundAssociates)
        {
            List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> newAssociates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>();
            string applicationfilter = "_adoxio_account_value eq " + accountId + " and _adoxio_profilename_value ne " + accountId;
            foreach (var assoc in foundAssociates)
            {
                if (accountId != assoc.EntityId)
                {
                    applicationfilter += " and adoxio_legalentityid ne " + assoc.EntityId;
                }
            }
            string[] expand = { "adoxio_Contact", "adoxio_Account"};

            var legalEntities = _dynamicsClient.Legalentities.Get(filter: applicationfilter, expand: expand).Value;
            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity associate = CreateAssociate(legalEntity);
                    newAssociates.Add(associate);
                }
            }
            var newFoundAssociates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>(foundAssociates);
            newFoundAssociates.AddRange(newAssociates);
            foreach (var assoc in newAssociates.ToList())
            {
                if (assoc.IsIndividual != true)
                {
                    var moreAssociates = CreateApplicationAssociatesScreeningRequest(assoc.Account.AccountId, newFoundAssociates);
                    assoc.Account.Associates = moreAssociates;
                }
            }
            return newAssociates;
        }

        private Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity associate = new Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity()
            {
                EntityId = legalEntity.AdoxioLegalentityid,
                Name = legalEntity.AdoxioName,
                PreviousAddresses = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.Address>(),
                Aliases = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.Alias>()
            };
            if (legalEntity.AdoxioIsindividual != null && legalEntity.AdoxioIsindividual == 1 && legalEntity.AdoxioContact != null)
            {
                associate.IsIndividual = true;
                associate.Contact = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    ContactId = legalEntity.AdoxioContact.Contactid,
                    FirstName = legalEntity.AdoxioContact.Firstname,
                    LastName = legalEntity.AdoxioContact.Lastname,
                    MiddleName = legalEntity.AdoxioContact.Middlename,
                    Email = legalEntity.AdoxioContact.Emailaddress1,
                    PhoneNumber = legalEntity.AdoxioContact.Telephone1,
                    SelfDisclosure = (legalEntity.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)legalEntity.AdoxioSelfdisclosure).ToString(),
                    Gender = (legalEntity.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)legalEntity.AdoxioGendercode).ToString(),
                    Birthplace = legalEntity.AdoxioBirthplace,
                    BirthDate = legalEntity.AdoxioDateofbirth,
                    BcIdCardNumber = legalEntity.AdoxioBcidcardnumber,
                    DriversLicenceNumber = legalEntity.AdoxioDriverslicencenumber,
                    Address = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = legalEntity.AdoxioContact.Address1Line1,
                        AddressStreet2 = legalEntity.AdoxioContact.Address1Line2,
                        AddressStreet3 = legalEntity.AdoxioContact.Address1Line3,
                        City = legalEntity.AdoxioContact.Address1City,
                        StateProvince = legalEntity.AdoxioContact.Address1Stateorprovince,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(legalEntity.AdoxioContact.Address1Postalcode)) ? legalEntity.AdoxioContact.Address1Postalcode : null,
                        Country = legalEntity.AdoxioContact.Address1Country
                    }
                };

                /* Add previous addresses */
                var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + legalEntity.AdoxioContact.Contactid).Value;
                foreach (var address in previousAddresses)
                {
                    var newAddress = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(address.AdoxioPostalcode)) ? address.AdoxioPostalcode : null,
                        Country = address.AdoxioCountry,
                        ToDate = address.AdoxioTodate,
                        FromDate = address.AdoxioFromdate
                    };
                    associate.PreviousAddresses.Add(newAddress);
                }

                /* Add aliases */
                var aliases = _dynamicsClient.Aliases.Get(filter: "_adoxio_contactid_value eq " + legalEntity.AdoxioContact.Contactid).Value;
                foreach (var alias in aliases)
                {
                    associate.Aliases.Add(new Gov.Lclb.Cllb.Interfaces.Spice.Models.Alias()
                    {
                        GivenName = alias.AdoxioFirstname,
                        Surname = alias.AdoxioLastname,
                        SecondName = alias.AdoxioMiddlename
                    });
                }
            }
            else
            {
                if (legalEntity._adoxioShareholderaccountidValue != null)
                {
                    var account = _dynamicsClient.Accounts.Get(filter: "accountid eq " + legalEntity._adoxioShareholderaccountidValue).Value;
                    associate.Account = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                    {
                        AccountId = account[0].Accountid,
                        Name = account[0].Name,
                        Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>()
                    };
                }
                else
                {
                    associate.Account = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                    {
                        AccountId = legalEntity.AdoxioAccount.Accountid,
                        Name = legalEntity.AdoxioAccount.Name,
                        Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>()
                    };
                }
                associate.IsIndividual = false;
            }
            return associate;
        }
    }
}
