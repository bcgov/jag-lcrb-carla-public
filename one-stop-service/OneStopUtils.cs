extern alias DV;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Gov.Jag.Lcrb.OneStopService.OneStop;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;
using IDataverseClient = DV::Gov.Lclb.Cllb.Interfaces.IDataverseClient;
using DV::Gov.Lclb.Cllb.Interfaces;
using OneStopHubStatusChange = DV::Gov.Lclb.Cllb.Interfaces.OneStopHubStatusChange;
using OneStopMessageStatus = DV::Gov.Lclb.Cllb.Interfaces.OneStopMessageStatus;
using IOneStopRestClient = Gov.Lclb.Cllb.Interfaces.IOneStopRestClient;
using OneStopRestClient = Gov.Lclb.Cllb.Interfaces.OneStopRestClient;

namespace Gov.Jag.Lcrb.OneStopService
{
    public enum ChangeNameType
    {
        ChangeName = 1,
        Transfer = 2,
        ThirdPartyOperator = 3
    }
    public class OneStopUtils
    {
        public const string ASYNCHRONOUS = "A";

        public const string ADDRESS_TYPE_CODE = "01";
        public const string CLIENT_NAME_TYPE_CODE = "02";
        public const string DOCUMENT_SUBTYPE = "000";

        /* OneStop Document Subtypes
        Change Status	113
        Change Name – establishment name	150
        Change name – operating name	103
        Change name - TPO	154
        Change name – deemed licensee	155
        Change address – physical	107
        Change address – mailing	108
        Change contact info – exec/receiver	157
        */


        public const string DOCUMENT_SUBTYPE_CHANGESTATUS = "113";
        public const string DOCUMENT_SUBTYPE_CHANGENAME = "150";
        public const string DOCUMENT_SUBTYPE_CHANGENAME_TRANSFER = "155";
        public const string DOCUMENT_SUBTYPE_CHANGENAME_THIRDPARTY = "154";
        public const string DOCUMENT_SUBTYPE_CHANGEADDRESS = "107";

        public const string SENDER_ID = "LCRB";
        public const string RECEIVER_ID = "BCSBNHUB";
        public const string USER_APPLICATION = "BF";
        public const string USER_ROLE = "01";
        public const string BUSINESS_PROGRAM_IDENTIFIER = "BB";
        public const string PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE = "150";
        public const string PROGRAM_ACCOUNT_TYPE_CODE = "01";
        public const string PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE = "01";
        public const string PROGRAM_ACCOUNT_STATUS_CODE_CLOSED = "02";
        public const string PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED = "03";
        public const string PROVINCE_STATE_CODE = "BC";
        public const string COUNTRY_CODE = "CA";

        public const string OPERATING_NAME_SEQUENCE_NUMBER = "1";
        public const string UPDATE_REASON_CODE = "01";
        public const string UPDATE_REASON_CODE_ADDRESS = "03";



        /// <summary>
        /// Maximum number of new licenses that will be sent per interval.
        /// </summary>
        private int maxLicencesPerInterval;

        private IConfiguration _configuration { get; }

        private readonly IOneStopRestClient _onestopRestClient;

        private IMemoryCache _cache;

        private readonly IDataverseClient _dataverse;

        public OneStopUtils(IConfiguration configuration, IMemoryCache cache, IDataverseClient dataverse)
        {
            _configuration = configuration;
            _cache = cache;
            _dataverse = dataverse;
            _onestopRestClient = SetupOneStopClient(configuration, Log.Logger);

            if (!string.IsNullOrEmpty(_configuration["maxLicencesPerInterval"]))
            {
                if (!int.TryParse(_configuration["maxLicencesPerInterval"], out maxLicencesPerInterval))
                {
                    maxLicencesPerInterval = 10;
                }
            }
            else
            {
                maxLicencesPerInterval = 10;
            }
        }

        private async Task<OneStopLicenceData?> FetchLicenceForOneStop(string licenceId)
        {
            var dvLicence = await _dataverse.GetLicenceByIdAsync(licenceId);
            if (dvLicence == null) return null;

            OneStopAccount licencee = null;
            if (dvLicence.adoxio_Licencee?.Id is Guid licenceeId && licenceeId != Guid.Empty)
            {
                var dvAccount = await _dataverse.GetAccountByIdAsync(licenceeId.ToString());
                if (dvAccount != null)
                {
                    licencee = new OneStopAccount
                    {
                        AccountNumber = dvAccount.AccountNumber,
                        Name = dvAccount.Name ?? dvLicence.adoxio_Licencee?.Name,
                        Email = dvAccount.EMailAddress1,
                        Phone = dvAccount.Address1_Telephone1,
                        Address1Line1 = dvAccount.Address1_Line1,
                        Address1City = dvAccount.Address1_City,
                        Address1PostalCode = dvAccount.Address1_PostalCode
                    };
                }
            }

            OneStopEstablishment est = null;
            if (dvLicence.adoxio_establishment?.Id is Guid estId && estId != Guid.Empty)
            {
                var dvEst = await _dataverse.GetEstablishmentByIdAsync(estId.ToString());
                if (dvEst != null)
                {
                    est = new OneStopEstablishment
                    {
                        Name = dvEst.adoxio_name,
                        AddressStreet = dvEst.adoxio_AddressStreet,
                        AddressCity = dvEst.adoxio_AddressCity,
                        AddressPostalCode = dvEst.adoxio_AddressPostalCode
                    };
                }
            }

            OneStopLicenceType licenceType = null;
            if (dvLicence.adoxio_LicenceType?.Id is Guid ltId && ltId != Guid.Empty)
            {
                var dvLt = await _dataverse.GetLicenceTypeByIdAsync(ltId.ToString());
                if (dvLt != null)
                {
                    licenceType = new OneStopLicenceType
                    {
                        LicenceTypeId = dvLt.Id.ToString(),
                        Name = dvLt.adoxio_name,
                        OneStopProgramAccountType = dvLt.adoxio_OneStopProgramAccountType
                    };
                }
            }

            return new OneStopLicenceData
            {
                LicenceId = dvLicence.Id.ToString(),
                LicenceNumber = dvLicence.adoxio_LicenceNumber,
                BusinessProgramAccountReferenceNumber = dvLicence.adoxio_BusinessProgramAccountReferenceNumber,
                OneStopSent = dvLicence.adoxio_onestopsent,
                ExpiryDate = dvLicence.adoxio_ExpiryDate.HasValue
                    ? new DateTimeOffset(dvLicence.adoxio_ExpiryDate.Value)
                    : (DateTimeOffset?)null,
                LicenceType = licenceType,
                Establishment = est,
                Licencee = licencee
            };
        }

        private async Task UpdateQueueItemForSend(PerformContext hangfireContext, string queueItemId, string payload, string response)
        {
            if (!string.IsNullOrEmpty(queueItemId) && Guid.TryParse(queueItemId, out var queueGuid))
            {
                var patchRecord = new adoxio_onestopmessageitem
                {
                    Id = queueGuid,
                    adoxio_DateTimeSent = DateTime.Now,
                    adoxio_Payload = payload,
                    adoxio_MessageStatus = response,
                    adoxio_MessageSendStatus = (adoxio_messagestatus)OneStopMessageStatus.Sent
                };
                try
                {
                    await _dataverse.UpdateOneStopMessageItemAsync(patchRecord);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Error while updating OneStop queue item {queueItemId} {e.Message}");
                    hangfireContext?.WriteLine($"Error while updating OneStop queue item {queueItemId} {e.Message}");
                }
            }
        }

        /// <summary>
        /// Hangfire job to send Change Address message to One stop.
        /// </summary>
        public async Task SendChangeAddressRest(PerformContext hangfireContext, string licenceGuidRaw, string queueItemId)
        {
            hangfireContext?.WriteLine("Starting OneStop REST ChangeAddress Job.");

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ChangeAddress();
            var licence = await FetchLicenceForOneStop(licenceGuid);

            if (hangfireContext != null && licence != null)
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");

            if (licence == null || licence.Establishment == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence,SendChangeAddressRest,hangfireContext {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence,SendChangeAddressRest {licenceGuid}.");
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
            }
            else
            {
                var innerXml = req.CreateXML(licence);
                innerXml = _onestopRestClient.CleanXML(innerXml);
                Log.Logger?.Information(innerXml);
                hangfireContext?.WriteLine(innerXml);

                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);
                await UpdateQueueItemForSend(hangfireContext, queueItemId, innerXml, outputXML);

                hangfireContext?.WriteLine(outputXML);
                hangfireContext?.WriteLine("End of OneStop REST ChangeAddress  Job.");
            }
        }

        /// <summary>
        /// Hangfire job to send Change Name message to One stop.
        /// </summary>
        public async Task SendChangeNameRest(PerformContext hangfireContext, string licenceGuidRaw, string queueItemId, bool isTransfer, ChangeNameType changeNameType)
        {
            hangfireContext?.WriteLine("Starting OneStop REST ChangeName Job.");

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            var req = new ChangeName();
            var licence = await FetchLicenceForOneStop(licenceGuid);

            if (hangfireContext != null && licence != null)
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");

            if (licence == null || licence.Establishment == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence,SendChangeNameRest,hangfireContext {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence,SendChangeNameRest {licenceGuid}.");
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
            }
            else
            {
                string targetBusinessNumber = null;
                if (changeNameType == ChangeNameType.Transfer)
                {
                    var dvLicence = await _dataverse.GetLicenceByIdAsync(licenceGuid);
                    if (dvLicence?.adoxio_ProposedOwner?.Id is Guid proposedOwnerId && proposedOwnerId != Guid.Empty)
                    {
                        var targetOwner = await _dataverse.GetAccountByIdAsync(proposedOwnerId.ToString());
                        if (targetOwner != null)
                            targetBusinessNumber = targetOwner.AccountNumber;
                    }
                }

                var innerXml = req.CreateXML(licence, changeNameType, targetBusinessNumber);
                innerXml = _onestopRestClient.CleanXML(innerXml);
                Log.Logger?.Information(innerXml);
                hangfireContext?.WriteLine(innerXml);

                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);
                await UpdateQueueItemForSend(hangfireContext, queueItemId, innerXml, outputXML);

                hangfireContext?.WriteLine(outputXML);
                hangfireContext?.WriteLine("End of OneStop REST ChangeName  Job.");
            }
        }

        /// <summary>
        /// Hangfire job to send Change Status message to One stop.
        /// </summary>
        public async Task SendChangeStatusRest(PerformContext hangfireContext, string licenceGuidRaw, OneStopHubStatusChange statusChange, string queueItemId)
        {
            hangfireContext?.WriteLine("Starting OneStop REST ChangeStatus Job.");

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            var req = new ChangeStatus();
            var licence = await FetchLicenceForOneStop(licenceGuid);

            if (hangfireContext != null && licence != null)
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");

            if (licence == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence,SendChangeStatusRest,hangfireContext {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence,SendChangeStatusRest {licenceGuid}.");
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
            }
            else
            {
                var innerXml = req.CreateXML(licence, statusChange);
                innerXml = _onestopRestClient.CleanXML(innerXml);
                Log.Logger?.Information(innerXml);
                hangfireContext?.WriteLine(innerXml);

                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);
                await UpdateQueueItemForSend(hangfireContext, queueItemId, innerXml, outputXML);

                hangfireContext?.WriteLine(outputXML);
                hangfireContext?.WriteLine("End of OneStop REST ProgramAccountDetailsBroadcast  Job.");
            }
        }

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop using REST.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendProgramAccountRequestREST(PerformContext hangfireContext, string licenceGuidRaw, string suffix, string queueItemId)
        {
            hangfireContext?.WriteLine("Starting OneStop ProgramAccountRequest Job.");

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            var req = new ProgramAccountRequest();
            hangfireContext?.WriteLine($"Getting Licence {licenceGuid}");

            var licence = await FetchLicenceForOneStop(licenceGuid);

            if (hangfireContext != null && licence != null)
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");

            if (licence == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence {licenceGuid}.");
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
            }
            else
            {
                if (licence.OneStopSent == null || licence.OneStopSent == false)
                {
                    var innerXml = req.CreateXML(licence, suffix);
                    innerXml = _onestopRestClient.CleanXML(innerXml);
                    Log.Logger?.Information(innerXml);

                    var outputXml = await _onestopRestClient.ReceiveFromPartner(innerXml);
                    await UpdateQueueItemForSend(hangfireContext, queueItemId, innerXml, outputXml);
                    hangfireContext?.WriteLine(outputXml);
                }
                else
                {
                    hangfireContext?.WriteLine($"Skipping ProgramAccountRequest for Licence {licence.LicenceNumber} {licenceGuid} as the record is marked as sent to OneStop.");
                    Log.Logger?.Error($"Skipping ProgramAccountRequest for Licence {licence.LicenceNumber} {licenceGuid} as the record is marked as sent to OneStop.");
                    var msg = $"Failed updating OneStop queue item {queueItemId}, licence.OneStopSent is True ";
                    await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                }
            }

            hangfireContext?.WriteLine("End of OneStop ProgramAccountRequest  Job.");
        }

        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessageRest(PerformContext hangfireContext, string licenceGuidRaw)
        {
            hangfireContext?.WriteLine("Starting OneStop REST ProgramAccountDetailsBroadcast Job.");

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            var req = new ProgramAccountDetailsBroadcast();
            var licence = await FetchLicenceForOneStop(licenceGuid);

            if (hangfireContext != null && licence != null)
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");

            if (licence == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence,SendProgramAccountDetails,hangfireContext {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence,SendProgramAccountDetails {licenceGuid}.");
            }
            else
            {
                var innerXml = req.CreateXML(licence);
                innerXml = _onestopRestClient.CleanXML(innerXml);
                Log.Logger?.Information(innerXml);
                hangfireContext?.WriteLine(innerXml);

                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);
                hangfireContext?.WriteLine(outputXML);
                hangfireContext?.WriteLine("End of OneStop REST ProgramAccountDetailsBroadcast  Job.");
            }
        }


        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            hangfireContext?.WriteLine("Starting check for new OneStop queue items job.");

            IList<adoxio_onestopmessageitem> result;
            try
            {
                result = await _dataverse.GetPendingOneStopMessagesAsync();
            }
            catch (Exception odee)
            {
                hangfireContext?.WriteLine("Error getting OneStop queue items");
                throw;
            }

            int currentItem = 0;
            foreach (var queueItem in result)
            {
                string licenceId = queueItem.adoxio_Licence?.Id.ToString();
                if (!string.IsNullOrEmpty(licenceId))
                {
                    var item = await FetchLicenceForOneStop(licenceId);
                    string queueItemId = queueItem.Id.ToString();

                    try
                    {
                        var msg = $"Processing One stop message item id {queueItemId}";
                        if (queueItem.adoxio_StatusChangeDescription == null)
                        {
                            msg = $"Failed updating OneStop queue item {queueItemId}, OneStopHubStatusChange is null";
                            await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                        }
                        else
                        {
                            switch ((OneStopHubStatusChange)queueItem.adoxio_StatusChangeDescription.Value)
                            {
                                case OneStopHubStatusChange.Issued:
                                case OneStopHubStatusChange.TransferComplete:
                                case OneStopHubStatusChange.LicenseeBn9Changed:
                                case OneStopHubStatusChange.LicenseeBn9Added:
                                case OneStopHubStatusChange.LicenseeBn9Removed:
                                    if ((OneStopHubStatusChange)queueItem.adoxio_StatusChangeDescription.Value == OneStopHubStatusChange.TransferComplete)
                                    {
                                        await SendChangeStatusRest(hangfireContext, licenceId,
                                            (OneStopHubStatusChange)queueItem.adoxio_StatusChangeDescription.Value, queueItemId);
                                    }

                                    bool isAgentLicenceType = false;
                                    var agentLicenceType = await _dataverse.GetLicenceTypeByNameAsync("Agent");
                                    if (agentLicenceType != null && item?.LicenceType?.LicenceTypeId == agentLicenceType.Id.ToString())
                                        isAgentLicenceType = true;

                                    if (item?.Establishment != null || isAgentLicenceType)
                                    {
                                        string programAccountCode = "001";
                                        if (item?.BusinessProgramAccountReferenceNumber != null)
                                            programAccountCode = item.BusinessProgramAccountReferenceNumber;

                                        string cacheKey = "_BPAR_" + licenceId;
                                        string suffix = programAccountCode.TrimStart('0');
                                        if (int.TryParse(suffix, out int newNumber))
                                            newNumber += 10;
                                        else
                                            newNumber = 10;

                                        _cache.Set(cacheKey, newNumber);
                                        hangfireContext?.WriteLine($"SET key {cacheKey} to {newNumber}");

                                        await SendProgramAccountRequestREST(hangfireContext, licenceId, suffix, queueItemId);
                                    }
                                    else
                                    {
                                        msg = $"Failed updating OneStop queue item {queueItemId}, Establishment is Null or isAgentLicenceType is False Value is {isAgentLicenceType}";
                                        await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                                    }

                                    break;
                                case OneStopHubStatusChange.Cancelled:
                                case OneStopHubStatusChange.EnteredDormancy:
                                case OneStopHubStatusChange.DormancyEnded:
                                case OneStopHubStatusChange.Expired:
                                case OneStopHubStatusChange.CancellationRemoved:
                                case OneStopHubStatusChange.Renewed:
                                case OneStopHubStatusChange.Suspended:
                                case OneStopHubStatusChange.SuspensionEnded:
                                case OneStopHubStatusChange.EndorsementApproved:
                                    await SendChangeStatusRest(hangfireContext, licenceId,
                                        (OneStopHubStatusChange)queueItem.adoxio_StatusChangeDescription.Value, queueItemId);
                                    break;

                                case OneStopHubStatusChange.ChangeOfAddress:
                                    await SendChangeAddressRest(hangfireContext, licenceId, queueItemId);
                                    break;
                                case OneStopHubStatusChange.ChangeOfName:
                                    await SendChangeNameRest(hangfireContext, licenceId, queueItemId, false, ChangeNameType.ChangeName);
                                    break;
                                case OneStopHubStatusChange.ChangeOfNameThirdPartyOperator:
                                    await SendChangeNameRest(hangfireContext, licenceId, queueItemId, false, ChangeNameType.ThirdPartyOperator);
                                    break;
                                case OneStopHubStatusChange.LicenceDeemedAtTransfer:
                                    await SendChangeNameRest(hangfireContext, licenceId, queueItemId, true, ChangeNameType.Transfer);
                                    break;
                                default:
                                    msg = $"Failed updating OneStop queue item {queueItemId}, OneStopHubStatusChange is {queueItem.adoxio_StatusChangeDescription.Value} ";
                                    await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                                    break;
                            }
                        }

                        currentItem++;
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e, "Unexpected Error while processing item.");
                        var msg = $"Failed updating OneStop queue item {queueItemId}, Error is {e.Message} ";
                        await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                    }

                    if (currentItem > maxLicencesPerInterval)
                        break;
                }
                else
                {
                    var queueItemId = queueItem.Id.ToString();
                    var msg = $"Failed updating OneStop queue item {queueItemId}, queueItem.adoxio_Licence is null, there's no licence associated";
                    await UpdateOneStopMessageStatus(hangfireContext, queueItemId, msg);
                }
            }

            hangfireContext?.WriteLine("End of check for new OneStop queue items");
        }

        private async Task UpdateOneStopMessageStatus(PerformContext hangfireContext, string onestopmessageitemid, string msg)
        {
            if (!string.IsNullOrEmpty(onestopmessageitemid) && Guid.TryParse(onestopmessageitemid, out var itemGuid))
            {
                Log.Logger.Information(msg);

                var patchRecord = new adoxio_onestopmessageitem
                {
                    Id = itemGuid,
                    adoxio_MessageStatusReason = msg,
                    adoxio_MessageSendStatus = (adoxio_messagestatus)OneStopMessageStatus.Failed
                };
                try
                {
                    await _dataverse.UpdateOneStopMessageItemAsync(patchRecord);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Error while updating OneStop queue item {onestopmessageitemid} {e.Message}");
                    hangfireContext?.WriteLine($"Error while updating OneStop queue item {onestopmessageitemid} {e.Message}");
                }
            }
        }

        public static IOneStopRestClient SetupOneStopClient(IConfiguration Configuration, ILogger logger)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{Configuration["ONESTOP_HUB_USERNAME"]}:{Configuration["ONESTOP_HUB_PASSWORD"]}");
            string authorization = "Basic " + Convert.ToBase64String(byteArray);
            var client = new OneStopRestClient(new Uri(Configuration["ONESTOP_HUB_REST_URI"]), authorization, logger);
            return client;
        }

        /// <summary>
        /// Extract a guid from a partnerNote.
        /// </summary>
        public static string GetGuidFromPartnerNote(string partnerNote)
        {
            string[] parts = partnerNote.Split(",");
            string result = parts[0];
            return result;
        }

        /// <summary>
        /// Extract a suffix from a partnerNote
        /// </summary>
        public static int GetSuffixFromPartnerNote(string partnerNote, ILogger logger)
        {
            int result = 0;
            int strPos = partnerNote.LastIndexOf("-");
            if (strPos > -1)
            {
                string suffix = partnerNote.Substring(strPos + 1);
                suffix = suffix.TrimStart('0');
                if (!int.TryParse(suffix, out result))
                    logger.Error($"ERROR - unable to parse suffix of {suffix} in partner note {partnerNote}");
            }
            return result;
        }

        /// <summary>
        /// Extract a Licence Number from a partnerNote.
        /// </summary>
        public static string GetLicenceNumberFromPartnerNote(string partnerNote)
        {
            string result = null;
            string[] parts = partnerNote.Split(",");
            if (parts.Length > 1)
            {
                string secondString = parts[1];
                string[] secondParts = secondString.Split("-");
                result = secondParts[0];
            }
            return result;
        }
    }
}
