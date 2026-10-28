using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeAddress
    {
        public string CreateXML(OneStopLicenceData licence)
        {
            if (licence == null)
                throw new Exception("The licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

            if (licence.BusinessProgramAccountReferenceNumber == null)
                licence.BusinessProgramAccountReferenceNumber = "1";

            var sbnChangeAddress = new SBNChangeAddress();
            sbnChangeAddress.header = GetHeader(licence);
            sbnChangeAddress.body = GetBody(licence);

            var serializer = new XmlSerializer(typeof(SBNChangeAddress));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeAddress);
                return textWriter.ToString();
            }
        }

        private SBNChangeAddressHeader GetHeader(OneStopLicenceData licence)
        {
            var header = new SBNChangeAddressHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGEADDRESS;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            header.partnerNote = licence.LicenceNumber + "-" + DateTime.Now.Ticks;
            header.CCRAHeader = GetCCRAHeader(licence);
            return header;
        }

        private SBNChangeAddressHeaderCCRAHeader GetCCRAHeader(OneStopLicenceData licence)
        {
            var ccraHeader = new SBNChangeAddressHeaderCCRAHeader();
            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);
            return ccraHeader;
        }

        private SBNChangeAddressHeaderCCRAHeaderUserCredentials GetUserCredentials(OneStopLicenceData licence)
        {
            var userCredentials = new SBNChangeAddressHeaderCCRAHeaderUserCredentials();
            userCredentials.businessRegistrationNumber = licence.Licencee.AccountNumber;
            userCredentials.legalName = licence.Licencee.Name;
            if (licence.Establishment != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            return userCredentials;
        }

        private SBNChangeAddressBody GetBody(OneStopLicenceData licence)
        {
            var body = new SBNChangeAddressBody();
            body.partnerInfo1 = licence.LicenceNumber;
            body.addressTypeCode = OneStopUtils.ADDRESS_TYPE_CODE;
            body.updateReasonCode = OneStopUtils.UPDATE_REASON_CODE_ADDRESS;
            body.address = new SBNChangeAddressBodyAddress();
            body.address.foreignLegacy = new SBNChangeAddressBodyAddressForeignLegacy();

            if (licence.Establishment != null)
            {
                body.address.foreignLegacy.addressDetailLine1 = licence.Establishment.AddressStreet;
                body.address.municipality = licence.Establishment.AddressCity;
                body.address.postalCode = licence.Establishment.AddressPostalCode;
            }

            body.address.provinceStateCode = OneStopUtils.PROVINCE_STATE_CODE;
            body.address.countryCode = OneStopUtils.COUNTRY_CODE;
            body.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            body.address.effectiveDate = DateTime.Now;
            body.businessProgramAccountReferenceNumber = licence.BusinessProgramAccountReferenceNumber;
            body.timeStamp = Utils.GetTimeStamp();

            return body;
        }
    }
}
