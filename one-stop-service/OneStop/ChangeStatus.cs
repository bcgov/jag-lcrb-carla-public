using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Jag.Lcrb.OneStopService;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Gov.Jag.Lcrb.OneStopService.OneStop.Util;

namespace Gov.Jag.Lcrb.OneStopService.OneStop
{
    public class ChangeStatus
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
            var sbnChangeStatus = new SBNChangeStatus();
            sbnChangeStatus.header = GetHeader(licence);
            sbnChangeStatus.body = GetBody(licence);

            var serializer = new XmlSerializer(typeof(SBNProgramAccountDetailsBroadcast1));
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, sbnChangeStatus);
                return textWriter.ToString();
            }
        }

        private SBNChangeStatusHeader GetHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var header = new SBNChangeStatusHeader();

            header.requestMode = OneStopUtils.ASYNCHRONOUS;
            header.documentSubType = OneStopUtils.DOCUMENT_SUBTYPE;
            header.senderID = OneStopUtils.SENDER_ID;
            header.receiverID = OneStopUtils.RECEIVER_ID;
            //any note wanted by LCRB. Currently in liquor is: licence Id, licence number - sequence number
            header.partnerNote = licence.AdoxioLicencenumber;

            header.CCRAHeader = GetCCRAHeader(licence);

            return header;
        }

        private SBNChangeStatusHeaderCCRAHeader GetCCRAHeader(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var ccraHeader = new SBNChangeStatusHeaderCCRAHeader();

            ccraHeader.userApplication = OneStopUtils.USER_APPLICATION;
            ccraHeader.userRole = OneStopUtils.USER_ROLE;
            ccraHeader.userCredentials = GetUserCredentials(licence);

            return ccraHeader;
        }

        private SBNChangeStatusHeaderCCRAHeaderUserCredentials GetUserCredentials(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var userCredentials = new SBNChangeStatusHeaderCCRAHeaderUserCredentials();

            //BN9 of licensee (Owner company)
            userCredentials.businessRegistrationNumber = licence.AdoxioLicencee.Accountnumber;
            //the name of the applicant (licensee)- last name, first name middle initial or company name
            userCredentials.legalName = licence.AdoxioLicencee.Name;
            //establishment (physical location of store)
            userCredentials.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            //last name of sole proprietor (if not sole prop then null)
            if (licence.AdoxioLicencee != null && licence.AdoxioLicencee.Primarycontactid != null && !string.IsNullOrEmpty(licence.AdoxioLicencee.Primarycontactid.Lastname))
            {
                userCredentials.lastName = licence.AdoxioLicencee.Primarycontactid.Lastname;
            }
            else
            {
                userCredentials.lastName = "N/A";
            }


            return userCredentials;
        }

        string GetPrimaryContact(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            // first create an XML object.
            var primaryContactDetails = new PrimaryContactDetails();

            if (licence.AdoxioLicencee != null)
            {
                primaryContactDetails.name = licence.AdoxioLicencee.Name;
                primaryContactDetails.email = licence.AdoxioLicencee.Emailaddress1;

                // 2019-07-11 - LDB has requested that the phone number only contain digits.

                string phoneDigitsOnly = "";

                if (licence.AdoxioLicencee.Telephone1 != null)
                {
                    phoneDigitsOnly = Regex.Replace(licence.AdoxioLicencee.Telephone1, "[^0-9]", "");
                }


                primaryContactDetails.phone = phoneDigitsOnly;
            }

            // convert the XML to a string.
            using (var stringwriter = new StringWriter())
            {

                XmlSerializer serializer = new XmlSerializer(primaryContactDetails.GetType());
                serializer.Serialize(stringwriter, primaryContactDetails);
                return stringwriter.ToString();
            }
        }

        private SBNChangeStatusBody GetBody(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var body = new SBNChangeStatusBody();

            // licence number
            body.partnerInfo1 = licence.AdoxioLicencenumber;

            body.statusData = new SBNChangeStatusBodyStatusData();

            body.statusData.businessProgramAccountReferenceNumber = 1;
            body.statusData.businessProgramIdentifier = "2";
            body.statusData.businessRegistrationNumber = 4;

            return body;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessCore GetBusinessCore(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var businessCore = new SBNProgramAccountDetailsBroadcastBodyBusinessCore();

            //always 01 for our requests
            businessCore.programAccountTypeCode = OneStopUtils.PROGRAM_ACCOUNT_TYPE_CODE;
            //licence number - dash sequence number. Sequence is always 1
            businessCore.crossReferenceProgramNumber = licence.AdoxioLicencenumber;

            return businessCore;
        }

        private SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus GetProgramAccountStatus(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var programAccountStatus = new SBNProgramAccountDetailsBroadcastBodyProgramAccountStatus();

            programAccountStatus.programAccountStatusCode = OneStopUtils.PROGRAM_ACCOUNT_STATUS_CODE_ACTIVE;
            //effective date of the licence (the date licence is issued or a future date if the licensee specifies a date they want the licence to start
            programAccountStatus.effectiveDate = DateTime.Now; //ToGetFromDynamics. Current date time for test purpose

            return programAccountStatus;
        }

        private SBNProgramAccountDetailsBroadcastBodyOperatingName GetOperatingName(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var operatingName = new SBNProgramAccountDetailsBroadcastBodyOperatingName();

            //store name
            operatingName.operatingName = licence.AdoxioEstablishment.AdoxioName;
            //only ever have 1 operating name
            operatingName.operatingNamesequenceNumber = OneStopUtils.OPERATING_NAME_SEQUENCE_NUMBER;

            return operatingName;
        }

        /**
         * Business Address (physical location of the store)
         */
        private SBNProgramAccountDetailsBroadcastBodyBusinessAddress GetBusinessAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //physical location of the store
            var businessAddress = new SBNProgramAccountDetailsBroadcastBodyBusinessAddress();

            businessAddress.foreignLegacy = GetForeignLegacyBusiness(licence);
            businessAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            businessAddress.provinceStateCode = "BC";
            businessAddress.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            businessAddress.countryCode = "CA";

            return businessAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy GetForeignLegacyBusiness(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacy = new SBNProgramAccountDetailsBroadcastBodyBusinessAddressForeignLegacy();

            foreignLegacy.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            //foreignLegacy.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacy;
        }

        /**
         * Mailing Address (for the licence)
         */
        private SBNProgramAccountDetailsBroadcastBodyMailingAddress GetMailingAddress(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            //mailing address for the licence
            var mailingAddress = new SBNProgramAccountDetailsBroadcastBodyMailingAddress();

            mailingAddress.foreignLegacy = GetForeignLegacyMailing(licence);
            mailingAddress.municipality = licence.AdoxioEstablishment.AdoxioAddresscity;
            mailingAddress.provinceStateCode = "BC";
            mailingAddress.postalCode = Utils.FormatPostalCode(licence.AdoxioEstablishment.AdoxioAddresspostalcode);
            mailingAddress.countryCode = "CA";

            return mailingAddress;
        }

        private SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy GetForeignLegacyMailing(MicrosoftDynamicsCRMadoxioLicences licence)
        {
            var foreignLegacyMailing = new SBNProgramAccountDetailsBroadcastBodyMailingAddressForeignLegacy();

            foreignLegacyMailing.addressDetailLine1 = licence.AdoxioEstablishment.AdoxioAddressstreet;
            //foreignLegacyMailing.addressDetailLine2 = "ToGetFromDynamics";

            return foreignLegacyMailing;
        }

    }
}
