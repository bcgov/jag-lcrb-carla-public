import { Injectable } from '@angular/core';
import { Headers, Response, ResponseContentType } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { FileSystemItem } from '../models/file-system-item.model';
import { AdoxioApplication } from '../models/adoxio-application.model';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AdoxioApplicationDataService {

  apiPath = 'api/adoxioapplication/';
  jsonHeaders: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  public files: FileSystemItem[] = [];

  constructor(private http: HttpClient) { }

  /**
   * Get all Dynamics Applications for the current user
   * */
  getAdoxioApplications(): Observable<AdoxioApplication[]> {
    return this.http.get<AdoxioApplication[]>(this.apiPath + 'current', { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Gets the number of submitted Applications for the current user
   * */
  getSubmittedApplicationCount(): Observable<number> {
    return this.http.get<number>(this.apiPath + 'current/submitted-count', { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  getAllCurrentApplications(): Observable<AdoxioApplication[]> {
    return this.http.get<AdoxioApplication[]>(this.apiPath + 'current', { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get a Dynamics Application by application ID
   * @param applicationId
   */
  getApplicationById(applicationId: string): Observable<AdoxioApplication> {
    return this.http.get<AdoxioApplication>(this.apiPath + applicationId, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }


  /**
   * Cancel the Dynamics Application
   * @param id
   */
  cancelApplication(id: string) {
    // call API
    return this.http.post(this.apiPath + id + '/cancel', { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Delete the Dynamics Application
   * @param id
   */
  deleteApplication(id: string) {
    // call API
    return this.http.post(this.apiPath + id + '/delete', { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }


  /**
   * Update the Dynamics Application
   * @param applicationData
   */
  updateApplication(applicationData: any) {
    // call API
    // console.log("===== AdoxioApplicationDataService.updateApplication: ", applicationData);
    return this.http.put(this.apiPath + applicationData.id, applicationData, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a Dynamics Application
   * @param applicationData
   */
  createApplication(applicationData: any): Observable<AdoxioApplication> {
    // call API
    // console.log("===== AdoxioApplicationDataService.createApplication: ", applicationData);
    return this.http.post<AdoxioApplication>(this.apiPath, applicationData, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
    return Promise.reject(errMsg);
  }

  /**
   * Get a file list of documents attached to the application by ID and document type
   * @param applicationId
   * @param documentType
   */
  getFileListAttachedToApplication(applicationId: string, documentType: string): Observable<FileSystemItem[]> {
    const headers = new HttpHeaders({});
    const attachmentURL = 'api/adoxioapplication/' + applicationId + '/attachments';
    const getFileURL = attachmentURL + '/' + documentType;
    return this.http.get<FileSystemItem[]>(getFileURL, { headers: headers })
      .pipe(catchError(this.handleError));

  }

  downloadFile(serverRelativeUrl: string, applicationId: string) {
    const headers = new HttpHeaders({});
    const attachmentURL = `api/file/${applicationId}/download-file/application?serverRelativeUrl=${encodeURIComponent(serverRelativeUrl)}`;
    return this.http.get(attachmentURL, { headers: headers, responseType: 'blob' })
      .pipe(catchError(this.handleError));

  }
}
