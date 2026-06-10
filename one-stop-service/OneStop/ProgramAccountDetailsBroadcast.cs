using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ProgramAccountDetailsBroadcast
    {
        public string CreateXML(OneStopLicenceData licence)
        {
            if (licence == null)
                throw new Exception("The licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

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

        private SBNProgramAccountDetailsBroadcastHeader GetProgramAccountDetailsBroadcastHeader(OneStopLicenceData licence)
        {
            var header = new SBNProgramAccountDetailsBroadcastHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            header.partnerNote = licence.LicenceNumber;
            header.CCRAHeader = GetCCRAHeader(licence);
            return header;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeader GetCCRAHeader(OneStopLicenceData licence)
        {
            var ccraHeader = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeader();
            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);
            return ccraHeader;
        }

        private SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials GetUserCredentials(OneStopLicenceData licence)
        {
            var userCredentials = new SBNProgramAccountDetailsBroadcastHeaderCCRAHeaderUserCredentials();
            userCredentials.businessRegistrationNumber = licence.Licencee.AccountNumber;
            userCredentials.legalName = licence.Licencee.Name;
            if (licence.Establishment != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);

            if (!string.IsNullOrEmpty(licence.Licencee.PrimaryContactLastName))
                userCredentials.lastName = licence.Licencee.PrimaryContactLastName;
            else
                userCredentials.lastName = "N/A";

            return userCredentials;
        }

        private string GetPrimaryContact(OneStopLicenceData licence)
        {
            var primaryContactDetails = new PrimaryContactDetails();
            if (licence.Licencee != null)
            {
                primaryContactDetails.name = licence.Licencee.Name;
                primaryContactDetails.email = licence.Licencee.Email;
                string phoneDigitsOnly = string.Empty;
                if (licence.Licencee.Phone != null)
                    phoneDigitsOnly = Regex.Replace(licence.Licencee.Phone, "[^0-9]", "");
                primaryContactDetails.phone = phoneDigitsOnly;
            }

            using (var stringwriter = new StringWriter())
            {
                XmlSerializer serializer = new XmlSerializer(primaryContactDetails.GetType());
                serializer.Serialize(stringwriter, primaryContactDetails);
                return stringwriter.ToString();
            }
        }

        private SBNProgramAccountDetailsBroadcastBody GetProgramAccountDetailsBroadcastBody(OneStopLicenceData licence)
        {
            var body = new SBNProgramAccountDetailsBroadcastBody();
            body.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            body.businessProgramAccountReferenceNumber = licence.BusinessProgramAccountReferenceNumber;

            if (licence.LicenceType?.OneStopProgramAccountType != null)
                body.SBNProgramTypeCode = licence.LicenceType.OneStopProgramAccountType.ToString();
            else if ("Cannabis Retail Store" == licence.LicenceType?.Name)
                body.SBNProgramTypeCode = OneStopUtils.PROGRAM_TYPE_CODE_CANNABIS_RETAIL_STORE;

            body.businessCore = GetBusinessCore(licence);
            body.programAccountStatus = GetProgramAccountStatus(licence);
            body.legalName = licence.Licencee.Name;
            body.operatingName = GetOperatingName(licence);
            body.businessAddress = GetBusinessAddress(licence);
            body.mailingAddress = GetMailingAddress(licence);
            body.partnerInfo1 = licence.LicenceNumber;
            body.partnerInfo3 = GetPrimaryContact(licence);

            if (licence.ExpiryDate != null)
                body.expiryDate = licence.ExpiryDate.Value.ToString("yyyy-MM-dd");

            return body;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessCore GetBusinessCore(OneStopLicenceData licence)
        {
            var businessCore = new SBNProgramAccountDetailsBroadcastBodyBusinessCore();
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            businessCore.crossReferenceProgramNumber = licence.LicenceNumber;
            return businessCore;
        }

        private SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus GetProgramAccountStatus(OneStopLicenceData licence)
        {
            var programAccountStatus = new SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus();
            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            programAccountStatus.effectiveDate = DateTime.Now;
            return programAccountStatus;
        }

        private SBNProgramAccountDetailsBroadcastBodyOperatingName GetOperatingName(OneStopLicenceData licence)
        {
            var operatingName = new SBNProgramAccountDetailsBroadcastBodyOperatingName();
            if (licence.Establishment != null)
                operatingName.operatingName = licence.Establishment.Name;
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;
            return operatingName;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessAddress GetBusinessAddress(OneStopLicenceData licence)
        {
            var businessAddress = new SBNProgramAccountDetailsBroadcastBodyBusinessAddress();
            businessAddress.foreignLegacy = GetForeignLegacyBusiness(licence);
            if (licence.Establishment != null)
            {
                businessAddress.municipality = licence.Establishment.AddressCity;
                businessAddress.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            }
            businessAddress.provinceStateCode = "BC";
            businessAddress.countryCode = "CA";
            return businessAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(OneStopLicenceData licence)
        {
            var foreignLegacy = new SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy();
            if (licence.Establishment != null)
                foreignLegacy.addressDetailLine1 = licence.Establishment.AddressStreet;
            return foreignLegacy;
        }

        private SBNProgramAccountDetailsBroadcastBodyMailingAddress GetMailingAddress(OneStopLicenceData licence)
        {
            var mailingAddress = new SBNProgramAccountDetailsBroadcastBodyMailingAddress();
            mailingAddress.foreignLegacy = GetForeignLegacyMailing(licence);
            if (licence.Establishment != null)
            {
                mailingAddress.municipality = licence.Establishment.AddressCity;
                mailingAddress.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            }
            mailingAddress.provinceStateCode = "BC";
            mailingAddress.countryCode = "CA";
            return mailingAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy GetForeignLegacyMailing(OneStopLicenceData licence)
        {
            var foreignLegacyMailing = new SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy();
            if (licence.Establishment != null)
                foreignLegacyMailing.addressDetailLine1 = licence.Establishment.AddressStreet;
            return foreignLegacyMailing;
        }
    }
}
