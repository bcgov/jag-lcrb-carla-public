using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeStatus
    {

        /**
         * Create Program Details Broadcast.
         * XML Message sent to the Hub broadcasting the details of the new cannabis licence issued.
         * The purpose is to broadcast licence details to partners subscribed to the Hub
         */
        public string CreateXML(MicrosoftDynamicsCRMadoxioLicences licence, OneStopHubStatusChange statusChange)
        {
            if (licence == null)
            {
                throw new Exception("The licence can not be null");
            }

            if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
            }

            if (licence.AdoxioBusinessprogramaccountreferencenumber == null)
            {
                // set to 1 and handle errors as encountered.
                licence.AdoxioBusinessprogramaccountreferencenumber = "1";
            }
            var sbnChangeStatus = new SBNChangeStatus();
            sbnChangeStatus.header = GetHeader(licence);
            sbnChangeStatus.body = GetBody(licence, statusChange);

            var serializer = new XmlSerializer(typeof(SBNChangeStatus));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeStatus);
                return textWriter.ToString();
            }
        }

        private SBNChangeStatusHeader GetHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var header = new SBNChangeStatusHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGESTATUS;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber + "-" + DateTime.Now.Ticks;

            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNChangeStatusHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNChangeStatusHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNChangeStatusHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNChangeStatusHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioLicencee.Name;
            //establishment (physical location of store)

            if (licence.AdoxioEstablishment != null)
            {
                userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            }
            
            return userCredentials;
        }


        private SBNChangeStatusBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence, OneStopHubStatusChange statusChange)
        {
            var body = new SBNChangeStatusBody();

            // licence number
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            body.statusData = new SBNChangeStatusBodyStatusData();


            body.statusData.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            body.statusData.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            body.statusData.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            // programAccountStatus
            body.statusData.programAccountStatus = GetProgramAccountStatus(licence, statusChange);

            // partnerInfo1
            body.partnerInfo1 = licence.AdoxioLicencenumber;
            
            // partnerInfo2 - date
            if (licence.AdoxioExpirydate != null)
            {
                body.partnerInfo2 = licence.AdoxioExpirydate.Value.DateTime; // may need to be ("yyyy-MM-dd");
            }
            

            body.statusData.timeStamp = DateTime.Now.ToString("yyyy-MM-DD-hh.mm.ss");

            return body;
        }


        private SBNChangeStatusBodyStatusDataProgramAccountStatus GetProgramAccountStatus(MicrosoftDynamicsCRMadoxioLicences licence, OneStopHubStatusChange statusChange)
        {
            var programAccountStatus = new SBNChangeStatusBodyStatusDataProgramAccountStatus();

            switch (statusChange)
            {
                case OneStopHubStatusChange.Cancelled:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED; // 02
                    programAccountStatus.programAccountReasonCode = "111";
                    break;

                case OneStopHubStatusChange.CancellationRemoved: 
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;

                case OneStopHubStatusChange.Expired:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    programAccountStatus.programAccountReasonCode = "112";
                    break;

                case OneStopHubStatusChange.Renewed:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;

                case OneStopHubStatusChange.Suspended:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    programAccountStatus.programAccountReasonCode = "114";
                    break;

                case OneStopHubStatusChange.SuspensionEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    // programAccountReasonCode = null
                    break;

                case OneStopHubStatusChange.TransferComplete:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    programAccountStatus.programAccountReasonCode = "113";
                    break;

                case OneStopHubStatusChange.EnteredDormancy:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    programAccountStatus.programAccountReasonCode = "115";
                    
                    break;

                case OneStopHubStatusChange.DormancyEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;
            }

            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now.AddHours(-8); //ToGetFromDynamics. Current date time for test purposes, adjusted for Pacific time.

            return programAccountStatus;
        }

    }
}
