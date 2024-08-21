import { format, addDays } from "date-fns";
import { is } from "date-fns/locale";
import * as moment from 'moment-timezone';

export class SepSchedule {
    id: string; // server side primary key
    specialEventId: string; // server side foreign key
    locationId: string; // server side foreign key
    eventStart: Date;
    eventEnd: Date;
    serviceStart: Date;
    serviceEnd: Date;
    mountainAdjustedPacificEventStart: Date;
    mountainAdjustedPacificEventEnd: Date;
    mountainAdjustedPacificServiceStart: Date;
    mountainAdjustedPacificServiceEnd: Date;
    liquorServiceHoursExtensionReason: string;
    disturbancePreventionMeasuresDetails: string;
    readonly vancouverTimeZone: string = "America/Vancouver";
    readonly edmontonTimeZone: string = "America/Edmonton";
    isPacificTimeZone: boolean;

    constructor(sched: IEventFormValue, isPacificTimeZone: boolean) {

        this.isPacificTimeZone = isPacificTimeZone;

        if (sched) {

            this.id = sched.id;

            if(isPacificTimeZone) { 
                this.eventStart = this.formatDate(sched.eventDate, sched.eventStartValue, true);
                this.eventEnd = this.formatDate(sched.eventDate, sched.eventEndValue, true);
                this.serviceStart = this.formatDate(sched.eventDate, sched.serviceStartValue, true);
                this.serviceEnd = this.formatDate(sched.eventDate, sched.serviceEndValue, true);
            } else {
                this.eventStart = this.formatDate(sched.eventDate, sched.eventStartValue, false);
                this.eventEnd = this.formatDate(sched.eventDate, sched.eventEndValue, false);
                this.serviceStart = this.formatDate(sched.eventDate, sched.serviceStartValue, false);
                this.serviceEnd = this.formatDate(sched.eventDate, sched.serviceEndValue, false);
            }
        }
    }

    // Convert the SepSchedule object to an IEventFormValue object
    toEventFormValue(): IEventFormValue {
        const result = { id: this.id } as IEventFormValue;
        result.eventDate = this.eventStart;

        if (this.eventStart) {
            result.eventStartValue = format(new Date(this.eventStart), "h:mm aa");
            if(this.mountainAdjustedPacificEventStart) {
                result.mountainAdjustedPacificEventStartValue = format(new Date(this.mountainAdjustedPacificEventStart), "h:mm aa");
            }
        }

        if (this.eventEnd) {
            result.eventEndValue = format(new Date(this.eventEnd), "h:mm aa");
            if(this.mountainAdjustedPacificEventEnd) {
                result.mountainAdjustedPacificEventEndValue = format(new Date(this.mountainAdjustedPacificEventEnd), "h:mm aa");
            }
        }

        if (this.serviceStart) {
            result.serviceStartValue = format(new Date(this.serviceStart), "h:mm aa");
            if(this.mountainAdjustedPacificServiceStart) {
                result.mountainAdjustedPacificServiceStartValue = format(new Date(this.mountainAdjustedPacificServiceStart), "h:mm aa");
            }
        }

        if (this.serviceEnd) {
            result.serviceEndValue = format(new Date(this.serviceEnd), "h:mm aa");
            if(this.mountainAdjustedPacificServiceEnd) {
                result.mountainAdjustedPacificServiceEndValue = format(new Date(this.mountainAdjustedPacificServiceEnd), "h:mm aa");
            }
        }

        result.liquorServiceHoursExtensionReason = this.liquorServiceHoursExtensionReason;
        result.disturbancePreventionMeasuresDetails = this.disturbancePreventionMeasuresDetails;
        
        return result;
    }

    // Get the number of service hours
    getServiceHours(): number {
        const serviceHours = parseInt(format(new Date(this.serviceEnd), "H")) - parseInt(format(new Date(this.serviceStart), "H"));
        return serviceHours;
    }

    private isNextDay(time: string): boolean {
        const dayBreakIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.dayBreak === true));
        const timeIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.value === time));
        return timeIndex >= dayBreakIndex;
    }

    // Convert the time to 24-hour format and convert the date to Pacific Time
    private formatDate(eventDate: Date, time: string, isPacificTimeZone: boolean): Date {
        let tempDate = new Date(eventDate);
        
        if (this.isNextDay(time)) {
            tempDate = addDays(tempDate, 1);
        }
        
        // Convert time to 24-hour format
        const time24 = convertTo24Hour(time);
        
        // Create a date string in the format "yyyy-MM-ddTHH:mm:ss"
        const dateString = `${format(tempDate, "yyyy-MM-dd")}T${time24}`;
        
        // Convert the date string to Pacific Time
        let dateInPacific = moment.tz(dateString, "America/Los_Angeles");
        
        // Convert to UTC, subtract an hour if isPacificTimeZone is false
        if (!isPacificTimeZone) {
            dateInPacific = dateInPacific.clone().tz("UTC").add(-1, 'hours');
        } else {
            dateInPacific = dateInPacific.clone().tz("UTC");
        }
        
        return dateInPacific.toDate();
    }
}

// Convert the time to 24-hour format
function convertTo24Hour(time) {
    const [main, period] = time.split(' ');
    let [hours, minutes] = main.split(':');

    if (period === 'PM' && hours !== '12') {
        hours = (Number(hours) + 12).toString();
    } else if (period === 'AM' && hours === '12') {
        hours = '00';
    }

    return `${hours.padStart(2, '0')}:${minutes}`;
}

export const TIME_SLOTS = [
    { value: "8:00 AM", name: "8:00 AM" },
    { value: "8:30 AM", name: "8:30 AM" },
    { value: "9:00 AM", name: "9:00 AM" },
    { value: "9:30 AM", name: "9:30 AM" },
    { value: "10:00 AM", name: "10:00 AM" },
    { value: "10:30 AM", name: "10:30 AM" },
    { value: "11:00 AM", name: "11:00 AM" },
    { value: "11:30 AM", name: "11:30 AM" },
    { value: "12:00 PM", name: "12:00 PM" },
    { value: "12:30 PM", name: "12:30 PM" },
    { value: "1:00 PM", name: "1:00 PM" },
    { value: "1:30 PM", name: "1:30 PM" },
    { value: "2:00 PM", name: "2:00 PM" },
    { value: "2:30 PM", name: "2:30 PM" },
    { value: "3:00 PM", name: "3:00 PM" },
    { value: "3:30 PM", name: "3:30 PM" },
    { value: "4:00 PM", name: "4:00 PM" },
    { value: "4:30 PM", name: "4:30 PM" },
    { value: "5:00 PM", name: "5:00 PM" },
    { value: "5:30 PM", name: "5:30 PM" },
    { value: "6:00 PM", name: "6:00 PM" },
    { value: "6:30 PM", name: "6:30 PM" },
    { value: "7:00 PM", name: "7:00 PM" },
    { value: "7:30 PM", name: "7:30 PM" },
    { value: "8:00 PM", name: "8:00 PM" },
    { value: "8:30 PM", name: "8:30 PM" },
    { value: "9:00 PM", name: "9:00 PM" },
    { value: "9:30 PM", name: "9:30 PM" },
    { value: "10:00 PM", name: "10:00 PM" },
    { value: "10:30 PM", name: "10:30 PM" },
    { value: "11:00 PM", name: "11:00 PM" },
    { value: "11:30 PM", name: "11:30 PM" },
    { value: "12:00 AM", name: "12:00 AM", dayBreak: true },
    { value: "12:30 AM", name: "12:30 AM" },
    { value: "1:00 AM", name: "1:00 AM" },
    { value: "1:30 AM", name: "1:30 AM" },
    { value: "2:00 AM", name: "2:00 AM" },
    { value: "2:30 AM", name: "2:30 AM" },
    { value: "3:00 AM", name: "3:00 AM" },
    { value: "3:30 AM", name: "3:30 AM" },
    { value: "4:00 AM", name: "4:00 AM" },
    { value: "4:30 AM", name: "4:30 AM" },
    { value: "5:00 AM", name: "5:00 AM" },
    { value: "5:30 AM", name: "5:30 AM" },
    { value: "6:00 AM", name: "6:00 AM" },
    { value: "6:30 AM", name: "6:30 AM" },
    { value: "7:00 AM", name: "7:00 AM" },
    { value: "7:30 AM", name: "7:30 AM" }
];

export interface IEventFormValue {
    id: string;
    eventDate: Date;
    eventStartValue: string;
    eventEndValue: string;
    serviceStartValue: string;
    serviceEndValue: string;
    mountainAdjustedPacificEventStartValue: string;
    mountainAdjustedPacificEventEndValue: string;
    mountainAdjustedPacificServiceStartValue: string;
    mountainAdjustedPacificServiceEndValue: string;
    liquorServiceHoursExtensionReason: string;
    disturbancePreventionMeasuresDetails: string;
    isPacificTimeZone: boolean;
}
