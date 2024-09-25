import { format, addDays } from "date-fns";

export class SepSchedule {
    id: string; // server side primary key
    specialEventId: string; // server side foreign key
    locationId: string; // server side foreign key
    eventStart: Date;
    eventEnd: Date;
    serviceStart: Date;
    serviceEnd: Date;
    liquorServiceHoursExtensionReason: string;
    disturbancePreventionMeasuresDetails: string;

    constructor(sched: IEventFormValue) {
        if (sched) {
            this.id = sched.id;
            this.eventStart = this.formatDate(sched.eventDate, sched.eventStartValue);
            this.eventEnd = this.formatDate(sched.eventDate, sched.eventEndValue);
            this.serviceStart = this.formatDate(sched.eventDate, sched.serviceStartValue);
            this.serviceEnd = this.formatDate(sched.eventDate, sched.serviceEndValue);
            this.liquorServiceHoursExtensionReason = sched.liquorServiceHoursExtensionReason;
            this.disturbancePreventionMeasuresDetails = sched.disturbancePreventionMeasuresDetails;
        }
    }

    toEventFormValue(): IEventFormValue {
        const result = { id: this.id } as IEventFormValue;
        result.eventDate = this.eventStart;



        if (this.eventStart) {
            result.eventStartValue = format(new Date(this.eventStart), "h:mm aa");
        }
        if (this.eventEnd) {
            result.eventEndValue = format(new Date(this.eventEnd), "h:mm aa");
        }
        if (this.serviceStart) {
            result.serviceStartValue = format(new Date(this.serviceStart), "h:mm aa");
        }
        if (this.serviceEnd) {
            result.serviceEndValue = format(new Date(this.serviceEnd), "h:mm aa");
        }
        result.liquorServiceHoursExtensionReason = this.liquorServiceHoursExtensionReason;
        result.disturbancePreventionMeasuresDetails = this.disturbancePreventionMeasuresDetails;
        
        return result;
    }

    getServiceHours(): number {
        const serviceHours = parseInt(format(new Date(this.serviceEnd), "H")) - parseInt(format(new Date(this.serviceStart), "H"));
        return serviceHours;
    }


    /**
     *
     * @param eventDate
     * @param time, assumed format "HH:MM [AM,PM]" e.g. '6:30 PM'
     */
    private formatDate(eventDate: Date, time: string): Date {

        let tempDate = new Date(eventDate);

        // let day = parseInt(format(new Date(eventDate), "dd"), 10);

        // console.log("formatting date: ", eventDate, time)
        if (this.isNextDay(time)) {
           // console.log("is next day")
            tempDate = addDays(tempDate, 1);
            // day += 1;
        }
        // const dateString = `${day} ${format(tempDate, "d MMM yyyy")} ${time}`;
        const dateString = `${format(tempDate, "d MMM yyyy")} ${time}`;
        // console.log("dateString:",dateString);
        const result = new Date(dateString);
        // console.log("result is: ", result);
        return result;
    }

    private isNextDay(time: string): boolean {
        const dayBreakIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.dayBreak === true));
        const timeIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.value === time));
        return timeIndex >= dayBreakIndex;
    }

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
    liquorServiceHoursExtensionReason: string;
    disturbancePreventionMeasuresDetails: string;
}
