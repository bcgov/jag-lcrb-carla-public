import { SepSchedule } from "./sep-schedule.model";
import { SepServiceArea } from "./sep-service-are.model";

export class SepLocation {
    locationId: string; // server side primary key
    locationPermitNumber: string;
    locationName: string;
    venueType: string;
    maximumNumberOfGuests: number;
    eventLocationStreet1: string;
    eventLocationStreet2: string;
    eventLocationCity: string;
    eventLocationProvince: string;
    eventLocationPostalCode: string;
    serviceAreas: SepServiceArea[] = [];
    eventDates: SepSchedule[] = [];
}
