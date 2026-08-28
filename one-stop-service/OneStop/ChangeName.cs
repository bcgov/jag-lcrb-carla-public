using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeName
    {
        public string CreateXML(OneStopLicenceData licence, ChangeNameType changeNameType, string targetBusinessNumber)
        {
            if (licence == null)
                throw new Exception("The licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

            if (licence.BusinessProgramAccountReferenceNumber == null)
                licence.BusinessProgramAccountReferenceNumber = "1";

            var sbnChangeName = new SBNChangeName();
            sbnChangeName.header = GetHeader(licence, changeNameType);
            sbnChangeName.body = GetBody(licence, changeNameType, targetBusinessNumber);

            var serializer = new XmlSerializer(typeof(SBNChangeName));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeName);
                return textWriter.ToString();
            }
        }

        private SBNChangeNameHeader GetHeader(OneStopLicenceData licence, ChangeNameType changeNameType)
        {
            var header = new SBNChangeNameHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            switch (changeNameType)
            {
                case ChangeNameType.ChangeName:
                    header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGENAME;
                    break;
                case ChangeNameType.ThirdPartyOperator:
                    header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGENAME_THIRDPARTY;
                    break;
                case ChangeNameType.Transfer:
                    header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGENAME_TRANSFER;
                    break;
            }
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            header.partnerNote = licence.LicenceNumber + "-" + DateTime.Now.Ticks;
            header.CCRAHeader = GetCCRAHeader(licence);
            return header;
        }

        private SBNChangeNameHeaderCCRAHeader GetCCRAHeader(OneStopLicenceData licence)
        {
            var ccraHeader = new SBNChangeNameHeaderCCRAHeader();
            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);
            return ccraHeader;
        }

        private SBNChangeNameHeaderCCRAHeaderUserCredentials GetUserCredentials(OneStopLicenceData licence)
        {
            var userCredentials = new SBNChangeNameHeaderCCRAHeaderUserCredentials();
            userCredentials.businessRegistrationNumber = licence.Licencee.AccountNumber;
            userCredentials.legalName = licence.Licencee.Name;
            if (licence.Establishment != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            return userCredentials;
        }

        private SBNChangeNameBody GetBody(OneStopLicenceData licence, ChangeNameType changeNameType, string targetBusinessNumber)
        {
            var body = new SBNChangeNameBody();
            body.name = new SBNChangeNameBodyName();
            body.name.clientNameTypeCode = OneStopUtils.CLIENT_NAME_TYPE_CODE;
            if (licence.Establishment != null)
                body.name.name = licence.Establishment.Name;
            body.name.operatingNamesequenceNumber = 1;
            body.name.updateReasonCode = OneStopUtils.UPDATE_REASON_CODE;
            body.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            body.businessProgramAccountReferenceNumber = licence.BusinessProgramAccountReferenceNumber;
            body.partnerInfo1 = licence.LicenceNumber;
            if (changeNameType == ChangeNameType.Transfer)
                body.partnerInfo2 = targetBusinessNumber;
            body.timeStamp = Utils.GetTimeStamp();
            return body;
        }
    }
}
