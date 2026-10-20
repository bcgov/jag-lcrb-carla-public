import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LicenceEvent } from '@models/licence-event.model';
import { fromPacificWallClockToUtc } from '@shared/date-fns';
import { startOfDay } from 'date-fns';
import { Observable } from 'rxjs';
import { DataService } from './data.service';

/**
 * Converts a picked date to the UTC instant of midnight Pacific on that date.
 *
 * @param {Date} value
 * @return {*}  {Date}
 */
function encodePacificDate(value: Date): Date {
  return value ? fromPacificWallClockToUtc(startOfDay(new Date(value))) : value;
}

/**
 * Sends the event's start and end dates as midnight Pacific, matching how the forms read them back.
 *
 * @param {LicenceEvent} licenceEvent
 * @return {*}  {LicenceEvent}
 */
function encodeDateRange(licenceEvent: LicenceEvent): LicenceEvent {
  return {
    ...licenceEvent,
    startDate: encodePacificDate(licenceEvent.startDate),
    endDate: encodePacificDate(licenceEvent.endDate)
  };
}

@Injectable()
export class LicenceEventsService extends DataService {
  apiPath = 'api/licenceevents/';

  constructor(private http: HttpClient) {
    super();
  }

  createLicenceEvent(licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http.post<LicenceEvent>(this.apiPath, encodeDateRange(licenceEvent), { headers: this.headers });
  }

  getLicenceEvent(eventId: string): Observable<LicenceEvent> {
    return this.http.get<LicenceEvent>(this.apiPath + eventId, { headers: this.headers });
  }

  updateLicenceEvent(eventId: string, licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http.put<LicenceEvent>(this.apiPath + eventId, encodeDateRange(licenceEvent), { headers: this.headers });
  }

  getLicenceEventsList(licenceId: string, num: number): Observable<LicenceEvent[]> {
    return this.http.get<LicenceEvent[]>(`${this.apiPath}list/${licenceId}/${num}`, { headers: this.headers });
  }

  getLicenceEventsListBatch(licenceIds: string[], num: number): Observable<LicenceEvent[]> {
    return this.http.post<LicenceEvent[]>(`${this.apiPath}list/batch/${num}`, licenceIds, { headers: this.headers });
  }
}
