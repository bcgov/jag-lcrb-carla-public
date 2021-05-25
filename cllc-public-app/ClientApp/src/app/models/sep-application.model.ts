import { SepLocation } from "./sep-location.model";

export class SepApplication {
    id: string; // server side primary key
    localId?: number;  // indexed db primary key
    tempJobNumber: string;
    eventStartDate: Date;
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
    isMajorSignificance: boolean;
    isMajorSignificanceRational: string;
    eligibilityLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    maximumNumberOfGuests: number;

    eventLocations: SepLocation[] = [];

} 