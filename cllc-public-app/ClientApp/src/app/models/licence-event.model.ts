export class LicenceEvent {
  id: string;
  name: string;
  contactName: string;
  contactPhone: string;
  contactEmail: string;
  startDate: Date;
  endDate: Date;
  venueDescription: string;
  AdditionalLocationInformation: string;
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
  postalCode: string;
  modifiedOn: Date;
  schedules: [];

  // picklists
  status: number;
  specificLocation: number;
  eventType: number;
  entertainment: number;
  foodService: number;
  eventClass: number;

  // unused
  eventNumber: string;
  externalId: string;
  importSequenceNumber: number;
  notifyEventInspector: boolean;
}

export const EventStatus = [
  {
    label: 'Draft',
    value: 845280004,
  },
  {
    label: 'In Review',
    value: 1
  },
  {
    label: 'Approved',
    value: 845280000
  },
  {
    label: 'Denied',
    value: 845280001
  },
  {
    label: 'Terminated',
    value: 845280002
  },
  {
    label: 'Cancelled',
    value: 845280003
  }
];

export const SpecificLocation = [
  {
    label: 'Indoors',
    value: 845280000,
  },
  {
    label: 'Outdoors',
    value: 845280001,
  },
  {
    label: 'Both',
    value: 845280002
  }
];

export const EventClass = [
  {
    label: 'Authorization',
    value: 845280000
  },
  {
    label: 'Notice',
    value: 845280001
  }
];

export const FoodService = [
  {
    label: 'Appetizers / Hors D\'Oeuvres',
    value: 845280000,
  },
  {
    label: 'Buffet',
    value: 845280001,
  },
  {
    label: 'Full Service Meal',
    value: 845280002,
  },
  {
    label: 'Other',
    value: 845280003,
  }
];

export const Entertainment = [
  {
    label: 'Adult Entertainment',
    value: 845280000
  },
  {
    label: 'Dance',
    value: 845280001
  },
  {
    label: 'Gambling',
    value: 845280002
  },
  {
    label: 'Live Entertainment',
    value: 845280003
  },
  {
    label: 'Live Music',
    value: 845280004
  },
  {
    label: 'None',
    value: 845280005
  },
  {
    label: 'Other',
    value: 845280006
  }
];

export const EventType = [
  {
    label: 'Caterer\'s Staff / Customer Appreciation',
    value: 845280000
  },
  {
    label: 'Community',
    value: 845280001
  },
  {
    label: 'Corporate',
    value: 845280002
  },
  {
    label: 'Other',
    value: 845280003
  },
  {
    label: 'Personal',
    value: 845280004
  }
];
