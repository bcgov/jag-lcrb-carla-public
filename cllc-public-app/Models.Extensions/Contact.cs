using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Public.Contexts.Microsoft.Dynamics.CRM;

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
        public static ViewModels.Contact ToViewModel(this Contact contact)
        {
            ViewModels.Contact result = null;
            if (contact != null)
            {
                result = new ViewModels.Contact();
                if (contact.Contactid != null)
                {
                    result.id = contact.Contactid.ToString();
                }

                result.name = contact.Fullname;
                result.address1_city = contact.Address1_city;
                result.address1_line1 = contact.Address1_line1;
                result.address1_postalcode = contact.Address1_postalcode;
                result.address1_stateorprovince = contact.Address1_stateorprovince;
                result.adoxio_canattendcompliancemeetings = contact.Adoxio_canattendcompliancemeetings;
                result.adoxio_canobtainlicenceinfofrombranch = contact.Adoxio_canobtainlicenceinfofrombranch;
                result.adoxio_canrepresentlicenseeathearings = contact.Adoxio_canrepresentlicenseeathearings;
                result.adoxio_cansigngrocerystoreproofofsalesrevenue = contact.Adoxio_cansigngrocerystoreproofofsalesrevenue;
                result.adoxio_cansignpermanentchangeapplications = contact.Adoxio_cansignpermanentchangeapplications;
                result.adoxio_cansigntemporarychangeapplications = contact.Adoxio_cansigntemporarychangeapplications;
                result.emailaddress1 = contact.Emailaddress1;
                result.firstname = contact.Firstname;
                result.lastname = contact.Lastname;
                result.telephone1 = contact.Telephone1;
            }            
            return result;
        }   
        
        public static void CopyValues(this Contact to, Contact from)
        {
            to.Fullname = from.Fullname;
            to.Emailaddress1 = from.Emailaddress1;
            to.Firstname = from.Firstname;
            to.Lastname = from.Lastname;
            to.Address1_city = from.Address1_city;
            to.Address1_line1 = from.Address1_line1;
            to.Address1_postalcode = from.Address1_postalcode;
            to.Address1_stateorprovince = from.Address1_stateorprovince;
            to.Adoxio_canattendcompliancemeetings = from.Adoxio_canattendcompliancemeetings;
            to.Adoxio_canobtainlicenceinfofrombranch = from.Adoxio_canobtainlicenceinfofrombranch;
            to.Adoxio_canrepresentlicenseeathearings = from.Adoxio_canrepresentlicenseeathearings;
            to.Adoxio_cansigngrocerystoreproofofsalesrevenue = from.Adoxio_cansigngrocerystoreproofofsalesrevenue;
            to.Adoxio_cansignpermanentchangeapplications = from.Adoxio_cansignpermanentchangeapplications;
            to.Adoxio_cansigntemporarychangeapplications = from.Adoxio_cansigntemporarychangeapplications;
            to.Telephone1 = from.Telephone1;
        }


        public static void CopyValues(this Contact to, ViewModels.Contact from)
        {
            to.Fullname = from.name;
            to.Emailaddress1 = from.emailaddress1;
            to.Firstname = from.firstname;
            to.Lastname = from.lastname;
            to.Address1_city = from.address1_city;
            to.Address1_line1 = from.address1_line1;
            to.Address1_postalcode = from.address1_postalcode;
            to.Address1_stateorprovince = from.address1_stateorprovince;
            to.Adoxio_canattendcompliancemeetings = from.adoxio_canattendcompliancemeetings;
            to.Adoxio_canobtainlicenceinfofrombranch = from.adoxio_canobtainlicenceinfofrombranch;
            to.Adoxio_canrepresentlicenseeathearings = from.adoxio_canrepresentlicenseeathearings;
            to.Adoxio_cansigngrocerystoreproofofsalesrevenue = from.adoxio_cansigngrocerystoreproofofsalesrevenue;
            to.Adoxio_cansignpermanentchangeapplications = from.adoxio_cansignpermanentchangeapplications;
            to.Adoxio_cansigntemporarychangeapplications = from.adoxio_cansigntemporarychangeapplications;
            to.Telephone1 = from.telephone1;
        }

        public static Contact ToModel(this ViewModels.Contact contact)
        {
            Contact result = null;
            if (contact != null)
            {
                result = new Contact();
                if (! string.IsNullOrEmpty(contact.id))
                {
                    result.Contactid = new Guid(contact.id);
                }
                result.Fullname = contact.name;
                result.Emailaddress1 = contact.emailaddress1;
                result.Firstname = contact.firstname;
                result.Lastname = contact.lastname;
                
                result.Address1_city = contact.address1_city;
                result.Address1_line1 = contact.address1_line1;
                result.Address1_postalcode = contact.address1_postalcode;
                result.Address1_stateorprovince = contact.address1_stateorprovince;
                result.Adoxio_canattendcompliancemeetings = contact.adoxio_canattendcompliancemeetings;
                result.Adoxio_canobtainlicenceinfofrombranch = contact.adoxio_canobtainlicenceinfofrombranch;
                result.Adoxio_canrepresentlicenseeathearings = contact.adoxio_canrepresentlicenseeathearings;
                result.Adoxio_cansigngrocerystoreproofofsalesrevenue = contact.adoxio_cansigngrocerystoreproofofsalesrevenue;
                result.Adoxio_cansignpermanentchangeapplications = contact.adoxio_cansignpermanentchangeapplications;
                result.Adoxio_cansigntemporarychangeapplications = contact.adoxio_cansigntemporarychangeapplications;
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
