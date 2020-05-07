using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Hangfire;
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
            _dynamicsClient = DynamicsSetupUtil.SetupDynamics(Configuration);
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
                try
                {
                    string contactId;
                    // 3 different ways to send an identifier 😷
                    if (workerResponse.RecordIdentifier == null)
                    {
                        MicrosoftDynamicsCRMcontact contact = _dynamicsClient.Contacts.Get(filter: $"adoxio_spdjobid eq {workerResponse.SpdJobId}").Value[0];
                        contactId = contact.Contactid;
                    }
                    else if(workerResponse.RecordIdentifier.Substring(0,2) == "WR")
                    {
                        // Check if using old WR record
                        MicrosoftDynamicsCRMadoxioPersonalhistorysummary history = _dynamicsClient.Personalhistorysummaries.Get(filter: $"adoxio_workerjobnumber eq '{workerResponse.RecordIdentifier}'").Value[0];
                        contactId = history._adoxioContactidValue;
                    }
                    else
                    {
                        contactId = workerResponse.RecordIdentifier;
                    }
                    
                    string filter = $"_adoxio_contactid_value eq {contactId}";
                    MicrosoftDynamicsCRMadoxioWorker worker = _dynamicsClient.Workers.Get(filter: filter).Value.FirstOrDefault();

                    if (worker != null)
                    {
                        // update the record.
                        MicrosoftDynamicsCRMadoxioWorker patchRecord = new MicrosoftDynamicsCRMadoxioWorker()
                        {
                            Statuscode = workerResponse.ScreeningResult switch
                            {
                                WorkerSecurityStatus.Pass => (int)WorkerSecurityStatusCode.Active,
                                WorkerSecurityStatus.Fail => (int)WorkerSecurityStatusCode.Rejected,
                                WorkerSecurityStatus.Withdrawn => (int)WorkerSecurityStatusCode.Withdrawn
                            },
                            AdoxioSecuritystatus = (int)workerResponse.ScreeningResult,
                            AdoxioSecuritycompletedon = DateTimeOffset.Now
                        };

                        // Do passed worker things
                        if (workerResponse.ScreeningResult == WorkerSecurityStatus.Pass)
                        {
                            patchRecord.AdoxioExpirydate = DateTimeOffset.Now.AddYears(2);
                        }

                        _dynamicsClient.Workers.Update(worker.AdoxioWorkerid, patchRecord);
                    }
                    else {
                        _logger.LogError($"Worker not found for spd job id: {workerResponse.RecordIdentifier}");
                        hangfireContext.WriteLine($"Worker not found for spd job id: {workerResponse.RecordIdentifier}");
                    }
                }
                catch (HttpOperationException odee)
                {
                    hangfireContext.WriteLine("Error updating worker security status");
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

            hangfireContext.WriteLine("Finished SPICE Import Job for Worker Screening.");
            _logger.LogError("Finished SPICE Import Job for Worker Screening.");
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveApplicationImportJob(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            _logger.LogError("Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                string appFilter = $"adoxio_jobnumber eq '{applicationResponse.RecordIdentifier}'";
                string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
                MicrosoftDynamicsCRMadoxioApplication application = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value.FirstOrDefault();

                if (application != null)
                {
                    var screeningRequest = await CreateApplicationScreeningRequest(application);
                    if (screeningRequest == null)
                    {
                        continue;
                    }
                    var associatesValidated = UpdateConsentExpiry(screeningRequest.Associates);
                    _logger.LogInformation($"Total associates consent expiry updated: {associatesValidated}");

                    // update the date of security status received and the status
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioDatereceivedspd = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = (int?)applicationResponse.Result
                    };

                    try
                    {
                        if(patchRecord.AdoxioChecklistsecurityclearancestatus != null)
                        {
                            _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"Error updating application - received an invalid status of {applicationResponse.Result}");
                            _logger.LogError($"Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (HttpOperationException odee)
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
        public async Task<IncompleteApplicationScreening> GenerateApplicationScreeningRequest(Guid applicationId)
        {
            string appFilter = "adoxio_applicationid eq " + applicationId;
            string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact", "owninguser" };
            var applications = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand);
       
            var application = applications.Value[0];
            return await CreateApplicationScreeningRequest(application);
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
        public bool SendApplicationScreeningRequest(Guid applicationId, IncompleteApplicationScreening applicationRequest)
        {
            var consentValidated = Validation.ValidateAssociateConsent(_dynamicsClient, (List<LegalEntity>)applicationRequest.Associates, _logger);

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
                    MicrosoftDynamicsCRMadoxioApplication update = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioChecklistsecurityclearancestatus = (int?)ApplicationSecurityStatus.Sending
                    };
                    _dynamicsClient.Applications.Update(applicationId.ToString(), update);

                    var result = SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload).GetAwaiter().GetResult();

                    _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
                    _logger.LogInformation($"Done Send Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

                    if (result.Response.StatusCode.ToString() == "OK")
                    {
                        update = new MicrosoftDynamicsCRMadoxioApplication()
                        {
                            AdoxioSecurityclearancegenerateddate = DateTimeOffset.UtcNow,
                            AdoxioChecklistsecurityclearancestatus = (int?)ApplicationSecurityStatus.Sent
                        };
                        _dynamicsClient.Applications.Update(applicationId.ToString(), update);
                        return true;
                    }
                    var msg = result.Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
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
                AdoxioChecklistsecurityclearancestatus = (int?)ApplicationSecurityStatus.Incomplete
            });
            return false;
        }

        /// <summary>
        /// Sends the worker screening request to spice.
        /// </summary>
        /// <returns>The worker screening request success boolean.</returns>
        /// <param name="workerScreeningRequest">Worker screening request.</param>
        public bool SendWorkerScreeningRequest(IncompleteWorkerScreening workerScreeningRequest)
        {
            List<IncompleteWorkerScreening> payload = new List<IncompleteWorkerScreening>
            {
                workerScreeningRequest
            };

            _logger.LogInformation($"Sending Worker Screening Request");

            var result = SpiceClient.ReceiveWorkerScreeningsWithHttpMessages(payload);

            _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
            _logger.LogInformation($"Done Send Worker Screening Request");

            return result.Response.StatusCode.ToString() == "OK";
        }

        public IncompleteWorkerScreening GenerateWorkerScreeningRequest(Guid workerId)
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
                    SpdJobId = worker.AdoxioContactId.AdoxioSpdjobid.ToString(),
                    ContactId = worker.AdoxioContactId.Contactid,
                    FirstName = worker.AdoxioContactId.Firstname,
                    LastName = worker.AdoxioContactId.Lastname,
                    MiddleName = worker.AdoxioContactId.Middlename,
                    Email = worker.AdoxioContactId.Emailaddress1,
                    PhoneNumber = worker.AdoxioContactId.Telephone1 ?? worker.AdoxioContactId.Mobilephone,
                    BirthDate = worker.AdoxioContactId.Birthdate,
                    SelfDisclosure = worker.AdoxioContactId.AdoxioSelfdisclosure != null ? ((GeneralYesNo)worker.AdoxioContactId.AdoxioSelfdisclosure).ToString() : null,
                    Gender = worker.AdoxioContactId.AdoxioGendercode != null ? ((AdoxioGenderCode)worker.AdoxioContactId.AdoxioGendercode).ToString() : null,
                    Birthplace = worker.AdoxioContactId.AdoxioBirthplace,
                    Address = new Address()
                    {
                        AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                        AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                        AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                        City = worker.AdoxioContactId.Address1City,
                        StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                        Postal = (Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                        Country = worker.AdoxioContactId.Address1Country
                    },
                    Aliases = new List<Alias>(),
                    PreviousAddresses = new List<Address>()
                };

                if (worker.AdoxioContactId.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard)
                {
                    request.Contact.BcIdCardNumber = worker.AdoxioContactId.AdoxioPrimaryidnumber;
                } else if (worker.AdoxioContactId.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence)
                {
                    request.Contact.DriversLicenceNumber = worker.AdoxioContactId.AdoxioPrimaryidnumber;
                }

                if (worker.AdoxioContactId.AdoxioSecondaryidentificationtype == (int)SecondaryIdentificationType.BCIDCard)
                {
                    request.Contact.BcIdCardNumber = worker.AdoxioContactId.AdoxioSecondaryidnumber;
                } else if(worker.AdoxioContactId.AdoxioSecondaryidentificationtype == (int)SecondaryIdentificationType.DriversLicence)
                {
                    request.Contact.DriversLicenceNumber = worker.AdoxioContactId.AdoxioSecondaryidnumber;
                }

                var aliases = _dynamicsClient.Aliases.Get(filter: "_adoxio_contactid_value eq " + worker.AdoxioContactId.Contactid).Value;
                foreach (var alias in aliases)
                {
                    request.Contact.Aliases.Add(new Alias()
                    {
                        GivenName = alias.AdoxioFirstname,
                        Surname = alias.AdoxioLastname,
                        SecondName = alias.AdoxioMiddlename
                    });
                }

                var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + worker.AdoxioContactId.Contactid).Value;
                foreach (var address in previousAddresses)
                {
                    request.Contact.PreviousAddresses.Add(new Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = address.AdoxioPostalcode,
                        Country = address.AdoxioCountry,
                        ToDate = address.AdoxioTodate,
                        FromDate = address.AdoxioFromdate
                    });
                }
            }

            _logger.LogInformation("Finished building Model");
            return request;
        }

        private async Task<IncompleteApplicationScreening> CreateApplicationScreeningRequest(MicrosoftDynamicsCRMadoxioApplication application)
        {
            try
            {
                MicrosoftDynamicsCRMadoxioLicencetype licenceType = _dynamicsClient.Licencetypes.Get(filter: $"adoxio_licencetypeid eq {application._adoxioLicencetypeValue}").Value[0];
                var screeningRequest = new IncompleteApplicationScreening()
                {
                    Name = application.AdoxioName,
                    ApplicationType = licenceType.AdoxioName == null ? "Cannabis Retail Store" : licenceType.AdoxioName,
                    RecordIdentifier = application.AdoxioJobnumber,
                    UrgentPriority = false,
                    Associates = new List<LegalEntity>(),
                    ApplicantType = SpiceApplicantType.Cannabis,
                    DateSent = DateTimeOffset.Now,
                    BusinessNumber = application.AdoxioApplicant?.Accountnumber,
                    ApplicantName = application.AdoxioNameofapplicant,
                    BusinessAddress = new Address()
                    {
                        AddressStreet1 = application.AdoxioApplicant?.Address1Line1,
                        City = application.AdoxioApplicant?.Address1City,
                        StateProvince = application.AdoxioApplicant?.Address1Stateorprovince,
                        Postal = (Validation.ValidatePostalCode(application.AdoxioApplicant?.Address1Postalcode)) ? application.AdoxioApplicant.Address1Postalcode : null,
                        Country = application.AdoxioApplicant?.Address1Country
                    },
                    ContactPerson = new Contact()
                    {
                        ContactId = application.AdoxioApplicant?._primarycontactidValue,
                        FirstName = application.AdoxioContactpersonfirstname,
                        LastName = application.AdoxioContactpersonlastname,
                        MiddleName = application.AdoxioContactmiddlename,
                        Email = application.AdoxioEmail,
                        PhoneNumber = application.AdoxioContactpersonphone
                    },
                    AssignedPerson = new Contact()
                    {
                        FirstName = application.Owninguser?.Firstname,
                        LastName = application.Owninguser?.Lastname
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
                        Email = application.AdoxioApplyingPerson.Emailaddress1, 
                    };
                }
                /* Add applicant details */
                if (application.AdoxioApplicant != null && application.AdoxioApplicant.AdoxioBusinesstype != null)
                {
                    BusinessType businessType = (BusinessType)application.AdoxioApplicant.AdoxioBusinesstype;
                    screeningRequest.ApplicantAccount = new Interfaces.Spice.Models.Account()
                    {
                        AccountId = application.AdoxioApplicant.Accountid,
                        Name = application.AdoxioApplicant.Name,
                        BcIncorporationNumber = application.AdoxioApplicant.AdoxioBcincorporationnumber,
                        BusinessType = businessType.ToString()
                    };

                    if (businessType == BusinessType.SoleProprietorship && application.AdoxioApplicant?._primarycontactidValue != null)
                    {
                        MicrosoftDynamicsCRMcontact owner = await _dynamicsClient.GetContactById(application.AdoxioApplicant._primarycontactidValue);
                        Contact contact = new Contact()
                        {
                            SpdJobId = owner.AdoxioSpdjobid.ToString(),
                            ContactId = owner.Contactid,
                            FirstName = owner.Firstname,
                            LastName = owner.Lastname,
                            MiddleName = owner.Middlename,
                            Email = owner.Emailaddress1,
                            PhoneNumber = owner.Telephone1 ?? owner.Mobilephone,
                            SelfDisclosure = (owner.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)owner.AdoxioSelfdisclosure).ToString(),
                            Gender = (owner.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)owner.AdoxioGendercode).ToString(),
                            Birthplace = owner.AdoxioBirthplace,
                            BirthDate = owner.Birthdate,
                            BcIdCardNumber = owner.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard ? owner.AdoxioPrimaryidnumber : null,
                            DriversLicenceNumber = owner.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence ? owner.AdoxioPrimaryidnumber : null,
                            DriverLicenceJurisdiction = owner.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence && owner.AdoxioIdentificationjurisdiction != null ? ((IdentificationJurisdiction)owner.AdoxioIdentificationjurisdiction).ToString() : null,
                            Address = new Address()
                            {
                                AddressStreet1 = owner.Address1Line1,
                                AddressStreet2 = owner.Address1Line2,
                                AddressStreet3 = owner.Address1Line3,
                                City = owner.Address1City,
                                StateProvince = owner.Address1Stateorprovince,
                                Postal = (Validation.ValidatePostalCode(owner.Address1Postalcode)) ? owner.Address1Postalcode : null,
                                Country = owner.Address1Country
                            }
                        };
                        LegalEntity entity = new LegalEntity()
                        {
                            EntityId = owner.Contactid,
                            IsIndividual = true,
                            Positions = new List<string> { "owner" },
                            Contact = contact,
                            PreviousAddresses = new List<Address>(),
                            Aliases = new List<Alias>()
                        };
                        /* Add previous addresses */
                        var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + owner.Contactid).Value;
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
                            entity.PreviousAddresses.Add(newAddress);
                        }

                        /* Add aliases */
                        var aliases = _dynamicsClient.Aliases.Get(filter: "_adoxio_contactid_value eq " + owner.Contactid).Value;
                        foreach (var alias in aliases)
                        {
                            entity.Aliases.Add(new Alias()
                            {
                                GivenName = alias.AdoxioFirstname,
                                Surname = alias.AdoxioLastname,
                                SecondName = alias.AdoxioMiddlename
                            });
                        }
                        screeningRequest.Associates.Add(entity);
                    }
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

                /* Add key personnel and deemed associates from application */
                string keypersonnelfilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_iskeypersonnel eq true and adoxio_isindividual eq 1)";
                string deemedassociatefilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_isdeemedassociate eq true and adoxio_isindividual eq 1)";
                string[] expand = { "adoxio_Contact" };
                var associates = _dynamicsClient.Legalentities.Get(filter: keypersonnelfilter + " or " + deemedassociatefilter, expand: expand).Value;
                if (associates != null)
                {
                    foreach (var legalEntity in associates)
                    {
                        try
                        {
                            LegalEntity entity = CreateAssociate(legalEntity);
                            if((bool)entity.IsIndividual)
                            {
                                screeningRequest.Associates.Add(entity);
                            }
                            else
                            {
                                var accountAssociates = CreateAssociatesForAccount(entity.Account.AccountId);
                                screeningRequest.Associates = screeningRequest.Associates.Concat(accountAssociates).ToList();
                            }
                        }
                        catch (ArgumentNullException e)
                        {
                            _logger.LogError(e, $"Attempted to create null associate: {legalEntity.AdoxioLegalentityid}");
                        }
                    }
                }

                /* Add associates from account */
                var moreAssociates = CreateAssociatesForAccount(application._adoxioApplicantValue);
                screeningRequest.Associates = screeningRequest.Associates.Concat(moreAssociates).ToList();
                /* remove duplicate associates */
                List<string> contactIds = new List<string>{};
                int i = 0;
                List<LegalEntity> finalAssociates = new List<LegalEntity>();
                foreach(var assoc in screeningRequest.Associates)
                {
                    if(!contactIds.Contains(assoc.Contact.ContactId))
                    {
                        finalAssociates.Add(assoc);
                        contactIds.Add(assoc.Contact.ContactId);
                    }
                    i++;
                }
                screeningRequest.Associates = finalAssociates;
                return screeningRequest;
            }
            catch (HttpOperationException odee)
            {
                _logger.LogError("Error creating application screening request");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                return null;
            }
        }

        private List<LegalEntity> CreateAssociatesForAccount(string accountId)
        {
            List<LegalEntity> newAssociates = new List<LegalEntity>();
            if (string.IsNullOrEmpty(accountId))
            {
                _logger.LogError("CreateAssociatesForAccount received a null accountId");
                return newAssociates;
            }
            string entityFilter = "_adoxio_account_value eq " + accountId + " and _adoxio_profilename_value ne " + accountId;
            entityFilter += " and adoxio_isdonotsendtospd ne true";
            string[] expand = { "adoxio_Contact", "adoxio_Account"};

            var legalEntities = _dynamicsClient.Legalentities.Get(filter: entityFilter, expand: expand).Value;
            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    try
                    {
                        LegalEntity associate = CreateAssociate(legalEntity);
                        if((bool)associate.IsIndividual)
                        {
                            newAssociates.Add(associate);
                        }
                        else
                        {
                            var moreAssociates = CreateAssociatesForAccount(associate.Account.AccountId);
                            newAssociates.AddRange(moreAssociates);
                        }
                    }
                    catch (ArgumentNullException e)
                    {
                        _logger.LogError(e, $"Attempted to create null associate: {legalEntity.AdoxioLegalentityid}");
                    }
                }
            }
            return newAssociates;
        }

        private LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            if(legalEntity == null)
            {
                throw new ArgumentNullException();
            }
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
                    PhoneNumber = legalEntity.AdoxioContact.Telephone1 ?? legalEntity.AdoxioContact.Mobilephone,
                    SelfDisclosure = (legalEntity.AdoxioContact.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)legalEntity.AdoxioContact.AdoxioSelfdisclosure).ToString(),
                    Gender = (legalEntity.AdoxioContact.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)legalEntity.AdoxioContact.AdoxioGendercode).ToString(),
                    Birthplace = legalEntity.AdoxioContact.AdoxioBirthplace,
                    BirthDate = legalEntity.AdoxioContact.Birthdate,
                    BcIdCardNumber = legalEntity.AdoxioContact.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard ? legalEntity.AdoxioContact.AdoxioPrimaryidnumber : null,
                    DriversLicenceNumber = legalEntity.AdoxioContact.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence ? legalEntity.AdoxioContact.AdoxioPrimaryidnumber : null,
                    DriverLicenceJurisdiction = legalEntity.AdoxioContact.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence && legalEntity.AdoxioContact.AdoxioIdentificationjurisdiction != null ? ((IdentificationJurisdiction)legalEntity.AdoxioContact.AdoxioIdentificationjurisdiction).ToString() : null,
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
                    associate.Account = new Interfaces.Spice.Models.Account()
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
                    associate.Account = new Interfaces.Spice.Models.Account()
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
                    associate.Account = new Interfaces.Spice.Models.Account();
                }
                associate.IsIndividual = false;
            }
            return associate;
        }
        
        private List<string> GetLegalEntityPositions(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
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

        private int UpdateConsentExpiry(IList<LegalEntity> associates)
        {
            var i = 0;
            foreach(var associate in associates)
            {
                if((bool) associate.IsIndividual)
                {
                    // update consent validated to yes and expire it in 3 months
                    MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact()
                    {
                        AdoxioConsentvalidated = 845280000,
                        AdoxioConsentvalidatedexpirydate = DateTimeOffset.Now.AddMonths(3)
                    };
                    _dynamicsClient.Contacts.Update(associate.Contact.ContactId, contact);
                    i += 1;
                }
                else
                {
                    i += UpdateConsentExpiry(associate.Account.Associates);
                }
            }
            return i;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SendFoundWorkers(PerformContext hangfireContext)
        {
            _logger.LogError("Starting SendFoundWorkers Job");
            hangfireContext.WriteLine("Starting SendFoundWorkers Job");

            // Query Dynamics for worker data
            string[] expand = { "adoxio_ContactId", "adoxio_worker_aliases", "adoxio_worker_previousaddresses" };
            string sendFilter = $"adoxio_consentvalidated eq {(int)WorkerConsentValidated.Yes} and adoxio_exporteddate eq null and adoxio_paymentreceived eq 1";
            IList<MicrosoftDynamicsCRMadoxioWorker> workers = _dynamicsClient.Workers.Get(filter: sendFilter, expand: expand).Value;
            
            if (workers.Count < 1)
            {
                _logger.LogError("No workers found for processing");
                hangfireContext.WriteLine("No workers found for processing");
            }
            else
            {
                _logger.LogError($"Found {workers.Count} workers to send to SPD.");
                hangfireContext.WriteLine($"Found {workers.Count} workers to send to SPD.");

                foreach (var worker in workers)
                {
                    IncompleteWorkerScreening screeningRequest = GenerateWorkerScreeningRequest(Guid.Parse(worker.AdoxioWorkerid));
                    var reqSuccess = SendWorkerScreeningRequest(screeningRequest);
                    if (reqSuccess)
                    {
                        hangfireContext.WriteLine($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        _logger.LogError($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        MicrosoftDynamicsCRMadoxioWorker workerPatch = new MicrosoftDynamicsCRMadoxioWorker()
                        {
                            AdoxioExporteddate = DateTime.UtcNow
                        };
                        _dynamicsClient.Workers.Update(worker.AdoxioWorkerid, workerPatch);
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

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SendFoundApplications(PerformContext hangfireContext)
        {
            string[] select = {"adoxio_applicationtypeid"};
            IList<MicrosoftDynamicsCRMadoxioApplicationtype> selectedAppTypes = _dynamicsClient.Applicationtypes.Get(filter: "adoxio_requiressecurityscreening eq true", select: select).Value;
            if (selectedAppTypes.Count == 0)
            {
                _logger.LogError("Failed to Start SendFoundApplicationsJob: No application types are set to send to SPD.");
                hangfireContext.WriteLine("Failed to Start SendFoundApplicationsJob: No application types are set to send to SPD.");
                return;
            }

            List<string> appTypes = selectedAppTypes.Select(a => a.AdoxioApplicationtypeid).ToList();
            _logger.LogError($"Starting SendFoundApplications Job for {selectedAppTypes.Count} application types");
            hangfireContext.WriteLine($"Starting SendFoundApplications Job for {selectedAppTypes.Count} application types");

            string sendFilter = "adoxio_checklistsenttospd eq 1 and adoxio_checklistsecurityclearancestatus eq " + (int?)ApplicationSecurityStatus.NotSent;

            var applications = _dynamicsClient.Applications.Get(filter: sendFilter).Value.Where(a => appTypes.Contains(a._adoxioApplicationtypeidValue));
            _logger.LogError($"Found {applications.Count()} applications to send to SPD.");
            hangfireContext.WriteLine($"Found {applications.Count()} applications to send to SPD.");

            foreach (var application in applications)
            {
                Guid.TryParse(application.AdoxioApplicationid, out Guid applicationId);

                var screeningRequest = await GenerateApplicationScreeningRequest(applicationId);
                var response = SendApplicationScreeningRequest(applicationId, screeningRequest);
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
