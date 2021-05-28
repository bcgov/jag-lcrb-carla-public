import { SepSchedule } from "./sep-schedule.model";
import { SepServiceArea } from "./sep-service-area.model";

export class SepLocation {
    id: string; // server side primary key
    locationPermitNumber: string;
    locationName: string;
    venueType: string;
    locationMaxGuests: number;
    locationNumberMinors: number;
    eventLocationStreet1: string;
    eventLocationStreet2: string;
    eventLocationCity: string;
    eventLocationProvince: string;
    eventLocationPostalCode: string;
    serviceAreas: SepServiceArea[] = [];
    eventDates: SepSchedule[] = [];
}
