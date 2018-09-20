using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebApplicationSoap.OneStop
{
    public class ProgramAccountRequest
    {
        /**
         * Create Program Account Request.
         * Initial XML message sent to the Hub requesting a new program account when a new cannabis licence is issued.
         * The purpose is to receive a Program Account Reference Number required by the Program Account Details Broadcast
         */
        public void CreateXML()
        {
            var programAccountRequest = new SBNCreateProgramAccountRequest1();

            programAccountRequest.header = GetProgramAccountRequestHeader();
            programAccountRequest.body = GetProgramAccountRequestBody();

            var serializer = new XmlSerializer(typeof(SBNCreateProgramAccountRequest1));
            using (var stream = new StreamWriter("C:\\delete\\Test_ProgramAccountRequest.xml"))
                serializer.Serialize(stream, programAccountRequest);
        }

        private SBNCreateProgramAccountRequestHeader GetProgramAccountRequestHeader()
        {
            var header = new SBNCreateProgramAccountRequestHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = "ToGetFromDynamics";
            header.CCRAHeader = GetCCRAHeader();

            return header;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeader GetCCRAHeader()
        {
            var ccraHeader = new SBNCreateProgramAccountRequestHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials();

            return ccraHeader;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials GetUserCredentials()
        {
            var userCredentials = new SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials();

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

        private SBNCreateProgramAccountRequestBody GetProgramAccountRequestBody()
        {
            var programAccountRequestBody = new SBNCreateProgramAccountRequestBody();

            //BN9
            programAccountRequestBody.businessRegistrationNumber = "ToGetFromDynamics";
            //this code identifies that the message is from LCRB.  It's the same in every message from LCRB
            programAccountRequestBody.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            //this identifies the licence type. Fixed number assigned by the OneStopHub
            programAccountRequestBody.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;
            programAccountRequestBody.businessCore = GetBusinessCore();
            programAccountRequestBody.programAccountStatus = GetProgramAccountStatus();
            //the name of the applicant(licensee)- lastName, firstName middleName or company name
            programAccountRequestBody.legalName = "ToGetFromDynamics";
            programAccountRequestBody.operatingName = getOperatingName();
            programAccountRequestBody.businessAddress = getBusinessAddress();
            programAccountRequestBody.mailingAddress = getMailingAddress();

            return programAccountRequestBody;
        }

        private SBNCreateProgramAccountRequestBodyBusinessCore GetBusinessCore()
        {
            var businessCore = new SBNCreateProgramAccountRequestBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = "ToGetFromDynamics";

            return businessCore;
        }

        private SBNCreateProgramAccountRequestBodyProgramAccountStatus GetProgramAccountStatus()
        {
            var programAccountStatus = new SBNCreateProgramAccountRequestBodyProgramAccountStatus();

            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now; //ToGetFromDynamics. Current date time for test purpose

            return programAccountStatus;
        }

        private SBNCreateProgramAccountRequestBodyOperatingName getOperatingName()
        {
            var operatingName = new SBNCreateProgramAccountRequestBodyOperatingName();

            //store name
            operatingName.operatingName = "ToGetFromDynamics";
            //only ever have 1 operating name
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;

            return operatingName;
        }

        /**
         * Business Address (physical location of the store)
         */
        private SBNCreateProgramAccountRequestBodyBusinessAddress getBusinessAddress()
        {
            //physical location of the store
            var businessAddress = new SBNCreateProgramAccountRequestBodyBusinessAddress();

            businessAddress.foreignLegacy = GetForeignLegacyBusiness();
            businessAddress.municipality = "ToGetFromDynamics";
            businessAddress.provinceStateCode = "ToGetFromDynamics";
            businessAddress.postalCode = "ToGetFromDynamics";
            businessAddress.countryCode = "ToGetFromDynamics";

            return businessAddress;
        }

        private SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness()
        {
            var foreignLegacy = new SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy();

            foreignLegacy.addressDetailLine1 = "ToGetFromDynamics";
            foreignLegacy.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacy;
        }

        /**
         * Mailing Address (for the licence)
         */
        private SBNCreateProgramAccountRequestBodyMailingAddress getMailingAddress()
        {
            //mailing address for the licence
            var mailingAddress = new SBNCreateProgramAccountRequestBodyMailingAddress();

            mailingAddress.foreignLegacy = GetForeignLegacyMailing();
            mailingAddress.municipality = "ToGetFromDynamics";
            mailingAddress.provinceStateCode = "ToGetFromDynamics";
            mailingAddress.postalCode = "ToGetFromDynamics";
            mailingAddress.countryCode = "ToGetFromDynamics";

            return mailingAddress;
        }

        private SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy GetForeignLegacyMailing()
        {
            var foreignLegacyMailing = new SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy();

            foreignLegacyMailing.addressDetailLine1 = "ToGetFromDynamics";
            foreignLegacyMailing.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacyMailing;
        }
    }
}
