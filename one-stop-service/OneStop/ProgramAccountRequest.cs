using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
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


            if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
            }
            var programAccountRequest = new SBNCreateProgramAccountRequest1();

            programAccountRequest.header = GetProgramAccountRequestHeader(licence, suffix);
            programAccountRequest.body = GetProgramAccountRequestBody(licence, suffix);

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

        private SBNCreateProgramAccountRequestHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNCreateProgramAccountRequestHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioLicencee.Name;
            //establishment (physical location of store)

            if (licence.AdoxioEstablishment != null)
            {
                userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            }
            else // attempt to get from the Account
            {
                if (licence.AdoxioLicencee != null)
                {
                    userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioLicencee.Address1Postalcode);
                }
            }
            //last name of sole proprietor (if not sole prop then null)
            userCredentials.lastName = "N/A";

            return userCredentials;
        }

        private SBNCreateProgramAccountRequestBody GetProgramAccountRequestBody(MicrosoftDynamicsCRMadoxioLicences licence, string suffix)
        {
            var programAccountRequestBody = new SBNCreateProgramAccountRequestBody();

            //BN9
            programAccountRequestBody.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //this code identifies that the message is from LCRB.  It's the same in every message from LCRB
            programAccountRequestBody.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            //this identifies the licence type. 
            if (licence?.AdoxioLicenceType?.AdoxioOnestopprogramaccounttype != null)
            {
                programAccountRequestBody.SBNProgramTypeCode = licence?.AdoxioLicenceType?.AdoxioOnestopprogramaccounttype.ToString();
            }
            else
            {
                if ("Cannabis Retail Store" == licence?.AdoxioLicenceType?.AdoxioName)
                {
                    programAccountRequestBody.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;
                }
            }

            programAccountRequestBody.businessCore = GetBusinessCore(licence, suffix);
            programAccountRequestBody.programAccountStatus = GetProgramAccountStatus();
            //the name of the applicant(licensee)- lastName, firstName middleName or company name
            programAccountRequestBody.legalName = licence.AdoxioLicencee.Name;
            programAccountRequestBody.operatingName = getOperatingName(licence);
            programAccountRequestBody.businessAddress = getBusinessAddress(licence);
            programAccountRequestBody.mailingAddress = getMailingAddress(licence);

            return programAccountRequestBody;
        }

        private SBNCreateProgramAccountRequestBodyBusinessCore GetBusinessCore(MicrosoftDynamicsCRMadoxioLicences licence, string suffix)
        {
            var businessCore = new SBNCreateProgramAccountRequestBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = licence.AdoxioLicencenumber + "-" + suffix;

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

            if (licence.AdoxioEstablishment != null)
            {
                //store name
                operatingName.operatingName = licence.AdoxioEstablishment.AdoxioName;
            }
            else // attempt to get from the Account
            {
                if (licence.AdoxioLicencee != null)
                {
                    operatingName.operatingName = licence.AdoxioLicencee.Name;
                }
            }
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

            if (licence.AdoxioEstablishment != null)
            {

                businessAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
                businessAddress.postalCode =
                    Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            }
            else // attempt to get from the Account
            {
                businessAddress.municipality = licence.AdoxioLicencee.Address1City;
                businessAddress.postalCode =
                    Utils.FormatPostalCode(licence.AdoxioLicencee.Address1Postalcode);
            }


            businessAddress.provinceStateCode = "BC"; // BC is province code for British Columbia
            
            businessAddress.countryCode = "CA"; // CA is country code for Canada

            return businessAddress;
        }

        private SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacy = new SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy();

            if (licence.AdoxioEstablishment != null)
            {
                foreignLegacy.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
                foreignLegacy.addressDetailLine2 = "N/A";
            }
            else // attempt to get from the Account
            {
                foreignLegacy.addressDetailLine1 = licence.AdoxioLicencee.Address1Line1;
                foreignLegacy.addressDetailLine2 = "N/A";
            }

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

            if (licence.AdoxioEstablishment != null)
            {
                mailingAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
                mailingAddress.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            }
            else // attempt to get from the Account
            {
                mailingAddress.municipality = licence.AdoxioLicencee.Address1City;
                mailingAddress.postalCode = Utils.FormatPostalCode(licence.AdoxioLicencee.Address1Postalcode);
            }
            mailingAddress.provinceStateCode = "BC";
            
            mailingAddress.countryCode = "CA";

            return mailingAddress;
        }

        private SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy GetForeignLegacyMailing(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacyMailing = new SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy();

            if (licence.AdoxioEstablishment != null)
            {
                foreignLegacyMailing.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            }
            else // attempt to get from the Account
            {
                foreignLegacyMailing.addressDetailLine1 = licence.AdoxioLicencee.Address1Line1;
            }
            //foreignLegacyMailing.addressDetailLine2 = 

            return foreignLegacyMailing;
        }

    }
}
