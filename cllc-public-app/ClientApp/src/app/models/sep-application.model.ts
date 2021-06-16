import { Account } from "./account.model";
import { Contact } from "./contact.model";
import { SepCity } from "./sep-city.model";
import { SepDrinkSalesForecast } from "./sep-drink-sales-forecast.model";
import { SepLocation } from "./sep-location.model";
import { Account } from "./account.model";
import { differenceInHours } from 'date-fns'

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

    //suggestedServings: number;
    maxServings: number;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];

    beer: number;
    wine: number;
    spirits: number;

    public get totalMaximumNumberOfGuests(): number {
        let maxGuests = 0;
        for (const location of this.eventLocations) {
            // accumulate the total hours of service by looping through the eventDates
            for(const area of location.serviceAreas){
              maxGuests += area.maximumNumberOfGuests || 0;
            }
        }

        return maxGuests;
    }

    public get maximumNumberOfAdults(): number {

        let maxMinors = 0;
        for (const location of this.eventLocations) {
            for(const area of location.serviceAreas) {
              // accumulate the total hours of service by looping through the eventDates
              maxMinors += area.numberOfMinors || 0;
            }
        }
        console.log("max adults:",this.totalMaximumNumberOfGuests, maxMinors);
        return this.totalMaximumNumberOfGuests - maxMinors;
    }

    public get serviceHours(): number {
        let serviceHours = 0;
        for (const location of this.eventLocations) {

            for (const hours of location.eventDates) {

                serviceHours += differenceInHours(new Date(hours.serviceEnd), new Date(hours.serviceStart)) || 0;
                console.log("hours:",hours.serviceEnd, hours.serviceStart, differenceInHours(new Date(hours.serviceEnd), new Date(hours.serviceStart)) );

            }
        }
        return serviceHours;
    }

    public get suggestedServings(): number {
      return Math.floor((this.serviceHours / 3) * (this.maximumNumberOfAdults) * 4);
    }

    public get maxSuggestedServings(): number {
      return  Math.floor(((this.serviceHours / 3) * (this.maximumNumberOfAdults) * 5));
    }

    //public get servings()
}

