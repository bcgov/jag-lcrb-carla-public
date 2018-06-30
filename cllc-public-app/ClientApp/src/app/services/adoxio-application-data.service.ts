import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";

import { AdoxioApplication } from "../models/adoxio-application.model";

@Injectable()
export class AdoxioApplicationDataService {
   constructor(private http: Http) { }

   getAdoxioApplications() {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.get("api/adoxioapplication/current", { headers: headers })
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
}
