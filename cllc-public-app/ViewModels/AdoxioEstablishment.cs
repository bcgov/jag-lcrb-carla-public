using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;



namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public class AdoxioEstablishment
    {
        // string form of the guid.
        public string id { get; set; }
        public Guid? _licencee_value { get; set; }
        public Guid? _licencetypeid_value { get; set; }
        public Guid? _municipality_value { get; set; }
        public Guid? _policejurisdiction_value { get; set; }
        public Guid? _primaryinspectorid_value { get; set; }
        public Guid? _territory_value { get; set; }
        public Guid? _createdby_value { get; set; }
        public Guid? _createdonbehalfby_value { get; set; }
        public Guid? _modifiedby_value { get; set; }
        public Guid? _modifiedonbehalfby_value { get; set; }
        public Guid? _ownerid_value { get; set; }
        public Guid? _owningbusinessunit_value { get; set; }
        public Guid? _owningteam_value { get; set; }
        public Guid? _owninguser_value { get; set; }
        public string Addresscity { get; set; }
        public string Addresspostalcode { get; set; }
        public string Addressstreet { get; set; }
        public bool? Alreadyopen { get; set; }
        public string Email { get; set; }
        public DateTimeOffset? Expectedopendate { get; set; }
        public int? Fridayclose { get; set; }
        public int? Fridayopen { get; set; }
        public bool? Hasduallicence { get; set; }
        public int? Isrural { get; set; }
        public bool? Isstandalonepatio { get; set; }
        public bool? Locatedatwinery { get; set; }
        public bool? Locatedonfirstnationland { get; set; }
        public bool? Mailsenttorestaurant { get; set; }
        public int? Mondayclose { get; set; }
        public int? Mondayopen { get; set; }
        public string Name { get; set; }
        public int? Occupantcapacity { get; set; }
        public int? Occupantload { get; set; }
        public string Parcelid { get; set; }
        public bool? Patronparticipation { get; set; }
        public string Phone { get; set; }
        public int? Saturdayclose { get; set; }
        public int? Saturdayopen { get; set; }
        public int? Sendmailtoestablishmentuponapproval { get; set; }
        public bool? Standardhours { get; set; }
        public int? Sundayclose { get; set; }
        public int? Sundayopen { get; set; }
        public int? Thursdayclose { get; set; }
        public int? Thursdayopen { get; set; }
        public int? Tuesdayclose { get; set; }
        public int? Tuesdayopen { get; set; }
        public int? Wednesdayclose { get; set; }
        public int? Wednesdayopen { get; set; }
        public DateTimeOffset? Createdon { get; set; }
        public int? Importsequencenumber { get; set; }
        public DateTimeOffset? Modifiedon { get; set; }
        public DateTimeOffset? Overriddencreatedon { get; set; }
        public int? StateCode { get; set; }
        public int? StatusCode { get; set; }
        public int? Timezoneruleversionnumber { get; set; }
        public int? Utcconversiontimezonecode { get; set; }
        public long? Versionnumber { get; set; }
    }
}
