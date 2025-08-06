import { Injectable } from "@angular/core";
import { FileSystemItem } from "@models/file-system-item.model";
import { Application } from "@models/application.model";
import { ApplicationSummary } from "@models/application-summary.model";
import { catchError } from "rxjs/operators";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable } from "rxjs";
import { DataService } from "./data.service";
import { CovidApplication } from "@models/covid-application.model";
import { OngoingLicenseeData } from "../models/ongoing-licensee-data";
import { PagingResult } from "@models/paging-result.model";
import { RelatedLicence } from "../models/related-licence";
import { ApplicationType } from "@models/application-type.model";

@Injectable()
export class ApplicationDataService extends DataService {

  apiPath = "api/applications/";

  files: FileSystemItem[] = [];

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get all Applications for the current user for the given application type.
   *
   * @param {string} applicationTypeName
   * @return {*}  {Observable<ApplicationType>}
   */
  getApplicationTypeByName(applicationTypeName: string): Observable<ApplicationType> {
    return this.http
      .get<ApplicationType>(`${this.apiPath}GetByName/${encodeURIComponent(applicationTypeName)}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get all Dynamics Applications for the current user
   * */
  getAdoxioApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(this.apiPath + "current", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get all  Applications for the current user for the given application type
   * */
  getApplicationsByType(applicationType: string): Observable<Application[]> {
    return this.http
      .get<Application[]>(`${this.apiPath}current/by-type?applicationType=${encodeURIComponent(applicationType)}`,
        { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Gets the count of all approved applications for the current user.
   *
   * @return {*}  {Observable<number>}
   */
  getApprovedApplicationCount(): Observable<number> {
    return this.http
      .get<number>(this.apiPath + 'current/approved-count', { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Gets the count of submitted cannabis retail store applications for the current user
   *
   * @return {*}  {Observable<number>}
   */
  getSubmittedCannabisRetailStoreApplicationCount(): Observable<number> {
    return this.http
      .get<number>(this.apiPath + 'current/cannabis-retail-store/submitted-count', { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getAllCurrentApplications(): Observable<ApplicationSummary[]> {
    return this.http.get<ApplicationSummary[]>(this.apiPath + "current", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  //LCSD-6357 Split getLGApprovalApplications into 3 parts
  getLGApprovalApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(this.apiPath + "current/lg-approvals", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  //LCSD-6357 part 1:
  getLGApprovalApplicationsDecisionNotMade(pageIndex: number = 0, pageSize: number = 10): Observable<PagingResult<Application>> {
    const url = `${this.apiPath}current/lg-approvals-decision-not-made?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<PagingResult<Application>>(url, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  //LCSD-6357 part 2:
  getLGApprovalApplicationsForZoning(pageIndex: number = 0, pageSize: number = 10): Observable<PagingResult<Application>> {
    const url = `${this.apiPath}current/lg-approvals-for-zoning?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<PagingResult<Application>>(url, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  //LCSD-6357 part 3:
  getLGApprovalApplicationsDicisionMadeButNoDocs(pageIndex: number = 0, pageSize: number = 10): Observable<PagingResult<Application>> {
    const url = `${this.apiPath}current/lg-approvals-dicision-made-but-no-docs?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<PagingResult<Application>>(url, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  getResolvedLGApplications(pageIndex: number = 0, pageSize: number = 10): Observable<PagingResult<Application>> {
    const url =`${this.apiPath}current/resolved-lg-applications?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    return this.http.get<PagingResult<Application>>(url, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getOngoingLicenseeChangeApplicationId(): Observable<string> {
    return this.http.get<string>(this.apiPath + "ongoing-licensee-application-id", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get the application data for a permanent change to licensee application.
   *
   * @param {string} applicationId
   * @return {*}  {Observable<any>}
   */
  getPermanentChangesToLicenseeData(applicationId: string): Observable<any> {
    let url = `${this.apiPath}permanent-change-to-licensee-data`;
    let params: Record<string, any> = { isLegalEntityReview: 'false' };

    if (applicationId) {
      params = { ...params, applicationId: applicationId };
    }

    return this.http.get<any>(url, { headers: this.headers, params: params }).pipe(catchError(this.handleError));
  }

  /**
   * Get the application data for a legal entity permanent change to licensee application.
   *
   * @param {string} applicationId
   * @return {*}  {Observable<any>}
   */
  getLegalEntityPermanentChangesToLicenseeData(applicationId: string): Observable<any> {
    let url = `${this.apiPath}permanent-change-to-licensee-data`;
    let params: Record<string, any> = { isLegalEntityReview: 'true' };

    if (applicationId) {
      params = { ...params, applicationId: applicationId };
    }

    return this.http.get<any>(url, { headers: this.headers, params: params }).pipe(catchError(this.handleError));
  }

  /**
   * // TODO: tiedhouse - finalize when the API controllers are done.
   *
   * Get or create the application for "permanent changes to licensee as a result of a legal entity review" (LE-PCL).
   *
   * Accepts either the Legal Entity Review (LER) Application ID or the LE-PCL Application ID.
   * - If the LER Application ID is provided, it will get or create the corresponding PCL application data.
   * - If the PCL Application ID is provided, it will return the existing PCL application data.
   *
   * Note: An LE-PCL is the same as a regular PCL, except that it was initiated by staff as a result of a legal entity
   * review rather than by the user.
   *
   * @param {string} applicationId Either the LER Application ID or the LE-PCL Application ID.
   * @return {*}  {Observable<Application>}
   */
  getOrCreateLegalEntityPermanentChangesToLicenseeApplication(applicationId: string): Observable<Application> {
    let url = `${this.apiPath}legal-entity-permanent-change-to-licensee-application/${applicationId}`;

    return this.http.get<Application>(url, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  getOngoingLicenseeData(type: "on-going" | "create"): Observable<OngoingLicenseeData> {
    return this.http.get<OngoingLicenseeData>(`${this.apiPath}licensee-data/${type}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  /**
   * Get a Dynamics Application by application ID
   * @param applicationId
   */
  getApplicationById(applicationId: string): Observable<Application> {
    return this.http.get<Application>(this.apiPath + applicationId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  /**
   * Cancel the Dynamics Application
   * @param id
   */
  cancelApplication(id: string) {
    // call API
    return this.http.post(this.apiPath + id + "/cancel", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Delete the Dynamics Application
   * @param id
   */
  deleteApplication(id: string) {
    // call API
    return this.http.post(this.apiPath + id + "/delete", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  /**
   * Update the Dynamics Application
   * @param applicationData
   */
  updateApplication(applicationData: Application): Observable<Application> {
    // call API
    // console.log("===== AdoxioApplicationDataService.updateApplication: ", applicationData);
    return this.http.put<Application>(this.apiPath + applicationData.id, applicationData, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

    /**
   * Update the Dynamics Application
   * @param applicationData
   */
  submitLegalEntityApplication(applicationData: Application): Observable<Application> {
    // call API
    // console.log("===== AdoxioApplicationDataService.SubmitLegalEntityApplication: ", applicationData);
    return this.http.put<Application>(this.apiPath +"legal_entity/" + applicationData.id, applicationData, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a Dynamics Application
   * @param applicationData
   */
  createApplication(applicationData: Application): Observable<Application> {
    // call API
    // console.log("===== AdoxioApplicationDataService.createApplication: ", applicationData);
    return this.http.post<Application>(this.apiPath, applicationData, { headers: this.headers });
  }

  /**
   * Create a Dynamics Application For Covid
   * @param applicationData
   */
  createCovidApplication(applicationData: CovidApplication): Observable<CovidApplication> {
    // call API
    return this.http.post<CovidApplication>(this.apiPath + "covid", applicationData, { headers: this.headers });
  }


  /**
   * Get a file list of documents attached to the application by ID and document type
   * @param applicationId
   * @param documentType
   */
  getFileListAttachedToApplication(applicationId: string, documentType: string): Observable<FileSystemItem[]> {
    const headers = new HttpHeaders({});
    const attachmentURL = `api/adoxioapplication/${applicationId}/attachments`;
    const getFileURL = attachmentURL + "/" + documentType;
    return this.http.get<FileSystemItem[]>(getFileURL, { headers: headers });
  }

  downloadFile(serverRelativeUrl: string, applicationId: string) {
    const headers = new HttpHeaders({});
    const attachmentURL =
      `api/file/${applicationId}/download-file/application?serverRelativeUrl=${encodeURIComponent(serverRelativeUrl)}`;
    return this.http.get(attachmentURL, { headers: headers, responseType: "blob" })
      .pipe(catchError(this.handleError));

  }

  // 20245-03-20 LCSD-6368 waynezen
  getAutocomplete(search: string): Observable<any[]> {

    return this.http.get<any[]>(this.apiPath + `autocomplete?jobnumber=${search}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


}
