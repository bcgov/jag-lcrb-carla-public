import { SepSchedule } from "./sep-schedule.model";

export class SepServiceArea {
    licencedAreaId: string; // server side primary key
    sepLocationId: string;
    specialEventId: string;
    minorPresent: boolean;
    licencedAreaMaxNumberOfGuests: number;
    maximumNumberOfGuests: number;
    isBothOutdoorIndoor: boolean;
    isIndoors: boolean;
    numberOfMinors: number;
    licencedAreaNumberOfMinors: number;
    setting: number;
    statusCode: number;
    stateCode: number;
    eventName: string;
    isOutdoors: boolean;
    licencedAreaDescription: string;
}
