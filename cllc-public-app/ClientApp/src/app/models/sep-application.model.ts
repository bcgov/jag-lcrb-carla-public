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

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    itemsToDelete: SepDeletedItems = new SepDeletedItems();

    public get maximumNumberOfGuests(): number {
      let maxGuests = 0;
      for(var location of this.eventLocations){
        // accumulate the total hours of service by looping through the eventDates
        maxGuests += location.maximumNumberOfGuests || 0;
      }

      return maxGuests;
    }

    public get maximumNumberOfAdults(): number {

        let maxMinors = 0;
        for(var location of this.eventLocations){
          // accumulate the total hours of service by looping through the eventDates
          maxMinors +=  location.locationNumberMinors || 0;
        }

      return this.maximumNumberOfGuests - maxMinors;
    }

    public get serviceHours(): number{
      let serviceHours = 0;
      for(var location of this.eventLocations){
        for(var hours of location.eventDates){
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
