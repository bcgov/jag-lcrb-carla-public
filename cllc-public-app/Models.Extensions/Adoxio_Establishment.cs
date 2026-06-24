extern alias DV;
using System;
using DV::Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Public.Utils;
using Microsoft.Xrm.Sdk;

namespace Gov.Lclb.Cllb.Public.Models
{
    /// <summary>
    /// ViewModel transforms.
    /// </summary>
    public static class Adoxio_EstablishmentExtensions
    {
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
    }
}
