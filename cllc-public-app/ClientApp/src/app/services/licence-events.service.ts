import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LicenceEvent } from '@models/licence-event.model';
import { fromPacificWallClock, toPacificWallClock } from '@shared/pacific-time';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { DataService } from './data.service';

type DateConverter = (value: Date | string) => Date;

/**
 * Event dates/times are always Pacific Time. The API works with real instants (UTC), while the UI works with
 * "Pacific wall-clock" Dates (local fields read as Pacific Time). Convert at this boundary only.
 */
function convertLicenceEventDates(event: LicenceEvent, convert: DateConverter): LicenceEvent {
  if (!event) {
    return event;
  }
  const result = { ...event };
  if (result.startDate) {
    result.startDate = convert(result.startDate);
  }
  if (result.endDate) {
    result.endDate = convert(result.endDate);
  }
  if (result.schedules) {
    result.schedules = result.schedules.map((sched) => {
      const converted = { ...sched };
      (['eventStartDateTime', 'eventEndDateTime', 'serviceStartDateTime', 'serviceEndDateTime'] as const).forEach(
        (key) => {
          if (converted[key]) {
            converted[key] = convert(converted[key]);
          }
        }
      );
      return converted;
    });
  }
  return result;
}

@Injectable()
export class LicenceEventsService extends DataService {
  apiPath = 'api/licenceevents/';

  constructor(private http: HttpClient) {
    super();
  }

  createLicenceEvent(licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http
      .post<LicenceEvent>(this.apiPath, convertLicenceEventDates(licenceEvent, fromPacificWallClock), {
        headers: this.headers
      })
      .pipe(map((event) => convertLicenceEventDates(event, toPacificWallClock)));
  }

  getLicenceEvent(eventId: string): Observable<LicenceEvent> {
    return this.http
      .get<LicenceEvent>(this.apiPath + eventId, { headers: this.headers })
      .pipe(map((event) => convertLicenceEventDates(event, toPacificWallClock)));
  }

  updateLicenceEvent(eventId: string, licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http
      .put<LicenceEvent>(this.apiPath + eventId, convertLicenceEventDates(licenceEvent, fromPacificWallClock), {
        headers: this.headers
      })
      .pipe(map((event) => convertLicenceEventDates(event, toPacificWallClock)));
  }

  getLicenceEventsList(licenceId: string, num: number): Observable<LicenceEvent[]> {
    return this.http
      .get<LicenceEvent[]>(`${this.apiPath}list/${licenceId}/${num}`, { headers: this.headers })
      .pipe(map((events) => events?.map((event) => convertLicenceEventDates(event, toPacificWallClock))));
  }

  getLicenceEventsListBatch(licenceIds: string[], num: number): Observable<LicenceEvent[]> {
    return this.http
      .post<LicenceEvent[]>(`${this.apiPath}list/batch/${num}`, licenceIds, { headers: this.headers })
      .pipe(map((events) => events?.map((event) => convertLicenceEventDates(event, toPacificWallClock))));
  }
}
