using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {

        /// <summary>
        /// Copy values from a Dynamics Account to another Dynamics Account
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromDynamics"></param>
        public static void CopyValues(this Account toDynamics, Account fromDynamics)
        {
            toDynamics.Accountid = fromDynamics.Accountid;
            toDynamics.Name = fromDynamics.Name;
            toDynamics.Description = fromDynamics.Description;
			toDynamics.Adoxio_externalid = fromDynamics.Adoxio_externalid;
            toDynamics.Adoxio_bcincorporationnumber = fromDynamics.Adoxio_bcincorporationnumber;
            toDynamics.Adoxio_dateofincorporationinbc = fromDynamics.Adoxio_dateofincorporationinbc;
            toDynamics.Accountnumber = fromDynamics.Accountnumber;
            toDynamics.Adoxio_pstnumber = fromDynamics.Adoxio_pstnumber;
            toDynamics.Emailaddress1 = fromDynamics.Emailaddress1;
            toDynamics.Telephone1 = fromDynamics.Telephone1;
            toDynamics.Address1_name = fromDynamics.Address1_name;
            toDynamics.Address1_line1 = fromDynamics.Address1_line1;
            toDynamics.Address1_city = fromDynamics.Address1_city;
            toDynamics.Address1_county = fromDynamics.Address1_county;
            toDynamics.Adoxio_stateprovince = fromDynamics.Adoxio_stateprovince;
            toDynamics.Address1_postalcode = fromDynamics.Address1_postalcode;
        }

        /// <summary>
        /// Copy values from a ViewModel to a Dynamics Account.
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        public static void CopyValues(this Account toDynamics, ViewModels.Account fromVM)
        {
            toDynamics.Name = fromVM.name;
            toDynamics.Description = fromVM.description;
			toDynamics.Adoxio_externalid = fromVM.externalId;
            toDynamics.Adoxio_bcincorporationnumber = fromVM.bcIncorporationNumber;
            toDynamics.Adoxio_dateofincorporationinbc = fromVM.dateOfIncorporationInBC;
            toDynamics.Accountnumber = fromVM.businessNumber;
            toDynamics.Adoxio_pstnumber = fromVM.pstNumber;
            toDynamics.Emailaddress1 = fromVM.contactEmail;
            toDynamics.Telephone1 = fromVM.contactPhone;
            toDynamics.Address1_name = fromVM.mailingAddressName;
            toDynamics.Address1_line1 = fromVM.mailingAddressStreet;
            toDynamics.Address1_city = fromVM.mailingAddressCity;
            toDynamics.Address1_county = fromVM.mailingAddressCountry;
            toDynamics.Adoxio_stateprovince = (int?) fromVM.mailingAddressProvince;
            toDynamics.Address1_postalcode = fromVM.mailingAddresPostalCode;
        }

        /// <summary>
        /// Create a new ViewModel from a Dynamics Account
        /// </summary>
        /// <param name="account"></param>
        public static ViewModels.Account ToViewModel(this Account account)
        {
            ViewModels.Account accountVM = null;
            if (account != null)
            {
                accountVM = new ViewModels.Account();
                if (account.Accountid != null)
                {
                    accountVM.id = account.Accountid.ToString();
                }

                accountVM.name = account.Name;
                accountVM.description = account.Description;
				accountVM.externalId = account.Adoxio_externalid;
                accountVM.bcIncorporationNumber = account.Adoxio_bcincorporationnumber;
                accountVM.dateOfIncorporationInBC = account.Adoxio_dateofincorporationinbc;
                accountVM.businessNumber = account.Accountnumber;
                accountVM.pstNumber = account.Adoxio_pstnumber;
                accountVM.contactEmail = account.Emailaddress1;
                accountVM.contactPhone = account.Telephone1;
                accountVM.mailingAddressName = account.Address1_name;
                accountVM.mailingAddressStreet = account.Address1_line1;
                accountVM.mailingAddressCity = account.Address1_city;
                accountVM.mailingAddressCountry = account.Address1_county;
                if (account.Adoxio_stateprovince != null)
		    accountVM.mailingAddressProvince = (ViewModels.Adoxio_stateprovince)account.Adoxio_stateprovince;
		else
		    accountVM.mailingAddressProvince = ViewModels.Adoxio_stateprovince.BC;
                accountVM.mailingAddresPostalCode = account.Address1_postalcode;

                if (account._primarycontactid_value != null)
                {
                    // add the primary contact.
                    accountVM.primarycontact = new ViewModels.Contact();
                    accountVM.primarycontact.id = account._primarycontactid_value.ToString();
                    // TODO - load other fields (if necessary)
                }
            }            
            return accountVM;
        }

        /// <summary>
        /// Create a new Dynamics Account from a ViewModel
        /// </summary>
        /// <param name="accountVM"></param>
        public static Account ToModel(this ViewModels.Account accountVM)
        {
            Account result = null;
            if (accountVM != null)
            {
                result = new Account();
                if (string.IsNullOrEmpty(accountVM.id))
                {
                    result.Accountid = new Guid();
                }
                else
                {
                    result.Accountid = new Guid(accountVM.id);
                }
                result.Name = accountVM.name;
				result.Adoxio_externalid = accountVM.externalId;
                result.Adoxio_bcincorporationnumber = accountVM.bcIncorporationNumber;
                result.Adoxio_dateofincorporationinbc = accountVM.dateOfIncorporationInBC;
                result.Accountnumber = accountVM.businessNumber;
                result.Adoxio_pstnumber = accountVM.pstNumber;
                result.Emailaddress1 = accountVM.contactEmail;
                result.Telephone1 = accountVM.contactPhone;
                result.Address1_name = accountVM.mailingAddressName;
                result.Address1_line1 = accountVM.mailingAddressStreet;
                result.Address1_city = accountVM.mailingAddressCity;
                result.Address1_county = accountVM.mailingAddressCountry;
                result.Adoxio_stateprovince = (int?)accountVM.mailingAddressProvince;
                result.Address1_postalcode = accountVM.mailingAddresPostalCode;
            }
            return result;
        }
    }
}
