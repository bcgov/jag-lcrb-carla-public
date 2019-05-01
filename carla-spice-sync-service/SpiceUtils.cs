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

        public async Task<ApplicationScreeningRequest> GenerateApplicationScreeningRequest(string ApplicationId)
        {
            // Query Dynamics for application data
            MicrosoftDynamicsCRMadoxioApplication application = await _dynamicsClient.GetApplicationByIdWithChildren(ApplicationId);

            /* Create application */
            ApplicationScreeningRequest request = new ApplicationScreeningRequest()
            {
                Name = application.AdoxioName,
                JobNumber = application.AdoxioJobnumber,
                UrgentPriority = false,
                ApplicantType = ApplicationScreeningRequest.Spice_ApplicantType.Cannabis,
                DateSent = DateTimeOffset.Now,
                BCeIDNumber = application.AdoxioBusinessnumber,
                ApplicantName = application.AdoxioNameofapplicant,
                Address = new Address()
                {
                    AddressStreet1 = application.AdoxioAddressstreet,
                    City = application.AdoxioAddresscity,
                    StateProvince = application.AdoxioAddressprovince,
                    Postal = application.AdoxioAddresspostalcode,
                    Country = application.AdoxioAddresscountry
                },
                ContactPerson = new Contact()
                {
                    FirstName = application.AdoxioContactpersonfirstname,
                    LastName = application.AdoxioContactpersonlastname,
                    MiddleName = application.AdoxioContactmiddlename,
                    ContactEmail = application.AdoxioEmail,
                    ContactPhone = application.AdoxioContactpersonphone
                },
                ApplyingPerson = new Contact()
                {
                    FirstName = application.AdoxioApplyingPerson.Firstname,
                    MiddleName = application.AdoxioApplyingPerson.Middlename,
                    LastName = application.AdoxioApplyingPerson.Lastname,
                    ContactEmail = application.AdoxioApplyingPerson.Emailaddress1
                }
            };

            /* Add applicant details */
            if (application.AdoxioApplicant != null)
            {
                request.Account = new Account()
                {
                    Name = application.AdoxioApplicant.Name
                };
            }

            /* Add establishment */
            if (application.AdoxioEstablishment != null)
            {
                request.Establishment = new Establishment()
                {
                    Name = application.AdoxioEstablishmentpropsedname,
                    PrimaryPhone = application.AdoxioEstablishmentphone,
                    PrimaryEmail = application.AdoxioEstablishmentemail,
                    ParcelId = application.AdoxioEstablishmentparcelid,
                    Address = new Address()
                    {
                        AddressStreet1 = application.AdoxioEstablishmentaddressstreet,
                        City = application.AdoxioEstablishmentaddresscity,
                        StateProvince = "BC",
                        Postal = application.AdoxioEstablishmentaddresspostalcode,
                        Country = "Canada"
                    }
                };
            }

            // get associates from dynamics
            string applicationfilter = "_adoxio_application_value eq " + ApplicationId;
            string[] expand = {"adoxio_Contact", "adoxio_legalentity_adoxio_previousaddress_LegalEntityId"};
            try
            {
                IEnumerable<MicrosoftDynamicsCRMadoxioLegalentity> legalEntities = _dynamicsClient.Legalentities.Get(filter: applicationfilter, expand: expand).Value;
                foreach (var legalEntity in legalEntities)
                {
                    LegalEntity associate = new LegalEntity()
                    {
                        Name = legalEntity.AdoxioName,
                        Contact = new Contact()
                        {
                            FirstName = legalEntity.AdoxioFirstname,
                            LastName = legalEntity.AdoxioLastname,
                            MiddleName = legalEntity.AdoxioMiddlename,
                            ContactEmail = legalEntity.AdoxioEmail,
                            ContactPhone = legalEntity.AdoxioPhonenumber,
                            Address = new Address()
                            {
                                AddressStreet1 = legalEntity.AdoxioContact.Address1Line1,
                                AddressStreet2 = legalEntity.AdoxioContact.Address1Line2,
                                AddressStreet3 = legalEntity.AdoxioContact.Address1Line3,
                                City = legalEntity.AdoxioContact.Address1City,
                                StateProvince = legalEntity.AdoxioContact.Address1Stateorprovince,
                                Postal = legalEntity.AdoxioContact.Address1Postalcode,
                                Country = legalEntity.AdoxioContact.Address1Country
                            }
                        },
                        InterestPercentage = Convert.ToDecimal(legalEntity.AdoxioInterestpercentage),
                        CommonVotingShares = legalEntity.AdoxioCommonvotingshares,
                        DateSharesIssued = legalEntity.AdoxioDateofsharesissued,
                        DateAppointed = legalEntity.AdoxioDateofappointment


                        // Might be needed here but its not in the table? might only be needed for worker screening
                        //RecordIdentifier = legalEntity.AdoxioLegalentityid, // TODO Should probably replace with non-Guid field
                        //Name = legalEntity.AdoxioName,
                        //GivenName = legalEntity.AdoxioFirstname,
                        //Surname = legalEntity.AdoxioLastname,
                        //SecondName = legalEntity.AdoxioMiddlename,
                        //BirthDate = legalEntity.AdoxioDateofbirth,
                        //Birthplace = legalEntity.AdoxioBirthplace,
                        //BCIdCardNumber = legalEntity.AdoxioBcidcardnumber,
                        //DriversLicence = legalEntity.AdoxioDriverslicencenumber,
                        //SelfDisclosure = (General_YesNo)legalEntity.AdoxioSelfdisclosure,
                        //Gender = (Adoxio_GenderCode)legalEntity.AdoxioGendercode,
                        //ContactPhone = legalEntity.AdoxioPhonenumber,
                        //ContactEmail = legalEntity.AdoxioEmail,
                    };

                    /* Add previous addresses */
                    foreach (var address in legalEntity.AdoxioLegalentityAdoxioPreviousaddressLegalEntityId)
                    {
                        var newAddress = new Address()
                        {
                            AddressStreet1 = address.AdoxioStreetaddress,
                            City = address.AdoxioCity,
                            StateProvince = address.AdoxioProvstate,
                            Postal = address.AdoxioPostalcode,
                            Country = address.AdoxioCountry,
                            ToDate = address.AdoxioTodate,
                            FromDate = address.AdoxioFromdate
                        };
                        associate.PreviousAddresses.Add(newAddress);
                    }

                    /* Add aliases */
                    var aliases = legalEntity.AdoxioLegalentityAliases;
                    foreach (var alias in aliases)
                    {
                        associate.Aliases.Add(new Alias()
                        {
                            GivenName = alias.AdoxioFirstname,
                            Surname = alias.AdoxioLastname,
                            SecondName = alias.AdoxioMiddlename
                        });
                    }

                    /* Add associate to application */
                    request.Associates.Add(associate);
                }
            }
            catch (OdataerrorException)
            {
                _logger.LogError("Error retrieving legal entities");
            }

            return request;
        }

        public async Task<Interfaces.Spice.Models.WorkerScreeningRequest> GenerateWorkerScreeningRequest(string WorkerId)
        {
            // Query Dynamics for application data
            var worker = await _dynamicsClient.GetWorkerById(WorkerId);

            /* Create application */
            Interfaces.Spice.Models.WorkerScreeningRequest request = new Interfaces.Spice.Models.WorkerScreeningRequest()
            {
                RecordIdentifier = worker.AdoxioWorkerid,
                Name = worker.AdoxioName,
                BirthDate = worker.AdoxioDateofbirth,
                SelfDisclosure =  worker.AdoxioSelfdisclosure, // (Interfaces.Spice.Models.WorkerScreeningRequest.General_YesNo)
                Gender = worker.AdoxioGendercode, // (Interfaces.Spice.Models.WorkerScreeningRequest.Adoxio_GenderCode)
                Birthplace = worker.AdoxioBirthplace,
                //BCIdCardNumber = worker.AdoxioBcidcardnumber,
                DriversLicence = worker.AdoxioDriverslicencenumber
            };

            /* Add applicant details */
            if (worker.AdoxioContactId != null)
            {
                request.Contact = new Interfaces.Spice.Models.Contact()
                {
                    FirstName = worker.AdoxioContactId.Firstname,
                    LastName = worker.AdoxioContactId.Lastname,
                    MiddleName = worker.AdoxioContactId.Middlename,
                    ContactEmail = worker.AdoxioContactId.Emailaddress1,
                    ContactPhone = worker.AdoxioContactId.Telephone1,
                    Address = new Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                        AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                        AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                        City = worker.AdoxioContactId.Address1City,
                        StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                        Postal = worker.AdoxioContactId.Address1Postalcode,
                        Country = worker.AdoxioContactId.Address1Country
                    }
                };

            }

            // TODO - add aliases, previous addresses.

            return request;
        }
    }
}
