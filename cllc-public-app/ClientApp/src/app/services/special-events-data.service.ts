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

  getPoliceApprovalSepApplications(): Observable<SepApplicationSummary[]> {
    const data: SepApplicationSummary = {
      specialEventId: '7d98a02b-e14f-43e4-a8c9-6763ba6a5e76',
      eventStartDate: new Date('2021-02-16T19:20:48+00:00'),
      dateSubmitted: new Date('2020-04-01T07:00:00+00:00'),
      eventName: 'Annual Block Watch Party',
      eventStatus: 999999,
      typeOfEvent: 1,
      maximumNumberOfGuests: 200,
    };

    return of([data]);
  }
}
