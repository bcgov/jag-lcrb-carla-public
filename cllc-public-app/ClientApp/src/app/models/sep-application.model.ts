import { SepLocation } from "./sep-location.model";

export class SepApplication {
    id?: number;  // indexed db primary key
    specialEventId: string; // server side primary key
    tempJobNumber: string;
    dateCreated: Date;
    lastUpdated: Date;
    eventName: string;
    applicantInfo: any;
    agreeToTnC: boolean;
    dateAgreedToTnC: Date;
    stepsCompleted: string[];
    eventStatus: string; 
    totalServings: number;
    invoiceTrigger: number;

    eligibilityAtPrivateResidence: boolean;
    eligibilityMajorSignificance: boolean;
    eligibilityMajorSignificanceRational: string;
    eligibilityLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    numMaxGuests: number;

    eventLocations: SepLocation[] = [];

} 