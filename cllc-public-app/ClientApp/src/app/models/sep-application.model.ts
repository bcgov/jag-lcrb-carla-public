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
    maximumNumberOfAdults: number;
    serviceHours: number;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    itemsToDelete: SepDeletedItems = new SepDeletedItems();

    getAttendees(app: SepApplication) {
      if(!app){
        return;
      }
        // the maximum number of guests is calculated by looping through each location
        this.maximumNumberOfGuests = 0;
        for(var location of app.eventLocations){
          // accumulate the total hours of service by looping through the eventDates
          this.maximumNumberOfGuests += location.maximumNumberOfGuests || 0;
          this.maximumNumberOfAdults += (location.maximumNumberOfGuests || 0) - (location.locationNumberMinors || 0);
        }
    }

    getServiceHours(app: SepApplication){
      if(!app){
        return;
      }

      this.serviceHours = 0;

              for(var location of app.eventLocations){
                for(var hours of location.eventDates){
                  this.serviceHours += hours.getServiceHours();
                }
              }
    }

  }


export class SepDeletedItems {
    eventDates: string[] = [];
    locations: string[] = [];
    serviceAreas: string[] = [];
    drinkSalesForecasts: string[] = [];
}
