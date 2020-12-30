using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeAddress
    {

        /**
         * Create Program Details Broadcast.
         * XML Message sent to the Hub broadcasting the details of the new cannabis licence issued.
         * The purpose is to broadcast licence details to partners subscribed to the Hub
         */
        public string CreateXML(MicrosoftDynamicsCRMadoxioLicences licence)
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
            var sbnChangeAddress = new SBNChangeAddress();
            sbnChangeAddress.header = GetHeader(licence);
            sbnChangeAddress.body = GetBody(licence);

            var serializer = new XmlSerializer(typeof(SBNChangeName));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeAddress);
                return textWriter.ToString();
            }
        }

        private SBNChangeAddressHeader GetHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var header = new SBNChangeAddressHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.NAMECHANGE_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber;

            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNChangeAddressHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNChangeAddressHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNChangeAddressHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNChangeAddressHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioLicencee.Name;
            //establishment (physical location of store)
            userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);

            return userCredentials;
        }


        private SBNChangeAddressBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var body = new SBNChangeAddressBody();

            // licence number
            body.partnerInfo1 = licence.AdoxioLicencenumber;
            body.addressTypeCode = OneStopUtils.ADDRESS_TYPE_CODE;
            body.updateReasonCode = OneStopUtils.UPDATE_REASON_CODE_ADDRESS;
            body.address = new SBNChangeAddressBodyAddress();
            body.address.foreignLegacy = new SBNChangeAddressBodyAddressForeignLegacy();
            body.address.foreignLegacy.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            body.address.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            body.address.provinceStateCode = OneStopUtils.PROVINCE_STATE_CODE;
            body.address.countryCode = OneStopUtils.COUNTRY_CODE;
            body.businessRegistrationNumber = licence.AdoxioLicencee.AdoxioBusinessregistrationnumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            body.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            
            // partnerInfo1
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            return body;
        }
    }
}
