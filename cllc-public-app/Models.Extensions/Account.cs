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
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressStreet2 != null))
            {
                toDynamics.Address1Line2 = fromVM.mailingAddressStreet2;
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
            if (copyIfNull || (!copyIfNull && fromVM.mailingAddressPostalCode != null))
            {
                toDynamics.Address1Postalcode = fromVM.mailingAddressPostalCode;
            }
            
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressName != null))
            {
                toDynamics.Address2Name = fromVM.physicalAddressName;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressStreet != null))
            {
                toDynamics.Address2Line1 = fromVM.physicalAddressStreet;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressStreet2 != null))
            {
                toDynamics.Address2Line2 = fromVM.physicalAddressStreet2;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressCity != null))
            {
                toDynamics.Address2City = fromVM.physicalAddressCity;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressCountry != null))
            {
                toDynamics.Address2County = fromVM.physicalAddressCountry;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressCountry != null))
            {
                toDynamics.Address2Stateorprovince = fromVM.physicalAddressProvince;
            }
            if (copyIfNull || (!copyIfNull && fromVM.physicalAddressPostalCode != null))
            {
                toDynamics.Address2Postalcode = fromVM.physicalAddressPostalCode;
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
                accountVM.mailingAddressStreet2 = account.Address1Line2;
                accountVM.mailingAddressCity = account.Address1City;
                accountVM.mailingAddressCountry = account.Address1County;
                accountVM.mailingAddressProvince = account.Address1Stateorprovince;
                accountVM.mailingAddressPostalCode = account.Address1Postalcode;

                accountVM.physicalAddressName = account.Address2Name;
                accountVM.physicalAddressStreet = account.Address2Line1;
                accountVM.physicalAddressStreet2 = account.Address2Line2;
                accountVM.physicalAddressCity = account.Address2City;
                accountVM.physicalAddressCountry = account.Address2County;
                accountVM.physicalAddressProvince = account.Address2Stateorprovince;
                accountVM.physicalAddressPostalCode = account.Address2Postalcode;

                if (account.Primarycontactid != null)
                {
                    accountVM.primarycontact = account.Primarycontactid.ToViewModel();
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
