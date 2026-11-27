using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    /// <summary>
    /// Builds the SBNChangeMiscData (Miscellaneous Data Update) message used to tell
    /// LDB, via the BC BN Hub, that an endorsement has been added to or cancelled on a
    /// licence. Replaces the manual email that was being sent for these events.
    /// <para>
    /// Unlike the other messages this document carries no CCRAHeader - the Messaging
    /// Technical Specification Guide states it "should be omitted" - and it is
    /// addressed to LDB rather than to the Hub.
    /// </para>
    /// </summary>
    public class ChangeMiscData
    {
        public string CreateXML(OneStopLicenceData licence, OneStopEndorsement endorsement, string updateReasonCode)
        {
            if (licence == null)
                throw new Exception("The licence can not be null");

            if (licence.Licencee == null)
                throw new Exception("The licence must have a Licencee");

            if (endorsement == null)
                throw new Exception("The endorsement can not be null");

            if (string.IsNullOrEmpty(licence.LicenceNumber))
                throw new Exception("The licence must have a LicenceNumber; LDB matches miscDataValue against an existing licence number");

            var codes = EndorsementMiscDataMap.Resolve(endorsement.Name, licence.LicenceSubCategory);
            if (codes == null)
                throw new Exception($"No OneStop miscDataTypeCode is mapped for endorsement '{endorsement.Name}'");

            if (licence.BusinessProgramAccountReferenceNumber == null)
                licence.BusinessProgramAccountReferenceNumber = "1";

            var sbnChangeMiscData = new SBNChangeMiscData();
            sbnChangeMiscData.header = GetHeader(licence, codes);
            sbnChangeMiscData.body = GetBody(licence, codes, updateReasonCode);

            var serializer = new XmlSerializer(typeof(SBNChangeMiscData));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeMiscData);
                return textWriter.ToString();
            }
        }

        private SBNChangeMiscDataHeader GetHeader(OneStopLicenceData licence, EndorsementMiscDataType codes)
        {
            var header = new SBNChangeMiscDataHeader();
            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = codes.DocumentSubType;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID_LDB;
            header.partnerNote = licence.LicenceNumber + "-" + DateTime.Now.Ticks;
            return header;
        }

        private SBNChangeMiscDataBody GetBody(OneStopLicenceData licence, EndorsementMiscDataType codes, string updateReasonCode)
        {
            var body = new SBNChangeMiscDataBody();
            body.businessRegistrationNumber = licence.Licencee.AccountNumber;
            body.businessProgramIdentifier = OneStopUtils.BUSINESS_PROGRAM_IDENTIFIER;
            // The sample messages carry a zero-padded three digit reference number.
            body.businessProgramAccountReferenceNumber =
                licence.BusinessProgramAccountReferenceNumber.PadLeft(3, '0');
            body.miscDataTypeCode = codes.MiscDataTypeCode;
            body.updateReasonCode = updateReasonCode;
            // LDB matches on miscDataValue, which must be a licence number it already holds.
            body.miscDataValue = licence.LicenceNumber;
            body.partnerInfo1 = licence.LicenceNumber;
            // partnerInfo2 is left unset: it is optional, is not stored in the BNI, and
            // neither LDB nor the Hub has confirmed what the sample value represents.
            body.partnerInfo3 = OneStopUtils.MISC_DATA_PARTNER_INFO_SENDER;
            return body;
        }
    }
}
