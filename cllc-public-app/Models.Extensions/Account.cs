using System;
using Gov.Lclb.Cllb.Interfaces.Models;


namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class AccountExtensions
    {

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
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressCountry != null))
            {
                toDynamics.Address1Stateorprovince = fromVM.mailingAddressProvince;
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
                accountVM.mailingAddressProvince = account.Address1Stateorprovince;
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
                    accountVM.businessType = Enum.ToObject(typeof(Gov.Lclb.Cllb.Public.ViewModels.AdoxioApplicantTypeCodes), account.AdoxioBusinesstype).ToString();
                }
            }
            return accountVM;
        }


    }
}
