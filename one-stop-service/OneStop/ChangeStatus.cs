extern alias DV;
using Gov.Jag.Lcrb.OneStopService;
using OneStopHubStatusChange = DV::Gov.Lclb.Cllb.Interfaces.OneStopHubStatusChange;
using System;
using System.IO;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeStatus
    {
        public string CreateXML(OneStopLicenceData licence, OneStopHubStatusChange statusChange)
        {
            if (licence == null)
                throw new Exception("The licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

            if (licence.BusinessProgramAccountReferenceNumber == null)
                licence.BusinessProgramAccountReferenceNumber = "1";

            var sbnChangeStatus = new SBNChangeStatus();
            sbnChangeStatus.header = GetHeader(licence);
            sbnChangeStatus.body = GetBody(licence, statusChange);

            var serializer = new XmlSerializer(typeof(SBNChangeStatus));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeStatus);
                return textWriter.ToString();
            }
        }

        private SBNChangeStatusHeader GetHeader(OneStopLicenceData licence)
        {
            var header = new SBNChangeStatusHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE_CHANGESTATUS;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            header.partnerNote = licence.LicenceNumber + "-" + DateTime.Now.Ticks;
            header.CCRAHeader = GetCCRAHeader(licence);
            return header;
        }

        private SBNChangeStatusHeaderCCRAHeader GetCCRAHeader(OneStopLicenceData licence)
        {
            var ccraHeader = new SBNChangeStatusHeaderCCRAHeader();
            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);
            return ccraHeader;
        }

        private SBNChangeStatusHeaderCCRAHeaderUserCredentials GetUserCredentials(OneStopLicenceData licence)
        {
            var userCredentials = new SBNChangeStatusHeaderCCRAHeaderUserCredentials();
            userCredentials.businessRegistrationNumber = licence.Licencee.AccountNumber;
            userCredentials.legalName = licence.Licencee.Name;
            if (licence.Establishment != null)
                userCredentials.postalCode = Utils.FormatPostalCode(licence.Establishment.AddressPostalCode);
            return userCredentials;
        }

        private SBNChangeStatusBody GetBody(OneStopLicenceData licence, OneStopHubStatusChange statusChange)
        {
            var body = new SBNChangeStatusBody();
            body.partnerInfo1 = licence.LicenceNumber;
            body.statusData = new SBNChangeStatusBodyStatusData();
            body.statusData.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.statusData.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            body.statusData.businessProgramAccountReferenceNumber = licence.BusinessProgramAccountReferenceNumber;
            body.statusData.programAccountStatus = GetProgramAccountStatus(licence, statusChange);
            body.partnerInfo1 = licence.LicenceNumber;
            if (licence.ExpiryDate != null)
                body.partnerInfo2 = licence.ExpiryDate.Value.DateTime;
            body.statusData.timeStamp = Utils.GetTimeStamp();
            return body;
        }

        private SBNChangeStatusBodyStatusDataProgramAccountStatus GetProgramAccountStatus(OneStopLicenceData licence, OneStopHubStatusChange statusChange)
        {
            var programAccountStatus = new SBNChangeStatusBodyStatusDataProgramAccountStatus();

            switch (statusChange)
            {
                case OneStopHubStatusChange.Cancelled:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    programAccountStatus.programAccountReasonCode = "111";
                    break;
                case OneStopHubStatusChange.CancellationRemoved:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;
                case OneStopHubStatusChange.Expired:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    programAccountStatus.programAccountReasonCode = "112";
                    break;
                case OneStopHubStatusChange.Renewed:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;
                case OneStopHubStatusChange.Suspended:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    programAccountStatus.programAccountReasonCode = "114";
                    break;
                case OneStopHubStatusChange.SuspensionEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;
                case OneStopHubStatusChange.TransferComplete:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_CLOSED;
                    programAccountStatus.programAccountReasonCode = "113";
                    break;
                case OneStopHubStatusChange.EnteredDormancy:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_SUSPENDED;
                    programAccountStatus.programAccountReasonCode = "115";
                    break;
                case OneStopHubStatusChange.DormancyEnded:
                    programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
                    programAccountStatus.programAccountReasonCode = null;
                    break;
            }

            programAccountStatus.effectiveDate = DateTime.Now.AddHours(-8);
            return programAccountStatus;
        }
    }
}
