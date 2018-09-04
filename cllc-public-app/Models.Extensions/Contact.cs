using Gov.Lclb.Cllb.Interfaces.Models;
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
