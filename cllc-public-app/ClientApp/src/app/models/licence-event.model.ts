import { EventLocation } from "./event-location.model";
import { LicenceEventSchedule } from "./licence-event-schedule";

export class LicenceEvent {
  id: string;
  name: string;
  contactName: string;
  contactPhone: string;
  contactEmail: string;
  startDate: Date;
  endDate: Date;
  venueDescription: string;
  additionalLocationInformation: string;
  foodServiceDescription: string;
  entertainmentDescription: string;
  clientHostname: string;
  eventTypeDescription: string;
  maxAttendance: number;
  maxStaffAttendance: number;
  minorsAttending: boolean;
  communityApproval: boolean;
  licenceId: string;
  accountId: string;
  street1: string;
  street2: string;
  city: string;
  province: string;
  postalCode: string;
  modifiedOn: Date;
  schedules: LicenceEventSchedule[];

  // picklists
  status: number;
  specificLocation: number;
  eventType: number;
  entertainment: number;
  foodService: number;
  eventClass: number;

  // security plan
  securityPlanRequested: boolean;
  // Event Description
  eventLiquorLayout: string;
  dailyEventAttendees: number;
  dailyMinorAttendees: number;
  occupantLoad: number;
  occupantLoadAvailable: boolean;
  occupantLoadServiceArea: number;
  occupantLoadServiceAreaAvailable: boolean;
  serviceAreaControlledDetails: string;
  staffingManagers: string;
  staffingBartenders: string;
  staffingServers: string;

  // Event Security
  securityPersonnel: string;
  securityPersonnelThroughCompany: number;
  securityCompanyName: string;
  securityCompanyAddress: string;
  securityCompanyCity: string;
  securityCompanyPostalCode: string;
  securityCompanyContactPerson: string;
  securityCompanyPhoneNumber: string;
  securityCompanyEmail: string;
  securityPoliceOfficerSummary: string;

  // Safe and Responsible service
  safeAndResponsibleMinorsNotAttending: boolean;
  safeAndResponsibleLiquorAreaControlled: boolean;
  safeAndResponsibleLiquorAreaControlledDescription: string;
  safeAndResponsibleMandatoryID: boolean;
  safeAndResponsibleSignsAdvisingMinors: boolean;
  safeAndResponsibleMinorsOther: boolean;
  safeAndResponsibleMinorsOtherDescription: string;
  safeAndResponsibleSignsAdvisingRemoval: boolean;
  safeAndResponsibleSignsAdvisingTwoDrink: boolean;
  safeAndResponsibleOverConsumptionOther: boolean;
  safeAndResponsibleOverConsumptionOtherDescription: string;
  safeAndResponsibleReadAppendix2: boolean;
  safeAndResponsibleDisturbancesOther: boolean;
  safeAndResponsibleDisturbancesOtherDescription: string;
  safeAndResponsibleAdditionalSafetyMeasures: string;
  safeAndResponsibleServiceAreaSupervision: string;
  securityPlanSubmitted: boolean;

  // security agreement
  declarationIsAccurate: boolean;

  // temp offsite auth
  sepLicensee: string;
  sepLicenceNumber: string;
  sepContactName: string;
  sepContactPhoneNumber: string;

  // unused
  eventNumber: string;
  externalId: string;
  importSequenceNumber: number;
  notifyEventInspector: boolean;

  // market
  isNoPreventingSaleofLiquor: boolean;
  isMarketManagedorCarried: boolean;
  isMarketOnlyVendors: boolean;
  isMarketHostsSixVendors: boolean;
  isMarketMaxAmountorDuration: boolean;
  mktOrganizerContactName: string;
  mktOrganizerContactPhone: string;
  registrationNumber: string;
  businessNumber: string;
  marketName: string;
  marketWebsite: string;
  marketEventType: number;
  marketDuration: number;
  isAllStaffServingitRight: boolean;
  isSalesAreaAvailandDefined: boolean;
  isSampleSizeCompliant: boolean;
  eventCategory: number;

  // temporary use area (TUA) events
  eventName: string;
  tuaEventType: number;
  isClosedToPublic: boolean;
  isWedding: boolean;
  isNetworkingParty: boolean;
  isConcert: boolean;
  isBanquet: boolean;
  isAmplifiedSound: boolean;
  isDancing: boolean;
  isReception: boolean;
  isLiveEntertainment: boolean;
  isGambling: boolean;
  isNoneOfTheAbove: boolean;
  isAgreement1: boolean;
  isAgreement2: boolean;
  eventLocations: EventLocation[];
}

export const EventStatus = [
  {
    label: "Draft",
    value: 845280004,
  },
  {
    label: "In Review",
    value: 1
  },
  {
    label: "Approved",
    value: 845280000
  },
  {
    label: "Denied",
    value: 845280001
  },
  {
    label: "Terminated",
    value: 845280002
  },
  {
    label: "Cancelled",
    value: 845280003
  },
  {
    label: "Submitted",
    value: 845280005
  }
];

export const SpecificLocation = [
  {
    label: "Indoors",
    value: 845280000,
  },
  {
    label: "Outdoors",
    value: 845280001,
  },
  {
    label: "Both",
    value: 845280002
  }
];

export const EventClass = [
  {
    label: "Authorization",
    value: 845280000
  },
  {
    label: "Notice",
    value: 845280001
  }
];

export const FoodService = [
  {
    label: "Appetizers / Hors D'Oeuvres",
    value: 845280000,
  },
  {
    label: "Buffet",
    value: 845280001,
  },
  {
    label: "Full Service Meal",
    value: 845280002,
  },
  {
    label: "Other",
    value: 845280003,
  }
];

export const Entertainment = [
  {
    label: "Adult Entertainment",
    value: 845280000
  },
  {
    label: "Dance",
    value: 845280001
  },
  {
    label: "Gambling",
    value: 845280002
  },
  {
    label: "Live Entertainment",
    value: 845280003
  },
  {
    label: "Live Music",
    value: 845280004
  },
  {
    label: "None",
    value: 845280005
  },
  {
    label: "Other",
    value: 845280006
  }
];

export const EventType = [
  {
    label: "Caterer's Staff / Customer Appreciation",
    value: 845280000
  },
  {
    label: "Community",
    value: 845280001
  },
  {
    label: "Corporate",
    value: 845280002
  },
  {
    label: "Other",
    value: 845280003
  },
  {
    label: "Personal",
    value: 845280004
  }
];

export const MarketEventType = [
  {
    label: "Artisan",
    value: 845280000
  },
  {
    label: "Farmers",
    value: 845280001
  },
  {
    label: "Annual",
    value: 845280002
  },
  {
    label: "Christmas",
    value: 845280003
  },
  {
    label: "Other",
    value: 845280004
  }
];

export const MarketDuration = [
  {
    label: "Weekly",
    value: 845280000
  },
  {
    label: "Bi-Weekly",
    value: 845280001
  },
  {
    label: "Monthly",
    value: 845280002
  },
  {
    label: "Once",
    value: 845280003
  }
];

export const EventCategory = [
  {
    label: "Catering",
    value: 845280000
  },
  {
    label: "Market",
    value: 845280001
  },
  {
    label: "Temporary Off-Site Sale",
    value: 845280002
  },
  {
    label: 'Temporary Use Area',
    value: 845280004
  },
  {
    label: 'All Ages Liquor Free',
    value: 100000000
  },
  {
    label: 'Take Home Sampling',
    value: 100000001
  }
];

export const TuaEventType = [
  {
    label: 'Invite Only',
    value: 845280000
  },
  {
    label: 'Open-Public',
    value: 845280001
  },
  {
    label: 'Ticketed',
    value: 845280002
  }
];
