export class LicenceEvent {
  id: string;
  name: string;
  contactName: string;
        // public string Name { get; set; }
        // public DateTimeOffset? StartDate { get; set; }
        // public DateTimeOffset? EndDate { get; set; }
        // public string VenueDescription { get; set; }
        // public string AdditionalLocationInformation { get; set; }
        // [JsonConverter(typeof(StringEnumConverter))]
        // public FoodService? FoodService { get; set; }
        // public string FoodServiceDescription { get; set; }
        // [JsonConverter(typeof(StringEnumConverter))]
        // public Entertainment? Entertainment { get; set; }
        // public string EntertainmentDescription { get; set; }
        // public string ContactPhone { get; set; }
        // public string ExternalId { get; set; }
        // public string ContactName { get; set; }
        // public string ContactEmail { get; set; }
        // public string EventNumber { get; set; }
        // public string ClientHostname { get; set; }
        // [JsonConverter(typeof(StringEnumConverter))]
        // public EventType? EventType { get; set; }
        // public string EventTypeDescription { get; set; }
        // public int? ImportSequenceNumber { get; set; }
        // [JsonConverter(typeof(StringEnumConverter))]
        // public SpecificLocation? SpecificLocation { get; set; }
        // [JsonConverter(typeof(StringEnumConverter))]
        // public EventClass? EventClass { get; set; }
        // public int? MaxAttendance { get; set; }
        // public bool? CommunityApproval { get; set; }
        // public bool? NotifyEventInspector { get; set; }
        // public string LicenceId { get; set; }
        // public string AccountId { get; set; }

        // public string Street1 { get; set; }
        // public string Street2 { get; set; }
        // public string City { get; set; }
        // public string Province { get; set; }
        // public string PostalCode { get; set; }
}

export enum EventStatus {
  Draft = 'Draft',
  InReview = 'In Review',
  Approved = 'Approved',
  Denied = 'Denied',
  Terminated = 'Terminated',
  Cancelled = 'Cancelled'
}

export enum SpecificLocation {
    Indoors = 'Indoors',
    Outdoors = 'Outdoors',
    Both = 'Both'
}

export enum EventClass {
  Approval = 'Approval',
  Notice = 'Notice'
}

export enum FoodService {
  AppetizersHorsDOeuvres = 'Appetizers / Hors D\'Oeuvres',
  Buffet = 'Buffet',
  FullServiceMeal = 'Full Service Meal',
  Other = 'Other'
}

export enum Entertainment {
  AdultEntertainment = 'Adult Entertainment',
  Dance = 'Dance',
  Gambling = 'Gambling',
  LiveEntertainment = 'Live Entertainment',
  LiveMusic = 'Live Music',
  None = 'None',
  Other = 'Other'
}

export enum EventType {
  CaterersStaffCustomerAppreciation = 'Caterer\'s Staff / Customer Appreciation',
  Community = 'Community',
  Corporate = 'Corporate',
  Other = 'Other',
  Personal = 'Personal'
}
