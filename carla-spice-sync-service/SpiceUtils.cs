extern alias DV;

using DV::Gov.Lclb.Cllb.Interfaces;
using DvContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using DvAccount = DV::Gov.Lclb.Cllb.Interfaces.Account;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Gov.Lclb.Cllb.Interfaces.Spice.Models;
using Gov.Lclb.Cllb.CarlaSpiceSync.Extensions;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Gov.Lclb.Cllb.CarlaSpiceSync
{
    public class SpiceUtils
    {
        private IConfiguration Configuration { get; }
        private readonly IDataverseClient _dataverse;
        public ISpiceClient SpiceClient;
        private readonly ILogger<SpiceUtils> _logger;

        public SpiceUtils(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = configuration;
            _logger = loggerFactory.CreateLogger<SpiceUtils>();
            _dataverse = new DataverseClient(configuration);
            SpiceClient = new SpiceClient(new HttpClient(), configuration);
        }

        public SpiceUtils(IConfiguration configuration, ILoggerFactory loggerFactory, IDataverseClient dataverse)
        {
            this.Configuration = configuration;
            _logger = loggerFactory.CreateLogger<SpiceUtils>();
            _dataverse = dataverse;
            SpiceClient = new SpiceClient(new HttpClient(), configuration);
        }

        public async Task ReceiveWorkerImportJob(PerformContext hangfireContext, List<CompletedWorkerScreening> responses)
        {
            hangfireContext.WriteLine("ReceiveWorkerImportJob - Starting SPICE Import Job for Worker Screening.");
            _logger.LogInformation("ReceiveWorkerImportJob - Starting SPICE Import Job for Worker Screening.");

            foreach (var workerResponse in responses)
            {
                try
                {
                    string contactId;
                    // 3 different ways to send an identifier 😷
                    if (workerResponse.RecordIdentifier == null)
                    {
                        contactId = null;
                        if (int.TryParse(workerResponse.SpdJobId, out int parsedSpdJobId))
                        {
                            var spdContact = await _dataverse.GetContactBySpdJobIdAsync(parsedSpdJobId);
                            contactId = spdContact?.Id.ToString();
                        }
                    }
                    else if (workerResponse.RecordIdentifier.Substring(0, 2) == "WR")
                    {
                        // Check if using old WR record
                        var history = await _dataverse.GetPersonalHistorySummaryByWorkerJobNumberAsync(workerResponse.RecordIdentifier);
                        contactId = history?.adoxio_contactid?.Id.ToString();
                    }
                    else
                    {
                        contactId = workerResponse.RecordIdentifier;
                    }

                    var workers = await _dataverse.GetWorkersByContactIdAsync(contactId);
                    var worker = workers.FirstOrDefault();

                    if (worker != null)
                    {
                        var patchRecord = new adoxio_worker();
                        patchRecord.adoxio_workerId = worker.Id;

                        patchRecord.statuscode = workerResponse.ScreeningResult switch
                        {
                            WorkerSecurityStatus.Pass => adoxio_worker_statuscode.Active,
                            WorkerSecurityStatus.Fail => (adoxio_worker_statuscode)WorkerSecurityStatusCode.Rejected,
                            WorkerSecurityStatus.Withdrawn => (adoxio_worker_statuscode)WorkerSecurityStatusCode.Withdrawn,
                            _ => adoxio_worker_statuscode.Active
                        };
                        patchRecord.adoxio_SecurityStatus = (adoxio_securitystatus)workerResponse.ScreeningResult;
                        patchRecord.adoxio_SecurityCompletedOn = DateTime.UtcNow;

                        // Do passed worker things
                        if (workerResponse.ScreeningResult == WorkerSecurityStatus.Pass)
                        {
                            patchRecord.adoxio_ExpiryDate = DateTime.UtcNow.AddYears(2);
                        }

                        await _dataverse.UpdateWorkerAsync(patchRecord);
                    }
                    else
                    {
                        _logger.LogWarning($"ReceiveWorkerImportJob - Worker not found for spd job id: {workerResponse.RecordIdentifier}");
                        hangfireContext.WriteLine($"ReceiveWorkerImportJob - Worker not found for spd job id: {workerResponse.RecordIdentifier}");
                    }
                }
                catch (HttpOperationException odee)
                {
                    hangfireContext.WriteLine("ReceiveWorkerImportJob - Error updating worker security status");
                    hangfireContext.WriteLine("ReceiveWorkerImportJob - Request:");
                    hangfireContext.WriteLine(odee.Request.Content);
                    hangfireContext.WriteLine("ReceiveWorkerImportJob - Response:");
                    hangfireContext.WriteLine(odee.Response.Content);

                    _logger.LogError(odee, "ReceiveWorkerImportJob - Error updating worker personal history");
                }
            }

            hangfireContext.WriteLine("ReceiveWorkerImportJob - Finished SPICE Import Job for Worker Screening.");
            _logger.LogInformation("ReceiveWorkerImportJob - Finished SPICE Import Job for Worker Screening.");
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        public async Task ReceiveApplicationImportJob(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("ReceiveApplicationImportJob - Starting SPICE Import Job for Application Screening.");
            _logger.LogInformation("ReceiveApplicationImportJob - Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                var application = await _dataverse.GetApplicationByJobNumberAsync(applicationResponse.RecordIdentifier);

                if (application != null)
                {
                    var screeningRequest = await CreateApplicationScreeningRequestV2(application);
                    if (screeningRequest == null)
                    {
                        continue;
                    }
                    var associatesValidated = await UpdateConsentExpiry(screeningRequest.Associates);
                    _logger.LogInformation($"ReceiveApplicationImportJob - Total associates consent expiry updated: {associatesValidated}");

                    var patchRecord = new adoxio_application();
                    patchRecord.adoxio_applicationId = application.Id;
                    patchRecord.adoxio_datereceivedspd = DateTime.UtcNow;
                    patchRecord.adoxio_ChecklistSecurityClearanceStatus =
                        applicationResponse.Result != null
                            ? (adoxio_application_adoxio_checklistsecurityclearancestatus?)applicationResponse.Result
                            : null;

                    try
                    {
                        if (patchRecord.adoxio_ChecklistSecurityClearanceStatus != null)
                        {
                            await _dataverse.UpdateApplicationAsync(patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"ReceiveApplicationImportJob - Error updating application - received an invalid status of {applicationResponse.Result}");
                            _logger.LogWarning($"ReceiveApplicationImportJob - Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (Exception ex)
                    {
                        hangfireContext.WriteLine("ReceiveApplicationImportJob - Error updating application");
                        _logger.LogError(ex, "ReceiveApplicationImportJob - Error updating application");
                    }
                }
            }

            hangfireContext.WriteLine("ReceiveApplicationImportJob - Finished SPICE Import Job for Application Screening.");
            _logger.LogInformation("ReceiveApplicationImportJob - Finished SPICE Import Job for Application Screening..");
        }

        /// <summary>
        /// Import application responses to Dynamics (V2 — uses LE Connections).
        /// </summary>
        public async Task ReceiveApplicationImportJobV2(PerformContext hangfireContext, List<CompletedApplicationScreening> responses)
        {
            hangfireContext.WriteLine("ReceiveApplicationImportJobV2 - Starting SPICE Import Job for Application Screening.");
            _logger.LogInformation("ReceiveApplicationImportJobV2 - Starting SPICE Import Job for Application Screening..");

            foreach (var applicationResponse in responses)
            {
                var application = await _dataverse.GetApplicationByJobNumberAsync(applicationResponse.RecordIdentifier);

                if (application != null)
                {
                    var screeningRequest = await CreateApplicationScreeningRequestV2(application);
                    if (screeningRequest == null)
                    {
                        continue;
                    }
                    var associatesValidated = await UpdateConsentExpiry(screeningRequest.Associates);
                    _logger.LogInformation($"ReceiveApplicationImportJobV2 - Total associates consent expiry updated: {associatesValidated}");

                    var patchRecord = new adoxio_application();
                    patchRecord.adoxio_applicationId = application.Id;
                    patchRecord.adoxio_datereceivedspd = DateTime.UtcNow;
                    patchRecord.adoxio_ChecklistSecurityClearanceStatus =
                        applicationResponse.Result != null
                            ? (adoxio_application_adoxio_checklistsecurityclearancestatus?)applicationResponse.Result
                            : null;

                    try
                    {
                        if (patchRecord.adoxio_ChecklistSecurityClearanceStatus != null)
                        {
                            await _dataverse.UpdateApplicationAsync(patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"ReceiveApplicationImportJobV2 - Error updating application - received an invalid status of {applicationResponse.Result}");
                            _logger.LogWarning($"ReceiveApplicationImportJobV2 - Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (Exception ex)
                    {
                        hangfireContext.WriteLine("ReceiveApplicationImportJobV2 - Error updating application");
                        _logger.LogError(ex, "ReceiveApplicationImportJobV2 - Error updating application");
                    }
                }
            }

            hangfireContext.WriteLine("ReceiveApplicationImportJobV2 - Finished SPICE Import Job for Application Screening.");
            _logger.LogInformation("ReceiveApplicationImportJobV2 - Finished SPICE Import Job for Application Screening..");
        }

        /// <summary>
        /// Generate an application screening request (using the new LE Connections entity instead of the Associations entity).
        /// </summary>
        public async Task<IncompleteApplicationScreening> GenerateApplicationScreeningRequestV2(Guid applicationId)
        {
            var application = await _dataverse.GetApplicationByIdAsync(applicationId.ToString());
            if (application == null)
            {
                _logger.LogError($"GenerateApplicationScreeningRequestV2 - Unable to find application {applicationId}");
                return null;
            }

            return await CreateApplicationScreeningRequestV2(application);
        }

        public async Task<adoxio_worker> GetWorker(Guid workerId)
        {
            try
            {
                return await _dataverse.GetWorkerByIdAsync(workerId.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"GetWorker - Unable to find worker {workerId}");
                return null;
            }
        }

        /// <summary>
        /// Sends the application screening request to spice.
        /// </summary>
        public async Task<bool> SendApplicationScreeningRequest(Guid applicationId, IncompleteApplicationScreening applicationRequest)
        {
            bool result = false;
            var consentValidated = await Validation.ValidateAssociateConsentAsync(_dataverse, applicationRequest.Associates as List<LegalEntity> ?? new List<LegalEntity>(applicationRequest.Associates));
            if (consentValidated)
            {
                List<IncompleteApplicationScreening> payload = new List<IncompleteApplicationScreening>
                {
                    applicationRequest
                };

                _logger.LogInformation($"SendApplicationScreeningRequest - Sending Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK}");
                _logger.LogInformation($"SendApplicationScreeningRequest - Application has {applicationRequest.Associates.Count} associates");

                try
                {
                    var sendingPatch = new adoxio_application();
                    sendingPatch.adoxio_applicationId = applicationId;
                    sendingPatch.adoxio_ChecklistSecurityClearanceStatus = adoxio_application_adoxio_checklistsecurityclearancestatus.RequestSending;
                    try
                    {
                        await _dataverse.UpdateApplicationAsync(sendingPatch);
                        _logger.LogInformation("SendApplicationScreeningRequest - Done updating application: setting 'sending' status");
                    }
                    catch (Exception odee)
                    {
                        _logger.LogError(odee, "SendApplicationScreeningRequest - Error updating application: setting 'sending' status");
                    }

                    HttpOperationResponse receiveApplicationScreeningsResult = null;
                    try
                    {
                        receiveApplicationScreeningsResult = SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload).GetAwaiter().GetResult();
                    }
                    catch (HttpOperationException e)
                    {
                        _logger.LogError(e, "SendApplicationScreeningRequest - Http error calling ReceiveApplicationScreeningsWithHttpMessagesAsync");
                        result = false;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "SendApplicationScreeningRequest - Unexpected error calling ReceiveApplicationScreeningsWithHttpMessagesAsync");
                        result = false;
                    }

                    if (receiveApplicationScreeningsResult != null && receiveApplicationScreeningsResult.Response.StatusCode.ToString() == "OK")
                    {
                        var sentPatch = new adoxio_application();
                        sentPatch.adoxio_applicationId = applicationId;
                        sentPatch.adoxio_SecurityClearanceGeneratedDate = DateTime.UtcNow;
                        sentPatch.adoxio_ChecklistSecurityClearanceStatus = adoxio_application_adoxio_checklistsecurityclearancestatus.RequestSent;
                        try
                        {
                            await _dataverse.UpdateApplicationAsync(sentPatch);
                            result = true;
                            _logger.LogInformation($"SendApplicationScreeningRequest - Done updating application: setting 'sent' status. {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now:yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK}");
                        }
                        catch (Exception odee)
                        {
                            _logger.LogError(odee, "SendApplicationScreeningRequest - Error updating application: setting 'sent' status");
                        }
                    }
                    else
                    {
                        var msg = receiveApplicationScreeningsResult?.Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        _logger.LogWarning($"SendApplicationScreeningRequest - response from Spice indicates a failure: {msg}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "SendApplicationScreeningRequest - Unexpected error");
                    result = false;
                }

                return result;
            }

            _logger.LogInformation("SendApplicationScreeningRequest - Consent not valid for all associates.");
            var incompletePatch = new adoxio_application();
            incompletePatch.adoxio_applicationId = applicationId;
            incompletePatch.adoxio_SecurityClearanceGeneratedDate = DateTime.UtcNow;
            incompletePatch.adoxio_ChecklistSecurityClearanceStatus = adoxio_application_adoxio_checklistsecurityclearancestatus.Incomplete;
            await _dataverse.UpdateApplicationAsync(incompletePatch);
            _logger.LogInformation($"SendApplicationScreeningRequest - Done updating application: setting 'incomplete' status.");
            return false;
        }

        /// <summary>
        /// Sends the worker screening request to spice.
        /// </summary>
        public bool SendWorkerScreeningRequest(IncompleteWorkerScreening workerScreeningRequest)
        {
            bool result = false;
            List<IncompleteWorkerScreening> payload = new List<IncompleteWorkerScreening>
            {
                workerScreeningRequest
            };

            _logger.LogInformation($"SendWorkerScreeningRequest - Sending Worker Screening Request");

            try
            {
                var receiveWorkerScreeningsResults = SpiceClient.ReceiveWorkerScreeningsWithHttpMessages(payload);

                _logger.LogInformation($"SendWorkerScreeningRequest - Response code was: {receiveWorkerScreeningsResults.Response.StatusCode}");
                _logger.LogInformation($"SendWorkerScreeningRequest - Done Send Worker Screening Request");

                result = receiveWorkerScreeningsResults.Response.StatusCode.ToString() == "OK";
            }
            catch (HttpOperationException e)
            {
                _logger.LogError(e, "SendWorkerScreeningRequest - Unexpected http error in Carla Spice Sync");
                result = false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SendWorkerScreeningRequest - Unexpected error in Carla Spice Sync");
                result = false;
            }

            return result;
        }

        public async Task<IncompleteWorkerScreening> GenerateWorkerScreeningRequest(Guid workerId)
        {
            var worker = await _dataverse.GetWorkerByIdAsync(workerId.ToString());
            if (worker?.adoxio_ContactId == null)
            {
                _logger.LogWarning($"GenerateWorkerScreeningRequest - Worker {workerId} not found or has no contact");
                return new IncompleteWorkerScreening();
            }

            var contact = await _dataverse.GetContactByIdAsync(worker.adoxio_ContactId.Id.ToString());

            var request = new IncompleteWorkerScreening();

            if (contact != null)
            {
                request.RecordIdentifier = worker.adoxio_workerId?.ToString();
                request.Contact = new Interfaces.Spice.Models.Contact()
                {
                    SpdJobId = contact.adoxio_SPDJOBID?.ToString(),
                    ContactId = contact.Id.ToString(),
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    MiddleName = contact.MiddleName,
                    Email = contact.EMailAddress1,
                    PhoneNumber = contact.Telephone1 ?? contact.MobilePhone,
                    BirthDate = contact.BirthDate,
                    SelfDisclosure = contact.adoxio_SelfDisclosure != null ? contact.adoxio_SelfDisclosure.ToString() : null,
                    Gender = contact.adoxio_GenderCode != null ? contact.adoxio_GenderCode.ToString() : null,
                    Birthplace = contact.adoxio_Birthplace,
                    Address = new Address()
                    {
                        AddressStreet1 = contact.Address1_Line1,
                        AddressStreet2 = contact.Address1_Line2,
                        AddressStreet3 = contact.Address1_Line3,
                        City = contact.Address1_City,
                        StateProvince = contact.Address1_StateOrProvince,
                        Postal = (Validation.ValidatePostalCode(contact.Address1_PostalCode)) ? contact.Address1_PostalCode : null,
                        Country = contact.Address1_Country
                    },
                    Aliases = new List<Alias>(),
                    PreviousAddresses = new List<Address>()
                };

                if (contact.adoxio_IdentificationType == adoxio_contact_adoxio_identificationtype.BCIDCard)
                {
                    request.Contact.BcIdCardNumber = contact.adoxio_PrimaryIDNumber;
                }
                else if (contact.adoxio_IdentificationType == adoxio_contact_adoxio_identificationtype.DriversLicence)
                {
                    request.Contact.DriversLicenceNumber = contact.adoxio_PrimaryIDNumber;
                }

                if (contact.adoxio_SecondaryIdentificationType == adoxio_contact_adoxio_secondaryidentificationtype.BCIDCard)
                {
                    request.Contact.BcIdCardNumber = contact.adoxio_SecondaryIDNumber;
                }
                else if (contact.adoxio_SecondaryIdentificationType == adoxio_contact_adoxio_secondaryidentificationtype.DriversLicence)
                {
                    request.Contact.DriversLicenceNumber = contact.adoxio_SecondaryIDNumber;
                }

                var aliases = await _dataverse.GetAliasesByContactIdAsync(contact.Id.ToString());
                foreach (var alias in aliases)
                {
                    request.Contact.Aliases.Add(new Alias()
                    {
                        GivenName = alias.adoxio_FirstName,
                        Surname = alias.adoxio_LastName,
                        SecondName = alias.adoxio_MiddleName
                    });
                }

                var previousAddresses = await _dataverse.GetPreviousAddressesByContactIdAsync(contact.Id.ToString());
                foreach (var address in previousAddresses)
                {
                    request.Contact.PreviousAddresses.Add(new Address()
                    {
                        AddressStreet1 = address.adoxio_StreetAddress,
                        City = address.adoxio_City,
                        StateProvince = address.adoxio_PROVSTATE,
                        Postal = address.adoxio_PostalCode,
                        Country = address.adoxio_Country,
                        ToDate = address.adoxio_ToDate,
                        FromDate = address.adoxio_FromDate
                    });
                }
            }

            _logger.LogInformation("GenerateWorkerScreeningRequest - Finished building Model");
            return request;
        }

        protected async Task<IncompleteApplicationScreening> CreateApplicationScreeningRequestV2(adoxio_application application)
        {
            try
            {
                _logger.LogInformation("CreateApplicationScreeningRequestV2 - Creating Application Screen Request");

                // Fetch related entities needed for the screening request
                DvContact applyingPerson = application.adoxio_ApplyingPerson?.Id != null
                    ? await _dataverse.GetContactByIdAsync(application.adoxio_ApplyingPerson.Id.ToString())
                    : null;

                DvAccount applicant = application.adoxio_Applicant?.Id != null
                    ? await _dataverse.GetAccountByIdAsync(application.adoxio_Applicant.Id.ToString())
                    : null;

                adoxio_applicationtype appType = application.adoxio_ApplicationTypeId?.Id != null
                    ? await _dataverse.GetApplicationTypeByIdAsync(application.adoxio_ApplicationTypeId.Id.ToString())
                    : null;

                var screeningRequest = new IncompleteApplicationScreening()
                {
                    Name = application.adoxio_name,
                    ApplicationType = appType?.adoxio_name,
                    RecordIdentifier = application.adoxio_JobNumber,
                    UrgentPriority = false,
                    Associates = new List<LegalEntity>(),
                    ApplicantType = SpiceApplicantType.Cannabis,
                    DateSent = DateTimeOffset.Now,
                    BusinessNumber = applicant?.AccountNumber,
                    ApplicantName = application.adoxio_NameofApplicant,
                    BusinessAddress = new Address()
                    {
                        AddressStreet1 = applicant?.Address1_Line1,
                        City = applicant?.Address1_City,
                        StateProvince = applicant?.Address1_StateOrProvince,
                        Postal = (Validation.ValidatePostalCode(applicant?.Address1_PostalCode)) ? applicant.Address1_PostalCode : null,
                        Country = applicant?.Address1_Country
                    },
                    ContactPerson = new Interfaces.Spice.Models.Contact()
                    {
                        ContactId = applicant?.PrimaryContactId?.Id.ToString(),
                        FirstName = application.adoxio_ContactPersonFirstName,
                        LastName = application.adoxio_ContactPersonLastName,
                        MiddleName = application.adoxio_ContactMiddleName,
                        Email = application.adoxio_Email,
                        PhoneNumber = application.adoxio_ContactPersonPhone
                    },
                    AssignedPerson = new Interfaces.Spice.Models.Contact()
                    {
                        FirstName = application.OwnerId?.Name?.Split(' ').FirstOrDefault(),
                        LastName = application.OwnerId?.Name?.Split(' ').Skip(1).FirstOrDefault()
                    }
                };

                if (applyingPerson != null)
                {
                    string companyName = null;
                    if (applyingPerson.ParentCustomerId != null)
                    {
                        var company = await _dataverse.GetAccountByIdAsync(applyingPerson.ParentCustomerId.Id.ToString());
                        companyName = company?.Name;
                    }
                    screeningRequest.ApplyingPerson = new Interfaces.Spice.Models.Contact()
                    {
                        SpdJobId = applyingPerson.adoxio_SPDJOBID?.ToString(),
                        ContactId = applyingPerson.Id.ToString(),
                        FirstName = applyingPerson.FirstName,
                        CompanyName = companyName,
                        MiddleName = applyingPerson.MiddleName,
                        LastName = applyingPerson.LastName,
                        Email = applyingPerson.EMailAddress1,
                    };
                }

                /* Add applicant details */
                if (applicant != null && applicant.adoxio_BusinessType != null)
                {
                    BusinessType businessType = (BusinessType)applicant.adoxio_BusinessType;
                    screeningRequest.ApplicantAccount = new Interfaces.Spice.Models.Account()
                    {
                        AccountId = applicant.Id.ToString(),
                        Name = applicant.Name,
                        BcIncorporationNumber = applicant.adoxio_BCIncorporationNumber,
                        BusinessType = businessType.ToString()
                    };
                }

                /* Add establishment */
                if (application.adoxio_LicenceEstablishment != null)
                {
                    screeningRequest.Establishment = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Establishment()
                    {
                        Name = application.adoxio_EstablishmentPropsedName,
                        PrimaryPhone = application.adoxio_EstablishmentPhone,
                        PrimaryEmail = application.adoxio_EstablishmentEmail,
                        ParcelId = application.adoxio_EstablishmentParcelID,
                        Address = new Address()
                        {
                            AddressStreet1 = application.adoxio_EstablishmentAddressStreet,
                            City = application.adoxio_EstablishmentAddressCity,
                            StateProvince = "BC",
                            Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.adoxio_EstablishmentAddressPostalCode)) ? application.adoxio_EstablishmentAddressPostalCode : null,
                            Country = "Canada"
                        }
                    };
                }

                /* Add associates from account */
                try
                {
                    var moreAssociates = await CreateAssociatesForAccountV2(
                        application.adoxio_Applicant?.Id.ToString(),
                        screeningRequest.Associates.Where(s => s.Account != null).Select(s => s.Account.AccountId).ToList());
                    screeningRequest.Associates = screeningRequest.Associates.Concat(moreAssociates).ToList();
                }
                catch (System.NullReferenceException e)
                {
                    _logger.LogError(e, $"CreateApplicationScreeningRequestV2 - NullReferenceException calling CreateAssociatesForAccountV2 for application id: {application.adoxio_applicationId}");
                }

                /* remove duplicate associates */
                var contactIds = new List<string>();
                var finalAssociates = new List<LegalEntity>();
                foreach (var assoc in screeningRequest.Associates)
                {
                    if (!contactIds.Contains(assoc.Contact.ContactId))
                    {
                        finalAssociates.Add(assoc);
                        contactIds.Add(assoc.Contact.ContactId);
                    }
                }
                screeningRequest.Associates = finalAssociates;

                List<IncompleteApplicationScreening> payload = new List<IncompleteApplicationScreening>
                {
                    screeningRequest
                };
                _logger.LogInformation("CreateApplicationScreeningRequestV2 - Screening Request Body");
                var serializationSettings = new JsonSerializerSettings
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

                var requestBody = SafeJsonConvert.SerializeObject(payload, serializationSettings);
                _logger.LogInformation("CreateApplicationScreeningRequestV2 - " + requestBody);
                return screeningRequest;
            }
            catch (Exception odee)
            {
                _logger.LogError(odee, "CreateApplicationScreeningRequestV2 - Error creating application screening request");
                return null;
            }
        }


        private async Task<List<LegalEntity>> CreateAssociatesForAccountV2(string accountId, List<string> accounts)
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
                    _logger.LogWarning("CreateAssociatesForAccountV2 - received a null accountId");
                    return newAssociates;
                }

                var leConnections = await _dataverse.GetActiveLeConnectionsByParentAccountIdAsync(accountId);
                if (leConnections != null)
                {
                    foreach (var leConnection in leConnections)
                    {
                        try
                        {
                            LegalEntity associate = await CreateAssociate(leConnection);
                            if ((bool)associate.IsIndividual)
                            {
                                newAssociates.Add(associate);
                            }
                            else
                            {
                                var moreAssociates = await CreateAssociatesForAccountV2(associate.Account.AccountId, accounts);
                                newAssociates.AddRange(moreAssociates);
                            }
                        }
                        catch (ArgumentNullException e)
                        {
                            _logger.LogError(e, $"CreateAssociatesForAccountV2 - Attempted to create null associate: {leConnection.adoxio_leconnectionId}");
                        }
                    }
                }
                return newAssociates;
            }
            catch (Exception hoe)
            {
                _logger.LogError(hoe, $"Exception in CreateAssociatesForAccountV2 for accountId: {accountId}");
                throw;
            }
        }

        private async Task<LegalEntity> CreateAssociate(adoxio_leconnection leConnection)
        {
            if (leConnection == null)
            {
                throw new ArgumentNullException();
            }

            LegalEntity associate = new LegalEntity()
            {
                EntityId = leConnection.adoxio_leconnectionId?.ToString(),
                Name = leConnection.adoxio_name,
                Title = leConnection.adoxio_JobTitle,
                Positions = GetLegalEntityPositions(leConnection),
                PreviousAddresses = new List<Address>(),
                Aliases = new List<Alias>()
            };

            if (leConnection.adoxio_IsIndividual == true && leConnection.adoxio_ChildProfileName?.Id != null)
            {
                var crmContact = await _dataverse.GetContactByIdAsync(leConnection.adoxio_ChildProfileName.Id.ToString());
                if (crmContact != null)
                {
                    associate.IsIndividual = true;
                    associate.TiedHouse = crmContact.adoxio_SelfDeclaredTiedHouse == adoxio_generalyesno.Yes;
                    associate.Contact = new Interfaces.Spice.Models.Contact()
                    {
                        SpdJobId = crmContact.adoxio_SPDJOBID?.ToString(),
                        ContactId = crmContact.Id.ToString(),
                        FirstName = crmContact.FirstName,
                        LastName = crmContact.LastName,
                        MiddleName = crmContact.MiddleName,
                        Email = crmContact.EMailAddress1,
                        PhoneNumber = crmContact.Telephone1 ?? crmContact.MobilePhone,
                        SelfDisclosure = crmContact.adoxio_SelfDisclosure != null ? crmContact.adoxio_SelfDisclosure.ToString() : null,
                        Gender = crmContact.adoxio_GenderCode != null ? crmContact.adoxio_GenderCode.ToString() : null,
                        Birthplace = crmContact.adoxio_Birthplace,
                        BirthDate = crmContact.BirthDate,
                        BcIdCardNumber = crmContact.adoxio_IdentificationType == adoxio_contact_adoxio_identificationtype.BCIDCard ? crmContact.adoxio_PrimaryIDNumber : null,
                        DriversLicenceNumber = crmContact.adoxio_IdentificationType == adoxio_contact_adoxio_identificationtype.DriversLicence ? crmContact.adoxio_PrimaryIDNumber : null,
                        DriverLicenceJurisdiction = crmContact.adoxio_IdentificationType == adoxio_contact_adoxio_identificationtype.DriversLicence && crmContact.adoxio_IdentificationJurisdiction != null ? crmContact.adoxio_IdentificationJurisdiction.ToString() : null,
                        Address = new Address()
                        {
                            AddressStreet1 = crmContact.Address1_Line1,
                            AddressStreet2 = crmContact.Address1_Line2,
                            AddressStreet3 = crmContact.Address1_Line3,
                            City = crmContact.Address1_City,
                            StateProvince = crmContact.Address1_StateOrProvince,
                            Postal = (Validation.ValidatePostalCode(crmContact.Address1_PostalCode)) ? crmContact.Address1_PostalCode : null,
                            Country = crmContact.Address1_Country
                        }
                    };

                    var previousAddresses = await _dataverse.GetPreviousAddressesByContactIdAsync(crmContact.Id.ToString());
                    foreach (var address in previousAddresses)
                    {
                        associate.PreviousAddresses.Add(new Address()
                        {
                            AddressStreet1 = address.adoxio_StreetAddress,
                            City = address.adoxio_City,
                            StateProvince = address.adoxio_PROVSTATE,
                            Postal = (Validation.ValidatePostalCode(address.adoxio_PostalCode)) ? address.adoxio_PostalCode : null,
                            Country = address.adoxio_Country,
                            ToDate = address.adoxio_ToDate,
                            FromDate = address.adoxio_FromDate
                        });
                    }

                    var aliases = await _dataverse.GetAliasesByContactIdAsync(crmContact.Id.ToString());
                    foreach (var alias in aliases)
                    {
                        associate.Aliases.Add(new Alias()
                        {
                            GivenName = alias.adoxio_FirstName,
                            Surname = alias.adoxio_LastName,
                            SecondName = alias.adoxio_MiddleName
                        });
                    }
                }
            }
            else
            {
                // Attach the child profile account or fall back to parent account
                string childAccountId = leConnection.adoxio_ChildProfileName?.Id.ToString();
                string parentAccountId = leConnection.adoxio_ParentAccount?.Id.ToString();

                DvAccount account = null;
                if (childAccountId != null)
                {
                    account = await _dataverse.GetAccountByIdAsync(childAccountId);
                }
                else if (parentAccountId != null)
                {
                    account = await _dataverse.GetAccountByIdAsync(parentAccountId);
                }

                if (account != null)
                {
                    associate.Account = new Interfaces.Spice.Models.Account()
                    {
                        AccountId = account.Id.ToString(),
                        Name = account.Name,
                        BcIncorporationNumber = account.adoxio_BCIncorporationNumber,
                        BusinessNumber = account.AccountNumber,
                        Associates = new List<LegalEntity>()
                    };
                }
                else
                {
                    _logger.LogWarning("CreateAssociate - Failed to find a child profile account for this LE Connection");
                    associate.Account = new Interfaces.Spice.Models.Account();
                }
                associate.IsIndividual = false;
            }
            return associate;
        }

        private List<string> GetLegalEntityPositions(adoxio_leconnection leConnection)
        {
            List<string> positions = new List<string>();

            var type = leConnection.adoxio_ConnectionType;
            if (type == null)
            {
                return positions;
            }

            switch (type)
            {
                case adoxio_leconnectiontypes.Director:
                    positions.Add("director");
                    break;

                case adoxio_leconnectiontypes.Officer:
                    positions.Add("officer");
                    break;

                case adoxio_leconnectiontypes.KeyPersonnel:
                case adoxio_leconnectiontypes.Representative:
                case adoxio_leconnectiontypes.SeniorManager:
                case adoxio_leconnectiontypes.Associate:
                    positions.Add("key personnel");
                    break;

                case adoxio_leconnectiontypes.Shareholder:
                    positions.Add("shareholder");
                    break;

                case adoxio_leconnectiontypes.Owner:
                case adoxio_leconnectiontypes.Beneficiary:
                    positions.Add("owner");
                    break;

                case adoxio_leconnectiontypes.Trustee:
                    positions.Add("trustee");
                    break;

                case adoxio_leconnectiontypes.Partner:
                    positions.Add("partner");
                    break;
            }
            return positions;
        }

        private async Task<int> UpdateConsentExpiry(IList<LegalEntity> associates)
        {
            var i = 0;
            foreach (var associate in associates)
            {
                if ((bool)associate.IsIndividual)
                {
                    var patch = new DvContact();
                    patch.ContactId = Guid.Parse(associate.Contact.ContactId);
                    patch.adoxio_ConsentValidated = adoxio_contact_adoxio_consentvalidated.Yes;
                    patch.adoxio_ConsentValidatedExpiryDate = DateTime.UtcNow.AddMonths(3);
                    await _dataverse.UpdateContactAsync(patch);
                    i += 1;
                }
                else
                {
                    i += await UpdateConsentExpiry(associate.Account.Associates);
                }
            }
            return i;
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SendFoundWorkers(PerformContext hangfireContext)
        {
            _logger.LogInformation("SendFoundWorkers - Starting SendFoundWorkers Job");
            hangfireContext.WriteLine("SendFoundWorkers - Starting SendFoundWorkers Job");

            var workers = await _dataverse.GetWorkersToSendAsync();

            if (workers.Count < 1)
            {
                _logger.LogInformation("SendFoundWorkers - No workers found for processing");
                hangfireContext.WriteLine("SendFoundWorkers - No workers found for processing");
            }
            else
            {
                _logger.LogInformation($"SendFoundWorkers - Found {workers.Count} workers to send to SPD.");
                hangfireContext.WriteLine($"SendFoundWorkers - Found {workers.Count} workers to send to SPD.");

                foreach (var worker in workers)
                {
                    var workerId = worker.adoxio_workerId ?? worker.Id;
                    IncompleteWorkerScreening screeningRequest = await GenerateWorkerScreeningRequest(workerId);
                    var reqSuccess = SendWorkerScreeningRequest(screeningRequest);
                    if (reqSuccess)
                    {
                        hangfireContext.WriteLine($"SendFoundWorkers - Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");
                        _logger.LogInformation($"SendFoundWorkers - Successfully sent worker {screeningRequest.RecordIdentifier} to SPD");

                        var workerPatch = new adoxio_worker();
                        workerPatch.adoxio_workerId = workerId;
                        workerPatch.adoxio_ExportedDate = DateTime.UtcNow;
                        await _dataverse.UpdateWorkerAsync(workerPatch);
                    }
                    else
                    {
                        hangfireContext.WriteLine($"SendFoundWorkers - Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                        _logger.LogWarning($"SendFoundWorkers - Failed to send worker {screeningRequest.RecordIdentifier} to SPD");
                    }
                }
            }

            _logger.LogInformation("SendFoundWorkers - End of SendFoundWorkers Job");
            hangfireContext.WriteLine("SendFoundWorkers - End of SendFoundWorkers Job");
        }

        [DisableConcurrentExecution(timeoutInSeconds: 10 * 60)]
        public async Task SendFoundApplicationsV2(PerformContext hangfireContext)
        {
            var selectedAppTypes = await _dataverse.GetApplicationTypesWithLeSectionAsync();
            if (selectedAppTypes.Count == 0)
            {
                _logger.LogWarning("SendFoundApplicationsV2 - Failed to Start SendFoundApplicationsV2: No application types are set to send to SPD.");
                hangfireContext.WriteLine("SendFoundApplicationsV2 - Failed to Start SendFoundApplicationsV2: No application types are set to send to SPD.");
                return;
            }

            var appTypeIds = selectedAppTypes.Select(a => a.adoxio_applicationtypeId?.ToString()).Where(id => id != null).ToList();
            _logger.LogInformation($"SendFoundApplicationsV2 - Starting SendFoundApplicationsV2 Job for {selectedAppTypes.Count} application types");
            hangfireContext.WriteLine($"SendFoundApplicationsV2 - Starting SendFoundApplicationsV2 Job for {selectedAppTypes.Count} application types");

            IList<adoxio_application> applications = null;
            try
            {
                var allApplications = await _dataverse.GetApplicationsToSendAsync();
                applications = allApplications
                    .Where(a => appTypeIds.Contains(a.adoxio_ApplicationTypeId?.Id.ToString()))
                    .ToList();
                _logger.LogInformation($"SendFoundApplicationsV2 - Found {applications.Count} applications to send to SPD.");
                hangfireContext.WriteLine($"SendFoundApplicationsV2 - Found {applications.Count} applications to send to SPD.");
            }
            catch (Exception odee)
            {
                hangfireContext.WriteLine("SendFoundApplicationsV2 - Error retrieving applications");
                _logger.LogError(odee, "SendFoundApplicationsV2 - Error retrieving applications");
            }

            if (applications != null)
            {
                foreach (var application in applications)
                {
                    var applicationId = application.adoxio_applicationId ?? application.Id;
                    try
                    {
                        var screeningRequest = await GenerateApplicationScreeningRequestV2(applicationId);
                        var response = await SendApplicationScreeningRequest(applicationId, screeningRequest);
                        if (response)
                        {
                            hangfireContext.WriteLine($"SendFoundApplicationsV2 - Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                            _logger.LogInformation($"SendFoundApplicationsV2 - Successfully sent application {screeningRequest.RecordIdentifier} to SPD");
                        }
                        else
                        {
                            hangfireContext.WriteLine($"SendFoundApplicationsV2 - Failed to send application {screeningRequest?.RecordIdentifier} to SPD");
                            _logger.LogWarning($"SendFoundApplicationsV2 - Failed to send application {screeningRequest?.RecordIdentifier} to SPD");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"SendFoundApplicationsV2 - Error occurred during Generate / Send Application Screening Request");
                    }
                }
            }

            _logger.LogInformation("SendFoundApplicationsV2 - End of SendFoundApplicationsV2 Job");
            hangfireContext.WriteLine("SendFoundApplicationsV2 - End of SendFoundApplicationsV2 Job");
        }
    }
}
