import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { DataService } from "./data.service";
import { SepApplication } from "@models/sep-application.model";
import { SepApplicationSummary } from "@models/sep-application-summary.model";
import { SepDrinkType } from "@models/sep-drink-type.model";

@Injectable()
export class SpecialEventsDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }


  getSpecialEvent(id: string): Observable<SepApplication> {
    const apiPath = `api/special-events/${id}`;
    return this.http.get<SepApplication>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getSepDrinkTypes(): Observable<SepDrinkType[]> {
    const apiPath = 'api/special-events/drink-types';
    return this.http.get<SepDrinkType[]>(apiPath, { headers: this.headers })
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

  policeAssignSepApplication(id: string, assigneeId: string) {
    return this.http.post<string>(`api/special-events/police/${id}/assign`, JSON.stringify(assigneeId) , { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeApproveSepApplication(id: string) {
    return this.http.post<string>(`api/special-events/police/${id}/approve`, {} , { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeDenySepApplication(id: string) {
    return this.http.post<string>(`api/special-events/police/${id}/deny`, {} , { headers: this.headers })
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
    const apiPath = `api/special-events/police/all`;
    return this.http.get<SepApplicationSummary[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceApprovalMySepApplications(): Observable<SepApplicationSummary[]> {
    const apiPath = `api/special-events/police/my`;
    return this.http.get<SepApplicationSummary[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}

export class AutoCompleteItem {
  id: string;
  name: string;
  policeJurisdictionName: string;
  lGINName: string;
}