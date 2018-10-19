using Gov.Lclb.Cllb.Interfaces.Models;
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
        public string CreateXML(MicrosoftDynamicsCRMadoxioLicences licence, string suffix)
        {
            if (licence == null)
            {
                throw new Exception("licence can not be null");
            }
            else if (licence.AdoxioEstablishment == null)
            {
                throw new Exception("The licence must have an Establishment");
            }
            else if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
            }

            var programAccountRequest = new SBNCreateProgramAccountRequest1();

            programAccountRequest.header = GetProgramAccountRequestHeader(licence, suffix);
            programAccountRequest.body = GetProgramAccountRequestBody(licence);

            var serializer = new XmlSerializer(typeof(SBNCreateProgramAccountRequest1));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, programAccountRequest);
                return textWriter.ToString();
            }
        }

        private SBNCreateProgramAccountRequestHeader GetProgramAccountRequestHeader(MicrosoftDynamicsCRMadoxioLicences licence, string suffix)
        {
            var header = new SBNCreateProgramAccountRequestHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencesid + "," + licence.AdoxioLicencenumber + "-" + suffix;
            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences application)
        {
            var ccraHeader = new SBNCreateProgramAccountRequestHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(application);

            return ccraHeader;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioAccountId.Name;
            //establishment (physical location of store)
            userCredentials.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode;
            //last name of sole proprietor (if not sole prop then null)
            userCredentials.lastName = "N/A";

            return userCredentials;
        }

        private SBNCreateProgramAccountRequestBody GetProgramAccountRequestBody(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var programAccountRequestBody = new SBNCreateProgramAccountRequestBody();

            //BN9
            programAccountRequestBody.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //this code identifies that the message is from LCRB.  It's the same in every message from LCRB
            programAccountRequestBody.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            //this identifies the licence type. Fixed number assigned by the OneStopHub
            programAccountRequestBody.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;
            programAccountRequestBody.businessCore = GetBusinessCore(licence);
            programAccountRequestBody.programAccountStatus = GetProgramAccountStatus();
            //the name of the applicant(licensee)- lastName, firstName middleName or company name
            programAccountRequestBody.legalName = licence.AdoxioAccountId.Name; 
            programAccountRequestBody.operatingName = getOperatingName(licence);
            programAccountRequestBody.businessAddress = getBusinessAddress(licence);
            programAccountRequestBody.mailingAddress = getMailingAddress(licence);

            return programAccountRequestBody;
        }

        private SBNCreateProgramAccountRequestBodyBusinessCore GetBusinessCore(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var businessCore = new SBNCreateProgramAccountRequestBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = licence.AdoxioBusinessprogramaccountreferencenumber;

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

        private SBNCreateProgramAccountRequestBodyOperatingName getOperatingName(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var operatingName = new SBNCreateProgramAccountRequestBodyOperatingName();

            //store name
            operatingName.operatingName = licence.AdoxioEstablishment.AdoxioName;
            //only ever have 1 operating name
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;

            return operatingName;
        }

        /**
         * Business Address (physical location of the store)
         */
        private SBNCreateProgramAccountRequestBodyBusinessAddress getBusinessAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //physical location of the store
            var businessAddress = new SBNCreateProgramAccountRequestBodyBusinessAddress();

            businessAddress.foreignLegacy = GetForeignLegacyBusiness(licence);
            businessAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            businessAddress.provinceStateCode = "BC"; // TODO: Verify this field
            businessAddress.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode; ;
            businessAddress.countryCode = "Canada"; // TODO: Verify this field

            return businessAddress;
        }

        private SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacy = new SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy();

            foreignLegacy.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            foreignLegacy.addressDetailLine2 = "N/A";

            return foreignLegacy;
        }

        /**
         * Mailing Address (for the licence)
         */
        private SBNCreateProgramAccountRequestBodyMailingAddress getMailingAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //mailing address for the licence
            var mailingAddress = new SBNCreateProgramAccountRequestBodyMailingAddress();

            mailingAddress.foreignLegacy = GetForeignLegacyMailing(licence);
            mailingAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            mailingAddress.provinceStateCode = "BC";
            mailingAddress.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode;
            mailingAddress.countryCode = "CA";

            return mailingAddress;
        }

        private SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy GetForeignLegacyMailing(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacyMailing = new SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy();

            foreignLegacyMailing.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            //foreignLegacyMailing.addressDetailLine2 = 

            return foreignLegacyMailing;
        }
    }
}
