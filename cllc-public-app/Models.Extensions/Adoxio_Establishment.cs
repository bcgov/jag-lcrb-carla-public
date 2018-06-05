using System;
using System.Collections.Generic;
using System.Linq;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.ViewModels;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_EstablishmentExtensions
    {


        /// <summary>
        /// Copy values from a Dynamics establishme t entity to another one
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Adoxio_establishment to, Adoxio_establishment from)
        {
            to.Adoxio_establishmentid = from.Adoxio_establishmentid;
            to.Adoxio_addresscity = from.Adoxio_addresscity;
            to.Adoxio_addresspostalcode = from.Adoxio_addresspostalcode;
            to.Adoxio_addressstreet = from.Adoxio_addressstreet;
            to.Adoxio_alreadyopen = from.Adoxio_alreadyopen;
            to.Adoxio_email = from.Adoxio_email;
            to.Adoxio_establishmentid = from.Adoxio_establishmentid;
            to.Adoxio_expectedopendate = from.Adoxio_expectedopendate;
            to.Adoxio_fridayclose = from.Adoxio_fridayclose;
            to.Adoxio_fridayopen = from.Adoxio_fridayopen;
            to.Adoxio_hasduallicence = from.Adoxio_hasduallicence;
            to.Adoxio_isrural = from.Adoxio_isrural;
            to.Adoxio_isstandalonepatio = from.Adoxio_isstandalonepatio;
            to.Adoxio_locatedatwinery = from.Adoxio_locatedatwinery;
            to.Adoxio_locatedonfirstnationland = from.Adoxio_locatedonfirstnationland;
            to.Adoxio_mailsenttorestaurant = from.Adoxio_mailsenttorestaurant;
            to.Adoxio_mondayclose = from.Adoxio_mondayclose;
            to.Adoxio_mondayopen = from.Adoxio_mondayopen;
            to.Adoxio_name = from.Adoxio_name;
            to.Adoxio_occupantcapacity = from.Adoxio_occupantcapacity;
            to.Adoxio_occupantload = from.Adoxio_occupantload;
            to.Adoxio_parcelid = from.Adoxio_parcelid;
            to.Adoxio_patronparticipation = from.Adoxio_patronparticipation;
            to.Adoxio_phone = from.Adoxio_phone;
            to.Adoxio_saturdayclose = from.Adoxio_saturdayclose;
            to.Adoxio_saturdayopen = from.Adoxio_saturdayopen;
            to.Adoxio_sendmailtoestablishmentuponapproval = from.Adoxio_sendmailtoestablishmentuponapproval;
            to.Adoxio_standardhours = from.Adoxio_standardhours;
            to.Adoxio_sundayclose = from.Adoxio_sundayclose;
            to.Adoxio_sundayopen = from.Adoxio_sundayopen;
            to.Adoxio_thursdayclose = from.Adoxio_thursdayclose;
            to.Adoxio_thursdayopen = from.Adoxio_thursdayopen;
            to.Adoxio_tuesdayclose = from.Adoxio_tuesdayclose;
            to.Adoxio_tuesdayopen = from.Adoxio_tuesdayopen;
            to.Adoxio_wednesdayclose = from.Adoxio_wednesdayclose;
            to.Adoxio_wednesdayopen = from.Adoxio_wednesdayopen;
            to.Createdon = from.Createdon;
            to.Importsequencenumber = from.Importsequencenumber;
            to.Modifiedon = from.Modifiedon;
            to.Overriddencreatedon = from.Overriddencreatedon;
            to.Statuscode = from.Statecode;
            to.Timezoneruleversionnumber = from.Timezoneruleversionnumber;
            to.Utcconversiontimezonecode = from.Utcconversiontimezonecode;
            to.Versionnumber = from.Versionnumber;
        }

        /// <summary>
        /// Copy values from a Dynamics establishme t entity to a view model.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this Adoxio_establishment to, ViewModels.AdoxioEstablishment from)
        {
            to.Adoxio_establishmentid = new Guid(from.id);

            if (to.Adoxio_addresscity != from.Addresscity)
            {
                to.Adoxio_addresscity = from.Addresscity;
            }
            if (to.Adoxio_addresspostalcode != from.Addresspostalcode)
            {
                to.Adoxio_addresspostalcode = from.Addresspostalcode;
            }
            if (to.Adoxio_addressstreet != from.Addressstreet)
            {
                to.Adoxio_addressstreet = from.Addressstreet;
            }
            if (to.Adoxio_alreadyopen != from.Alreadyopen)
            {
                to.Adoxio_alreadyopen = from.Alreadyopen;
            }
            if (to.Adoxio_email != from.Email)
            {
                to.Adoxio_email = from.Email;
            }
            if (to.Adoxio_expectedopendate != from.Expectedopendate)
            {
                to.Adoxio_expectedopendate = from.Expectedopendate;
            }
            if (to.Adoxio_fridayclose != from.Fridayclose)
            {
                to.Adoxio_fridayclose = from.Fridayclose;
            }
            if (to.Adoxio_fridayopen != from.Fridayopen)
            {
                to.Adoxio_fridayopen = from.Fridayopen;
            }
            if (to.Adoxio_hasduallicence != from.Hasduallicence)
            {
                to.Adoxio_hasduallicence = from.Hasduallicence;
            }
            if (to.Adoxio_isrural != from.Isrural)
            {
                to.Adoxio_isrural = from.Isrural;
            }
            if (to.Adoxio_isstandalonepatio != from.Isstandalonepatio)
            {
                to.Adoxio_isstandalonepatio = from.Isstandalonepatio;
            }
            if (to.Adoxio_locatedatwinery != from.Locatedatwinery)
            {
                to.Adoxio_locatedatwinery = from.Locatedatwinery;
            }
            if (to.Adoxio_locatedonfirstnationland != from.Locatedonfirstnationland)
            {
                to.Adoxio_locatedonfirstnationland = from.Locatedonfirstnationland;
            }
            if (to.Adoxio_mailsenttorestaurant != from.Mailsenttorestaurant)
            {
                to.Adoxio_mailsenttorestaurant = from.Mailsenttorestaurant;
            }
            if (to.Adoxio_mondayclose != from.Mondayclose)
            {
                to.Adoxio_mondayclose = from.Mondayclose;
            }
            if (to.Adoxio_mondayopen != from.Mondayopen)
            {
                to.Adoxio_mondayopen = from.Mondayopen;
            }
            if (to.Adoxio_name != from.Name)
            {
                to.Adoxio_name = from.Name;
            }
            if (to.Adoxio_occupantcapacity != from.Occupantcapacity)
            {
                to.Adoxio_occupantcapacity = from.Occupantcapacity;
            }
            if (to.Adoxio_occupantload != from.Occupantload)
            {
                to.Adoxio_occupantload = from.Occupantload;
            }
            if (to.Adoxio_parcelid != from.Parcelid)
            {
                to.Adoxio_parcelid = from.Parcelid;
            }
            if (to.Adoxio_patronparticipation != from.Patronparticipation)
            {
                to.Adoxio_patronparticipation = from.Patronparticipation;
            }
            if (to.Adoxio_phone != from.Phone)
            {
                to.Adoxio_phone = from.Phone;
            }
            if (to.Adoxio_saturdayclose != from.Saturdayclose)
            {
                to.Adoxio_saturdayclose = from.Saturdayclose;
            }
            if (to.Adoxio_saturdayopen != from.Saturdayopen)
            {
                to.Adoxio_saturdayopen = from.Saturdayopen;
            }
            if (to.Adoxio_sendmailtoestablishmentuponapproval != from.Sendmailtoestablishmentuponapproval)
            {
                to.Adoxio_sendmailtoestablishmentuponapproval = from.Sendmailtoestablishmentuponapproval;
            }
            if (to.Adoxio_standardhours != from.Standardhours)
            {
                to.Adoxio_standardhours = from.Standardhours;
            }
            if (to.Adoxio_sundayclose != from.Sundayclose)
            {
                to.Adoxio_sundayclose = from.Sundayclose;
            }
            if (to.Adoxio_sundayopen != from.Sundayopen)
            {
                to.Adoxio_sundayopen = from.Sundayopen;
            }
            if (to.Adoxio_thursdayclose != from.Thursdayclose)
            {
                to.Adoxio_thursdayclose = from.Thursdayclose;
            }
            if (to.Adoxio_thursdayopen != from.Thursdayopen)
            {
                to.Adoxio_thursdayopen = from.Thursdayopen;
            }
            if (to.Adoxio_tuesdayclose != from.Tuesdayclose)
            {
                to.Adoxio_tuesdayclose = from.Tuesdayclose;
            }
            if (to.Adoxio_tuesdayopen != from.Tuesdayopen)
            {
                to.Adoxio_tuesdayopen = from.Tuesdayopen;
            }
            if (to.Adoxio_wednesdayclose != from.Wednesdayclose)
            {
                to.Adoxio_wednesdayclose = from.Wednesdayclose;
            }
            if (to.Adoxio_wednesdayopen != from.Wednesdayopen)
            {
                to.Adoxio_wednesdayopen = from.Wednesdayopen;
            }
            if (to.Createdon != from.Createdon)
            {
                to.Createdon = from.Createdon;
            }
            if (to.Importsequencenumber != from.Importsequencenumber)
            {
                to.Importsequencenumber = from.Importsequencenumber;
            }
            if (to.Modifiedon != from.Modifiedon)
            {
                to.Modifiedon = from.Modifiedon;
            }
            if (to.Overriddencreatedon != from.Overriddencreatedon)
            {
                to.Overriddencreatedon = from.Overriddencreatedon;
            }
            if (to.Statuscode != from.StatusCode)
            {
                to.Statuscode = from.StatusCode;
            }
            if (to.Statecode != from.StateCode)
            {
                to.Statecode = from.StateCode;
            }
            if (to.Timezoneruleversionnumber != from.Timezoneruleversionnumber)
            {
                to.Timezoneruleversionnumber = from.Timezoneruleversionnumber;
            }
            if (to.Utcconversiontimezonecode != from.Utcconversiontimezonecode)
            {
                to.Utcconversiontimezonecode = from.Utcconversiontimezonecode;
            }
            if (to.Versionnumber != from.Versionnumber)
            {
                to.Versionnumber = from.Versionnumber;
            }
        }

        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.AdoxioEstablishment ToViewModel(this Adoxio_establishment adoxio_establishment)
        {
            ViewModels.AdoxioEstablishment result = null;
            if (adoxio_establishment != null)
            {
                result = new ViewModels.AdoxioEstablishment();
                if (adoxio_establishment.Adoxio_establishmentid != null)
                {
                    result.id = adoxio_establishment.Adoxio_establishmentid.ToString();
                }

                result._licencee_value = adoxio_establishment._adoxio_licencee_value;
                result._licencetypeid_value = adoxio_establishment._adoxio_licencetypeid_value;
                result._municipality_value = adoxio_establishment._adoxio_municipality_value;
                result._policejurisdiction_value = adoxio_establishment._adoxio_policejurisdiction_value;
                result._primaryinspectorid_value = adoxio_establishment._adoxio_primaryinspectorid_value;
                result._territory_value = adoxio_establishment._adoxio_territory_value;
                result._createdby_value = adoxio_establishment._createdby_value;
                result._createdonbehalfby_value = adoxio_establishment._createdonbehalfby_value;
                result._modifiedby_value = adoxio_establishment._modifiedby_value;
                result._modifiedonbehalfby_value = adoxio_establishment._modifiedonbehalfby_value;
                result._ownerid_value = adoxio_establishment._ownerid_value;
                result._owningbusinessunit_value = adoxio_establishment._owningbusinessunit_value;
                result._owningteam_value = adoxio_establishment._owningteam_value;
                result._owninguser_value = adoxio_establishment._owninguser_value;
                result.Addresscity = adoxio_establishment.Adoxio_addresscity;
                result.Addresspostalcode = adoxio_establishment.Adoxio_addresspostalcode;
                result.Addressstreet = adoxio_establishment.Adoxio_addressstreet;
                result.Alreadyopen = adoxio_establishment.Adoxio_alreadyopen;
                result.Email = adoxio_establishment.Adoxio_email;
                result.Expectedopendate = adoxio_establishment.Adoxio_expectedopendate;
                result.Fridayclose = adoxio_establishment.Adoxio_fridayclose;
                result.Fridayopen = adoxio_establishment.Adoxio_fridayopen;
                result.Hasduallicence = adoxio_establishment.Adoxio_hasduallicence;
                result.Isrural = adoxio_establishment.Adoxio_isrural;
                result.Isstandalonepatio = adoxio_establishment.Adoxio_isstandalonepatio;
                result.Locatedatwinery = adoxio_establishment.Adoxio_locatedatwinery;
                result.Locatedonfirstnationland = adoxio_establishment.Adoxio_locatedonfirstnationland;
                result.Mailsenttorestaurant = adoxio_establishment.Adoxio_mailsenttorestaurant;
                result.Mondayclose = adoxio_establishment.Adoxio_mondayclose;
                result.Mondayopen = adoxio_establishment.Adoxio_mondayopen;
                result.Name = adoxio_establishment.Adoxio_name;
                result.Occupantcapacity = adoxio_establishment.Adoxio_occupantcapacity;
                result.Occupantload = adoxio_establishment.Adoxio_occupantload;
                result.Parcelid = adoxio_establishment.Adoxio_parcelid;
                result.Patronparticipation = adoxio_establishment.Adoxio_patronparticipation;
                result.Phone = adoxio_establishment.Adoxio_phone;
                result.Saturdayclose = adoxio_establishment.Adoxio_saturdayclose;
                result.Saturdayopen = adoxio_establishment.Adoxio_saturdayopen;
                result.Sendmailtoestablishmentuponapproval = adoxio_establishment.Adoxio_sendmailtoestablishmentuponapproval;
                result.Standardhours = adoxio_establishment.Adoxio_standardhours;
                result.Sundayclose = adoxio_establishment.Adoxio_sundayclose;
                result.Sundayopen = adoxio_establishment.Adoxio_sundayopen;
                result.Thursdayclose = adoxio_establishment.Adoxio_thursdayclose;
                result.Thursdayopen = adoxio_establishment.Adoxio_thursdayopen;
                result.Tuesdayclose = adoxio_establishment.Adoxio_tuesdayclose;
                result.Tuesdayopen = adoxio_establishment.Adoxio_tuesdayopen;
                result.Wednesdayclose = adoxio_establishment.Adoxio_wednesdayclose;
                result.Wednesdayopen = adoxio_establishment.Adoxio_wednesdayopen;
                result.Createdon = adoxio_establishment.Createdon;
                result.Importsequencenumber = adoxio_establishment.Importsequencenumber;
                result.Modifiedon = adoxio_establishment.Modifiedon;
                result.Overriddencreatedon = adoxio_establishment.Overriddencreatedon;
                result.StatusCode = adoxio_establishment.Statuscode;
                result.StateCode = adoxio_establishment.Statecode;
                result.Timezoneruleversionnumber = adoxio_establishment.Timezoneruleversionnumber;
                result.Utcconversiontimezonecode = adoxio_establishment.Utcconversiontimezonecode;
                result.Versionnumber = adoxio_establishment.Versionnumber;

            }
            return result;
        }

        /// <summary>
        /// Convert a establishme t entity to a model
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Adoxio_establishment ToModel(this ViewModels.AdoxioEstablishment from)
        {
            Adoxio_establishment result = null;
            if (from != null)
            {
                result = new Adoxio_establishment();

                result.Adoxio_establishmentid = new Guid(from.id);
                result._adoxio_licencee_value = from._licencee_value;
                result._adoxio_licencetypeid_value = from._licencetypeid_value;
                result._adoxio_municipality_value = from._municipality_value;
                result._adoxio_policejurisdiction_value = from._policejurisdiction_value;
                result._adoxio_primaryinspectorid_value = from._primaryinspectorid_value;
                result._adoxio_territory_value = from._territory_value;
                result._createdby_value = from._createdby_value;
                result._createdonbehalfby_value = from._createdonbehalfby_value;
                result._modifiedby_value = from._modifiedby_value;
                result._modifiedonbehalfby_value = from._modifiedonbehalfby_value;
                result._ownerid_value = from._ownerid_value;
                result._owningbusinessunit_value = from._owningbusinessunit_value;
                result._owningteam_value = from._owningteam_value;
                result._owninguser_value = from._owninguser_value;
                result.Adoxio_addresscity = from.Addresscity;
                result.Adoxio_addresspostalcode = from.Addresspostalcode;
                result.Adoxio_addressstreet = from.Addressstreet;
                result.Adoxio_alreadyopen = from.Alreadyopen;
                result.Adoxio_email = from.Email;
                result.Adoxio_expectedopendate = from.Expectedopendate;
                result.Adoxio_fridayclose = from.Fridayclose;
                result.Adoxio_fridayopen = from.Fridayopen;
                result.Adoxio_hasduallicence = from.Hasduallicence;
                result.Adoxio_isrural = from.Isrural;
                result.Adoxio_isstandalonepatio = from.Isstandalonepatio;
                result.Adoxio_locatedatwinery = from.Locatedatwinery;
                result.Adoxio_locatedonfirstnationland = from.Locatedonfirstnationland;
                result.Adoxio_mailsenttorestaurant = from.Mailsenttorestaurant;
                result.Adoxio_mondayclose = from.Mondayclose;
                result.Adoxio_mondayopen = from.Mondayopen;
                result.Adoxio_name = from.Name;
                result.Adoxio_occupantcapacity = from.Occupantcapacity;
                result.Adoxio_occupantload = from.Occupantload;
                result.Adoxio_parcelid = from.Parcelid;
                result.Adoxio_patronparticipation = from.Patronparticipation;
                result.Adoxio_phone = from.Phone;
                result.Adoxio_saturdayclose = from.Saturdayclose;
                result.Adoxio_saturdayopen = from.Saturdayopen;
                result.Adoxio_sendmailtoestablishmentuponapproval = from.Sendmailtoestablishmentuponapproval;
                result.Adoxio_standardhours = from.Standardhours;
                result.Adoxio_sundayclose = from.Sundayclose;
                result.Adoxio_sundayopen = from.Sundayopen;
                result.Adoxio_thursdayclose = from.Thursdayclose;
                result.Adoxio_thursdayopen = from.Thursdayopen;
                result.Adoxio_tuesdayclose = from.Tuesdayclose;
                result.Adoxio_tuesdayopen = from.Tuesdayopen;
                result.Adoxio_wednesdayclose = from.Wednesdayclose;
                result.Adoxio_wednesdayopen = from.Wednesdayopen;
                result.Createdon = from.Createdon;
                result.Importsequencenumber = from.Importsequencenumber;
                result.Modifiedon = from.Modifiedon;
                result.Overriddencreatedon = from.Overriddencreatedon;
                result.Statuscode = from.StatusCode;
                result.Statecode = from.StateCode;
                result.Timezoneruleversionnumber = from.Timezoneruleversionnumber;
                result.Utcconversiontimezonecode = from.Utcconversiontimezonecode;
                result.Versionnumber = from.Versionnumber;
            }
            return result;
        }
    }
}
