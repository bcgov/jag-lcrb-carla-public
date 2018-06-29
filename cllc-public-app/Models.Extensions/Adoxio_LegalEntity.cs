using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_LegalEntityExtensions
    {


        /// <summary>
        /// Copy values from a Dynamics legal entity to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Adoxio_legalentity to, Adoxio_legalentity from)
        {
            to.Adoxio_legalentityid = from.Adoxio_legalentityid;
            to.Adoxio_commonnonvotingshares = from.Adoxio_commonnonvotingshares;
            to.Adoxio_commonvotingshares = from.Adoxio_commonvotingshares;
            to.Adoxio_dateofbirth = from.Adoxio_dateofbirth;
            to.Adoxio_firstname = from.Adoxio_firstname;
            to.Adoxio_interestpercentage = from.Adoxio_interestpercentage;
            to.Adoxio_isindividual = from.Adoxio_isindividual;
            to.Adoxio_lastname = from.Adoxio_lastname;
            to.Adoxio_legalentitytype = from.Adoxio_legalentitytype;
            to.Adoxio_middlename = from.Adoxio_middlename;
            to.Adoxio_name = from.Adoxio_name;
            to.Adoxio_position = from.Adoxio_position;
			to.Adoxio_ispartner = from.Adoxio_ispartner;
			to.Adoxio_isshareholder = from.Adoxio_isshareholder;
			to.Adoxio_istrustee = from.Adoxio_istrustee;
			to.Adoxio_isdirector = from.Adoxio_isdirector;
			to.Adoxio_isofficer = from.Adoxio_isofficer;
			to.Adoxio_isowner = from.Adoxio_isowner;
            to.Adoxio_preferrednonvotingshares = from.Adoxio_preferrednonvotingshares;
            to.Adoxio_preferredvotingshares = from.Adoxio_preferredvotingshares;
            to.Adoxio_sameasapplyingperson = from.Adoxio_sameasapplyingperson;
            to.Adoxio_dateofappointment = from.Adoxio_dateofappointment;
        }

        /// <summary>
        /// Copy values from View Model to Dynamics legal entity
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Adoxio_legalentity to, ViewModels.AdoxioLegalEntity from, Interfaces.Microsoft.Dynamics.CRM.System _system)
        {
                     
            to.Adoxio_commonnonvotingshares = from.commonnonvotingshares;
            to.Adoxio_commonvotingshares = from.commonvotingshares;
            to.Adoxio_dateofbirth = from.dateofbirth;
            to.Adoxio_firstname = from.firstname;
            to.Adoxio_interestpercentage = from.interestpercentage;
            to.Adoxio_isindividual = (from.isindividual != null && (bool)from.isindividual) ? 1 : 0;
            to.Adoxio_lastname = from.lastname;
            to.Adoxio_legalentitytype = (int?)from.legalentitytype;
            to.Adoxio_middlename = from.middlename;
            to.Adoxio_name = from.name;
            to.Adoxio_position = (int?)from.position;
			to.Adoxio_ispartner = false;
            to.Adoxio_isshareholder = false;
            to.Adoxio_istrustee = false;
			to.Adoxio_isdirector = false;
			to.Adoxio_isofficer = false;
			to.Adoxio_isowner = false;
            switch ((int?)from.position)
            {
                case 0:
					to.Adoxio_ispartner = true;
                    break;
                case 1:
					to.Adoxio_isshareholder = true;
                    break;
                case 2:
					to.Adoxio_istrustee = true;
                    break;
                case 3:
					to.Adoxio_isdirector = true;
                    break;
                case 4:
					to.Adoxio_isofficer = true;
                    break;
                case 5:
					to.Adoxio_isowner = true;
                    break;
            }
            to.Adoxio_preferrednonvotingshares = from.preferrednonvotingshares;
            to.Adoxio_preferredvotingshares = from.preferredvotingshares;
            to.Adoxio_sameasapplyingperson = (from.sameasapplyingperson != null && (bool)from.sameasapplyingperson) ? 1 : 0;
            to.Adoxio_email = from.email;
            to.Adoxio_dateofappointment = from.dateofappointment;
            // Assigning the account this way throws exception:
            // System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            //if (from.account.id != null)
            //{
            //    // fetch the account from Dynamics.
            //    var getAccountTask = _system.GetAccountById(null, Guid.Parse(from.account.id));
            //    getAccountTask.Wait();
            //    to.Adoxio_Account= getAccountTask.Result;
            //}
            // adoxio_dateemailsent
        }


        /// <summary>
        /// Copy values from View Model to Dynamics legal entity
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioLegalentity to, ViewModels.AdoxioLegalEntity from)
        {

            to.AdoxioCommonnonvotingshares = from.commonnonvotingshares;
            to.AdoxioCommonvotingshares = from.commonvotingshares;
            to.AdoxioDateofbirth = from.dateofbirth;
            to.AdoxioFirstname = from.firstname;
            to.AdoxioInterestpercentage = (double?) from.interestpercentage;
            to.AdoxioIsindividual = (from.isindividual != null && (bool)from.isindividual) ? 1 : 0;
            to.AdoxioLastname = from.lastname;
            to.AdoxioLegalentitytype = (int?)from.legalentitytype;
            to.AdoxioMiddlename = from.middlename;
            to.AdoxioName = from.name;
            to.AdoxioPosition = (int?)from.position;
			to.AdoxioIspartner = false;
			to.AdoxioIsshareholder = false;
			to.AdoxioIstrustee = false;
			to.AdoxioIsdirector = false;
			to.AdoxioIsofficer = false;
			to.AdoxioIsowner = false;
			switch ((int?)from.position)
			{
				case 0:
                    to.AdoxioIspartner = true;
                    break;
				case 1:
                    to.AdoxioIsshareholder = true;
                    break;
				case 2:
                    to.AdoxioIstrustee = true;
                    break;
				case 3:
                    to.AdoxioIsdirector = true;
                    break;
				case 4:
                    to.AdoxioIsofficer = true;
                    break;
				case 5:
                    to.AdoxioIsowner = true;
                    break;
			}
            to.AdoxioPreferrednonvotingshares = from.preferrednonvotingshares;
            to.AdoxioPreferredvotingshares = from.preferredvotingshares;
            to.AdoxioSameasapplyingperson = (from.sameasapplyingperson != null && (bool)from.sameasapplyingperson) ? 1 : 0;
            to.AdoxioEmail = from.email;
            to.AdoxioDateofappointment = from.dateofappointment;
            // Assigning the account this way throws exception:
            // System.InvalidOperationException: Collection was modified; enumeration operation may not execute.
            //if (from.account.id != null)
            //{
            //    // fetch the account from Dynamics.
            //    var getAccountTask = _system.GetAccountById(null, Guid.Parse(from.account.id));
            //    getAccountTask.Wait();
            //    to.Adoxio_Account= getAccountTask.Result;
            //}
            // adoxio_dateemailsent
        }

        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.AdoxioLegalEntity ToViewModel(this Adoxio_legalentity adoxio_legalentity)
        {
            ViewModels.AdoxioLegalEntity result = null;
            if (adoxio_legalentity != null)
            {
                result = new ViewModels.AdoxioLegalEntity();
                if (adoxio_legalentity.Adoxio_legalentityid != null)
                {
                    result.id = adoxio_legalentity.Adoxio_legalentityid.ToString();
                }
                
                result.commonnonvotingshares = adoxio_legalentity.Adoxio_commonnonvotingshares;
                result.commonvotingshares = adoxio_legalentity.Adoxio_commonvotingshares;
                result.dateofbirth = adoxio_legalentity.Adoxio_dateofbirth;
                result.firstname = adoxio_legalentity.Adoxio_firstname;
                result.interestpercentage = adoxio_legalentity.Adoxio_interestpercentage;
                // convert from int to bool.
                result.isindividual = (adoxio_legalentity.Adoxio_isindividual != null && adoxio_legalentity.Adoxio_isindividual != 0);
                result.lastname = adoxio_legalentity.Adoxio_lastname;
                if (adoxio_legalentity.Adoxio_legalentitytype != null)
                {
                    result.legalentitytype = (Adoxio_applicanttypecodes)adoxio_legalentity.Adoxio_legalentitytype;
                }
                
                result.middlename = adoxio_legalentity.Adoxio_middlename;
                result.name = adoxio_legalentity.Adoxio_name;
                result.email = adoxio_legalentity.Adoxio_email;
                if (adoxio_legalentity.Adoxio_position != null)
                {
                    result.position = (PositionOptions)adoxio_legalentity.Adoxio_position;
                }
                
                result.preferrednonvotingshares = adoxio_legalentity.Adoxio_preferrednonvotingshares;
                result.preferredvotingshares = adoxio_legalentity.Adoxio_preferredvotingshares;
                // convert from int to bool.
                result.sameasapplyingperson = (adoxio_legalentity.Adoxio_sameasapplyingperson != null && adoxio_legalentity.Adoxio_sameasapplyingperson != 0);
                result.dateofappointment = adoxio_legalentity.Adoxio_dateofappointment;

                // populate the account.
                if (adoxio_legalentity.Adoxio_Account != null)
                {
                    result.account = adoxio_legalentity.Adoxio_Account.ToViewModel();
                }

                result.accountId = adoxio_legalentity._adoxio_account_value.ToString();
                
            }            
            return result;
        }

        /// <summary>
        /// Convert a Dynamics Legal Entity to a ViewModel
        /// </summary>        
        public static ViewModels.AdoxioLegalEntity ToViewModel(this MicrosoftDynamicsCRMadoxioLegalentity adoxio_legalentity)
        {
            ViewModels.AdoxioLegalEntity result = null;
            if (adoxio_legalentity != null)
            {
                result = new ViewModels.AdoxioLegalEntity();
                if (adoxio_legalentity.AdoxioLegalentityid != null)
                {
                    result.id = adoxio_legalentity.AdoxioLegalentityid.ToString();
                }

                result.commonnonvotingshares = adoxio_legalentity.AdoxioCommonnonvotingshares;
                result.commonvotingshares = adoxio_legalentity.AdoxioCommonvotingshares;
                result.dateofbirth = adoxio_legalentity.AdoxioDateofbirth;
                result.firstname = adoxio_legalentity.AdoxioFirstname;
                result.interestpercentage = (decimal?) adoxio_legalentity.AdoxioInterestpercentage;
                // convert from int to bool.
                result.isindividual = (adoxio_legalentity.AdoxioIsindividual != null && adoxio_legalentity.AdoxioIsindividual != 0);
                result.lastname = adoxio_legalentity.AdoxioLastname;
                if (adoxio_legalentity.AdoxioLegalentitytype != null)
                {
                    result.legalentitytype = (Adoxio_applicanttypecodes)adoxio_legalentity.AdoxioLegalentitytype;
                }

                result.middlename = adoxio_legalentity.AdoxioMiddlename;
                result.name = adoxio_legalentity.AdoxioName;
                result.email = adoxio_legalentity.AdoxioEmail;
				if ((bool)adoxio_legalentity.AdoxioIspartner)
                {
					result.position = PositionOptions.Partner;
                }
				else if ((bool)adoxio_legalentity.AdoxioIsshareholder)
                {
					result.position = PositionOptions.Shareholder;
                }
				else if ((bool)adoxio_legalentity.AdoxioIstrustee)
				{
					result.position = PositionOptions.Trustee;
                }
				else if ((bool)adoxio_legalentity.AdoxioIsdirector)
				{
					result.position = PositionOptions.Director;
                }
				else if ((bool)adoxio_legalentity.AdoxioIsofficer)
				{
					result.position = PositionOptions.Officer;
                }
				else if ((bool)adoxio_legalentity.AdoxioIsowner)
				{
					result.position = PositionOptions.Owner;
                }

                result.preferrednonvotingshares = adoxio_legalentity.AdoxioPreferrednonvotingshares;
                result.preferredvotingshares = adoxio_legalentity.AdoxioPreferredvotingshares;
                // convert from int to bool.
                result.sameasapplyingperson = (adoxio_legalentity.AdoxioSameasapplyingperson != null && adoxio_legalentity.AdoxioSameasapplyingperson != 0);
                result.dateofappointment = adoxio_legalentity.AdoxioDateofappointment;

                // populate the account.
                if (adoxio_legalentity.AdoxioAccount != null)
                {
                    result.account = adoxio_legalentity.AdoxioAccount.ToViewModel();
                }

            }
            return result;
        }

        /// <summary>
        /// Convert a legal entity to a model
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Adoxio_legalentity ToModel(this ViewModels.AdoxioLegalEntity from)
        {
            Adoxio_legalentity result = null;
            if (from != null)
            {
                result = new Adoxio_legalentity();
                
                result.Adoxio_legalentityid = new Guid(from.id);
                result.Adoxio_commonnonvotingshares = from.commonnonvotingshares;
                result.Adoxio_commonvotingshares = from.commonvotingshares;
                result.Adoxio_dateofbirth = from.dateofbirth;
                result.Adoxio_firstname = from.firstname;
                result.Adoxio_interestpercentage = from.interestpercentage;
                result.Adoxio_isindividual = (from.isindividual != null && (bool)from.isindividual) ? 1 : 0; 
                result.Adoxio_lastname = from.lastname;
                result.Adoxio_legalentitytype = (int?) from.legalentitytype;
                result.Adoxio_middlename = from.middlename;
                result.Adoxio_name = from.name;
                result.Adoxio_position = (int?) from.position;
				switch ((int?)from.position)
                {
                    case 0:
						result.Adoxio_ispartner = true;
                        break;
                    case 1:
						result.Adoxio_isshareholder = true;
                        break;
                    case 2:
						result.Adoxio_istrustee = true;
                        break;
                    case 3:
						result.Adoxio_isdirector = true;
                        break;
                    case 4:
						result.Adoxio_isofficer = true;
                        break;
                    case 5:
						result.Adoxio_isowner = true;
                        break;
                }
                result.Adoxio_preferrednonvotingshares = from.preferrednonvotingshares;
                result.Adoxio_preferredvotingshares = from.preferredvotingshares;
                result.Adoxio_sameasapplyingperson = (from.sameasapplyingperson != null && (bool)from.sameasapplyingperson) ? 1 : 0;
                result.Adoxio_dateofappointment = from.dateofappointment;
            }
            return result;
        }

        /// <summary>
        /// Get the query string to filter a legal entity by position(s)
        /// Partner     = adoxio_position value of 0
        /// Shareholder = adoxio_position value of 1
        /// Trustee     = adoxio_position value of 2
        /// Director    = adoxio_position value of 3
        /// Officer     = adoxio_position value of 4
        /// Owner       = adoxio_position value of 5 
        /// </summary>
        /// <param name="positionType"></param>
        /// <returns>string</returns>
        public static string GetPositionFilter(string positionType)
        {
            String filter = null;
            positionType = positionType.ToLower();

            switch (positionType)
            {
                case "partner":
                    filter = "adoxio_position eq 0";
                    break;
                case "shareholder":
                    filter = "adoxio_position eq 1";
                    break;
                case "trustee":
                    filter = "adoxio_position eq 2";
                    break;
                case "director":
                    filter = "adoxio_position eq 3";
                    break;
                case "officer":
                    filter = "adoxio_position eq 4";
                    break;
                case "owner":
                    filter = "adoxio_position eq 5";
                    break;
                case "director-officer":
					filter = "(adoxio_position eq 3 or adoxio_position eq 4)";
                    break;
                case "director-officer-shareholder":
					filter = "(adoxio_position eq 3 or adoxio_position eq 4 or adoxio_position eq 1)";
                    break;
            }

            return filter;
        }


    }

}
