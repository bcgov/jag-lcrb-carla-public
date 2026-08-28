using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ProgramAccountRequest
    {
        public string CreateXML(OneStopLicenceData licence, string suffix)
        {
            if (licence == null)
                throw new Exception("licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

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

        private SBNCreateProgramAccountRequestHeader GetProgramAccountRequestHeader(OneStopLicenceData licence, string suffix)
        {
            var header = new SBNCreateProgramAccountRequestHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            header.partnerNote = licence.LicenceId + "," + licence.LicenceNumber + "-" + suffix;
            header.CCRAHeader = GetCCRAHeader(licence);
            return header;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeader GetCCRAHeader(OneStopLicenceData licence)
        {
            var ccraHeader = new SBNCreateProgramAccountRequestHeaderCCRAHeader();
            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);
            return ccraHeader;
        }

        private SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials GetUserCredentials(OneStopLicenceData licence)
        {
            var userCredentials = new SBNCreateProgramAccountRequestHeaderCCRAHeaderUserCredentials();
            userCredentials.businessRegistrationNumber = licence.Licencee.AccountNumber;
            userCredentials.legalName = licence.Licencee.Name;

            if (licence.Establishment != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            else if (licence.Licencee != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Licencee.Address1PostalCode);

            userCredentials.lastName = "N/A";
            return userCredentials;
        }

        private SBNCreateProgramAccountRequestBody GetProgramAccountRequestBody(OneStopLicenceData licence, string suffix)
        {
            var body = new SBNCreateProgramAccountRequestBody();
            body.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            if (licence.LicenceType?.OneStopProgramAccountType != null)
                body.SBNProgramTypeCode = licence.LicenceType.OneStopProgramAccountType.ToString();
            else if ("Cannabis Retail Store" == licence.LicenceType?.Name)
                body.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;

            body.businessCore = GetBusinessCore(licence, suffix);
            body.programAccountStatus = GetProgramAccountStatus();
            body.legalName = licence.Licencee.Name;
            body.operatingName = GetOperatingName(licence);
            body.businessAddress = GetBusinessAddress(licence);
            body.mailingAddress = GetMailingAddress(licence);
            return body;
        }

        private SBNCreateProgramAccountRequestBodyBusinessCore GetBusinessCore(OneStopLicenceData licence, string suffix)
        {
            var businessCore = new SBNCreateProgramAccountRequestBodyBusinessCore();
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            businessCore.crossReferenceProgramNumber = licence.LicenceNumber + "-" + suffix;
            return businessCore;
        }

        private SBNCreateProgramAccountRequestBodyProgramAccountStatus GetProgramAccountStatus()
        {
            var programAccountStatus = new SBNCreateProgramAccountRequestBodyProgramAccountStatus();
            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            programAccountStatus.effectiveDate = DateTime.Now;
            return programAccountStatus;
        }

        private SBNCreateProgramAccountRequestBodyOperatingName GetOperatingName(OneStopLicenceData licence)
        {
            var operatingName = new SBNCreateProgramAccountRequestBodyOperatingName();
            if (licence.Establishment != null)
                operatingName.operatingName = licence.Establishment.Name;
            else if (licence.Licencee != null)
                operatingName.operatingName = licence.Licencee.Name;
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;
            return operatingName;
        }

        private SBNCreateProgramAccountRequestBodyBusinessAddress GetBusinessAddress(OneStopLicenceData licence)
        {
            var businessAddress = new SBNCreateProgramAccountRequestBodyBusinessAddress();
            businessAddress.foreignLegacy = GetForeignLegacyBusiness(licence);

            if (licence.Establishment != null)
            {
                businessAddress.municipality = licence.Establishment.AddressCity;
                businessAddress.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            }
            else if (licence.Licencee != null)
            {
                businessAddress.municipality = licence.Licencee.Address1City;
                businessAddress.postalCode = Utils.FormatPostalCode(licence.Licencee.Address1PostalCode);
            }

            businessAddress.provinceStateCode = "BC";
            businessAddress.countryCode = "CA";
            return businessAddress;
        }

        private SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(OneStopLicenceData licence)
        {
            var foreignLegacy = new SBNCreateProgramAccountRequestBodyBusinessAddressForeignLegacy();
            if (licence.Establishment != null)
            {
                foreignLegacy.addressDetailLine1 = licence.Establishment.AddressStreet;
                foreignLegacy.addressDetailLine2 = "N/A";
            }
            else if (licence.Licencee != null)
            {
                foreignLegacy.addressDetailLine1 = licence.Licencee.Address1Line1;
                foreignLegacy.addressDetailLine2 = "N/A";
            }
            return foreignLegacy;
        }

        private SBNCreateProgramAccountRequestBodyMailingAddress GetMailingAddress(OneStopLicenceData licence)
        {
            var mailingAddress = new SBNCreateProgramAccountRequestBodyMailingAddress();
            mailingAddress.foreignLegacy = GetForeignLegacyMailing(licence);

            if (licence.Establishment != null)
            {
                mailingAddress.municipality = licence.Establishment.AddressCity;
                mailingAddress.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            }
            else if (licence.Licencee != null)
            {
                mailingAddress.municipality = licence.Licencee.Address1City;
                mailingAddress.postalCode = Utils.FormatPostalCode(licence.Licencee.Address1PostalCode);
            }

            mailingAddress.provinceStateCode = "BC";
            mailingAddress.countryCode = "CA";
            return mailingAddress;
        }

        private SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy GetForeignLegacyMailing(OneStopLicenceData licence)
        {
            var foreignLegacyMailing = new SBNCreateProgramAccountRequestBodyMailingAddressForeignLegacy();
            if (licence.Establishment != null)
                foreignLegacyMailing.addressDetailLine1 = licence.Establishment.AddressStreet;
            else if (licence.Licencee != null)
                foreignLegacyMailing.addressDetailLine1 = licence.Licencee.Address1Line1;
            return foreignLegacyMailing;
        }
    }
}
