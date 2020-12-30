using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeName
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
            var sbnChangeStatus = new SBNChangeName();
            sbnChangeStatus.header = GetHeader(licence);
            sbnChangeStatus.body = GetBody(licence);

            var serializer = new XmlSerializer(typeof(SBNChangeName));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeStatus);
                return textWriter.ToString();
            }
        }

        private SBNChangeNameHeader GetHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var header = new SBNChangeNameHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.NAMECHANGE_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber;

            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNChangeNameHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNChangeNameHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNChangeNameHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNChangeNameHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioLicencee.Name;
            //establishment (physical location of store)
            userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);

            return userCredentials;
        }

        private SBNChangeNameBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var body = new SBNChangeNameBody();

            // licence number
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            body.name = new SBNChangeNameBodyName();
            body.name.clientNameTypeCode = OneStopUtils.CLIENT_NAME_TYPE_CODE;
            body.name.name = licence.AdoxioEstablishment.AdoxioName;
            body.name.operatingNamesequenceNumber = 1;
            body.name.updateReasonCode = OneStopUtils.UPDATE_REASON_CODE;

            body.businessRegistrationNumber = licence.AdoxioLicencee.AdoxioBusinessregistrationnumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            body.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            
            // partnerInfo1
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            return body;
        }

    }
}
