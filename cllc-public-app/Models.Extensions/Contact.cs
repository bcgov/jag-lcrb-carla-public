using Gov.Lclb.Cllb.Interfaces.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public static ViewModels.Contact ToViewModel(this MicrosoftDynamicsCRMcontact contact)
        {
            ViewModels.Contact result = null;
            if (contact != null)
            {
                result = new ViewModels.Contact();
                if (contact.Contactid != null)
                {
                    result.id = contact.Contactid;
                }

                result.name = contact.Fullname;
                result.address1_city = contact.Address1City;
                result.address1_country = contact.Address1Country;
                result.address1_line1 = contact.Address1Line1;
                result.address1_postalcode = contact.Address1Postalcode;
                result.address1_stateorprovince = contact.Address1Stateorprovince;
                result.adoxio_canattendcompliancemeetings = contact.AdoxioCanattendcompliancemeetings;
                result.adoxio_canobtainlicenceinfofrombranch = contact.AdoxioCanobtainlicenceinfofrombranch;
                result.adoxio_canrepresentlicenseeathearings = contact.AdoxioCanrepresentlicenseeathearings;
                result.adoxio_cansigngrocerystoreproofofsalesrevenue = contact.AdoxioCansigngrocerystoreproofofsalesrevenue;
                result.adoxio_cansignpermanentchangeapplications = contact.AdoxioCansignpermanentchangeapplications;
                result.adoxio_cansigntemporarychangeapplications = contact.AdoxioCansigntemporarychangeapplications;
                result.emailaddress1 = contact.Emailaddress1;
                result.firstname = contact.Firstname;
                result.middlename = contact.Middlename;
                result.lastname = contact.Lastname;
                result.telephone1 = contact.Telephone1;
            }            
            return result;
        }   
        
        public static void CopyHeaderValues(this MicrosoftDynamicsCRMcontact to, IHttpContextAccessor httpContextAccessor)       
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


            to.Emailaddress1 = smgov_useremail;
            to.Firstname = smgov_givenname;
            to.Middlename = smgov_givennames;
            to.Lastname = smgov_surname;
            to.Address1Line1 = smgov_streetaddress;            
            to.Address1Postalcode = smgov_postalcode;
            to.Address1City = smgov_city;            
            to.Address1Stateorprovince = smgov_stateorprovince;
            to.Address1Country = smgov_country;
        }


        /// <summary>
        /// Return a Dynamics gender code for the given string.
        /// </summary>
        /// <param name="genderCode"></param>
        /// <returns>
        /// 1 - M
        /// 2 - F
        /// 3 - Other
        /// </returns>
        static int? GetIntGenderCode(string genderCode)
        {
            // possible values:
            
            int? result = null;

            if (! string.IsNullOrEmpty(genderCode))
            {
                string upper = genderCode.ToUpper();
                if (upper.Equals("MALE") || upper.Equals("M"))
                {
                    result = 1;
                }
                else if (upper.Equals("FEMALE") || upper.Equals("F"))
                {
                    result = 2;
                }
                else
                {
                    result = 3;
                }
            }

            return result;
        }

        public static void CopyHeaderValues(this MicrosoftDynamicsCRMadoxioWorker to, IHttpContextAccessor httpContextAccessor)
        {
            var headers = httpContextAccessor.HttpContext.Request.Headers;
            string smgov_useremail = headers["SMGOV_USEREMAIL"];
            // the following fields appear to just have a guid in them, not a driver's licence.
            string smgov_useridentifier = headers["SMGOV_USERIDENTIFIER"];
            string smgov_useridentifiertype = headers["SMGOV_USERIDENTIFIERTYPE"];

            // birthdate is YYYY-MM-DD
            string smgov_birthdate = headers["SMGOV_BIRTHDATE"];
            // Male / Female / Unknown. 
            string smgov_sex = headers["SMGOV_SEX"];
            string smgov_givenname = headers["SMGOV_GIVENNAME"];
            string smgov_givennames = headers["SMGOV_GIVENNAMES"];
            string smgov_surname = headers["SMGOV_SURNAME"];

            if (!string.IsNullOrEmpty(smgov_givenname)) {
                to.AdoxioFirstname = smgov_givenname;
            }

            if (!string.IsNullOrEmpty(smgov_givennames))
            {
                to.AdoxioMiddlename = smgov_givennames;
            }

            if (!string.IsNullOrEmpty(smgov_surname))
            {
                to.AdoxioLastname = smgov_surname;
            }
            if (!string.IsNullOrEmpty(smgov_useremail))
            {
                to.AdoxioEmail = smgov_useremail;
            }
            

            if (!string.IsNullOrEmpty(smgov_birthdate) && DateTimeOffset.TryParse (smgov_birthdate, out DateTimeOffset tempDate))
            {
                to.AdoxioDateofbirth = tempDate;
            }
            if (!string.IsNullOrEmpty(smgov_sex))
            {
                to.AdoxioGendercode = GetIntGenderCode(smgov_sex);
            }

        }

        

        public static void CopyValues(this MicrosoftDynamicsCRMcontact to, ViewModels.Contact from)
        {
            to.Fullname = from.name;
            to.Emailaddress1 = from.emailaddress1;
            to.Firstname = from.firstname;
            to.Middlename = from.middlename;
            to.Lastname = from.lastname;
            to.Address1City = from.address1_city;
            to.Address1Country = from.address1_country;
            to.Address1Line1 = from.address1_line1;
            to.Address1Postalcode = from.address1_postalcode;
            to.Address1Stateorprovince = from.address1_stateorprovince;
            to.AdoxioCanattendcompliancemeetings = from.adoxio_canattendcompliancemeetings;
            to.AdoxioCanobtainlicenceinfofrombranch = from.adoxio_canobtainlicenceinfofrombranch;
            to.AdoxioCanrepresentlicenseeathearings = from.adoxio_canrepresentlicenseeathearings;
            to.AdoxioCansigngrocerystoreproofofsalesrevenue = from.adoxio_cansigngrocerystoreproofofsalesrevenue;
            to.AdoxioCansignpermanentchangeapplications = from.adoxio_cansignpermanentchangeapplications;
            to.AdoxioCansigntemporarychangeapplications = from.adoxio_cansigntemporarychangeapplications;
            to.Telephone1 = from.telephone1;            
        }

        public static MicrosoftDynamicsCRMcontact ToModel(this ViewModels.Contact contact)
        {
            MicrosoftDynamicsCRMcontact result = null;
            if (contact != null)
            {
                result = new MicrosoftDynamicsCRMcontact();
                if (! string.IsNullOrEmpty(contact.id))
                {
                    result.Contactid = contact.id;
                }
                result.Fullname = contact.name;
                result.Emailaddress1 = contact.emailaddress1;
                result.Firstname = contact.firstname;
                result.Lastname = contact.lastname;
                result.Middlename = contact.middlename;
                
                result.Address1City = contact.address1_city;
                result.Address1Country = contact.address1_country;
                result.Address1Line1 = contact.address1_line1;
                result.Address1Postalcode = contact.address1_postalcode;
                result.Address1Stateorprovince = contact.address1_stateorprovince;
                result.AdoxioCanattendcompliancemeetings = contact.adoxio_canattendcompliancemeetings;
                result.AdoxioCanobtainlicenceinfofrombranch = contact.adoxio_canobtainlicenceinfofrombranch;
                result.AdoxioCanrepresentlicenseeathearings = contact.adoxio_canrepresentlicenseeathearings;
                result.AdoxioCansigngrocerystoreproofofsalesrevenue = contact.adoxio_cansigngrocerystoreproofofsalesrevenue;
                result.AdoxioCansignpermanentchangeapplications = contact.adoxio_cansignpermanentchangeapplications;
                result.AdoxioCansigntemporarychangeapplications = contact.adoxio_cansigntemporarychangeapplications;
                result.Telephone1 = contact.telephone1;


                if (string.IsNullOrEmpty(result.Fullname) && (!string.IsNullOrEmpty(result.Firstname) || !string.IsNullOrEmpty(result.Lastname)))
                {
                    result.Fullname = "";
                    if (!string.IsNullOrEmpty(result.Firstname))
                    {
                        result.Fullname += result.Firstname;
                    }
                    if (!string.IsNullOrEmpty(result.Lastname))
                    {
                        if (!string.IsNullOrEmpty(result.Fullname))
                        {
                            result.Fullname += " ";
                        }
                        result.Fullname += result.Lastname;
                    }
                }
            }
            return result;
        }
    }
}
