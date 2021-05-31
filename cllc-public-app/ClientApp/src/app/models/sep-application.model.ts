import { Contact } from "./contact.model";
import { SepCity } from "./sep-city.model";
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

    isPrivateResidence: boolean;
    isMajorSignificance: boolean;
    isMajorSignificanceRational: string;
    isLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    maximumNumberOfGuests: number;
    sepCity: SepCity;
    applicant: Contact;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    itemsToDelete: SepDeletedItems = new SepDeletedItems();
}

export class SepDeletedItems {
    eventDates: string[] = [];
    locations: string[] = [];
    serviceAreas: string[] = [];
    drinkSalesForecasts: string[] = [];
}