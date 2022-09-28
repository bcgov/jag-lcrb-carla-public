import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { DataService } from "./data.service";
import { SepApplication, SepTermAndCondtion } from "@models/sep-application.model";
import { SepApplicationSummary } from "@models/sep-application-summary.model";
import { SepDrinkType } from "@models/sep-drink-type.model";
import { SepPoliceJobSummary } from "@models/sep-police-job-summary";
import { SepPoliceHome } from "@models/sep-police-home";
import { Contact } from "@models/contact.model";
import { PagingResult } from "@models/paging-result.model";

@Injectable()
export class SpecialEventsDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

  getSpecialEventPolice(id: string): Observable<SepApplication> {
    const apiPath = `api/special-events/police/${id}`;
    return this.http.get<SepApplication>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getSpecialEventForApplicant(id: string): Observable<SepApplication> {
    const apiPath = `api/special-events/applicant/${id}`;
    return this.http.get<SepApplication>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getSubmittedApplications(): Observable<SepApplicationSummary[]> {
    const apiPath = `api/special-events/current/submitted`;
    return this.http.get<SepApplicationSummary[]>(apiPath, { headers: this.headers })
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
    return this.http.post<SepApplication>('api/special-events/', data, { headers: this.headers })
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
   * Generate an invoice for a special event
   * @param id - special event id
   */
   submitSepApplication(id: string) {
    return this.http.post<SepApplication>(`api/special-events/submit/${id}`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Generate an invoice for a special event
   * @param id - special event id
   */
   generateInvoiceSepApplication(id: string) {
    return this.http.post<SepApplication>(`api/special-events/generate-invoice/${id}`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a terms and conditions for a special event application in Dynamics
   * @param data - special event application termis and conditions
   */
  updateSepTermsAndConditions(data: SepTermAndCondtion[], id: string) {
    return this.http.put<SepTermAndCondtion[]>(`api/special-events/terms-and-conditions/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeAssignSepApplication(id: string, assigneeId: string) {
    return this.http.post<Contact>(`api/special-events/police/${id}/assign`, JSON.stringify(assigneeId), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeApproveSepApplication(id: string) {
    return this.http.post<string>(`api/special-events/police/${id}/approve`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeDenySepApplication(id: string, reason: string) {
    return this.http.post<string>(`api/special-events/police/${id}/deny`, {reason}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  policeCancelSepApplication(id: string, reason: string) {
    return this.http.post<string>(`api/special-events/police/${id}/cancel`,{reason}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * set the municipality for a special event to the sep city provided
   * @param id - special event id
   * @param cityId - sep city id
   */
  policeSetMunicipality(id: string, cityId: string) {
    return this.http.post<SepApplication>(`api/special-events/police/${id}/setMunicipality/${cityId}`, {}, { headers: this.headers })
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

  getPoliceApprovalSepApplications(): Observable<SepPoliceJobSummary> {
    const apiPath = `api/special-events/police/all`;
    return this.http.get<SepPoliceJobSummary>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPolicePendingReviewSepApplications(pageIndex: number = 0, pageSize: number = 10, sort: string, sortdir: string): Observable<PagingResult<SepApplicationSummary>> {
    const apiPath = `api/special-events/police/pending-review?pageIndex=${pageIndex}&pageSize=${pageSize}&sort=${sort}&sortdir=${sortdir}`;
    return this.http.get<PagingResult<SepApplicationSummary>>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceApprovedSepApplications(pageIndex: number = 0, pageSize: number = 10, sort: string, sortdir: string): Observable<PagingResult<SepApplicationSummary>> {
    const apiPath = `api/special-events/police/approved?pageIndex=${pageIndex}&pageSize=${pageSize}&sort=${sort}&sortdir=${sortdir}`;
    return this.http.get<PagingResult<SepApplicationSummary>>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceDeniedSepApplications(pageIndex: number = 0, pageSize: number = 10, sort: string, sortdir: string): Observable<PagingResult<SepApplicationSummary>> {
    const apiPath = `api/special-events/police/denied?pageIndex=${pageIndex}&pageSize=${pageSize}&sort=${sort}&sortdir=${sortdir}`;
    return this.http.get<PagingResult<SepApplicationSummary>>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceApprovalMySepApplications(): Observable<SepPoliceJobSummary> {
    const apiPath = `api/special-events/police/my`;
    return this.http.get<SepPoliceJobSummary>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getPoliceHome(): Observable<SepPoliceHome> {
    const apiPath = `api/special-events/police/home`;
    return this.http.get<SepPoliceHome>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getClaimInfo(jobNumber: string) {
    const apiPath = `api/special-events/claim-info/${jobNumber}`;
    return this.http.get<any>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  linkClaimToContact(jobNumber: string) {
    const apiPath = `api/special-events/link-claim-to-contact/${jobNumber}`;
    return this.http.get<any>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}

export class AutoCompleteItem {
  id: string;
  name: string;
  policeJurisdictionName: string;
  lGINName: string;
}
