﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gov.Lclb.Cllb.Public.ViewModels
{

    public enum Adoxio_stateprovince
    {
        [Display(Name = "Alberta")]
        AB = 845280000,
        [Display(Name = "British Columbia")]
        BC,
        [Display(Name = "Manitoba")]
        MN,
        [Display(Name = "New Brunswick")]
        NB,
        [Display(Name = "Newfoundland and Labrador")]
        NL,
        [Display(Name = "Northwest Territories")]
        NT,
        [Display(Name = "Nova Scotia")]
        NS,
        [Display(Name = "Nunavut")]
        NU,
        [Display(Name = "Ontario")]
        ON,
        [Display(Name = "Prince Edward Island")]
        PE,
        [Display(Name = "Quebec")]
        QC,
        [Display(Name = "Saskatchewan")]
        SK,
        [Display(Name = "Yukon")]
        YT
    }

    public class Account
    {
        public string id { get; set; }
        public string LocalGovernmentId { get; set; }
        public string name { get; set; } //dynamics = name
        public string description { get; set; } //dynamics = description
        public string externalId { get; set; }
        public string bcIncorporationNumber { get; set; } //dynamics = adoxio_bcincorporationnumber
        public DateTimeOffset? dateOfIncorporationInBC { get; set; } //dynamics = adoxio_dateofincorporationinbc
        public string businessNumber { get; set; } //dynamics = accountnumber
        public string pstNumber { get; set; } //dynamics = adoxio_pstnumber
        public string contactEmail { get; set; } //dynamics = emailaddress1
        public string contactPhone { get; set; } //dynamics = telephone1

        public string mailingAddressName { get; set; } //dynamics = address1_name
        public string mailingAddressStreet { get; set; } //dynamics = address1_line1
        public string mailingAddressStreet2 { get; set; } //dynamics = address1_line2
        public string mailingAddressCity { get; set; } //dynamics = address1_city
        public string mailingAddressCountry { get; set; } //dynamics = address1_country
        public string mailingAddressProvince { get; set; } //dynamics = address1_stateorprovince
        public string mailingAddressPostalCode { get; set; } //dynamics = address1_postalcode

        public string physicalAddressName { get; set; } //dynamics = address2_name
        public string physicalAddressStreet { get; set; } //dynamics = address2_line1
        public string physicalAddressStreet2 { get; set; } //dynamics = address2_line2
        public string physicalAddressCity { get; set; } //dynamics = address2_city
        public string physicalAddressCountry { get; set; } //dynamics = address2_country
        public string physicalAddressProvince { get; set; } //dynamics = address2_stateorprovince
        public string physicalAddressPostalCode { get; set; } //dynamics = address2_postalcode

        public bool? TermsOfUseAccepted { get; set; }
        public DateTimeOffset? TermsOfUseAcceptedDate { get; set; }

        public Contact primarycontact { get; set; }

        public string businessType { get; set; }

        public string websiteUrl { get; set; }
        // public string accountUrls { get; set; } //dynamics = adoxio_accounturls TODO: Re-implement this

        // SEP Police Review Limits
        public bool? isLateHoursApproval { get; set; }
        public int? maxGuestsForPublicEvents { get; set; }
        public int? maxGuestsForPrivateEvents { get; set; }
        public int? maxGuestsForFamilyEvents { get; set; }

        public bool isCorporateDetailsComplete(AdoxioApplicantTypeCodes? legalentitytype, bool corporateDetailsFilesExists)
        {
            var isComplete = false;
            var tiedHouse = new TiedHouseConnection();
            switch (legalentitytype)
            {
                case AdoxioApplicantTypeCodes.PrivateCorporation:
                case AdoxioApplicantTypeCodes.PublicCorporation:
                case AdoxioApplicantTypeCodes.UnlimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.LimitedLiabilityCorporation:
                case AdoxioApplicantTypeCodes.Society:
                    isComplete = !string.IsNullOrEmpty(bcIncorporationNumber) &&
                        !string.IsNullOrEmpty(businessNumber) &&
                        (dateOfIncorporationInBC != null) &&
                        !string.IsNullOrEmpty(contactEmail) &&
                        !string.IsNullOrEmpty(contactPhone) &&
                        !string.IsNullOrEmpty(mailingAddressName) &&
                        !string.IsNullOrEmpty(mailingAddressStreet) &&
                        !string.IsNullOrEmpty(mailingAddressCity) &&
                        !string.IsNullOrEmpty(mailingAddressCountry) &&
                        (mailingAddressProvince != null) &&// TODO: This field should be a string(by Moffat)
                        !string.IsNullOrEmpty(mailingAddressPostalCode) &&
                        corporateDetailsFilesExists;

                    break;
                case AdoxioApplicantTypeCodes.SoleProprietorship:
                    isComplete = !string.IsNullOrEmpty(businessNumber) &&
                        !string.IsNullOrEmpty(contactEmail) &&
                        !string.IsNullOrEmpty(contactPhone) &&
                        !string.IsNullOrEmpty(mailingAddressName) &&
                        !string.IsNullOrEmpty(mailingAddressStreet) &&
                        !string.IsNullOrEmpty(mailingAddressCity) &&
                        !string.IsNullOrEmpty(mailingAddressCountry) &&
                        (mailingAddressProvince != null) &&
                        !string.IsNullOrEmpty(mailingAddressPostalCode);
                    break;
                case AdoxioApplicantTypeCodes.GeneralPartnership:
                case AdoxioApplicantTypeCodes.LimitedLiabilityPartnership:
                case AdoxioApplicantTypeCodes.LimitedPartnership:
                    isComplete = !string.IsNullOrEmpty(businessNumber) &&
                        !string.IsNullOrEmpty(contactEmail) &&
                        !string.IsNullOrEmpty(contactPhone) &&
                        !string.IsNullOrEmpty(mailingAddressName) &&
                        !string.IsNullOrEmpty(mailingAddressStreet) &&
                        !string.IsNullOrEmpty(mailingAddressCity) &&
                        !string.IsNullOrEmpty(mailingAddressCountry) &&
                        (mailingAddressProvince != null) &&
                        !string.IsNullOrEmpty(mailingAddressPostalCode) &&
                        corporateDetailsFilesExists;
                    break;
                default:
                    isComplete = false;
                    break;
            }
            return isComplete;
        }

    }

    public class TransferAccount
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public AdoxioApplicantTypeCodes? BusinessType { get; set; }
        public string ContactName { get; set; }
    }
}