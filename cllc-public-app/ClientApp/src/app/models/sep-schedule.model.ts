import { fromUtcToPacificWallClock, fromPacificWallClockToUtc } from '@shared/date-fns';
import { addDays, format, subDays } from 'date-fns';

export class SepSchedule {
  id: string; // server side primary key
  specialEventId: string; // server side foreign key
  locationId: string; // server side foreign key
  eventStart: Date;
  eventEnd: Date;
  serviceStart: Date;
  serviceEnd: Date;
  createdOn: Date; // when the times were encoded; unsaved drafts keep it so they decode correctly later
  liquorServiceHoursExtensionReason: string;
  disturbancePreventionMeasuresDetails: string;

  constructor(sched: IEventFormValue) {
    if (sched) {
      this.id = sched.id;
      this.createdOn = new Date();
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

    result.eventDate = this.eventStart ? fromUtcToPacificWallClock(this.eventStart, this.createdOn) : this.eventStart;

    if (this.eventStart) {
      result.eventStartValue = format(fromUtcToPacificWallClock(this.eventStart, this.createdOn), 'h:mm aa');
      // formatDate saved after-midnight starts on the next day, so step back to the event date.
      if (this.isNextDay(result.eventStartValue)) {
        result.eventDate = subDays(result.eventDate, 1);
      }
    }
    if (this.eventEnd) {
      result.eventEndValue = format(fromUtcToPacificWallClock(this.eventEnd, this.createdOn), 'h:mm aa');
    }
    if (this.serviceStart) {
      result.serviceStartValue = format(fromUtcToPacificWallClock(this.serviceStart, this.createdOn), 'h:mm aa');
    }
    if (this.serviceEnd) {
      result.serviceEndValue = format(fromUtcToPacificWallClock(this.serviceEnd, this.createdOn), 'h:mm aa');
    }
    result.liquorServiceHoursExtensionReason = this.liquorServiceHoursExtensionReason;
    result.disturbancePreventionMeasuresDetails = this.disturbancePreventionMeasuresDetails;

    return result;
  }

  /**
   * Formats a date and time string into a UTC Date, taking into account if the time is on the next day.
   *
   * @param eventDate
   * @param time, assumed format "HH:MM [AM,PM]" e.g. '6:30 PM'
   */
  private formatDate(eventDate: Date, time: string): Date {
    let tempDate = new Date(eventDate);

    if (this.isNextDay(time)) {
      tempDate = addDays(tempDate, 1);
    }

    const dateString = `${format(tempDate, 'd MMM yyyy')} ${time}`;
    const result = new Date(dateString);

    return fromPacificWallClockToUtc(result);
  }

  private isNextDay(time: string): boolean {
    const dayBreakIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find((slot) => slot.dayBreak === true));
    const timeIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find((slot) => slot.value === time));
    return timeIndex >= dayBreakIndex;
  }
}

export const TIME_SLOTS = [
  { value: '8:00 AM', name: '8:00 AM' },
  { value: '8:30 AM', name: '8:30 AM' },
  { value: '9:00 AM', name: '9:00 AM' },
  { value: '9:30 AM', name: '9:30 AM' },
  { value: '10:00 AM', name: '10:00 AM' },
  { value: '10:30 AM', name: '10:30 AM' },
  { value: '11:00 AM', name: '11:00 AM' },
  { value: '11:30 AM', name: '11:30 AM' },
  { value: '12:00 PM', name: '12:00 PM' },
  { value: '12:30 PM', name: '12:30 PM' },
  { value: '1:00 PM', name: '1:00 PM' },
  { value: '1:30 PM', name: '1:30 PM' },
  { value: '2:00 PM', name: '2:00 PM' },
  { value: '2:30 PM', name: '2:30 PM' },
  { value: '3:00 PM', name: '3:00 PM' },
  { value: '3:30 PM', name: '3:30 PM' },
  { value: '4:00 PM', name: '4:00 PM' },
  { value: '4:30 PM', name: '4:30 PM' },
  { value: '5:00 PM', name: '5:00 PM' },
  { value: '5:30 PM', name: '5:30 PM' },
  { value: '6:00 PM', name: '6:00 PM' },
  { value: '6:30 PM', name: '6:30 PM' },
  { value: '7:00 PM', name: '7:00 PM' },
  { value: '7:30 PM', name: '7:30 PM' },
  { value: '8:00 PM', name: '8:00 PM' },
  { value: '8:30 PM', name: '8:30 PM' },
  { value: '9:00 PM', name: '9:00 PM' },
  { value: '9:30 PM', name: '9:30 PM' },
  { value: '10:00 PM', name: '10:00 PM' },
  { value: '10:30 PM', name: '10:30 PM' },
  { value: '11:00 PM', name: '11:00 PM' },
  { value: '11:30 PM', name: '11:30 PM' },
  { value: '12:00 AM', name: '12:00 AM', dayBreak: true },
  { value: '12:30 AM', name: '12:30 AM' },
  { value: '1:00 AM', name: '1:00 AM' },
  { value: '1:30 AM', name: '1:30 AM' },
  { value: '2:00 AM', name: '2:00 AM' },
  { value: '2:30 AM', name: '2:30 AM' },
  { value: '3:00 AM', name: '3:00 AM' },
  { value: '3:30 AM', name: '3:30 AM' },
  { value: '4:00 AM', name: '4:00 AM' },
  { value: '4:30 AM', name: '4:30 AM' },
  { value: '5:00 AM', name: '5:00 AM' },
  { value: '5:30 AM', name: '5:30 AM' },
  { value: '6:00 AM', name: '6:00 AM' },
  { value: '6:30 AM', name: '6:30 AM' },
  { value: '7:00 AM', name: '7:00 AM' },
  { value: '7:30 AM', name: '7:30 AM' }
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
