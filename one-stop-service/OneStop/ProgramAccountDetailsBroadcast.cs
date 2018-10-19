using Gov.Lclb.Cllb.Interfaces.Models;
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
        public string CreateXML(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            if(licence == null)
            {
                throw new Exception("The licence can not be null");
            }
            else if (licence.AdoxioEstablishment == null)
            {
                throw new Exception("The licence must have an Establishment");
            }
            else if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
            }
            var programAccountDetailsBroadcast = new SBNProgramAccountDetailsBroadcast1();
            programAccountDetailsBroadcast.header = GetProgramAccountDetailsBroadcastHeader(licence);
            programAccountDetailsBroadcast.body = GetProgramAccountDetailsBroadcastBody(licence);

            var serializer = new XmlSerializer(typeof(SBNProgramAccountDetailsBroadcast1));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, programAccountDetailsBroadcast);
                return textWriter.ToString();
            }
        }

        private SBNProgramAccountDetailsBroadcastHeader GetProgramAccountDetailsBroadcastHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var header = new SBNProgramAccountDetailsBroadcastHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber;
            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.AdoxioBcincorporationnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioAccountId.Name;
            //establishment (physical location of store)
            userCredentials.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode;
            //last name of sole proprietor (if not sole prop then null)
            userCredentials.lastName = "N/A";

            return userCredentials;
        }

        private SBNProgramAccountDetailsBroadcastBody GetProgramAccountDetailsBroadcastBody(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var programAccountDetailsBroadcastBody = new SBNProgramAccountDetailsBroadcastBody();

            // BN9
            programAccountDetailsBroadcastBody.businessRegistrationNumber = licence.AdoxioLicencee.AdoxioBcincorporationnumber;
            
            // this code identifies that the message is from LCRB.  It's the same in every message from LCRB
            programAccountDetailsBroadcastBody.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            
            // reference number received on SBNCreateProgramAccountResponseBody.businessProgramAccountReferenceNumber
            programAccountDetailsBroadcastBody.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            
            // this identifies the licence type. Fixed number assigned by the OneStopHub
            programAccountDetailsBroadcastBody.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;

            programAccountDetailsBroadcastBody.businessCore = GetBusinessCore(licence);

            programAccountDetailsBroadcastBody.programAccountStatus = GetProgramAccountStatus(licence);
            
            // the legal name of the establishment
            programAccountDetailsBroadcastBody.legalName = licence.AdoxioAccountId.Name;

            programAccountDetailsBroadcastBody.operatingName = getOperatingName(licence);

            programAccountDetailsBroadcastBody.businessAddress = getBusinessAddress(licence);

            programAccountDetailsBroadcastBody.mailingAddress = getMailingAddress(licence);
            
            // licence number
            programAccountDetailsBroadcastBody.partnerInfo1 = licence.AdoxioLicencenumber;
            
            // licence subtype code – not applicable to cannabis
            //programAccountDetailsBroadcastBody.partnerInfo2 = "ToGetFromDynamics";
            
            // licence expiry date
            programAccountDetailsBroadcastBody.expiryDate = licence.AdoxioExpirydate.ToString();

            return programAccountDetailsBroadcastBody;
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

        private SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus GetProgramAccountStatus(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var programAccountStatus = new SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus();

            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now; //ToGetFromDynamics. Current date time for test purpose

            return programAccountStatus;
        }

        private SBNProgramAccountDetailsBroadcastBodyOperatingName getOperatingName(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var operatingName = new SBNProgramAccountDetailsBroadcastBodyOperatingName();

            //store name
            operatingName.operatingName = licence.AdoxioEstablishment.AdoxioName;
            //only ever have 1 operating name
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;

            return operatingName;
        }

        /**
         * Business Address (physical location of the store)
         */
        private SBNProgramAccountDetailsBroadcastBodyBusinessAddress getBusinessAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //physical location of the store
            var businessAddress = new SBNProgramAccountDetailsBroadcastBodyBusinessAddress();

            businessAddress.foreignLegacy = GetForeignLegacyBusiness(licence);
            businessAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            businessAddress.provinceStateCode = "BC";
            businessAddress.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode;
            businessAddress.countryCode = "CA";

            return businessAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacy = new SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy();

            foreignLegacy.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            //foreignLegacy.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacy;
        }

        /**
         * Mailing Address (for the licence)
         */
        private SBNProgramAccountDetailsBroadcastBodyMailingAddress getMailingAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //mailing address for the licence
            var mailingAddress = new SBNProgramAccountDetailsBroadcastBodyMailingAddress();

            mailingAddress.foreignLegacy = GetForeignLegacyMailing(licence);
            mailingAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            mailingAddress.provinceStateCode = "BC";
            mailingAddress.postalCode = licence.AdoxioEstablishment.AdoxioAddresspostalcode;
            mailingAddress.countryCode = "CA";

            return mailingAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy GetForeignLegacyMailing(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacyMailing = new SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy();

            foreignLegacyMailing.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            //foreignLegacyMailing.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacyMailing;
        }
    }
}
