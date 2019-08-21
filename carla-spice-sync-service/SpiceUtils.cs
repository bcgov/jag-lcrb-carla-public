using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
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
            SpiceClient = SetupSpiceClient(Configuration);
        }

        public SpiceClient SetupSpiceClient(IConfiguration Configuration)
        {
            string spiceURI = Configuration["SPICE_URI"];
            string token = Configuration["SPICE_JWT_TOKEN"];

            // create JWT credentials
            TokenCredentials credentials = new TokenCredentials(token);

            return new SpiceClient(new Uri(spiceURI), credentials);
        }

        public void ReceiveWorkerImportJob(PerformContext hangfireContext, List<CompletedWorkerScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Worker Screening.");
            _logger.LogError("Starting SPICE Import Job for Worker Screening.");

            foreach (var workerResponse in responses)
            {
                // search for the Personal History Record.
                MicrosoftDynamicsCRMcontact contact = _dynamicsClient.Contacts.Get(filter: $"adoxio_spdjobid eq {workerResponse.RecordIdentifier}").Value[0];
                string historyFilter = $"_adoxio_contactid_value eq {contact.Contactid}";
                MicrosoftDynamicsCRMadoxioPersonalhistorysummary record = _dynamicsClient.Personalhistorysummaries.Get(filter: historyFilter).Value[0];

                if (record != null)
                {
                    UpdateContactConsent(record._adoxioContactidValue);

                    // update the record.
                    MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchRecord = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary()
                    {
                        AdoxioSecuritystatus = WorkerSecurityScreeningResultTranslate.GetTranslatedSecurityStatus(workerResponse.Result),
                        AdoxioCompletedon = DateTimeOffset.Now
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

            hangfireContext.WriteLine("Finished SPICE Import Job for Worker Screening.");
            _logger.LogError("Finished SPICE Import Job for Worker Screening.");
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        public void ReceiveApplicationImportJob(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            _logger.LogError("Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                string appFilter = $"adoxio_jobnumber eq '{applicationResponse.RecordIdentifier}'";
                string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
                MicrosoftDynamicsCRMadoxioApplication applicationRecord = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value[0];

                if (applicationRecord != null)
                {
                    var screeningRequest = CreateApplicationScreeningRequest(applicationRecord);
                    var associatesValidated = UpdateConsentExpiry(screeningRequest.Associates);
                    _logger.LogInformation($"Total associates consent expiry updated: {associatesValidated}");

                    // update the date of security status received and the status
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioDatereceivedspd = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus(applicationResponse.Result)
                    };

                    try
                    {
                        if(patchRecord.AdoxioChecklistsecurityclearancestatus != null)
                        {
                            _dynamicsClient.Applications.Update(applicationRecord.AdoxioApplicationid, patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"Error updating application - received an invalid status of {applicationResponse.Result}");
                            _logger.LogError($"Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
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

            hangfireContext.WriteLine("Finished SPICE Import Job for Application Screening.");
            _logger.LogError("Finished SPICE Import Job for Application Screening..");
        }

        /// <summary>
        /// Generate an application screening request
        /// </summary>
        /// <returns></returns>
        public IncompleteApplicationScreening GenerateApplicationScreeningRequest(Guid applicationId)
        {
            string appFilter = "adoxio_applicationid eq " + applicationId;
            string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
            var applications = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand);
       
            var application = applications.Value[0];
            var screeningRequest = CreateApplicationScreeningRequest(application);
            return screeningRequest;
        }

        public MicrosoftDynamicsCRMadoxioWorker GetWorker(Guid workerId)
        {
            try
            {
                return _dynamicsClient.Workers.Get(filter: $"adoxio_workerid eq {workerId.ToString()}").Value[0];
            }
            catch
            {
                _logger.LogError($"Unable to find worker {workerId.ToString()}");
                return null;
            }
        }

        /// <summary>
        /// Sends the application screening request to spice.
        /// </summary>
        /// <returns>The application screening request success boolean.</returns>
        /// <param name="applicationRequest">Application request.</param>
        public async Task<bool> SendApplicationScreeningRequest(Guid applicationId, IncompleteApplicationScreening applicationRequest)
        {
            var consentValidated = Validation.ValidateAssociateConsent(_dynamicsClient, (List<LegalEntity>)applicationRequest.Associates);

            if (consentValidated)
            {
                List<IncompleteApplicationScreening> payload = new List<IncompleteApplicationScreening>
                {
                    applicationRequest
                };

                _logger.LogInformation($"Sending Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");
                _logger.LogInformation($"Application has {applicationRequest.Associates.Count} associates");

                try
                {
                    var result = await SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload);

                    _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
                    _logger.LogInformation($"Done Send Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

                    if (result.Response.StatusCode.ToString() == "OK")
                    {
                        MicrosoftDynamicsCRMadoxioApplication update = new MicrosoftDynamicsCRMadoxioApplication()
                        {
                            AdoxioSecurityclearancegenerateddate = DateTimeOffset.Now,
                            AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("REQUEST SENT")
                        };
                        _dynamicsClient.Applications.Update(applicationId.ToString(), update);
                        return true;
                    }
                    var msg = await result.Response.Content.ReadAsStringAsync();
                    throw new SystemException(msg);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return false;
                }
            }

            _logger.LogError("Consent not valid for all associates.");
            _dynamicsClient.Applications.Update(applicationId.ToString(), new MicrosoftDynamicsCRMadoxioApplication()
            {
                AdoxioSecurityclearancegenerateddate = DateTimeOffset.Now,
                AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("CONSENT NOT VALIDATED")
            });
            return false;
        }

        /// <summary>
        /// Sends the worker screening request to spice.
        /// </summary>
        /// <returns>The worker screening request success boolean.</returns>
        /// <param name="workerScreeningRequest">Worker screening request.</param>
        public async Task<bool> SendWorkerScreeningRequest(IncompleteWorkerScreening workerScreeningRequest)
        {
            WorkersGetResponseModel workerResponse = _dynamicsClient.Workers.Get(filter: "adoxio_workerid eq " + workerScreeningRequest.RecordIdentifier);
            if(workerResponse.Value.Count > 0)
            {
                List<IncompleteWorkerScreening> payload = new List<IncompleteWorkerScreening>
                {
                    workerScreeningRequest
                };

                _logger.LogInformation($"Sending Worker Screening Request");

                var result = await SpiceClient.ReceiveWorkerScreeningsWithHttpMessagesAsync(payload);

                _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
                _logger.LogInformation($"Done Send Worker Screening Request");

                return result.Response.StatusCode.ToString() == "OK";
            }
            _logger.LogError($"Worker {workerScreeningRequest.RecordIdentifier} not found");
            return false;
        }

        public async Task<IncompleteWorkerScreening> GenerateWorkerScreeningRequest(Guid workerId)
        {
            string filter = $"adoxio_workerid eq {workerId}";
            var fields = new List<string> { "adoxio_ContactId" };
            MicrosoftDynamicsCRMadoxioWorker worker = _dynamicsClient.Workers.Get(filter: filter, expand: fields).Value[0];
            /* Create request */
            IncompleteWorkerScreening request = new IncompleteWorkerScreening();

            /* Add applicant details */
            if (worker.AdoxioContactId != null)
            {
                request.RecordIdentifier = worker.AdoxioWorkerid;
                request.Contact = new Contact()
                {
                    BcIdCardNumber = worker.AdoxioContactId.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard ? worker.AdoxioContactId.AdoxioPrimaryidnumber : null,
                    DriversLicenceNumber = worker.AdoxioContactId.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence ? worker.AdoxioContactId.AdoxioPrimaryidnumber : null,
                    SpdJobId = worker.AdoxioContactId.AdoxioSpdjobid.ToString(),
                    ContactId = worker.AdoxioContactId.Contactid,
                    FirstName = worker.AdoxioContactId.Firstname,
                    LastName = worker.AdoxioContactId.Lastname,
                    MiddleName = worker.AdoxioContactId.Middlename,
                    Email = worker.AdoxioContactId.Emailaddress1,
                    PhoneNumber = worker.AdoxioContactId.Telephone1 != null ? worker.AdoxioContactId.Telephone1 : worker.AdoxioContactId.Mobilephone,
                    BirthDate = worker.AdoxioContactId.Birthdate,
                    SelfDisclosure = worker.AdoxioContactId.AdoxioSelfdisclosure != null ? ((GeneralYesNo)worker.AdoxioContactId.AdoxioSelfdisclosure).ToString() : null,
                    Gender = worker.AdoxioContactId.AdoxioGendercode != null ? ((AdoxioGenderCode)worker.AdoxioContactId.AdoxioGendercode).ToString() : null,
                    Birthplace = worker.AdoxioBirthplace,
                    Address = new Address()
                    {
                        AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                        AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                        AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                        City = worker.AdoxioContactId.Address1City,
                        StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                        Postal = (Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                        Country = worker.AdoxioContactId.Address1Country
                    }
                };

                request.Address = new Address()
                {
                    AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                    AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                    AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                    City = worker.AdoxioContactId.Address1City,
                    StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                    Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                    Country = worker.AdoxioContactId.Address1Country
                };
            }

            if (worker.AdoxioWorkerAliases != null)
            {
                request.Aliases = new List<Alias>();
                foreach (var alias in worker.AdoxioWorkerAliases)
                {
                    Alias newAlias = new Alias()
                    {
                        GivenName = alias.AdoxioLastname,
                        Surname = alias.AdoxioFirstname,
                        SecondName = alias.AdoxioMiddlename,  
                    };
                    request.Aliases.Add(newAlias);
                }
            }

            if (worker.AdoxioWorkerPreviousaddresses != null)
            {
                request.PreviousAddresses = new List<Address>();
                foreach (var address in worker.AdoxioWorkerPreviousaddresses)
                {
                    Address newAddress = new Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = address.AdoxioPostalcode,
                        Country = address.AdoxioCountry,
                        ToDate = address.AdoxioTodate,
                        FromDate = address.AdoxioFromdate
                    };
                    request.PreviousAddresses.Add(newAddress);
                }
            }

            _logger.LogInformation("Finished building Model");
            return request;
        }

        private IncompleteApplicationScreening CreateApplicationScreeningRequest(MicrosoftDynamicsCRMadoxioApplication application)
        {
            MicrosoftDynamicsCRMadoxioLicencetype licenceType = _dynamicsClient.Licencetypes.Get(filter: $"adoxio_licencetypeid eq {application._adoxioLicencetypeValue}").Value[0];
            var screeningRequest = new IncompleteApplicationScreening()
            {
                Name = application.AdoxioName,
                ApplicationType = licenceType.AdoxioName,
                RecordIdentifier = application.AdoxioJobnumber,
                UrgentPriority = false,
                ApplicantType = SpiceApplicantType.Cannabis,
                DateSent = DateTimeOffset.Now,
                BusinessNumber = application.AdoxioApplicant.Accountnumber,
                ApplicantName = application.AdoxioNameofapplicant,
                BusinessAddress = new Address()
                {
                    AddressStreet1 = application.AdoxioApplicant.Address1Line1,
                    City = application.AdoxioApplicant.Address1City,
                    StateProvince = application.AdoxioApplicant.Address1Stateorprovince,
                    Postal = (Validation.ValidatePostalCode(application.AdoxioApplicant.Address1Postalcode)) ? application.AdoxioApplicant.Address1Postalcode : null,
                    Country = application.AdoxioApplicant.Address1Country
                },
                ContactPerson = new Contact()
                {
                    ContactId = application.AdoxioApplicant._primarycontactidValue,
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
                screeningRequest.ApplyingPerson = new Contact()
                {
                    SpdJobId = application.AdoxioApplyingPerson.AdoxioSpdjobid.ToString(),
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
                BusinessType businessType = (BusinessType)application.AdoxioApplicant.AdoxioBusinesstype;
                screeningRequest.ApplicantAccount = new Account()
                {
                    AccountId = application.AdoxioApplicant.Accountid,
                    Name = application.AdoxioApplicant.Name,
                    BcIncorporationNumber = application.AdoxioApplicant.AdoxioBcincorporationnumber,
                    BusinessType = businessType.ToString()
                };
            }

            /* Add establishment */
            if (application.AdoxioEstablishment != null)
            {
                screeningRequest.Establishment = new Establishment()
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
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.AdoxioEstablishmentaddresspostalcode)) ? application.AdoxioEstablishmentaddresspostalcode : null,
                        Country = "Canada"
                    }
                };
            }

            /* Add key personnel and deemed associates */
            screeningRequest.Associates = new List<LegalEntity>();
            string keypersonnelfilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_iskeypersonnel eq true and adoxio_isindividual eq 1)";
            string deemedassociatefilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_isdeemedassociate eq true and adoxio_isindividual eq 1)";
            string[] expand = { "adoxio_Contact" };
            var associates = _dynamicsClient.Legalentities.Get(filter: keypersonnelfilter + " or " + deemedassociatefilter, expand: expand).Value;
            if (associates != null)
            {
                foreach (var legalEntity in associates)
                {
                    LegalEntity person = CreateAssociate(legalEntity);
                    screeningRequest.Associates.Add(person);
                }
            }

            /* If sole prop add contact person as associate */
            if (application.AdoxioApplicanttype == (int)BusinessType.SoleProprietorship)
            {
                screeningRequest.Associates.Add(new LegalEntity()
                {
                    EntityId = screeningRequest.ContactPerson.ContactId,
                    IsIndividual = true,
                    Positions = new List<string> { "owner" },
                    Contact = screeningRequest.ContactPerson,
                    PreviousAddresses = new List<Address>(),
                    Aliases = new List<Alias>()
                });
            }

            /* Add associates from account */
            var moreAssociates = CreateApplicationAssociatesScreeningRequest(application._adoxioApplicantValue, screeningRequest.Associates);
            screeningRequest.Associates = screeningRequest.Associates.Concat(moreAssociates).ToList();

            return screeningRequest;
        }

        private List<LegalEntity> CreateApplicationAssociatesScreeningRequest(string accountId, IList<LegalEntity> foundAssociates)
        {
            List<LegalEntity> newAssociates = new List<LegalEntity>();
            string applicationfilter = "_adoxio_account_value eq " + accountId + " and _adoxio_profilename_value ne " + accountId;
            foreach (var assoc in foundAssociates)
            {
                if (accountId != assoc.EntityId && assoc.Contact != null)
                {
                    applicationfilter += " and _adoxio_contact_value ne " + assoc.Contact.ContactId;
                }
            }
            string[] expand = { "adoxio_Contact", "adoxio_Account"};

            var legalEntities = _dynamicsClient.Legalentities.Get(filter: applicationfilter, expand: expand).Value;
            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    LegalEntity associate = CreateAssociate(legalEntity);
                    newAssociates.Add(associate);
                }
            }
            var newFoundAssociates = new List<LegalEntity>(foundAssociates);
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

        private LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            LegalEntity associate = new LegalEntity()
            {
                EntityId = legalEntity.AdoxioLegalentityid,
                Name = legalEntity.AdoxioName,
                Title = legalEntity.AdoxioJobtitle,
                Positions = GetLegalEntityPositions(legalEntity),
                PreviousAddresses = new List<Address>(),
                Aliases = new List<Alias>()
            };
            
            if (legalEntity.AdoxioIsindividual != null && legalEntity.AdoxioIsindividual == 1 && legalEntity.AdoxioContact != null)
            {
                associate.IsIndividual = true;
                associate.TiedHouse = legalEntity.AdoxioContact.AdoxioSelfdeclaredtiedhouse == 1;
                associate.Contact = new Contact()
                {
                    SpdJobId = legalEntity.AdoxioContact.AdoxioSpdjobid.ToString(),
                    ContactId = legalEntity.AdoxioContact.Contactid,
                    FirstName = legalEntity.AdoxioContact.Firstname,
                    LastName = legalEntity.AdoxioContact.Lastname,
                    MiddleName = legalEntity.AdoxioContact.Middlename,
                    Email = legalEntity.AdoxioContact.Emailaddress1,
                    PhoneNumber = legalEntity.AdoxioContact.Telephone1 != null ? legalEntity.AdoxioContact.Telephone1 : legalEntity.AdoxioContact.Mobilephone,
                    SelfDisclosure = (legalEntity.AdoxioContact.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)legalEntity.AdoxioContact.AdoxioSelfdisclosure).ToString(),
                    Gender = (legalEntity.AdoxioContact.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)legalEntity.AdoxioContact.AdoxioGendercode).ToString(),
                    Birthplace = legalEntity.AdoxioContact.AdoxioBirthplace,
                    BirthDate = legalEntity.AdoxioContact.Birthdate,
                    BcIdCardNumber = legalEntity.AdoxioContact.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard ? legalEntity.AdoxioContact.AdoxioPrimaryidnumber : null,
                    DriversLicenceNumber = legalEntity.AdoxioContact.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence ? legalEntity.AdoxioContact.AdoxioPrimaryidnumber : null,
                    Address = new Address()
                    {
                        AddressStreet1 = legalEntity.AdoxioContact.Address1Line1,
                        AddressStreet2 = legalEntity.AdoxioContact.Address1Line2,
                        AddressStreet3 = legalEntity.AdoxioContact.Address1Line3,
                        City = legalEntity.AdoxioContact.Address1City,
                        StateProvince = legalEntity.AdoxioContact.Address1Stateorprovince,
                        Postal = (Validation.ValidatePostalCode(legalEntity.AdoxioContact.Address1Postalcode)) ? legalEntity.AdoxioContact.Address1Postalcode : null,
                        Country = legalEntity.AdoxioContact.Address1Country
                    }
                };

                /* Add previous addresses */
                var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + legalEntity.AdoxioContact.Contactid).Value;
                foreach (var address in previousAddresses)
                {
                    var newAddress = new Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = (Validation.ValidatePostalCode(address.AdoxioPostalcode)) ? address.AdoxioPostalcode : null,
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
                    associate.Aliases.Add(new Alias()
                    {
                        GivenName = alias.AdoxioFirstname,
                        Surname = alias.AdoxioLastname,
                        SecondName = alias.AdoxioMiddlename
                    });
                }
            }
            else
            {
                // Attach the account
                if (legalEntity._adoxioShareholderaccountidValue != null)
                {
                    var account = _dynamicsClient.Accounts.Get(filter: "accountid eq " + legalEntity._adoxioShareholderaccountidValue).Value;
                    associate.Account = new Account()
                    {
                        AccountId = account[0].Accountid,
                        Name = account[0].Name,
                        BcIncorporationNumber = account[0].AdoxioBcincorporationnumber,
                        BusinessNumber = account[0].Accountnumber,
                        Associates = new List<LegalEntity>()
                    };
                }
                else if (legalEntity.AdoxioAccount != null)
                {
                    associate.Account = new Account()
                    {
                        AccountId = legalEntity.AdoxioAccount.Accountid,
                        Name = legalEntity.AdoxioAccount.Name,
                        BcIncorporationNumber = legalEntity.AdoxioAccount.AdoxioBcincorporationnumber,
                        BusinessNumber = legalEntity.AdoxioAccount.Accountnumber,
                        Associates = new List<LegalEntity>()
                    };
                }
                else
                {
                    _logger.LogError("Failed to find a shareholder account found");
                    associate.Account = new Account();
                }
                associate.IsIndividual = false;
            }
            return associate;
        }
        public List<string> GetLegalEntityPositions(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            List<string> positions = new List<string>();
            if (legalEntity.AdoxioIsdirector != null && (bool)legalEntity.AdoxioIsdirector)
            {
                positions.Add("director");
            }
            if (legalEntity.AdoxioIsofficer != null && (bool)legalEntity.AdoxioIsofficer)
            {
                positions.Add("officer");
            }
            if (legalEntity.AdoxioIsseniormanagement != null && (bool)legalEntity.AdoxioIsseniormanagement)
            {
                positions.Add("senior manager");
            }
            if (legalEntity.AdoxioIskeypersonnel != null && (bool)legalEntity.AdoxioIskeypersonnel)
            {
                positions.Add("key personnel");
            }
            if (legalEntity.AdoxioIsshareholder != null && (bool)legalEntity.AdoxioIsshareholder)
            {
                positions.Add("shareholder");
            }
            if (legalEntity.AdoxioIsowner != null && (bool)legalEntity.AdoxioIsowner)
            {
                positions.Add("owner");
            }
            if (legalEntity.AdoxioIstrustee != null && (bool)legalEntity.AdoxioIstrustee)
            {
                positions.Add("trustee");
            }
            if (legalEntity.AdoxioIsdeemedassociate !=null && (bool)legalEntity.AdoxioIsdeemedassociate)
            {
                positions.Add("deemed associate");
            }
            if (legalEntity.AdoxioIspartner != null && (bool)legalEntity.AdoxioIspartner)
            {
                positions.Add("partner");
            }
            return positions;
        }

        public int UpdateConsentExpiry(IList<LegalEntity> associates)
        {
            var i = 0;
            foreach(var associate in associates)
            {
                _logger.LogError(associate.Name);
                if((bool) associate.IsIndividual)
                {
                    UpdateContactConsent(associate.Contact.ContactId);
                    i += 1;
                }
                else
                {
                    i += UpdateConsentExpiry(associate.Account.Associates);
                }
            }
            return i;
        }

        public void UpdateContactConsent(string ContactId)
        {
            // update consent validated to yes
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact()
            {
                AdoxioConsentvalidated = 845280000,
                AdoxioConsentvalidatedexpirydate = DateTimeOffset.Now.AddMonths(3)
            };
            _dynamicsClient.Contacts.Update(ContactId, contact);
        }

        public async Task SendFoundWorkers(PerformContext hangfireContext)
        {
            _logger.LogError("Starting SendFoundWorkers Job");
            hangfireContext.WriteLine("Starting SendFoundWorkers Job");

            // Query Dynamics for worker data
            string[] expand = { "adoxio_ContactId", "adoxio_worker_aliases", "adoxio_worker_previousaddresses" };
            string sendFilter = $"adoxio_consentvalidated eq {(int)WorkerConsentValidated.Yes} and adoxio_spdexportdate eq null";
            WorkersGetResponseModel workers = _dynamicsClient.Workers.Get(filter: sendFilter, expand: expand);
            
            if (workers.Value.Count < 1)
            {
                _logger.LogError("No workers found for processing");
                hangfireContext.WriteLine("No workers found for processing");
            }
            else
            {
                _logger.LogError($"Found {workers.Value.Count} workers to send to SPD.");
                hangfireContext.WriteLine($"Found {workers.Value.Count} workers to send to SPD.");

                foreach (var worker in workers.Value)
                {
                    IncompleteWorkerScreening screeningRequest = await GenerateWorkerScreeningRequest(Guid.Parse(worker.AdoxioWorkerid));
                    var response = await SendWorkerScreeningRequest(screeningRequest);
                    if (response)
                    {
                        hangfireContext.WriteLine($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        _logger.LogError($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                    }
                    else
                    {
                        hangfireContext.WriteLine($"Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                        _logger.LogError($"Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                    }
                }
            }
            
            _logger.LogError("End of SendFoundWorkers Job");
            hangfireContext.WriteLine("End of SendFoundWorkers Job");
        }

        public async Task SendFoundApplications(PerformContext hangfireContext)
        {
            _logger.LogError("Starting SendFoundApplications Job");
            hangfireContext.WriteLine("Starting SendFoundApplications Job");

            string sendFilter = "adoxio_checklistsenttospd eq 1 and adoxio_checklistsecurityclearancestatus eq " + ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("REQUEST NOT SENT");
            var applications = _dynamicsClient.Applications.Get(filter: sendFilter).Value;
            _logger.LogError($"Found {applications.Count} applications to send to SPD.");
            hangfireContext.WriteLine($"Found {applications.Count} applications to send to SPD.");

            foreach (var application in applications)
            {
                Guid.TryParse(application.AdoxioApplicationid, out Guid applicationId);

                var screeningRequest = GenerateApplicationScreeningRequest(applicationId);
                var response = await SendApplicationScreeningRequest(applicationId, screeningRequest);
                if (response)
                {
                    hangfireContext.WriteLine($"Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                    _logger.LogError($"Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                }
                else
                {
                    hangfireContext.WriteLine($"Failed to send application {screeningRequest.RecordIdentifier} to SPD");
                    _logger.LogError($"Failed to send application {screeningRequest.RecordIdentifier} to SPD");
                }
            }
            
            _logger.LogError("End of SendFoundApplications Job");
            hangfireContext.WriteLine("End of SendFoundApplications Job");
        }
    }
}
