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
        public string CreateXML(MicrosoftDynamicsCRMadoxioLicences licence, bool isTransfer, string targetBusinessNumber)
        {
            if (licence == null)
            {
                throw new Exception("The licence can not be null");
            }

            if (licence.AdoxioLicencee == null)
            {
                throw new Exception("The licence must have an AdoxioLicencee");
            }

            if (licence.AdoxioBusinessprogramaccountreferencenumber == null)
            {
                // set to 1 and handle errors as encountered.
                licence.AdoxioBusinessprogramaccountreferencenumber = "1";
            }
            var sbnChangeStatus = new SBNChangeName();
            sbnChangeStatus.header = GetHeader(licence, isTransfer);


            sbnChangeStatus.body = GetBody(licence, isTransfer, targetBusinessNumber);

            var serializer = new XmlSerializer(typeof(SBNChangeName));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeStatus);
                return textWriter.ToString();
            }
        }

        private SBNChangeNameHeader GetHeader(MicrosoftDynamicsCRMadoxioLicences licence, bool isTransfer)
        {
            var header = new SBNChangeNameHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            if (isTransfer)
            {
                header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGENAME_TRANSFER;
            }
            else
            {
                header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGENAME;
            }
            


            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber + "-" + DateTime.Now.Ticks;

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
            if (licence.AdoxioEstablishment != null)
            {
                userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            }
            return userCredentials;
        }

        private SBNChangeNameBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence, bool isTransfer, string targetBusinessNumber)
        {
            var body = new SBNChangeNameBody();

            body.name = new SBNChangeNameBodyName();
            body.name.clientNameTypeCode = OneStopUtils.CLIENT_NAME_TYPE_CODE;
            if (licence.AdoxioEstablishment != null)
            {
                body.name.name = licence.AdoxioEstablishment.AdoxioName;
            }
            
            body.name.operatingNamesequenceNumber = 1;
            body.name.updateReasonCode = OneStopUtils.UPDATE_REASON_CODE;

            body.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;

            body.businessProgramAccountReferenceNumber = licence.AdoxioBusinessprogramaccountreferencenumber;
            
            // partnerInfo1
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            if (isTransfer)
            {
                body.partnerInfo2 = targetBusinessNumber;
            }

            // 
            body.timeStamp = Utils.GetTimeStamp();

            return body;
        }

    }
}
