import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { DataService } from "./data.service";
import { SepApplication } from "@models/sep-application.model";
import { SepApplicationSummary } from "@models/sep-application-summary.model";

@Injectable()
export class SpecialEventsDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }


  getSpecialEvent(id: string): Observable<SepApplication[]> {
    const apiPath = `api/special-events/${id}`;
    return this.http.get<SepApplication[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a new special event application in Dynamics
   * @param data - special event application data
   */
  createSepApplication(data: SepApplication) {
    return this.http.post<SepApplication>("api/special-events/", data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a  special event application in Dynamics
   * @param data - special event application data
   */
  updateSepApplication(data: SepApplication, id: string) {
    return this.http.put<SepApplication>(`api/special-events/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * delete a  special event application in Dynamics
   * @param data - special event application data
   */
  deleteSepApplication(id: string) {
    return this.http.post<SepApplication>(`api/special-events/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getSepCityAutocompleteData(name: string, defaults: boolean) {
    const apiPath = `api/special-events/sep-city/autocomplete?name=${name}&defaults=${defaults}`;
    return this.http.get<AutoCompleteItem[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceApprovalSepApplications(): Observable<SepApplicationSummary[]> {
    const mock1: SepApplicationSummary = {
      specialEventId: '00000000-0000-0000-0000-00000000000',
      eventStartDate: new Date('2021-10-25T19:20:48+00:00'),
      dateSubmitted: new Date('2020-02-01T07:00:00+00:00'),
      eventName: 'Annual Block Watch Party',
      eventStatus: 999999,
      typeOfEvent: 1,
      maximumNumberOfGuests: 200,
    };

    const mock2: SepApplicationSummary = {
      specialEventId: '11111111-1111-1111-1111-111111111111',
      eventStartDate: new Date('2021-02-16T19:20:48+00:00'),
      dateSubmitted: new Date('2020-03-01T07:00:00+00:00'),
      eventName: 'Office Christmas Party',
      eventStatus: 999999,
      typeOfEvent: 1,
      maximumNumberOfGuests: 50,
    };

    const mock3: SepApplicationSummary = {
      specialEventId: '22222222-2222-2222-2222-222222222222',
      eventStartDate: new Date('2021-06-20T19:20:48+00:00'),
      dateSubmitted: new Date('2020-08-11T07:00:00+00:00'),
      eventName: 'Community Event',
      eventStatus: 999999,
      typeOfEvent: 1,
      maximumNumberOfGuests: 1300,
    };

    return of([mock1, mock2, mock3]);
  }
}

export class AutoCompleteItem {
  id: string;
  name: string;
  policeJurisdictionName: string;
  lGINName: string;
}