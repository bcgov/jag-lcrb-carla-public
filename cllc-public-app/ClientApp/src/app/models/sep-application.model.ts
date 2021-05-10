import { SepLocation } from "./sep-location.model";

export class SepApplication {
    id: number
    specialEventId: string;
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
