export class SepApplication {
    id: number
    tempJobNumber: string;
    dateCreated: Date;
    lastUpdated: Date;
    eventName: string;
    applicantInfo: any;
    agreeToTnC: boolean;
    dateAgreedToTnC: Date;
    stepsCompleted: string[];
    eventStatus: string;

    eligibilityAtPrivateResidence: boolean;
    eligibilityMajorSignificance: boolean;
    eligibilityMajorSignificanceRational: string;
    eligibilityLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    numMaxGuests: number;

    locations: SepLocation[] = [];

}

export class SepLocation {
    id: number;
    locationPermitNumber: string;
    locationName: string;
    venueType: string;
    locationMaxGuests: number;
    eventLocationStreet1: string;
    eventLocationStreet2: string;
    eventLocationCity: string;
    eventLocationProvince: string;
    eventLocationPostalCode: string;

    eventDates: SepSchedule[] = [];
    serviceAreas: SepServiceArea[] = [];
}

export class SepSchedule {
    id: number;
    eventDate: Date;
    eventStart: string;
    eventEnd: string;
    ServiceStart: string;
    ServiceEnd: string;
}

export class SepServiceArea {
    id: number;
    description: string;
    numAreaMaxGuests: number;
    setting: string;
    isMinorsPresent: boolean;
    numMinors: number;
}