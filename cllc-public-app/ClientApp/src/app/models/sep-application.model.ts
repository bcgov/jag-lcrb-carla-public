import { SepInvoice } from "@models/sep-invoice.model";
import { Account } from "./account.model";
import { Contact } from "./contact.model";
import { SepCity } from "./sep-city.model";
import { SepDrinkSalesForecast } from "./sep-drink-sales-forecast.model";
import { SepLocation } from "./sep-location.model";
import { differenceInHours } from "date-fns";

/**
 * Special Event Application Model.
 *
 * TODO: This model is out of date and missing properties.
 *
 * @export
 * @class SepApplication
 */
export class SepApplication {
    applicant: Contact;
    applicantInfo: any;
    beer: number;
    cancelReason?: string;
    chargingForLiquorReason: string;
    dateAgreedToTsAndCs: Date;
    dateCreated: Date;
    dateSubmitted: Date;
    dateIssued: Date;
    denialReason?: string;
    donateOrConsular: boolean;
    eventName: string;
    eventStartDate: Date;
    eventStatus: string;
    eventType: string;
    fundraisingPurpose: string;
    hostOrganizationAddress: string;
    netEstimatedPST: number;
    hostOrganizationName: string;
    howProceedsWillBeUsedDescription: string;
    id: string; // server side primary key
    invoiceTrigger: boolean;
    isAgreeTsAndCs: boolean;
    isBeerGarden: boolean;
    isGSTRegisteredOrg: boolean;
    isLocalSignificance: boolean;
    isLocationLicensed: string;
    isMajorSignificance: boolean;
    isMajorSignificanceRational: string;
    isSupportLocalArtsOrSports: boolean;
    isManufacturingExclusivity: boolean;
    isOnPublicProperty: boolean;
    isPrivateResidence: boolean;
    isTastingEvent: boolean;
    lastStepCompleted: string;
    lastUpdated: Date;
    lcrbApproval?: string;
    lcrbApprovalBy?: Contact;
    localId?: number;  // indexed db primary key
    userId: string; // the id of the user who owns the record
    maxServings: number;
    nonProfitName: string;
    permitNumber: string;
    policeAccount?: Account;
    policeApproval?: string;
    policeDecisionBy?: Contact;
    privateOrPublic?: string;
    responsibleBevServiceNumber: string;
    sepCity: SepCity;
    specialEventDescription: string;
    spirits: number;
    tempJobNumber: string;
    totalServings: number;
    wine: number;

    eventLocations: SepLocation[] = [];
    drinksSalesForecasts: SepDrinkSalesForecast[] = [];
    termsAndConditions: SepTermAndCondtion[] = [];
    totalRevenue: number;
    totalProceeds: number;
    totalPurchaseCost: number;

    invoice: SepInvoice;

    public get totalMaximumNumberOfGuests(): number {
        let maxGuests = 0;
        for (const location of this.eventLocations) {
            for (const area of location.serviceAreas) {
                maxGuests += area.licencedAreaMaxNumberOfGuests || 0;
            }
        }
        return maxGuests;
    }

    public get maximumNumberOfAdults(): number {

        let maxMinors = 0;
        for (const location of this.eventLocations) {
            for (const area of location.serviceAreas) {
                maxMinors += area.licencedAreaNumberOfMinors || 0;
            }
        }
        return this.totalMaximumNumberOfGuests - maxMinors;
    }

    public get serviceHours(): number {
        let serviceHours = 0;
        for (const location of this.eventLocations) {
            for (const hours of location.eventDates) {
                serviceHours += Math.min(differenceInHours(new Date(hours.serviceEnd), new Date(hours.serviceStart)) || 0, 17);
            }
        }
        return serviceHours;
    }

    public get suggestedServings(): number {
        return Math.floor((this.serviceHours / 3) * (this.maximumNumberOfAdults) * 4);
    }

    public get maxSuggestedServings(): number {
        return Math.floor(((this.serviceHours / 3) * (this.maximumNumberOfAdults) * 5));
    }
}

export class SepTermAndCondtion {
    id: string;
    content: string;
    originator: string;
}

