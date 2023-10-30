using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using Serilog;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Gov.Jag.Lcrb.OneStopService.OneStop;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

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

        public OneStopUtils(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
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

        private void UpdateQueueItemForSend(IDynamicsClient dynamicsClient, PerformContext hangfireContext, string queueItemId, string payload, string response)
        {
            if (!string.IsNullOrEmpty(queueItemId))
            {
                MicrosoftDynamicsCRMadoxioOnestopmessageitem patchRecord =
                    new MicrosoftDynamicsCRMadoxioOnestopmessageitem()
                    {
                        AdoxioDatetimesent = DateTime.Now,
                        AdoxioPayload = payload,
                        AdoxioMessagestatus = response,
                        AdoxioMessagesendstatus= (int)OneStopMessageStatus.Sent
                    };
                try
                {
                    dynamicsClient.Onestopmessageitems.Update(queueItemId, patchRecord);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Error while updating OneStop queue item {queueItemId} {e.Message}");
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Error while updating OneStop queue item {queueItemId} {e.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Hangfire job to send Change Address message to One stop.
        /// </summary>
        public async Task SendChangeAddressRest(PerformContext hangfireContext, string licenceGuidRaw, string queueItemId)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST ChangeAddress Job.");
            }

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ChangeAddress();
            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null || licence.AdoxioEstablishment == null)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence,SendChangeAddressRest,hangfireContext {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence,SendChangeAddressRest {licenceGuid}.");
                }
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItemId, msg);
            }
            else
            {
                var innerXml = req.CreateXML(licence);
                innerXml = _onestopRestClient.CleanXML(innerXml);

                if (Log.Logger != null)
                {
                    Log.Logger.Information(innerXml);
                }

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(innerXml);
                }

                //send message to Onestop hub

                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);
                UpdateQueueItemForSend(dynamicsClient, hangfireContext, queueItemId, innerXml, outputXML);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End of OneStop REST ChangeAddress  Job.");
                }
            }
        }

        /// <summary>
        /// Hangfire job to send Change Status message to One stop.
        /// </summary>
        public async Task SendChangeNameRest(PerformContext hangfireContext, string licenceGuidRaw, string queueItemId, bool isTransfer)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST ChangeName Job.");
            }

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ChangeName();
            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null || licence.AdoxioEstablishment == null)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence,SendChangeNameRest,hangfireContext {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence,SendChangeNameRest {licenceGuid}.");
                }
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItemId, msg);
            }
            else
            {
                string targetBusinessNumber = null;
                
                ChangeNameType changeNameType = ChangeNameType.ChangeName;
                if (isTransfer && !string.IsNullOrEmpty(licence._adoxioProposedownerValue))
                {
                    changeNameType = ChangeNameType.Transfer;
                    var targetOwner = dynamicsClient.GetAccountById(licence._adoxioProposedownerValue);
                    if (targetOwner != null)
                    {
                        targetBusinessNumber = targetOwner.Accountnumber;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(licence._adoxioThirdpartyoperatoridValue))
                    {
                        changeNameType = ChangeNameType.ThirdPartyOperator;
                    }
                }

                var innerXml = req.CreateXML(licence, changeNameType, targetBusinessNumber);

                innerXml = _onestopRestClient.CleanXML(innerXml);

                if (Log.Logger != null)
                {
                    Log.Logger.Information(innerXml);
                }

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(innerXml);
                }

                //send message to Onestop hub
                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);

                UpdateQueueItemForSend(dynamicsClient, hangfireContext, queueItemId, innerXml, outputXML);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End of OneStop REST ChangeName  Job.");
                }
            }
        }

        /// <summary>
        /// Hangfire job to send Change Status message to One stop.
        /// </summary>
        public async Task SendChangeStatusRest(PerformContext hangfireContext, string licenceGuidRaw, OneStopHubStatusChange statusChange, string queueItemId)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST ChangeStatus Job.");
            }

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ChangeStatus();
            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if ( licence == null )
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence,SendChangeStatusRest,hangfireContext {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence,SendChangeStatusRest {licenceGuid}.");
                }
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItemId, msg);
            }
            else
            {


                var innerXml = req.CreateXML(licence, statusChange);
                innerXml = _onestopRestClient.CleanXML(innerXml);

                if (Log.Logger != null)
                {
                    Log.Logger.Information(innerXml);
                }

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(innerXml);
                }

                //send message to Onestop hub
                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);

                UpdateQueueItemForSend(dynamicsClient, hangfireContext, queueItemId, innerXml, outputXML);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End of OneStop REST ProgramAccountDetailsBroadcast  Job.");
                }
            }
        }

        /// <summary>
        /// Hangfire job to send LicenceCreationMessage to One stop using REST.
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task SendProgramAccountRequestREST(PerformContext hangfireContext, string licenceGuidRaw, string suffix, string queueItemId)
        {
            hangfireContext?.WriteLine("Starting OneStop ProgramAccountRequest Job.");
            
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            // prepare soap message
            var req = new ProgramAccountRequest();

            hangfireContext?.WriteLine($"Getting Licence {licenceGuid}");
            

            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null)
            {
                hangfireContext?.WriteLine($"Unable to get licence {licenceGuid}.");
                Log.Logger?.Error($"Unable to get licence {licenceGuid}.");
                var msg = $"Failed updating OneStop queue item {queueItemId}, licence is null ";
                UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItemId, msg);
            }
            else
            {
                // only send the request if Dynamics says the licence is not sent yet.
                if (licence.AdoxioOnestopsent == null || licence.AdoxioOnestopsent == false)
                {

                    var innerXml = req.CreateXML(licence, suffix);

                    innerXml = _onestopRestClient.CleanXML(innerXml);

                    Log.Logger?.Information(innerXml);
                    // send message to Onestop hub
                    var outputXml = await _onestopRestClient.ReceiveFromPartner(innerXml);

                    UpdateQueueItemForSend(dynamicsClient, hangfireContext, queueItemId, innerXml, outputXml);

                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine(outputXml);
                    }
                }
                else
                {
                    hangfireContext?.WriteLine($"Skipping ProgramAccountRequest for Licence {licence.AdoxioName} {licenceGuid} as the record is marked as sent to OneStop.");
                    Log.Logger?.Error($"Skipping ProgramAccountRequest for Licence {licence.AdoxioName} {licenceGuid} as the record is marked as sent to OneStop.");
                    var msg = $"Failed updating OneStop queue item {queueItemId}, licence.AdoxioOnestopsent is True ";
                    UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItemId, msg);
                }

            }


            hangfireContext?.WriteLine("End of OneStop ProgramAccountRequest  Job.");
            
        }



        /// <summary>
        /// Hangfire job to send LicenceDetailsMessage to One stop.
        /// </summary>
        public async Task SendProgramAccountDetailsBroadcastMessageRest(PerformContext hangfireContext, string licenceGuidRaw)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting OneStop REST ProgramAccountDetailsBroadcast Job.");
            }

            string licenceGuid = Utils.ParseGuid(licenceGuidRaw);

            //prepare soap content
            var req = new ProgramAccountDetailsBroadcast();
            var licence = dynamicsClient.GetLicenceByIdWithChildren(licenceGuid);

            if (hangfireContext != null && licence != null)
            {
                hangfireContext.WriteLine($"Got Licence {licenceGuid}.");
            }

            if (licence == null)
            {
                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine($"Unable to get licence,SendProgramAccountDetails,hangfireContext {licenceGuid}.");
                }

                if (Log.Logger != null)
                {
                    Log.Logger.Error($"Unable to get licence,SendProgramAccountDetails {licenceGuid}.");
                }
            }
            else
            {
                var innerXml = req.CreateXML(licence);

                innerXml = _onestopRestClient.CleanXML(innerXml);

                if (Log.Logger != null)
                {
                    Log.Logger.Information(innerXml);
                }

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(innerXml);
                }

                //send message to Onestop hub
                var outputXML = await _onestopRestClient.ReceiveFromPartner(innerXml);

                if (hangfireContext != null)
                {
                    hangfireContext.WriteLine(outputXML);
                    hangfireContext.WriteLine("End of OneStop REST ProgramAccountDetailsBroadcast  Job.");
                }
            }
        }


        /// <summary>
        /// Hangfire job to check for and send recent items in the queue
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public async Task CheckForNewLicences(PerformContext hangfireContext)
        {
            IDynamicsClient dynamicsClient = DynamicsSetupUtil.SetupDynamics(_configuration);
            if (hangfireContext != null)
            {
                hangfireContext.WriteLine("Starting check for new OneStop queue items job.");
            }
            IList<MicrosoftDynamicsCRMadoxioOnestopmessageitem> result;

            try
            {
                string filter = "adoxio_messagesendstatus eq 845280002";
                List<string> _orderby = new List<String>() { "createdon" };
                
                result = dynamicsClient.Onestopmessageitems.Get(filter: filter, orderby: _orderby).Value;
            }
            catch (HttpOperationException odee)
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

            int currentItem = 0;
            // now for each one process it.
            foreach (var queueItem in result)
            {
                if (!string.IsNullOrEmpty(queueItem._adoxioLicenceValue))
                {
                    var item = dynamicsClient.GetLicenceByIdWithChildren(queueItem._adoxioLicenceValue);
                    
                    string licenceId = item.AdoxioLicencesid;
                    try
                    {

                        var msg = $"Processing One stop message item id {queueItem.AdoxioOnestopmessageitemid}";
                        switch ((OneStopHubStatusChange) queueItem.AdoxioStatuschangedescription)
                        {
                            case OneStopHubStatusChange.Issued:
                            case OneStopHubStatusChange.TransferComplete:
                            case OneStopHubStatusChange.LicenseeBn9Changed:
                            case OneStopHubStatusChange.LicenseeBn9Added:
                            case OneStopHubStatusChange.LicenseeBn9Removed:
                                if ((OneStopHubStatusChange) queueItem.AdoxioStatuschangedescription ==
                                    OneStopHubStatusChange.TransferComplete)
                                {
                                    // send a change status to the old licensee
                                    await SendChangeStatusRest(hangfireContext, licenceId,
                                        (OneStopHubStatusChange) queueItem.AdoxioStatuschangedescription,
                                        queueItem.AdoxioOnestopmessageitemid);
                                }


                                // determine if it is an Agent licence type.
                                bool isAgentLicenceType = false;
                                var agentLicenceType = dynamicsClient.GetAdoxioLicencetypeByName("Agent");
                                if (agentLicenceType != null && item._adoxioLicencetypeValue == agentLicenceType.AdoxioLicencetypeid)
                                {
                                    isAgentLicenceType = true;
                                }

                                // Do not attempt to send licence records that have no establishment (for example, Marketer Licence records)
                                if (item.AdoxioEstablishment != null || isAgentLicenceType)
                                {

                                    string programAccountCode = "001";
                                    if (item.AdoxioBusinessprogramaccountreferencenumber != null)
                                    {
                                        programAccountCode = item.AdoxioBusinessprogramaccountreferencenumber;
                                    }

                                    // set the maximum code.
                                    string cacheKey = "_BPAR_" + item.AdoxioLicencesid;
                                    string suffix = programAccountCode.TrimStart('0');
                                    if (int.TryParse(suffix, out int newNumber))
                                    {
                                        newNumber += 10; // 10 tries.                           
                                    }
                                    else
                                    {
                                        newNumber = 10;
                                    }

                                    _cache.Set(cacheKey, newNumber);

                                    if (hangfireContext != null)
                                    {
                                        hangfireContext.WriteLine($"SET key {cacheKey} to {newNumber}");
                                    }

                                    await SendProgramAccountRequestREST(hangfireContext, licenceId, suffix,
                                        queueItem.AdoxioOnestopmessageitemid);

                                }else
                                {
                                    msg = $"Failed updating OneStop queue item {queueItem.AdoxioOnestopmessageitemid}, Establishment is Null or  isAgentLicenceType is False Value is {isAgentLicenceType}";
                                    UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItem.AdoxioOnestopmessageitemid, msg);
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
                                    (OneStopHubStatusChange) queueItem.AdoxioStatuschangedescription,
                                    queueItem.AdoxioOnestopmessageitemid);
                                break;

                            case OneStopHubStatusChange.ChangeOfAddress:
                                await SendChangeAddressRest(hangfireContext, licenceId,
                                    queueItem.AdoxioOnestopmessageitemid);
                                break;
                            case OneStopHubStatusChange.ChangeOfName:
                                await SendChangeNameRest(hangfireContext, licenceId,
                                    queueItem.AdoxioOnestopmessageitemid, false);

                                break;
                            case OneStopHubStatusChange.LicenceDeemedAtTransfer:
                                await SendChangeNameRest(hangfireContext, licenceId,
                                    queueItem.AdoxioOnestopmessageitemid, true);
                                break;
                            default:
                                msg = $"Failed updating OneStop queue item {queueItem.AdoxioOnestopmessageitemid}, OneStopHubStatusChange is {queueItem.AdoxioStatuschangedescription} ";
                                UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItem.AdoxioOnestopmessageitemid, msg);
                                break;
                        }

                        currentItem++;
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error(e, "Unexpected Error while processing item.");
                        var msg = $"Failed updating OneStop queue item {queueItem.AdoxioOnestopmessageitemid}, Error is {e.Message} ";
                        UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItem.AdoxioOnestopmessageitemid, msg);

                    }

                    if (currentItem > maxLicencesPerInterval)
                    {
                        break; // exit foreach    
                    }

                }
                else
                {
                    var msg = $"Failed updating OneStop queue item {queueItem.AdoxioOnestopmessageitemid}, queueItem._adoxioLicenceValue is null, there's no licence associated";
                    UpdateOneStopMessageStatus(hangfireContext, dynamicsClient, queueItem.AdoxioOnestopmessageitemid, msg);
                }
            }
            


            hangfireContext.WriteLine("End of check for new OneStop queue items");
        }

        private void UpdateOneStopMessageStatus(PerformContext hangfireContext, IDynamicsClient dynamicsClient, string onestopmessageitemid , string msg)
        {
            if (!string.IsNullOrEmpty(onestopmessageitemid))
            {
                Log.Logger.Information(msg);

                MicrosoftDynamicsCRMadoxioOnestopmessageitem patchRecord = new MicrosoftDynamicsCRMadoxioOnestopmessageitem()
                {
                    AdoxioMessagestatusreason = msg,
                    AdoxioMessagesendstatus = (int)OneStopMessageStatus.Failed
                };
                try
                {
                    dynamicsClient.Onestopmessageitems.Update(onestopmessageitemid, patchRecord);
                }
                catch (Exception e)
                {
                    Log.Logger.Error(e, $"Error while updating OneStop queue item {onestopmessageitemid} {e.Message}");
                    if (hangfireContext != null)
                    {
                        hangfireContext.WriteLine($"Error while updating OneStop queue item {onestopmessageitemid} {e.Message}");
                    }
                }
            }
        }

        public static IOneStopRestClient SetupOneStopClient(IConfiguration Configuration, ILogger logger)
        {
            //create authorization header 
            var byteArray = Encoding.ASCII.GetBytes($"{Configuration["ONESTOP_HUB_USERNAME"]}:{Configuration["ONESTOP_HUB_PASSWORD"]}");
            string authorization = "Basic " + Convert.ToBase64String(byteArray);

            //create client
            var client = new OneStopRestClient(new Uri(Configuration["ONESTOP_HUB_REST_URI"]), authorization, logger);
            return client;
        }

        /// <summary>
        /// Extract a guid from a partnerNote.
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <returns></returns>
        public static string GetGuidFromPartnerNote(string partnerNote)
        {
            string[] parts = partnerNote.Split(",");
            string result = parts[0];

            return result;
        }

        /// <summary>
        /// Extract a suffix from a partnerNote
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static int GetSuffixFromPartnerNote(string partnerNote, ILogger logger)
        {
            int result = 0;
            int strPos = partnerNote.LastIndexOf("-");
            if (strPos > -1)
            {
                string suffix = partnerNote.Substring(strPos + 1);

                suffix = suffix.TrimStart('0');
                if (!int.TryParse(suffix, out result))
                {
                    logger.Error($"ERROR - unable to parse suffix of {suffix} in partner note {partnerNote}");
                }
            }

            return result;
        }

        /// <summary>
        /// Extract a Licence Number from a partnerNote.
        /// </summary>
        /// <param name="partnerNote"></param>
        /// <returns></returns>
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
