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
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;
using Contact = Gov.Lclb.Cllb.Interfaces.Contact;
using System.Data;
using System.Xml.Linq;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class SpiceUtils
    {

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamicsClient;
        public ISpiceClient SpiceClient;

        public SpiceUtils(IConfiguration configuration)
        {
            this.Configuration = configuration;
            _dynamicsClient = DynamicsSetupUtil.SetupDynamics(configuration);
            SpiceClient = new SpiceClient(new HttpClient(), configuration);
        }

        public void ReceiveWorkerImportJob(PerformContext hangfireContext, List<CompletedWorkerScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Worker Screening.");
            Log.Logger.Error("Starting SPICE Import Job for Worker Screening.");

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
                    else if (workerResponse.RecordIdentifier.Substring(0, 2) == "WR")
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
                    else
                    {
                        Log.Logger.Error($"Worker not found for spd job id: {workerResponse.RecordIdentifier}");
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

                    Log.Logger.Error(odee, "Error updating worker personal history");

                }
            }

            hangfireContext.WriteLine("Finished SPICE Import Job for Worker Screening.");
            Log.Logger.Error("Finished SPICE Import Job for Worker Screening.");
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveApplicationImportJob(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            Log.Logger.Error("Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                string appFilter = $"adoxio_jobnumber eq '{applicationResponse.RecordIdentifier}'";
                string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact", "adoxio_ApplicationTypeId" };
                MicrosoftDynamicsCRMadoxioApplication application = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value.FirstOrDefault();

                if (application != null)
                {
                    var screeningRequest = await CreateApplicationScreeningRequestV2(application);
                    if (screeningRequest == null)
                    {
                        continue;
                    }
                    var associatesValidated = UpdateConsentExpiry(screeningRequest.Associates);
                    Log.Logger.Information($"Total associates consent expiry updated: {associatesValidated}");

                    // update the date of security status received and the status
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioDatereceivedspd = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = (int?)applicationResponse.Result
                    };

                    try
                    {
                        if (patchRecord.AdoxioChecklistsecurityclearancestatus != null)
                        {
                            _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"Error updating application - received an invalid status of {applicationResponse.Result}");
                            Log.Logger.Error($"Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (HttpOperationException odee)
                    {
                        hangfireContext.WriteLine("Error updating application");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        Log.Logger.Error(odee, "Error updating application");
                    }
                }
            }

            hangfireContext.WriteLine("Finished SPICE Import Job for Application Screening.");
            Log.Logger.Error("Finished SPICE Import Job for Application Screening..");
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveApplicationImportJobV2(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            Log.Logger.Error("Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                string appFilter = $"adoxio_jobnumber eq '{applicationResponse.RecordIdentifier}'";
                string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
                MicrosoftDynamicsCRMadoxioApplication application = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value.FirstOrDefault();

                if (application != null)
                {
                    var screeningRequest = await CreateApplicationScreeningRequestV2(application);
                    if (screeningRequest == null)
                    {
                        continue;
                    }
                    var associatesValidated = UpdateConsentExpiry(screeningRequest.Associates);
                    Log.Logger.Information($"Total associates consent expiry updated: {associatesValidated}");

                    // update the date of security status received and the status
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioDatereceivedspd = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = (int?)applicationResponse.Result
                    };

                    try
                    {
                        if (patchRecord.AdoxioChecklistsecurityclearancestatus != null)
                        {
                            _dynamicsClient.Applications.Update(application.AdoxioApplicationid, patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"Error updating application - received an invalid status of {applicationResponse.Result}");
                            Log.Logger.Error($"Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (HttpOperationException odee)
                    {
                        hangfireContext.WriteLine("Error updating application");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        Log.Logger.Error(odee, "Error updating application");
                    }
                }
            }

            hangfireContext.WriteLine("Finished SPICE Import Job for Application Screening.");
            Log.Logger.Error("Finished SPICE Import Job for Application Screening..");
        }

        /// <summary>
        /// Generate an application screening request (using the new LE Connections entity instead of the Associations entity)
        /// </summary>
        /// <returns></returns>
        public async Task<IncompleteApplicationScreening> GenerateApplicationScreeningRequestV2(Guid applicationId)
        {
            MicrosoftDynamicsCRMadoxioApplicationCollection applications;
            string appFilter = $"adoxio_applicationid eq {applicationId}";

            string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact", "owninguser", "adoxio_ApplicationTypeId" };
            try
            {
                applications = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand);
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Unable to get applications");
                return null;
            }

            var application = applications.Value[0];

            return await CreateApplicationScreeningRequestV2(application);
        }

        public MicrosoftDynamicsCRMadoxioWorker GetWorker(Guid workerId)
        {
            try
            {
                return _dynamicsClient.Workers.Get(filter: $"adoxio_workerid eq {workerId.ToString()}").Value[0];
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Unable to find worker {workerId.ToString()}");
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
            bool result = false;
            var consentValidated = Validation.ValidateAssociateConsent(_dynamicsClient, (List<LegalEntity>)applicationRequest.Associates);
            if (consentValidated)
            {
                List<IncompleteApplicationScreening> payload = new List<IncompleteApplicationScreening>
                {
                    applicationRequest
                };

                Log.Logger.Information($"Sending Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");
                Log.Logger.Information($"Application has {applicationRequest.Associates.Count} associates");

                try
                {
                    MicrosoftDynamicsCRMadoxioApplication update = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioChecklistsecurityclearancestatus = (int?)ApplicationSecurityStatus.Sending
                    };
                    try
                    {
                        _dynamicsClient.Applications.Update(applicationId.ToString(), update);
                    }
                    catch (HttpOperationException odee)
                    {
                        Log.Logger.Error(odee, "Error setting sending status for application");
                    }

                    var receiveApplicationScreeningsResult = SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload).GetAwaiter().GetResult();


                    if (receiveApplicationScreeningsResult.Response.StatusCode.ToString() == "OK")
                    {
                        update = new MicrosoftDynamicsCRMadoxioApplication()
                        {
                            AdoxioSecurityclearancegenerateddate = DateTimeOffset.UtcNow,
                            AdoxioChecklistsecurityclearancestatus = (int?)ApplicationSecurityStatus.Sent
                        };
                        try
                        {
                            _dynamicsClient.Applications.Update(applicationId.ToString(), update);
                            result = true;
                            Log.Logger.Information($"Done Send Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");
                        }
                        catch (HttpOperationException odee)
                        {
                            Log.Logger.Error(odee, "Error updating application");
                        }

                    }
                    else
                    {
                        var msg = receiveApplicationScreeningsResult.Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Log.Logger.Error(msg);
                    }

                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, "Unexpected error in Carla Spice Sync");
                    result = false;
                }

                return result;
            }

            Log.Logger.Error("Consent not valid for all associates.");
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

            Log.Logger.Information($"Sending Worker Screening Request");

            var result = SpiceClient.ReceiveWorkerScreeningsWithHttpMessages(payload);

            Log.Logger.Information($"Response code was: {result.Response.StatusCode.ToString()}");
            Log.Logger.Information($"Done Send Worker Screening Request");

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
                request.Contact = new Interfaces.Spice.Models.Contact()
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
                }
                else if (worker.AdoxioContactId.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence)
                {
                    request.Contact.DriversLicenceNumber = worker.AdoxioContactId.AdoxioPrimaryidnumber;
                }

                if (worker.AdoxioContactId.AdoxioSecondaryidentificationtype == (int)SecondaryIdentificationType.BCIDCard)
                {
                    request.Contact.BcIdCardNumber = worker.AdoxioContactId.AdoxioSecondaryidnumber;
                }
                else if (worker.AdoxioContactId.AdoxioSecondaryidentificationtype == (int)SecondaryIdentificationType.DriversLicence)
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

            Log.Logger.Information("Finished building Model");
            return request;
        }
        
        protected async Task<IncompleteApplicationScreening> CreateApplicationScreeningRequestV2(MicrosoftDynamicsCRMadoxioApplication application)
        {
            try
            {
                Log.Logger.Information("Creating Application Screen Request");

                var screeningRequest = new IncompleteApplicationScreening()
                {
                    Name = application.AdoxioName,                    
                    ApplicationType = application.AdoxioApplicationTypeId.AdoxioName,
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
                    ContactPerson = new Interfaces.Spice.Models.Contact()
                    {
                        ContactId = application.AdoxioApplicant?._primarycontactidValue,
                        FirstName = application.AdoxioContactpersonfirstname,
                        LastName = application.AdoxioContactpersonlastname,
                        MiddleName = application.AdoxioContactmiddlename,
                        Email = application.AdoxioEmail,
                        PhoneNumber = application.AdoxioContactpersonphone
                    },
                    AssignedPerson = new Interfaces.Spice.Models.Contact()
                    {
                        FirstName = application.Owninguser?.Firstname,
                        LastName = application.Owninguser?.Lastname
                    }
                };
                if (application.AdoxioApplyingPerson != null)
                {
                    string companyName = null;
                    if (application.AdoxioApplyingPerson._parentcustomeridValue != null)
                    {
                        MicrosoftDynamicsCRMaccount company = _dynamicsClient.Accounts.Get(filter: "accountid eq " + application.AdoxioApplyingPerson._parentcustomeridValue).Value[0];
                        companyName = company.Name;
                    }
                    screeningRequest.ApplyingPerson = new Interfaces.Spice.Models.Contact()
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

                /* Add associates from account */
                try
                {
                    var moreAssociates = CreateAssociatesForAccountV2(application._adoxioApplicantValue, screeningRequest.Associates.Where(s => s.Account != null).Select(s => s.Account.AccountId).ToList());
                    screeningRequest.Associates = screeningRequest.Associates.Concat(moreAssociates).ToList();
                }
                catch (System.NullReferenceException e)
                {
                    Log.Logger.Error(e, $"NullReferenceException calling CreateAssociatesForAccountV2 for application id: {application.AdoxioApplicationid}");
                }

                /* remove duplicate associates */
                List<string> contactIds = new List<string> { };
                int i = 0;
                List<LegalEntity> finalAssociates = new List<LegalEntity>();
                foreach (var assoc in screeningRequest.Associates)
                {
                    if (!contactIds.Contains(assoc.Contact.ContactId))
                    {
                        finalAssociates.Add(assoc);
                        contactIds.Add(assoc.Contact.ContactId);
                    }
                    i++;
                }
                screeningRequest.Associates = finalAssociates;
                Log.Logger.Information("Screening Request Body");
                var SerializationSettings = new JsonSerializerSettings
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
                    ContractResolver = new ReadOnlyJsonContractResolver(),
                    Converters = new List<JsonConverter>
                    {
                        new Iso8601TimeSpanConverter()
                    }
                };
                var requestBody = SafeJsonConvert.SerializeObject(screeningRequest, SerializationSettings);
                Log.Logger.Information(requestBody);
                return screeningRequest;
            }
            catch (HttpOperationException odee)
            {
                Log.Logger.Error(odee, "Error creating application screening request");
                return null;
            }
        }


        private List<LegalEntity> CreateAssociatesForAccountV2(string accountId, List<string> accounts)
        {
            try
            {
                List<LegalEntity> newAssociates = new List<LegalEntity>();
                if (accounts.Contains(accountId))
                {
                    return newAssociates;
                }
                else
                {
                    accounts.Add(accountId);
                }

                if (string.IsNullOrEmpty(accountId))
                {
                    Log.Logger.Error("CreateAssociatesForAccountV2 received a null accountId");
                    return newAssociates;
                }
                // Select ACTIVE le-connections that match the key personnel or deemed associate filter
                string entityFilter = $"statecode eq 0 and _adoxio_parentaccount_value eq {accountId} and _adoxio_childprofilename_value ne {accountId}";
                entityFilter += $" and adoxio_securityscreeningrequired eq true";
                string[] expand = { "adoxio_ChildProfileName_contact", "adoxio_ChildProfileName_account", "adoxio_ParentAccount" };

                var leConnections = _dynamicsClient.Leconnections.Get(filter: entityFilter, expand: expand).Value;
                if (leConnections != null)
                {
                    foreach (var leConnection in leConnections)
                    {
                        try
                        {
                            LegalEntity associate = CreateAssociate(leConnection);
                            if ((bool)associate.IsIndividual)
                            {
                                newAssociates.Add(associate);
                            }
                            else
                            {
                                var moreAssociates = CreateAssociatesForAccountV2(associate.Account.AccountId, accounts);
                                newAssociates.AddRange(moreAssociates);
                            }
                        }
                        catch (ArgumentNullException e)
                        {
                            Log.Logger.Error(e, $"Attempted to create null associate: {leConnection.AdoxioLeconnectionid}");
                        }
                    }
                }
                return newAssociates;
            }
            catch (HttpOperationException hoe)
            {
                Log.Logger.Error(hoe, $"HttpOperationException in CreateAssociatesForAccountV2 for accountId: {accountId}");
                throw hoe;
            }
            catch (System.NullReferenceException e)
            {
                Log.Logger.Error(e, $"NullReferenceException in CreateAssociatesForAccountV2 for accountId: {accountId}");
                throw e;
            }
        }

        private LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            if (legalEntity == null)
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
                associate.Contact = new Interfaces.Spice.Models.Contact()
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
                    Log.Logger.Error("Failed to find a shareholder account found");
                    associate.Account = new Interfaces.Spice.Models.Account();
                }
                associate.IsIndividual = false;
            }
            return associate;
        }

        private LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLeconnection leConnection)
        {
            if (leConnection == null)
            {
                throw new ArgumentNullException();
            }

            LegalEntity associate = new LegalEntity()
            {
                EntityId = leConnection.AdoxioLeconnectionid,
                Name = leConnection.AdoxioName,
                Title = leConnection.AdoxioJobtitle,
                Positions = GetLegalEntityPositions(leConnection),
                PreviousAddresses = new List<Address>(),
                Aliases = new List<Alias>()
            };

            if (leConnection.AdoxioIsindividual != null && leConnection.AdoxioIsindividual.Value && leConnection.AdoxioChildProfileNameContact != null)
            {
                var crmContact = leConnection.AdoxioChildProfileNameContact;
                associate.IsIndividual = true;
                associate.TiedHouse = crmContact.AdoxioSelfdeclaredtiedhouse == 1;
                associate.Contact = new Interfaces.Spice.Models.Contact()
                {
                    SpdJobId = crmContact.AdoxioSpdjobid.ToString(),
                    ContactId = crmContact.Contactid,
                    FirstName = crmContact.Firstname,
                    LastName = crmContact.Lastname,
                    MiddleName = crmContact.Middlename,
                    Email = crmContact.Emailaddress1,
                    PhoneNumber = crmContact.Telephone1 ?? crmContact.Mobilephone,
                    SelfDisclosure = (crmContact.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)crmContact.AdoxioSelfdisclosure).ToString(),
                    Gender = (crmContact.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)crmContact.AdoxioGendercode).ToString(),
                    Birthplace = crmContact.AdoxioBirthplace,
                    BirthDate = crmContact.Birthdate,
                    BcIdCardNumber = crmContact.AdoxioIdentificationtype == (int)IdentificationType.BCIDCard ? crmContact.AdoxioPrimaryidnumber : null,
                    DriversLicenceNumber = crmContact.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence ? crmContact.AdoxioPrimaryidnumber : null,
                    DriverLicenceJurisdiction = crmContact.AdoxioIdentificationtype == (int)IdentificationType.DriversLicence && crmContact.AdoxioIdentificationjurisdiction != null ? ((IdentificationJurisdiction)crmContact.AdoxioIdentificationjurisdiction).ToString() : null,
                    Address = new Address()
                    {
                        AddressStreet1 = crmContact.Address1Line1,
                        AddressStreet2 = crmContact.Address1Line2,
                        AddressStreet3 = crmContact.Address1Line3,
                        City = crmContact.Address1City,
                        StateProvince = crmContact.Address1Stateorprovince,
                        Postal = (Validation.ValidatePostalCode(crmContact.Address1Postalcode)) ? crmContact.Address1Postalcode : null,
                        Country = crmContact.Address1Country
                    }
                };

                /* Add previous addresses */
                var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + crmContact.Contactid).Value;
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
                var aliases = _dynamicsClient.Aliases.Get(filter: "_adoxio_contactid_value eq " + crmContact.Contactid).Value;
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
                if (leConnection.AdoxioChildProfileNameAccount != null)
                {
                    associate.Account = new Interfaces.Spice.Models.Account()
                    {
                        AccountId = leConnection.AdoxioChildProfileNameAccount.Accountid,
                        Name = leConnection.AdoxioChildProfileNameAccount.Name,
                        BcIncorporationNumber = leConnection.AdoxioChildProfileNameAccount.AdoxioBcincorporationnumber,
                        BusinessNumber = leConnection.AdoxioChildProfileNameAccount.Accountnumber,
                        Associates = new List<LegalEntity>()
                    };
                }
                else if (leConnection.AdoxioParentAccount != null)
                {
                    associate.Account = new Interfaces.Spice.Models.Account()
                    {
                        AccountId = leConnection.AdoxioParentAccount.Accountid,
                        Name = leConnection.AdoxioParentAccount.Name,
                        BcIncorporationNumber = leConnection.AdoxioParentAccount.AdoxioBcincorporationnumber,
                        BusinessNumber = leConnection.AdoxioParentAccount.Accountnumber,
                        Associates = new List<LegalEntity>()
                    };
                }
                else
                {
                    Log.Logger.Error("Failed to find a child profile account for this LE Connection");
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
            if (legalEntity.AdoxioIsdeemedassociate != null && (bool)legalEntity.AdoxioIsdeemedassociate)
            {
                positions.Add("deemed associate");
            }
            if (legalEntity.AdoxioIspartner != null && (bool)legalEntity.AdoxioIspartner)
            {
                positions.Add("partner");
            }
            return positions;
        }

        private List<string> GetLegalEntityPositions(MicrosoftDynamicsCRMadoxioLeconnection leConnection)
        {
            List<string> positions = new List<string>();

            // return early with empty array if connection type not set
            var type = leConnection.AdoxioConnectiontype;
            if (type == null)
            {
                return positions;
            }

            switch ((ConnectionType)type)
            {
                case ConnectionType.Director:
                    positions.Add("director");
                    break;

                case ConnectionType.Officer:
                    positions.Add("officer");
                    break;

                case ConnectionType.KeyPersonnel:
                case ConnectionType.Representative:
                case ConnectionType.SeniorManager:
                case ConnectionType.Associate:
                    positions.Add("key personnel");
                    break;

                case ConnectionType.Shareholder:
                    positions.Add("shareholder");
                    break;

                case ConnectionType.Owner:
                case ConnectionType.Beneficiary:
                    positions.Add("owner");
                    break;

                case ConnectionType.Trustee:
                    positions.Add("trustee");
                    break;

                case ConnectionType.Partner:
                    positions.Add("partner");
                    break;
            }
            return positions;
        }

        private int UpdateConsentExpiry(IList<LegalEntity> associates)
        {
            var i = 0;
            foreach (var associate in associates)
            {
                if ((bool)associate.IsIndividual)
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
            Log.Logger.Error("Starting SendFoundWorkers Job");
            hangfireContext.WriteLine("Starting SendFoundWorkers Job");

            // Query Dynamics for worker data
            string[] expand = { "adoxio_ContactId", "adoxio_worker_aliases", "adoxio_worker_previousaddresses" };
            string sendFilter = $"adoxio_consentvalidated eq {(int)WorkerConsentValidated.Yes} and adoxio_exporteddate eq null and adoxio_paymentreceived eq 1";
            IList<MicrosoftDynamicsCRMadoxioWorker> workers = _dynamicsClient.Workers.Get(filter: sendFilter, expand: expand).Value;

            if (workers.Count < 1)
            {
                Log.Logger.Error("No workers found for processing");
                hangfireContext.WriteLine("No workers found for processing");
            }
            else
            {
                Log.Logger.Error($"Found {workers.Count} workers to send to SPD.");
                hangfireContext.WriteLine($"Found {workers.Count} workers to send to SPD.");

                foreach (var worker in workers)
                {
                    IncompleteWorkerScreening screeningRequest = GenerateWorkerScreeningRequest(Guid.Parse(worker.AdoxioWorkerid));
                    var reqSuccess = SendWorkerScreeningRequest(screeningRequest);
                    if (reqSuccess)
                    {
                        hangfireContext.WriteLine($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        Log.Logger.Error($"Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        MicrosoftDynamicsCRMadoxioWorker workerPatch = new MicrosoftDynamicsCRMadoxioWorker()
                        {
                            AdoxioExporteddate = DateTime.UtcNow
                        };
                        _dynamicsClient.Workers.Update(worker.AdoxioWorkerid, workerPatch);
                    }
                    else
                    {
                        hangfireContext.WriteLine($"Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                        Log.Logger.Error($"Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                    }
                }
            }

            Log.Logger.Error("End of SendFoundWorkers Job");
            hangfireContext.WriteLine("End of SendFoundWorkers Job");
        }
        
        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SendFoundApplicationsV2(PerformContext hangfireContext)
        {
            string[] select = { "adoxio_applicationtypeid" };
            IList<MicrosoftDynamicsCRMadoxioApplicationtype> selectedAppTypes = _dynamicsClient.Applicationtypes.Get(filter: "adoxio_haslesection eq true", select: select).Value;
            if (selectedAppTypes.Count == 0)
            {
                Log.Logger.Error("Failed to Start SendFoundApplicationsV2: No application types are set to send to SPD.");
                hangfireContext.WriteLine("Failed to Start SendFoundApplicationsV2: No application types are set to send to SPD.");
                return;
            }

            List<string> appTypes = selectedAppTypes.Select(a => a.AdoxioApplicationtypeid).ToList();
            Log.Logger.Error($"Starting SendFoundApplicationsV2 Job for {selectedAppTypes.Count} application types");
            hangfireContext.WriteLine($"Starting SendFoundApplicationsV2 Job for {selectedAppTypes.Count} application types");


            // adjusted 2/9/2021 to pick up items that are in limbo (sending) status
            string sendFilter = $"adoxio_checklistsenttospd eq 1 and (adoxio_checklistsecurityclearancestatus eq {(int?)ApplicationSecurityStatus.NotSent} or adoxio_checklistsecurityclearancestatus eq {(int?)ApplicationSecurityStatus.Sending})";

            IEnumerable<MicrosoftDynamicsCRMadoxioApplication> applications = null;
            try
            {
                applications = _dynamicsClient.Applications.Get(filter: sendFilter).Value.Where(a => appTypes.Contains(a._adoxioApplicationtypeidValue));
                Log.Logger.Error($"Found {applications.Count()} applications to send to SPD.");
                hangfireContext.WriteLine($"Found {applications.Count()} applications to send to SPD.");
            }
            catch (HttpOperationException odee)
            {
                hangfireContext.WriteLine("Error updating application");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);

                Log.Logger.Error(odee, "Error updating application");
            }


            if (applications != null)
            {
                foreach (var application in applications)
                {
                    Guid.TryParse(application.AdoxioApplicationid, out Guid applicationId);
                    try
                    {
                        var screeningRequest = await GenerateApplicationScreeningRequestV2(applicationId);
                        var response = SendApplicationScreeningRequest(applicationId, screeningRequest);
                        if (response)
                        {
                            hangfireContext.WriteLine(
                                $"Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                            Log.Logger.Error(
                                $"Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                        }
                        else
                        {
                            hangfireContext.WriteLine(
                                $"Failed to send application {screeningRequest.RecordIdentifier} to SPD");
                            Log.Logger.Error($"Failed to send application {screeningRequest.RecordIdentifier} to SPD");
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e,$"Error occured during Generate / Send Application Screening Request");
                    }
                    
                }
            }
            
            Log.Logger.Error("End of SendFoundApplicationsV2 Job");
            hangfireContext.WriteLine("End of SendFoundApplicationsV2 Job");
        }
    }
}
