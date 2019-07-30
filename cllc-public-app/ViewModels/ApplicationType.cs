using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.Public.ViewModels
{
    public enum FormControlState
    {
        No,
        Yes,
        Readonly
    }

    public class ApplicationType
    {
        public string Id;
        public string ActionText;
        public string Name;
        public string Title;

        public bool? ShowPropertyDetails;
        public bool? ShowCurrentProperty;
        public bool? ShowHoursOfSale;
        public bool? ShowAssociatesFormUpload;
        public bool? ShowFinancialIntegrityFormUpload;
        public bool? ShowSupportingDocuments;
        public bool? ShowDeclarations;
        public bool? EstablishmetNameIsReadOnly;

        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? StoreContactInfo { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? EstablishmentName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? newEstablishmentAddress { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? CurrentEstablishmentAddress { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? Signage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? ValidInterest { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? FloorPlan { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public FormControlState? SitePlan { get; set; }

        public LicenseType LicenseType;

        public List<ApplicationTypeContent> contentTypes;
    }
}
