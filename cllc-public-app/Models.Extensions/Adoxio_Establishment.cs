using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_EstablishmentExtensions
    {


        /// <summary>
        /// Copy values from a Dynamics establishme t entity to a view model.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        public static void CopyValues(this MicrosoftDynamicsCRMadoxioEstablishment to, ViewModels.Establishment from)
        {
            // Only copy email and phone number
            if (from.Email != null)
            {
                to.AdoxioEmail = from.Email;
            }

            if (from.Phone != null)
            {
                to.AdoxioPhone = from.Phone;
            }

        }

        /// <summary>
        /// Convert a given voteQuestion to a ViewModel
        /// </summary>        
        public static ViewModels.Establishment ToViewModel(this MicrosoftDynamicsCRMadoxioEstablishment adoxio_establishment)
        {
            ViewModels.Establishment result = null;
            if (adoxio_establishment != null)
            {
                result = new ViewModels.Establishment();
                if (adoxio_establishment.AdoxioEstablishmentid != null)
                {
                    result.id = adoxio_establishment.AdoxioEstablishmentid.ToString();
                }

                result._licencee_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioLicenceeValue);
                result._licencetypeid_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioLicencetypeidValue);
                //result._municipality_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioMunicipalityValue);
                result._policejurisdiction_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioPdjurisdictionValue);
                result._primaryinspectorid_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioPrimaryinspectoridValue);
                result._territory_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._adoxioTerritoryValue);
                result._createdby_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._createdbyValue);
                result._createdonbehalfby_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._createdonbehalfbyValue);
                result._modifiedby_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._modifiedbyValue);
                result._modifiedonbehalfby_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._modifiedonbehalfbyValue);
                result._ownerid_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._owneridValue);
                result._owningbusinessunit_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._owningbusinessunitValue);
                result._owningteam_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._owningteamValue);
                result._owninguser_value = GuidUtility.SafeNullableGuidConvert(adoxio_establishment._owninguserValue);
                result.Addresscity = adoxio_establishment.AdoxioAddresscity;
                result.Addresspostalcode = adoxio_establishment.AdoxioAddresspostalcode;
                result.Addressstreet = adoxio_establishment.AdoxioAddressstreet;
                result.Alreadyopen = adoxio_establishment.AdoxioAlreadyopen;
                result.Email = adoxio_establishment.AdoxioEmail;
                result.Expectedopendate = adoxio_establishment.AdoxioExpectedopendate;
                result.Fridayclose = adoxio_establishment.AdoxioFridayclose;
                result.Fridayopen = adoxio_establishment.AdoxioFridayopen;
                result.Hasduallicence = adoxio_establishment.AdoxioHasduallicence;
                result.Isrural = adoxio_establishment.AdoxioIsrural;
                result.Isstandalonepatio = adoxio_establishment.AdoxioIsstandalonepatio;
                result.Locatedatwinery = adoxio_establishment.AdoxioLocatedatwinery;
                result.Locatedonfirstnationland = adoxio_establishment.AdoxioLocatedonfirstnationland;
                result.Mailsenttorestaurant = adoxio_establishment.AdoxioMailsenttorestaurant;
                result.Mondayclose = adoxio_establishment.AdoxioMondayclose;
                result.Mondayopen = adoxio_establishment.AdoxioMondayopen;
                result.Name = adoxio_establishment.AdoxioName;
                result.Occupantcapacity = adoxio_establishment.AdoxioOccupantcapacity;
                result.Occupantload = adoxio_establishment.AdoxioOccupantload;
                result.Parcelid = adoxio_establishment.AdoxioParcelid;
                result.Patronparticipation = adoxio_establishment.AdoxioPatronparticipation;
                result.Phone = adoxio_establishment.AdoxioPhone;
                result.Saturdayclose = adoxio_establishment.AdoxioSaturdayclose;
                result.Saturdayopen = adoxio_establishment.AdoxioSaturdayopen;
                result.Sendmailtoestablishmentuponapproval = adoxio_establishment.AdoxioSendmailtoestablishmentuponapproval;
                result.Standardhours = adoxio_establishment.AdoxioStandardhours;
                result.Sundayclose = adoxio_establishment.AdoxioSundayclose;
                result.Sundayopen = adoxio_establishment.AdoxioSundayopen;
                result.Thursdayclose = adoxio_establishment.AdoxioThursdayclose;
                result.Thursdayopen = adoxio_establishment.AdoxioThursdayopen;
                result.Tuesdayclose = adoxio_establishment.AdoxioTuesdayclose;
                result.Tuesdayopen = adoxio_establishment.AdoxioTuesdayopen;
                result.Wednesdayclose = adoxio_establishment.AdoxioWednesdayclose;
                result.Wednesdayopen = adoxio_establishment.AdoxioWednesdayopen;
                result.Createdon = adoxio_establishment.Createdon;
                result.Importsequencenumber = adoxio_establishment.Importsequencenumber;
                result.Modifiedon = adoxio_establishment.Modifiedon;
                result.Overriddencreatedon = adoxio_establishment.Overriddencreatedon;
                result.StatusCode = adoxio_establishment.Statuscode;
                result.StateCode = adoxio_establishment.Statecode;
                result.Timezoneruleversionnumber = adoxio_establishment.Timezoneruleversionnumber;
                result.Utcconversiontimezonecode = adoxio_establishment.Utcconversiontimezonecode;
                if (adoxio_establishment.Versionnumber != null)
                {
                    result.Versionnumber = adoxio_establishment.Versionnumber;
                }

            }
            return result;
        }

        /// <summary>
        /// Convert a establishme t entity to a model
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public static MicrosoftDynamicsCRMadoxioEstablishment ToModel(this ViewModels.Establishment from)
        {
            MicrosoftDynamicsCRMadoxioEstablishment result = null;
            if (from != null)
            {
                result = new MicrosoftDynamicsCRMadoxioEstablishment();

                result.AdoxioEstablishmentid = from.id;
                result._adoxioLicenceeValue = from._licencee_value.ToString();
                result._adoxioLicencetypeidValue = from._licencetypeid_value.ToString();
                //result. = from._municipality_value.ToString();
                result._adoxioPdjurisdictionValue = from._policejurisdiction_value.ToString();
                result._adoxioPrimaryinspectoridValue = from._primaryinspectorid_value.ToString();
                result._adoxioTerritoryValue = from._territory_value.ToString();
                result._createdbyValue = from._createdby_value.ToString();
                result._createdonbehalfbyValue = from._createdonbehalfby_value.ToString();
                result._modifiedbyValue = from._modifiedby_value.ToString();
                result._modifiedonbehalfbyValue = from._modifiedonbehalfby_value.ToString();
                result._owneridValue = from._ownerid_value.ToString();
                result._owningbusinessunitValue = from._owningbusinessunit_value.ToString();
                result._owningteamValue = from._owningteam_value.ToString();
                result._owninguserValue = from._owninguser_value.ToString();
                result.AdoxioAddresscity = from.Addresscity;
                result.AdoxioAddresspostalcode = from.Addresspostalcode;
                result.AdoxioAddressstreet = from.Addressstreet;
                result.AdoxioAlreadyopen = from.Alreadyopen;
                result.AdoxioEmail = from.Email;
                result.AdoxioExpectedopendate = from.Expectedopendate;
                result.AdoxioFridayclose = from.Fridayclose;
                result.AdoxioFridayopen = from.Fridayopen;
                result.AdoxioHasduallicence = from.Hasduallicence;
                result.AdoxioIsrural = from.Isrural;
                result.AdoxioIsstandalonepatio = from.Isstandalonepatio;
                result.AdoxioLocatedatwinery = from.Locatedatwinery;
                result.AdoxioLocatedonfirstnationland = from.Locatedonfirstnationland;
                result.AdoxioMailsenttorestaurant = from.Mailsenttorestaurant;
                result.AdoxioMondayclose = from.Mondayclose;
                result.AdoxioMondayopen = from.Mondayopen;
                result.AdoxioName = from.Name;
                result.AdoxioOccupantcapacity = from.Occupantcapacity;
                result.AdoxioOccupantload = from.Occupantload;
                result.AdoxioParcelid = from.Parcelid;
                result.AdoxioPatronparticipation = from.Patronparticipation;
                result.AdoxioPhone = from.Phone;
                result.AdoxioSaturdayclose = from.Saturdayclose;
                result.AdoxioSaturdayopen = from.Saturdayopen;
                result.AdoxioSendmailtoestablishmentuponapproval = from.Sendmailtoestablishmentuponapproval;
                result.AdoxioStandardhours = from.Standardhours;
                result.AdoxioSundayclose = from.Sundayclose;
                result.AdoxioSundayopen = from.Sundayopen;
                result.AdoxioThursdayclose = from.Thursdayclose;
                result.AdoxioThursdayopen = from.Thursdayopen;
                result.AdoxioTuesdayclose = from.Tuesdayclose;
                result.AdoxioTuesdayopen = from.Tuesdayopen;
                result.AdoxioWednesdayclose = from.Wednesdayclose;
                result.AdoxioWednesdayopen = from.Wednesdayopen;
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
