import { Injectable } from '@angular/core';
import { Http, Headers, Response, ResponseContentType } from "@angular/http";
import "rxjs/add/operator/toPromise";
import { FileSystemItem } from '../models/file-system-item.model';
import { AdoxioApplication } from "../models/adoxio-application.model";
import { catchError } from 'rxjs/operators';

@Injectable()
export class AdoxioApplicationDataService {

  apiPath = "api/adoxioapplication/";
  jsonHeaders: Headers = new Headers({'Content-Type': 'application/json'});
  public files: FileSystemItem[] = [];

  constructor(private http: Http) { }

  /**
   * Get all Dynamics Applications for the current user
   * */
  getAdoxioApplications() {
    return this.http.get(this.apiPath + "current", { headers: this.jsonHeaders })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let allAdoxioApplications = [];

         data.forEach((entry) => {
           let adoxioApplication = new AdoxioApplication();
           adoxioApplication.id = entry.id;
           adoxioApplication.name = entry.name;
           adoxioApplication.applyingPerson = entry.applyingPerson;
           adoxioApplication.jobNumber = entry.jobNumber;
           adoxioApplication.licenseType = entry.licenseType;
           adoxioApplication.establishmentName = entry.establishmentName;
           adoxioApplication.establishmentAddress = entry.establishmentAddress;
           adoxioApplication.applicationStatus = entry.applicationStatus;
           allAdoxioApplications.push(adoxioApplication);
         });
         
         return allAdoxioApplications;
       })
       .catch(this.handleError);
  }

  /**
   * Gets the number of submitted Applications for the current user
   * */
  getSubmittedApplicationCount() {
    return this.http.get(this.apiPath + "current/submitted-count", { headers: this.jsonHeaders })
       .toPromise()
       .then((res: Response) => {
         return res.json();
       })
       .catch(this.handleError);
  }

  getAllCurrentApplications() {
    return this.http.get(this.apiPath + "current", { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get a Dynamics Application by application ID
   * @param applicationId
   */
  getApplicationById(applicationId: string) {
    return this.http.get(this.apiPath + applicationId, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }


  /**
   * Cancel the Dynamics Application
   * @param id
   */
  cancelApplication(id: string) {
    //call API    
    return this.http.post(this.apiPath + id + "/cancel", { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Delete the Dynamics Application
   * @param id
   */
  deleteApplication(id: string) {
    //call API    
    return this.http.post(this.apiPath + id + "/delete", { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }


  /**
   * Update the Dynamics Application
   * @param applicationData
   */
  updateApplication(applicationData: any) {
    //call API
    //console.log("===== AdoxioApplicationDataService.updateApplication: ", applicationData);
    return this.http.put(this.apiPath + applicationData.id, applicationData, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a Dynamics Application
   * @param applicationData
   */
  createApplication(applicationData: any) {
    //call API
    //console.log("===== AdoxioApplicationDataService.createApplication: ", applicationData);
    return this.http.post(this.apiPath, applicationData, { headers: this.jsonHeaders })
      .pipe(catchError(this.handleError));
  }

   private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || "";
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
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
  getFileListAttachedToApplication(applicationId: string, documentType: string) {
    const headers = new Headers({});
    const attachmentURL = 'api/adoxioapplication/' + applicationId + '/attachments';
    const getFileURL = attachmentURL + '/' + documentType;
    return this.http.get(getFileURL, { headers: headers })
      .pipe(catchError(this.handleError));
      //.map((data: Response) => { return <FileSystemItem[]>data.json() })
      //.subscribe((data) => {
      //  // convert bytes to KB
      //  data.forEach((entry) => {
      //    entry.size = entry.size / 1024
      //  });
      //  this.files = data;
      //});

  }
  downloadFile(serverRelativeUrl: string, applicationId: string) {
    const headers = new Headers({});
    const attachmentURL = `api/adoxioapplication/download-file/${applicationId}?serverRelativeUrl=${encodeURIComponent(serverRelativeUrl)}`;
    return this.http.get(attachmentURL, { responseType: ResponseContentType.Blob })
      .map(res => res.blob())
      .pipe(catchError(this.handleError));

  }
}
