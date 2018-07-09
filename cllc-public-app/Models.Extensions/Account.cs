using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Interfaces.Models;


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
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// updateIfNull defaults to true
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull"></param>
        public static void CopyValues(this Account toDynamics, ViewModels.Account fromVM, Boolean copyIfNull)
        {
            if (copyIfNull || (!copyIfNull && fromVM.name!= null))
            {
                toDynamics.Name = fromVM.name;
            }
            if (copyIfNull || (!copyIfNull && fromVM.description != null))
            {
                toDynamics.Description = fromVM.description;
            }
            if (copyIfNull || (!copyIfNull && fromVM.externalId != null))
            {
                toDynamics.Adoxio_externalid = fromVM.externalId;
            }
            if (copyIfNull || (!copyIfNull && fromVM.bcIncorporationNumber != null))
            {
                toDynamics.Adoxio_bcincorporationnumber = fromVM.bcIncorporationNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.dateOfIncorporationInBC != null))
            {
                toDynamics.Adoxio_dateofincorporationinbc = fromVM.dateOfIncorporationInBC;
            }
            if (copyIfNull || (!copyIfNull && fromVM.businessNumber != null))
            {
                toDynamics.Accountnumber = fromVM.businessNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.pstNumber != null))
            {
                toDynamics.Adoxio_pstnumber = fromVM.pstNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactEmail != null))
            {
                toDynamics.Emailaddress1 = fromVM.contactEmail;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactPhone != null))
            {
                toDynamics.Telephone1 = fromVM.contactPhone;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressName != null))
            {
                toDynamics.Address1_name = fromVM.mailingAddressName;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressStreet != null))
            {
                toDynamics.Address1_line1 = fromVM.mailingAddressStreet;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCity != null))
            {
                toDynamics.Address1_city = fromVM.mailingAddressCity;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCountry != null))
            {
                toDynamics.Address1_county = fromVM.mailingAddressCountry;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressProvince != null))
            {
                if (fromVM.mailingAddressProvince >= ViewModels.Adoxio_stateprovince.AB &&
                fromVM.mailingAddressProvince <= ViewModels.Adoxio_stateprovince.YT)
                    toDynamics.Adoxio_stateprovince = (int?)fromVM.mailingAddressProvince;
                else
                    toDynamics.Adoxio_stateprovince = (int?)ViewModels.Adoxio_stateprovince.BC;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddresPostalCode != null))
            {
                toDynamics.Address1_postalcode = fromVM.mailingAddresPostalCode;
            }
        }

        /// <summary>
        /// Copy values from a ViewModel to a Dynamics Account.
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// updateIfNull defaults to true
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMaccount toDynamics, ViewModels.Account fromVM, Boolean copyIfNull)
        {
            if (copyIfNull || (!copyIfNull && fromVM.name != null))
            {
                toDynamics.Name = fromVM.name;
            }
            if (copyIfNull || (!copyIfNull && fromVM.description != null))
            {
                toDynamics.Description = fromVM.description;
            }
            if (copyIfNull || (!copyIfNull && fromVM.externalId != null))
            {
                toDynamics.AdoxioExternalid = fromVM.externalId;
            }
            if (copyIfNull || (!copyIfNull && fromVM.bcIncorporationNumber != null))
            {
                toDynamics.AdoxioBcincorporationnumber = fromVM.bcIncorporationNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.dateOfIncorporationInBC != null))
            {
                toDynamics.AdoxioDateofincorporationinbc = fromVM.dateOfIncorporationInBC;
            }
            if (copyIfNull || (!copyIfNull && fromVM.businessNumber != null))
            {
                toDynamics.Accountnumber = fromVM.businessNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.pstNumber != null))
            {
                toDynamics.AdoxioPstnumber = fromVM.pstNumber;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactEmail != null))
            {
                toDynamics.Emailaddress1 = fromVM.contactEmail;
            }
            if (copyIfNull || (!copyIfNull && fromVM.contactPhone != null))
            {
                toDynamics.Telephone1 = fromVM.contactPhone;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressName != null))
            {
                toDynamics.Address1Name = fromVM.mailingAddressName;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressStreet != null))
            {
                toDynamics.Address1Line1 = fromVM.mailingAddressStreet;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCity != null))
            {
                toDynamics.Address1City = fromVM.mailingAddressCity;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCountry != null))
            {
                toDynamics.Address1County = fromVM.mailingAddressCountry;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressProvince != null))
            {
                if (fromVM.mailingAddressProvince >= ViewModels.Adoxio_stateprovince.AB &&
                fromVM.mailingAddressProvince <= ViewModels.Adoxio_stateprovince.YT)
                    toDynamics.AdoxioStateprovince = (int?)fromVM.mailingAddressProvince;
                else
                    toDynamics.AdoxioStateprovince = (int?)ViewModels.Adoxio_stateprovince.BC;
            }
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddresPostalCode != null))
            {
                toDynamics.Address1Postalcode = fromVM.mailingAddresPostalCode;
            }
            
            // business type must be set only during creation, not in update (removed from copyValues() )
            //	toDynamics.AdoxioBusinesstype = (int)Enum.Parse(typeof(ViewModels.Adoxio_applicanttypecodes), fromVM.businessType, true);
        }

        /// <summary>
        /// Copy values from a ViewModel to a Dynamics Account.
        /// If parameter copyIfNull is false then do not copy a null value. Mainly applies to updates to the account.
        /// updateIfNull defaults to true
        /// </summary>
        /// <param name="toDynamics"></param>
        /// <param name="fromVM"></param>
        /// <param name="copyIfNull"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMaccount toDynamics, ViewModels.Account fromVM)
        {
            bool copyIfNull = true;
            toDynamics.CopyValues(fromVM, copyIfNull);            
        }

        /// <summary>
        /// Create a new ViewModel from a Dynamics Account
        /// </summary>
        /// <param name="account"></param>
        public static ViewModels.Account ToViewModel(this MicrosoftDynamicsCRMaccount account)
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
                accountVM.externalId = account.AdoxioExternalid;
                accountVM.bcIncorporationNumber = account.AdoxioBcincorporationnumber;
                accountVM.dateOfIncorporationInBC = account.AdoxioDateofincorporationinbc;
                accountVM.businessNumber = account.Accountnumber;
                accountVM.pstNumber = account.AdoxioPstnumber;
                accountVM.contactEmail = account.Emailaddress1;
                accountVM.contactPhone = account.Telephone1;
                accountVM.mailingAddressName = account.Address1Name;
                accountVM.mailingAddressStreet = account.Address1Line1;
                accountVM.mailingAddressCity = account.Address1City;
                accountVM.mailingAddressCountry = account.Address1County;
                if (account.AdoxioStateprovince != null)
                    accountVM.mailingAddressProvince = (ViewModels.Adoxio_stateprovince)account.AdoxioStateprovince;
                else
                    accountVM.mailingAddressProvince = ViewModels.Adoxio_stateprovince.BC;
                accountVM.mailingAddresPostalCode = account.Address1Postalcode;

                if (account.Primarycontactid != null)
                {
                    // add the primary contact.
                    accountVM.primarycontact = new ViewModels.Contact();
                    accountVM.primarycontact.id = account.Primarycontactid.Contactid.ToString();
                    // TODO - load other fields (if necessary)
                }

				if (account.AdoxioBusinesstype != null)
				{
					accountVM.businessType = Enum.ToObject(typeof(Gov.Lclb.Cllb.Public.ViewModels.Adoxio_applicanttypecodes), account.AdoxioBusinesstype).ToString();
				}
            }
            return accountVM;
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
				if (accountVM.mailingAddressProvince >= ViewModels.Adoxio_stateprovince.AB &&
				    accountVM.mailingAddressProvince <= ViewModels.Adoxio_stateprovince.YT)
					result.Adoxio_stateprovince = (int?)accountVM.mailingAddressProvince;
                else
					result.Adoxio_stateprovince = (int?)ViewModels.Adoxio_stateprovince.BC;
                result.Address1_postalcode = accountVM.mailingAddresPostalCode;
            }
            return result;
        }
    }
}
