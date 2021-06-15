import { Contact } from "./contact.model";
import { SepCity } from "./sep-city.model";
import { SepDrinkSalesForecast } from "./sep-drink-sales-forecast.model";
import { SepLocation } from "./sep-location.model";
import { Account } from "./account.model";

export class SepApplication {
    id: string; // server side primary key
    localId?: number;  // indexed db primary key
    tempJobNumber: string;
    eventStartDate: Date;
    dateCreated: Date;
    dateSubmitted: Date;
    lastUpdated: Date;
    eventName: string;
    applicantInfo: any;
    isAgreeTsAndCs: boolean;
    dateAgreedToTsAndCs: Date;
    lastStepCompleted: string;
    eventStatus: string;
    totalServings: number;
    invoiceTrigger: boolean;
    responsibleBevServiceNumber: string;

    isPrivateResidence: boolean;
    isMajorSignificance: boolean;
    isMajorSignificanceRational: string;
    isLocalSignificance: boolean;

    permitNumber: string;
    isTastingEvent: boolean;
    isBeerGarden: boolean;
    hostOrganizationName: string;
    hostOrganizationAddress: string;
    specialEventDescription: string;
    isLocationLicensed: string;
    isOnPublicProperty: boolean;

    sepCity: SepCity;
    applicant: Contact;

    policeAccount?: Account;
    policeDecisionBy?: Contact;
    policeApproval?: string;
    lcrbApprovalBy?: Contact;
    lcrbApproval?: string;
    denialReason?: string;
    cancelReason?: string;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    itemsToDelete: SepDeletedItems = new SepDeletedItems();

    beer: number;
    wine: number;
    spirits: number;

    public get totalMaximumNumberOfGuests(): number {
        let maxGuests = 0;
        for (const location of this.eventLocations) {
            // accumulate the total hours of service by looping through the eventDates
            maxGuests += location.maximumNumberOfGuests || 0;
        }

        return maxGuests;
    }

    public get maximumNumberOfAdults(): number {

        let maxMinors = 0;
        for (const location of this.eventLocations) {
            // accumulate the total hours of service by looping through the eventDates
            maxMinors += location.numberOfMinors || 0;
        }
        return this.totalMaximumNumberOfGuests - maxMinors;
    }

    public get serviceHours(): number {
        let serviceHours = 0;
        for (const location of this.eventLocations) {
            for (const hours of location.eventDates) {
                serviceHours += hours.getServiceHours();
            }
        }
        return serviceHours;
    }
}


export class SepDeletedItems {
    eventDates: string[] = [];
    locations: string[] = [];
    serviceAreas: string[] = [];
    drinkSalesForecasts: string[] = [];
}
