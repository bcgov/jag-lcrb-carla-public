import { format } from 'date-fns'

export class SepSchedule {
    eventScheduleId: string; // server side primary key
    specialEventId: string; // server side foreign key
    locationId: string; // server side foreign key
    eventStart: Date;
    eventEnd: Date;
    serviceStart: Date;
    serviceEnd: Date;

    constructor(sched: IEventFormValue) {
        if (sched) {
            this.eventStart = this.formatDate(sched.eventDate, sched.eventStartValue);
            this.eventEnd = this.formatDate(sched.eventDate, sched.eventEndValue);
            this.serviceStart = this.formatDate(sched.eventDate, sched.serviceStartValue);
            this.serviceEnd = this.formatDate(sched.eventDate, sched.serviceEndValue);
        }
    }

    toEventFormValue(): IEventFormValue {
        let result = {} as IEventFormValue;
        result.eventDate = this.eventStart;
        if (this.eventStart) {
            result.eventStartValue = format(new Date(this.eventStart), 'h:mm aa');
        }
        if (this.eventEnd) {
            result.eventEndValue = format(new Date(this.eventEnd), 'h:mm aa');
        }
        if (this.serviceStart) {
            result.serviceStartValue = format(new Date(this.serviceStart), 'h:mm aa');
        }
        if (this.serviceEnd) {
            result.serviceEndValue = format(new Date(this.serviceEnd), 'h:mm aa');
        }

        return result;
    }

    public getServiceHours() {
      let serviceHours = parseInt(format(new Date(this.serviceEnd), "H")) - parseInt(format(new Date(this.serviceStart), "H"));
      return serviceHours;
    }



    /**
     *
     * @param eventDate
     * @param time, assumed format "HH:MM [AM,PM]" e.g. '6:30 PM'
     */
    private formatDate(eventDate: Date, time: string): Date {
        let result = new Date(eventDate);

        let matches = time && time.match(/(\d+):(\d+)\s?(AM|PM)/);
        if (matches?.length > 0) {
            let hour: number = parseInt(matches[1], 10);
            let minute = parseInt(matches[2], 10);
            let amPm = matches[3];

            if (amPm === 'AM' && ((hour >= 1 && hour <= 7) || 12)) { // 12 AM to 7 AM
                result.setDate(result.getDate() + 1);
            }
            if (amPm == 'PM' && hour !== 12) {
                hour += 12;
            }
            result.setHours(hour);
            result.setMinutes(minute);
            result.setSeconds(0);
            result.setMilliseconds(0);
        }
        return result;
    }

}

export interface IEventFormValue {
    eventDate: Date;
    eventStartValue: string;
    eventEndValue: string;
    serviceStartValue: string;
    serviceEndValue: string;
}
