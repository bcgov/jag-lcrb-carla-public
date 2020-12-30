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

            if (licence.AdoxioEstablishment == null)
            {
                throw new Exception("The licence must have an Establishment");
            }

            if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
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
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber;

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
            userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            
            return userCredentials;
        }

        string GetPrimaryContact(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            // first create an XML object.
            var primaryContactDetails = new PrimaryContactDetails();

            if (licence.AdoxioLicencee != null)
            {
                primaryContactDetails.name = licence.AdoxioLicencee.Name;
                primaryContactDetails.email = licence.AdoxioLicencee.Emailaddress1;

                // 2019-07-11 - LDB has requested that the phone number only contain digits.

                string phoneDigitsOnly = "";

                if (licence.AdoxioLicencee.Telephone1 != null)
                {
                    phoneDigitsOnly = Regex.Replace(licence.AdoxioLicencee.Telephone1, "[^0-9]", "");
                }


                primaryContactDetails.phone = phoneDigitsOnly;
            }

            // convert the XML to a string.
            using (var stringwriter = new StringWriter())
            {

                XmlSerializer serializer = new XmlSerializer(primaryContactDetails.GetType());
                serializer.Serialize(stringwriter, primaryContactDetails);
                return stringwriter.ToString();
            }
        }

        private SBNChangeStatusBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence, OneStopHubStatusChange statusChange)
        {
            var body = new SBNChangeStatusBody();

            // licence number
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            body.statusData = new SBNChangeStatusBodyStatusData();


            body.statusData.businessRegistrationNumber = licence.AdoxioLicencee.AdoxioBusinessregistrationnumber;
            body.statusData.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            body.statusData.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            // programAccountStatus
            body.statusData.programAccountStatus = GetProgramAccountStatus(licence, statusChange);

            // partnerInfo1
            body.partnerInfo1 = licence.AdoxioLicencenumber;
            
            // partnerInfo2 - date

            return body;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessCore GetBusinessCore(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var businessCore = new SBNProgramAccountDetailsBroadcastBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = licence.AdoxioLicencenumber;

            return businessCore;
        }

        private SBNChangeStatusBodyStatusDataProgramAccountStatus GetProgramAccountStatus(MicrosoftDynamicsCRMadoxioLicences licence, OneStopHubStatusChange statusChange)
        {
            var programAccountStatus = new SBNChangeStatusBodyStatusDataProgramAccountStatus();

            switch (statusChange)
            {
                case OneStopHubStatusChange.Cancelled:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED; // 02
                    //  programAccountReasonCode = 111
                    break;

                case OneStopHubStatusChange.Expired:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    // programAccountReasonCode = 112 (expired)
                    break;

                case OneStopHubStatusChange.Renewed:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    // programAccountReasonCode = 112 (expired)
                    break;

                case OneStopHubStatusChange.Suspended:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    // programAccountReasonCode = 114 (expired)
                    break;

                case OneStopHubStatusChange.SuspensionEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    // programAccountReasonCode = null
                    break;

                case OneStopHubStatusChange.TransferComplete:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    // programAccountReasonCode = 113 (transfer)
                    break;

                case OneStopHubStatusChange.EnteredDormancy:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    // programAccountReasonCode = 115 (dormant)
                    break;

                case OneStopHubStatusChange.DormancyEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    // programAccountReasonCode = null
                    break;
            }

            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now; //ToGetFromDynamics. Current date time for test purpose

            return programAccountStatus;
        }

    }
}
