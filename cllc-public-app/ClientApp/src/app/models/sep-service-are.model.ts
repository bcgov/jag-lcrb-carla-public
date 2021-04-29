import { SepSchedule } from "./sep-schedule.model";

export class SepServiceArea {
    id: number;
    description: string;
    numAreaMaxGuests: number;
    setting: string;
    isMinorsPresent: boolean;
    numMinors: number;
    eventDates: SepSchedule[] = [];
}