using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpdSync;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SpdSync.models.Contact;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpiceUtils
    {
        public ILogger _logger { get; }

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamicsClient;

        public SpiceUtils(IConfiguration Configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(SpdUtils));
            _dynamicsClient = DynamicsUtil.SetupDynamics(Configuration);
        }

        /// <summary>
        /// Hangfire job to receive an import from SPICE.
        /// </summary>
        public void ReceiveImportJob(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job.");
            _logger.LogError("Starting SPICE Import Job.");

            ImportResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Import responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private void ImportResponses(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
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
        /// Generate an application screening request
        /// </summary>
        /// <returns></returns>

        private async Task<ApplicationScreeningRequest> GenerateScreeningRequest(string ApplicationId)
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
            string[] expand = {"adoxio_Contact"};
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
                    
                    //legalEntity.AdoxioLegalentityAdoxioPreviousaddressLegalEntityId
                    /* TODO Add previous addresses */

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

                    /* Add worker to application */
                    request.Associates.Add(associate);
                }
            }
            catch (OdataerrorException)
            {
                _logger.LogError("Error retrieving legal entities");
            }

            return request;
        }
    }
}
