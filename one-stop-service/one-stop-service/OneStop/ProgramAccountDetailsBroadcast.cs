using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebApplicationSoap.OneStop
{
    public class ProgramAccountDetailsBroadcast
    {

        /**
         * Create Program Details Broadcast.
         * XML Message sent to the Hub broadcasting the details of the new cannabis licence issued.
         * The purpose is to broadcast licence details to partners subscribed to the Hub
         */
        public void CreateXML()
        {
            var programAccountDetailsBroadcast = new SBNProgramAccountDetailsBroadcast1();

            programAccountDetailsBroadcast.header = GetProgramAccountDetailsBroadcastHeader();
            programAccountDetailsBroadcast.body = GetProgramAccountDetailsBroadcastBody();

            var serializer = new XmlSerializer(typeof(SBNProgramAccountDetailsBroadcast1));
            using (var stream = new StreamWriter("C:\\delete\\Test_ProgramAccountDetailsBroadcast.xml"))
                serializer.Serialize(stream, programAccountDetailsBroadcast);
        }

        private SBNProgramAccountDetailsBroadcastHeader GetProgramAccountDetailsBroadcastHeader()
        {
            var header = new SBNProgramAccountDetailsBroadcastHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = "ToGetFromDynamics";
            header.CCRAHeader = GetCCRAHeader();

            return header;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeader GetCCRAHeader()
        {
            var ccraHeader = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials();

            return ccraHeader;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials GetUserCredentials()
        {
            var userCredentials = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = "ToGetFromDynamics";
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = "ToGetFromDynamics";
            //establishment (physical location of store)
            userCredentials.postalCode = "ToGetFromDynamics";
            //last name of sole proprietor (if not sole prop then null)
            userCredentials.lastName = "ToGetFromDynamics";

            return userCredentials;
        }

        private SBNProgramAccountDetailsBroadcastBody GetProgramAccountDetailsBroadcastBody()
        {
            var programAccountDetailsBroadcastBody = new SBNProgramAccountDetailsBroadcastBody();

            // BN9
            programAccountDetailsBroadcastBody.businessRegistrationNumber = "ToGetFromDynamics";
            
            // this code identifies that the message is from LCRB.  It's the same in every message from LCRB
            programAccountDetailsBroadcastBody.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            
            // reference number received on SBNCreateProgramAccountResponseBody.businessProgramAccountReferenceNumber
            programAccountDetailsBroadcastBody.businessProgramAccountReferenceNumber = "ToGetFromDynamics";
            
            // this identifies the licence type. Fixed number assigned by the OneStopHub
            programAccountDetailsBroadcastBody.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;

            programAccountDetailsBroadcastBody.businessCore = GetBusinessCore();

            programAccountDetailsBroadcastBody.programAccountStatus = GetProgramAccountStatus();
            
            // the name of the applicant(licensee)- lastName, firstName middleName or company name
            programAccountDetailsBroadcastBody.legalName = "ToGetFromDynamics";

            programAccountDetailsBroadcastBody.operatingName = getOperatingName();

            programAccountDetailsBroadcastBody.businessAddress = getBusinessAddress();

            programAccountDetailsBroadcastBody.mailingAddress = getMailingAddress();
            
            // licence number
            programAccountDetailsBroadcastBody.partnerInfo1 = "ToGetFromDynamics";
            
            // licence subtype code – not applicable to cannabis
            //programAccountDetailsBroadcastBody.partnerInfo2 = "ToGetFromDynamics";
            
            // licence expiry date
            programAccountDetailsBroadcastBody.expiryDate = "ToGetFromDynamics";

            return programAccountDetailsBroadcastBody;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessCore GetBusinessCore()
        {
            var businessCore = new SBNProgramAccountDetailsBroadcastBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = "ToGetFromDynamics";

            return businessCore;
        }

        private SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus GetProgramAccountStatus()
        {
            var programAccountStatus = new SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus();

            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now; //ToGetFromDynamics. Current date time for test purpose

            return programAccountStatus;
        }

        private SBNProgramAccountDetailsBroadcastBodyOperatingName getOperatingName()
        {
            var operatingName = new SBNProgramAccountDetailsBroadcastBodyOperatingName();

            //store name
            operatingName.operatingName = "ToGetFromDynamics";
            //only ever have 1 operating name
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;

            return operatingName;
        }

        /**
         * Business Address (physical location of the store)
         */
        private SBNProgramAccountDetailsBroadcastBodyBusinessAddress getBusinessAddress()
        {
            //physical location of the store
            var businessAddress = new SBNProgramAccountDetailsBroadcastBodyBusinessAddress();

            businessAddress.foreignLegacy = GetForeignLegacyBusiness();
            businessAddress.municipality = "ToGetFromDynamics";
            businessAddress.provinceStateCode = "ToGetFromDynamics";
            businessAddress.postalCode = "ToGetFromDynamics";
            businessAddress.countryCode = "ToGetFromDynamics";

            return businessAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness()
        {
            var foreignLegacy = new SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy();

            foreignLegacy.addressDetailLine1 = "ToGetFromDynamics";
            foreignLegacy.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacy;
        }

        /**
         * Mailing Address (for the licence)
         */
        private SBNProgramAccountDetailsBroadcastBodyMailingAddress getMailingAddress()
        {
            //mailing address for the licence
            var mailingAddress = new SBNProgramAccountDetailsBroadcastBodyMailingAddress();

            mailingAddress.foreignLegacy = GetForeignLegacyMailing();
            mailingAddress.municipality = "ToGetFromDynamics";
            mailingAddress.provinceStateCode = "ToGetFromDynamics";
            mailingAddress.postalCode = "ToGetFromDynamics";
            mailingAddress.countryCode = "ToGetFromDynamics";

            return mailingAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy GetForeignLegacyMailing()
        {
            var foreignLegacyMailing = new SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy();

            foreignLegacyMailing.addressDetailLine1 = "ToGetFromDynamics";
            foreignLegacyMailing.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacyMailing;
        }
    }
}
