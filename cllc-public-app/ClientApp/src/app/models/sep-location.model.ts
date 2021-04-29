import { SepServiceArea } from "./sep-service-are.model";

export class SepLocation {
    id: number;
    locationPermitNumber: string;
    locationName: string;
    venueType: string;
    locationMaxGuests: number;
    eventLocationStreet1: string;
    eventLocationStreet2: string;
    eventLocationCity: string;
    eventLocationProvince: string;
    eventLocationPostalCode: string;
    serviceAreas: SepServiceArea[] = [];
}
