import { SepDrinkSalesForecast } from "./sep-drink-sales-forecast.model";
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
    isAgreeTsAndCs: boolean;
    dateAgreedToTsAndCs: Date;
    stepsCompleted: string[];
    eventStatus: string;
    totalServings: number;
    invoiceTrigger: number;
    responsibleBevServiceNumber: string;

    isPrivateResidence: boolean;
    isMajorSignificance: boolean;
    isMajorSignificanceRational: string;
    isLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    maximumNumberOfGuests: number;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    itemsToDelete: SepDeletedItems = new SepDeletedItems();

    isOnPublicProperty: boolean;
    majorSignificanceRationale: string;
    privateOrPublic: boolean;
    responsibleBevServiceNumberDoesNotHave: boolean;
    specialEventDescription: string;
    admissionFee: string;
    isLocationLicensed: string;
    hostOrganizationName: string;
    hostOrganizationAddress: string;
    hostOrganizationCategory: string;
}

export class SepDeletedItems {
    eventDates: string[] = [];
    locations: string[] = [];
    serviceAreas: string[] = [];
    drinkSalesForecasts: string[] = [];
}