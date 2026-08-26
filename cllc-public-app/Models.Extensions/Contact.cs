extern alias DV;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using Gov.Lclb.Cllb.Interfaces;
using Contact = Gov.Lclb.Cllb.Public.ViewModels.Contact;
using DataverseContact = DV::Gov.Lclb.Cllb.Interfaces.Contact;
using adoxio_gender = DV::Gov.Lclb.Cllb.Interfaces.adoxio_gender;
using adoxio_generalyesno = DV::Gov.Lclb.Cllb.Interfaces.adoxio_generalyesno;
using adoxio_contact_adoxio_identificationtype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_identificationtype;
using adoxio_contact_adoxio_secondaryidentificationtype = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_secondaryidentificationtype;
using adoxio_contact_adoxio_cascomplete = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_cascomplete;
using adoxio_contact_adoxio_consentvalidated = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_consentvalidated;
using adoxio_contact_adoxio_phslivesincanada = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phslivesincanada;
using adoxio_contact_adoxio_phshaslivedincanada = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phshaslivedincanada;
using adoxio_contact_adoxio_phsexpired = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsexpired;
using adoxio_contact_adoxio_phscomplete = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phscomplete;
using adoxio_contact_adoxio_phsconnectionstootherlicences = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsconnectionstootherlicences;
using adoxio_contact_adoxio_phscanadiandrugalchoholdrivingoffence = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phscanadiandrugalchoholdrivingoffence;
using adoxio_contact_adoxio_phsforeigndrugalchoholoffence = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsforeigndrugalchoholoffence;
using adoxio_contact_adoxio_phsexclusivemfg = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsexclusivemfg;
using adoxio_contact_adoxio_phsfinancialint = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsfinancialint;
using adoxio_contact_adoxio_phsprofitagreement = DV::Gov.Lclb.Cllb.Interfaces.adoxio_contact_adoxio_phsprofitagreement;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class ContactExtensions
    {
        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        // ---- Xrm.Sdk Contact (Dataverse SDK) extensions ----

        public static Contact ToViewModel(this DataverseContact contact)
        {
            if (contact == null) return null;
            var result = new Contact();
            result.id = contact.ContactId?.ToString();
            result.name = contact.FullName;
            result.address1_city = contact.Address1_City;
            result.address1_country = contact.Address1_Country;
            result.address1_line1 = contact.Address1_Line1;
            result.jobTitle = contact.JobTitle;
            result.address1_postalcode = contact.Address1_PostalCode;
            result.address1_stateorprovince = contact.Address1_StateOrProvince;
            result.address2_city = contact.Address2_City;
            result.address2_country = contact.Address2_Country;
            result.address2_line1 = contact.Address2_Line1;
            result.address2_postalcode = contact.Address2_PostalCode;
            result.address2_stateorprovince = contact.Address2_StateOrProvince;
            result.adoxio_canattendcompliancemeetings = contact.adoxio_CanAttendComplianceMeetings;
            result.adoxio_canobtainlicenceinfofrombranch = contact.adoxio_CanObtainLicenceInfoFromBranch;
            result.adoxio_canrepresentlicenseeathearings = contact.adoxio_CanRepresentLicenseeAtHearings;
            result.adoxio_cansigngrocerystoreproofofsalesrevenue = contact.adoxio_CanSignGroceryStoreProofOfSalesRevenue;
            result.adoxio_cansignpermanentchangeapplications = contact.adoxio_CanSignPermanentChangeApplications;
            result.adoxio_cansigntemporarychangeapplications = contact.adoxio_CanSignTemporaryChangeApplications;
            result.emailaddress1 = contact.EMailAddress1;
            result.firstname = contact.FirstName;
            result.middlename = contact.MiddleName;
            result.lastname = contact.LastName;
            result.telephone1 = contact.Telephone1;
            result.Birthdate = contact.BirthDate.HasValue ? new DateTimeOffset(contact.BirthDate.Value) : (DateTimeOffset?)null;
            result.BirthPlace = contact.adoxio_Birthplace;
            result.Gender = (Gender?)(int?)contact.adoxio_GenderCode;
            result.MobilePhone = contact.MobilePhone;
            result.PrimaryIdNumber = contact.adoxio_PrimaryIDNumber;
            result.SecondaryIdNumber = contact.adoxio_SecondaryIDNumber;
            result.PrimaryIdentificationType = (IdentificationType?)(int?)contact.adoxio_IdentificationType;
            result.SecondaryIdentificationType = (IdentificationType?)(int?)contact.adoxio_SecondaryIdentificationType;
            result.IsWorker = contact.adoxio_IsWorker;
            result.SelfDisclosure = (int?)contact.adoxio_SelfDisclosure;
            result.PhsConnectionsDetails = contact.adoxio_PHSConnectionsDetails;
            result.PhsLivesInCanada = (YesNoOptions?)(int?)contact.adoxio_PHSLivesInCanada;
            result.PhsHasLivedInCanada = (YesNoOptions?)(int?)contact.adoxio_PHSHasLivedInCanada;
            result.PhsExpired = (YesNoOptions?)(int?)contact.adoxio_PHSExpired;
            result.PhsComplete = (YesNoOptions?)(int?)contact.adoxio_PHSComplete;
            result.PhsConnectionsToOtherLicences = (YesNoOptions?)(int?)contact.adoxio_PHSConnectionsToOtherLicences;
            result.PhsCanadianDrugAlchoholDrivingOffence = (YesNoOptions?)(int?)contact.adoxio_PHSCanadianDrugAlchoholDrivingOffence;
            result.PhsDateSubmitted = contact.adoxio_PHSDateSubmitted;
            result.PhsForeignDrugAlchoholOffence = (YesNoOptions?)(int?)contact.adoxio_PHSForeignDrugAlchoholOffence;
            result.PhsExclusiveMFG = (YesNoOptions?)(int?)contact.adoxio_PHSExclusiveMFG;
            result.phsExclusiveDetails = contact.adoxio_PHSExclusiveDetails;
            result.phsFinancialInt = (YesNoOptions?)(int?)contact.adoxio_phsFinancialInt;
            result.phsFinancialIntDetails = contact.adoxio_PHSFinancialInterestDetails;
            result.phsProfitAgreement = (YesNoOptions?)(int?)contact.adoxio_PHSProfitAgreement;
            result.phsProfitAgreementDetails = contact.adoxio_PHSProfitAgreementDetails;
            result.CasComplete = (YesNoOptions?)(int?)contact.adoxio_cascomplete;
            return result;
        }

        public static void CopyValues(this DataverseContact to, Contact from)
        {
            to.EMailAddress1 = from.emailaddress1;
            to.Telephone1 = from.telephone1;
            to.CopyValuesNoEmailPhone(from);
        }

        public static void CopyValuesNoEmailPhone(this DataverseContact to, Contact from)
        {
            to.FirstName = from.firstname;
            to.MiddleName = from.middlename;
            to.LastName = from.lastname;
            to.JobTitle = from.jobTitle;
            to.EMailAddress1 = from.emailaddress1;
            to.Address1_City = from.address1_city;
            to.Address1_Country = from.address1_country;
            to.Address1_Line1 = from.address1_line1;
            if (!string.IsNullOrEmpty(from.address1_postalcode))
                to.Address1_PostalCode = from.address1_postalcode.Replace(" ", "");
            to.Address1_StateOrProvince = from.address1_stateorprovince;
            to.Address2_City = from.address2_city;
            to.Address2_Country = from.address2_country;
            to.Address2_Line1 = from.address2_line1;
            to.Address2_PostalCode = from.address2_postalcode;
            to.Address2_StateOrProvince = from.address2_stateorprovince;
            to.adoxio_CanAttendComplianceMeetings = from.adoxio_canattendcompliancemeetings;
            to.adoxio_CanObtainLicenceInfoFromBranch = from.adoxio_canobtainlicenceinfofrombranch;
            to.adoxio_CanRepresentLicenseeAtHearings = from.adoxio_canrepresentlicenseeathearings;
            to.adoxio_CanSignGroceryStoreProofOfSalesRevenue = from.adoxio_cansigngrocerystoreproofofsalesrevenue;
            to.adoxio_CanSignPermanentChangeApplications = from.adoxio_cansignpermanentchangeapplications;
            to.adoxio_CanSignTemporaryChangeApplications = from.adoxio_cansigntemporarychangeapplications;
            to.BirthDate = from.Birthdate?.DateTime;
            to.adoxio_Birthplace = from.BirthPlace;
            to.adoxio_GenderCode = (adoxio_gender?)(int?)from.Gender;
            to.MobilePhone = from.MobilePhone;
            to.adoxio_PrimaryIDNumber = from.PrimaryIdNumber;
            to.adoxio_SecondaryIDNumber = from.SecondaryIdNumber;
            to.adoxio_IsWorker = from.IsWorker;
            to.adoxio_SelfDisclosure = (adoxio_generalyesno?)(int?)from.SelfDisclosure;
            to.adoxio_IdentificationType = (adoxio_contact_adoxio_identificationtype?)(int?)from.PrimaryIdentificationType;
            to.adoxio_SecondaryIdentificationType = (adoxio_contact_adoxio_secondaryidentificationtype?)(int?)from.SecondaryIdentificationType;
            to.adoxio_cascomplete = (adoxio_contact_adoxio_cascomplete?)(int?)from.CasComplete;
            to.adoxio_casdatesubmitted = from.CasDateSubmitted?.DateTime;
            to.adoxio_ConsentValidated = (adoxio_contact_adoxio_consentvalidated?)(int?)from.CasConsentValidated;
            to.adoxio_ConsentValidatedExpiryDate = from.CasConsentValidatedExpiryDate?.DateTime;
            to.adoxio_PHSLivesInCanada = (adoxio_contact_adoxio_phslivesincanada?)(int?)from.PhsLivesInCanada;
            to.adoxio_PHSHasLivedInCanada = (adoxio_contact_adoxio_phshaslivedincanada?)(int?)from.PhsHasLivedInCanada;
            to.adoxio_PHSExpired = (adoxio_contact_adoxio_phsexpired?)(int?)from.PhsExpired;
            to.adoxio_PHSComplete = (adoxio_contact_adoxio_phscomplete?)(int?)from.PhsComplete;
            to.adoxio_PHSConnectionsToOtherLicences = (adoxio_contact_adoxio_phsconnectionstootherlicences?)(int?)from.PhsConnectionsToOtherLicences;
            to.adoxio_PHSCanadianDrugAlchoholDrivingOffence = (adoxio_contact_adoxio_phscanadiandrugalchoholdrivingoffence?)(int?)from.PhsCanadianDrugAlchoholDrivingOffence;
            to.adoxio_PHSDateSubmitted = from.PhsDateSubmitted?.DateTime;
            to.adoxio_PHSForeignDrugAlchoholOffence = (adoxio_contact_adoxio_phsforeigndrugalchoholoffence?)(int?)from.PhsForeignDrugAlchoholOffence;
            to.adoxio_PHSExclusiveMFG = (adoxio_contact_adoxio_phsexclusivemfg?)(int?)from.PhsExclusiveMFG;
            to.adoxio_PHSExclusiveDetails = from.phsExclusiveDetails;
            to.adoxio_phsFinancialInt = (adoxio_contact_adoxio_phsfinancialint?)(int?)from.phsFinancialInt;
            to.adoxio_PHSFinancialInterestDetails = from.phsFinancialIntDetails;
            to.adoxio_PHSProfitAgreement = (adoxio_contact_adoxio_phsprofitagreement?)(int?)from.phsProfitAgreement;
            to.adoxio_PHSProfitAgreementDetails = from.phsProfitAgreementDetails;
            to.adoxio_PHSConnectionsDetails = from.PhsConnectionsDetails;
        }

        public static void CopyHeaderValues(this Contact to, IHeaderDictionary headers)
        {
            string smgov_useremail = headers["smgov_useremail"];
            string smgov_birthdate = headers["smgov_birthdate"];
            string smgov_sex = headers["smgov_sex"];
            string smgov_streetaddress = headers["smgov_streetaddress"];
            string smgov_city = headers["smgov_city"];
            string smgov_postalcode = headers["smgov_postalcode"];
            string smgov_stateorprovince = headers["smgov_province"];
            string smgov_country = headers["smgov_country"];
            string smgov_givenname = headers["smgov_givenname"];
            string smgov_givennames = headers["smgov_givennames"];
            string smgov_surname = headers["smgov_surname"];
            string smgov_userdisplayname = headers["smgov_userdisplayname"];

            to.address1_line1 = smgov_streetaddress;
            to.address1_postalcode = smgov_postalcode;
            to.address1_city = smgov_city;
            to.address1_stateorprovince = smgov_stateorprovince;
            to.address1_country = smgov_country;

            if (!string.IsNullOrEmpty(smgov_givenname)) to.firstname = smgov_givenname;
            if (!string.IsNullOrEmpty(smgov_givennames)) to.middlename = smgov_givennames.Replace(smgov_givenname ?? "", "").Trim();
            if (!string.IsNullOrEmpty(smgov_surname)) to.lastname = smgov_surname;
            if (!string.IsNullOrEmpty(smgov_useremail)) to.emailaddress1 = smgov_useremail;
            if (!string.IsNullOrEmpty(smgov_sex)) to.Gender = (Gender)GetIntGenderCode(smgov_sex);
            if (!string.IsNullOrEmpty(smgov_birthdate) && DateTimeOffset.TryParse(smgov_birthdate, out DateTimeOffset tempDate))
                to.Birthdate = tempDate;
            if (string.IsNullOrEmpty(to.lastname) && smgov_userdisplayname != null)
                to.lastname = smgov_userdisplayname.GetLastName();
            if (string.IsNullOrEmpty(to.firstname) && smgov_userdisplayname != null)
                to.firstname = smgov_userdisplayname.GetFirstName();
        }

        public static void CopyHeaderValues(this ViewModels.Worker to, IHeaderDictionary headers)
        {
            string smgov_useremail = headers["smgov_useremail"];
            string smgov_birthdate = headers["smgov_birthdate"];
            string smgov_sex = headers["smgov_sex"];
            string smgov_givenname = headers["smgov_givenname"];
            string smgov_givennames = headers["smgov_givennames"];
            string smgov_surname = headers["smgov_surname"];

            if (!string.IsNullOrEmpty(smgov_givenname)) to.firstname = smgov_givenname;
            if (!string.IsNullOrEmpty(smgov_givennames)) to.middlename = smgov_givennames.Replace(smgov_givenname ?? "", "").Trim();
            if (!string.IsNullOrEmpty(smgov_surname)) to.lastname = smgov_surname;
            if (!string.IsNullOrEmpty(smgov_useremail)) to.email = smgov_useremail;
            if (!string.IsNullOrEmpty(smgov_birthdate) && DateTimeOffset.TryParse(smgov_birthdate, out DateTimeOffset tempDate))
                to.dateofbirth = tempDate;
            if (!string.IsNullOrEmpty(smgov_sex)) to.gender = (Gender)GetIntGenderCode(smgov_sex);
        }

        static int? GetIntGenderCode(string genderCode)
        {
            if (string.IsNullOrEmpty(genderCode)) return null;
            string upper = genderCode.ToUpper();
            if (upper == "MALE" || upper == "M") return 1;
            if (upper == "FEMALE" || upper == "F") return 2;
            return 3;
        }

        public static void CopyHeaderValues(this DataverseContact to, IHttpContextAccessor httpContextAccessor)
        {
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            string smgov_useremail = headers["SMGOV_USEREMAIL"];
            string smgov_birthdate = headers["SMGOV_BIRTHDATE"];
            string smgov_sex = headers["SMGOV_SEX"];
            string smgov_streetaddress = headers["SMGOV_STREETADDRESS"];
            string smgov_city = headers["SMGOV_CITY"];
            string smgov_postalcode = headers["SMGOV_POSTALCODE"];
            string smgov_stateorprovince = headers["SMGOV_STATEORPROVINCE"];
            string smgov_country = headers["SMGOV_COUNTRY"];
            string smgov_givenname = headers["SMGOV_GIVENNAME"];
            string smgov_givennames = headers["SMGOV_GIVENNAMES"];
            string smgov_surname = headers["SMGOV_SURNAME"];

            if (!string.IsNullOrEmpty(smgov_useremail)) to.EMailAddress1 = smgov_useremail;
            if (!string.IsNullOrEmpty(smgov_givenname)) to.FirstName = smgov_givenname;
            if (!string.IsNullOrEmpty(smgov_givennames)) to.MiddleName = smgov_givennames;
            if (!string.IsNullOrEmpty(smgov_surname)) to.LastName = smgov_surname;
            if (!string.IsNullOrEmpty(smgov_streetaddress)) to.Address1_Line1 = smgov_streetaddress;
            if (!string.IsNullOrEmpty(smgov_postalcode)) to.Address1_PostalCode = smgov_postalcode;
            if (!string.IsNullOrEmpty(smgov_city)) to.Address1_City = smgov_city;
            if (!string.IsNullOrEmpty(smgov_stateorprovince)) to.Address1_StateOrProvince = smgov_stateorprovince;
            if (!string.IsNullOrEmpty(smgov_country)) to.Address1_Country = smgov_country;
            if (!string.IsNullOrEmpty(smgov_sex))
            {
                var genderInt = GetIntGenderCode(smgov_sex);
                if (genderInt.HasValue) to.adoxio_GenderCode = (adoxio_gender?)genderInt;
            }
        }

        public static void CopyContactUserSettings(this DataverseContact contact, Contact newContact)
        {
            contact.Address1_Line1 = newContact.address1_line1;
            if (!string.IsNullOrEmpty(newContact.address1_postalcode))
                contact.Address1_PostalCode = newContact.address1_postalcode.Replace(" ", "");
            contact.Address1_City = newContact.address1_city;
            contact.Address1_StateOrProvince = newContact.address1_stateorprovince;
            contact.Address1_Country = newContact.address1_country;
            contact.FirstName = newContact.firstname;
            contact.MiddleName = newContact.middlename;
            contact.LastName = newContact.lastname;
            contact.EMailAddress1 = newContact.emailaddress1;
            contact.adoxio_GenderCode = (adoxio_gender?)(int?)newContact.Gender;
            contact.BirthDate = newContact.Birthdate?.DateTime;
        }
    }
}
