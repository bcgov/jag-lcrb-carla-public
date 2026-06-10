extern alias DV;
using System;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.Xrm.Sdk;

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

            if (from.IsOpen != null)
            {
                to.AdoxioIsopen = from.IsOpen;
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
                    result.id = adoxio_establishment.AdoxioEstablishmentid;
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
                result.IsOpen = adoxio_establishment.AdoxioIsopen;
                if (adoxio_establishment.Versionnumber != null)
                {
                    result.Versionnumber = adoxio_establishment.Versionnumber;
                }

            }
            return result;
        }

        // -----------------------------------------------------------------------
        // Dataverse SDK (DV) versions
        // -----------------------------------------------------------------------

        public static void CopyValues(this adoxio_establishment to, ViewModels.Establishment from)
        {
            if (from.Email != null) to.adoxio_Email = from.Email;
            if (from.Phone != null) to.adoxio_Phone = from.Phone;
            if (from.IsOpen != null) to.adoxio_IsOpen = from.IsOpen;
        }

        public static ViewModels.Establishment ToViewModel(this adoxio_establishment e)
        {
            if (e == null) return null;
            var result = new ViewModels.Establishment();
            result.id = e.Id == Guid.Empty ? null : e.Id.ToString();
            result._licencee_value = e.adoxio_Licencee?.Id;
            result._licencetypeid_value = e.adoxio_LicenceTypeId?.Id;
            result._policejurisdiction_value = e.adoxio_PDJurisdiction?.Id;
            result._primaryinspectorid_value = e.adoxio_PrimaryInspectorId?.Id;
            result._territory_value = e.adoxio_Territory?.Id;
            result._createdby_value = e.GetAttributeValue<EntityReference>("createdby")?.Id;
            result._createdonbehalfby_value = e.GetAttributeValue<EntityReference>("createdonbehalfby")?.Id;
            result._modifiedby_value = e.GetAttributeValue<EntityReference>("modifiedby")?.Id;
            result._modifiedonbehalfby_value = e.GetAttributeValue<EntityReference>("modifiedonbehalfby")?.Id;
            result._ownerid_value = e.GetAttributeValue<EntityReference>("ownerid")?.Id;
            result._owningbusinessunit_value = e.GetAttributeValue<EntityReference>("owningbusinessunit")?.Id;
            result._owningteam_value = e.GetAttributeValue<EntityReference>("owningteam")?.Id;
            result._owninguser_value = e.GetAttributeValue<EntityReference>("owninguser")?.Id;
            result.Addresscity = e.adoxio_AddressCity;
            result.Addresspostalcode = e.adoxio_AddressPostalCode;
            result.Addressstreet = e.adoxio_AddressStreet;
            result.Alreadyopen = e.adoxio_AlreadyOpen;
            result.Email = e.adoxio_Email;
            result.Expectedopendate = e.adoxio_ExpectedOpenDate.HasValue
                ? (System.DateTimeOffset?)new System.DateTimeOffset(e.adoxio_ExpectedOpenDate.Value, System.TimeSpan.Zero)
                : null;
            result.Fridayclose = (int?)e.adoxio_FridayClose;
            result.Fridayopen = (int?)e.adoxio_FridayOpen;
            result.Hasduallicence = e.adoxio_HasDualLicence;
            result.Isrural = (int?)e.adoxio_IsRural;
            result.Isstandalonepatio = e.adoxio_IsStandalonePatio;
            result.Locatedatwinery = e.adoxio_LocatedAtWinery;
            result.Locatedonfirstnationland = e.adoxio_LocatedOnFirstNationLand;
            result.Mailsenttorestaurant = e.adoxio_MailSentToRestaurant;
            result.Mondayclose = (int?)e.adoxio_MondayClose;
            result.Mondayopen = (int?)e.adoxio_MondayOpen;
            result.Name = e.adoxio_name;
            result.Occupantcapacity = e.adoxio_OccupantCapacity;
            result.Occupantload = e.adoxio_OccupantLoad;
            result.Parcelid = e.adoxio_ParcelID;
            result.Patronparticipation = e.adoxio_PatronParticipation;
            result.Phone = e.adoxio_Phone;
            result.Saturdayclose = (int?)e.adoxio_SaturdayClose;
            result.Saturdayopen = (int?)e.adoxio_SaturdayOpen;
            result.Sendmailtoestablishmentuponapproval = (int?)e.adoxio_SendMailToEstablishmentUponApproval;
            result.Standardhours = e.adoxio_StandardHours;
            result.Sundayclose = (int?)e.adoxio_SundayClose;
            result.Sundayopen = (int?)e.adoxio_SundayOpen;
            result.Thursdayclose = (int?)e.adoxio_ThursdayClose;
            result.Thursdayopen = (int?)e.adoxio_ThursdayOpen;
            result.Tuesdayclose = (int?)e.adoxio_TuesdayClose;
            result.Tuesdayopen = (int?)e.adoxio_TuesdayOpen;
            result.Wednesdayclose = (int?)e.adoxio_WednesdayClose;
            result.Wednesdayopen = (int?)e.adoxio_WednesdayOpen;
            result.StatusCode = (int?)e.statuscode;
            result.StateCode = (int?)e.statecode;
            result.IsOpen = e.adoxio_IsOpen;
            var createdon = e.GetAttributeValue<System.DateTime?>("createdon");
            result.Createdon = createdon.HasValue ? (System.DateTimeOffset?)new System.DateTimeOffset(createdon.Value, System.TimeSpan.Zero) : null;
            var modifiedon = e.GetAttributeValue<System.DateTime?>("modifiedon");
            result.Modifiedon = modifiedon.HasValue ? (System.DateTimeOffset?)new System.DateTimeOffset(modifiedon.Value, System.TimeSpan.Zero) : null;
            var overriddencreatedon = e.GetAttributeValue<System.DateTime?>("overriddencreatedon");
            result.Overriddencreatedon = overriddencreatedon.HasValue ? (System.DateTimeOffset?)new System.DateTimeOffset(overriddencreatedon.Value, System.TimeSpan.Zero) : null;
            result.Importsequencenumber = e.GetAttributeValue<int?>("importsequencenumber");
            result.Timezoneruleversionnumber = e.GetAttributeValue<int?>("timezoneruleversionnumber");
            result.Utcconversiontimezonecode = e.GetAttributeValue<int?>("utcconversiontimezonecode");
            result.Versionnumber = e.GetAttributeValue<long?>("versionnumber")?.ToString();
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
                result.AdoxioIsopen = from.IsOpen;
            }
            return result;
        }
    }
}
