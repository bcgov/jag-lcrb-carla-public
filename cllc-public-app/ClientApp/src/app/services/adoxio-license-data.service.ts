import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";

import { AdoxioLicense } from "../models/adoxio-license.model";

@Injectable()
export class AdoxioLicenseDataService {
   constructor(private http: Http) { }

   getAdoxioLicenses() {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.get("api/adoxiolicense/current", {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let allAdoxioLicenses = [];

         data.forEach((entry) => {
           let adoxioLicense = new AdoxioLicense();
           adoxioLicense.establishmentName = entry.establishmentName;
           adoxioLicense.establishmentAddress = entry.establishmentAddress;
           adoxioLicense.licenseType = entry.licenseType;
           adoxioLicense.licenseStatus = entry.licenseStatus;
           adoxioLicense.licenseNumber = entry.licenseNumber;
           allAdoxioLicenses.push(adoxioLicense);
         });
         
         return allAdoxioLicenses;
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
