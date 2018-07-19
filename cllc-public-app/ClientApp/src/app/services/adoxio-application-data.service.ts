import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";
import { FileSystemItem } from '../models/file-system-item.model';

import { AdoxioApplication } from "../models/adoxio-application.model";

@Injectable()
export class AdoxioApplicationDataService {

  apiPath = "api/adoxioapplication/";
  public files: FileSystemItem[] = [];

  constructor(private http: Http) { }

  /**
   * Get all Dynamics Applications for the current user
   * */
  getAdoxioApplications() {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

    return this.http.get(this.apiPath + "current", { headers: headers })
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
   * Get a Dynamics Application by application ID
   * @param applicationId
   */
  getApplicationById(applicationId: string) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    return this.http.get(this.apiPath + applicationId, { headers: headers });
  }

  /**
   * Update the Dynamics Application
   * @param applicationData
   */
  updateApplication(applicationData: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    //call API
    //console.log("===== AdoxioApplicationDataService.updateApplication: ", applicationData);
    return this.http.put(this.apiPath + applicationData.id, applicationData, { headers: headers });
  }

  /**
   * Create a Dynamics Application
   * @param applicationData
   */
  createApplication(applicationData: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    //call API
    //console.log("===== AdoxioApplicationDataService.createApplication: ", applicationData);
    return this.http.post(this.apiPath, applicationData, { headers: headers });
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
    return this.http.get(getFileURL, { headers: headers });
      //.map((data: Response) => { return <FileSystemItem[]>data.json() })
      //.subscribe((data) => {
      //  // convert bytes to KB
      //  data.forEach((entry) => {
      //    entry.size = entry.size / 1024
      //  });
      //  this.files = data;
      //});

  }
}
