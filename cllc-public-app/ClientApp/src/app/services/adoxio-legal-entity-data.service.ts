import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { AdoxioLegalEntity } from "../models/adoxio-legalentities.model";
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AdoxioLegalEntityDataService {
   constructor(private http: Http) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getLegalEntitiesbyPosition(positionType: string) {

    let apiPath = "api/adoxiolegalentity/position/" + positionType;
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    // call API
    return this.http.get(apiPath, {headers: headers})
      .toPromise()
      .then((res: Response) => {
        //console.log(res);
        let data = res.json();
        let legalEntitiesList = [];

        data.forEach((entry) => {
          let adoxioLegalEntity = new AdoxioLegalEntity();
          adoxioLegalEntity.account = entry.account;
          adoxioLegalEntity.commonnonvotingshares = entry.commonnonvotingshares;
          adoxioLegalEntity.commonvotingshares = entry.commonvotingshares;
          adoxioLegalEntity.dateofbirth = entry.dateofbirth;
          adoxioLegalEntity.firstname = entry.firstname
          adoxioLegalEntity.id = entry.id;
          adoxioLegalEntity.interestpercentage = entry.interestpercentage;
          adoxioLegalEntity.isindividual = entry.isindividual;
          adoxioLegalEntity.lastname = entry.lastname;
          adoxioLegalEntity.legalentitytype = entry.legalentitytype;
          adoxioLegalEntity.middlename = entry.middlename;
          adoxioLegalEntity.name = entry.name;
          adoxioLegalEntity.otherlegalentitytype = entry.otherlegalentitytype;
          adoxioLegalEntity.position = entry.position;
          adoxioLegalEntity.preferrednonvotingshares = entry.preferrednonvotingshares;
          adoxioLegalEntity.preferredvotingshares = entry.preferredvotingshares;
          adoxioLegalEntity.relatedentities = entry.relatedentities;
          adoxioLegalEntity.sameasapplyingperson = entry.sameasapplyingperson;
          adoxioLegalEntity.shareholderType = entry.shareholderType;
          adoxioLegalEntity.email = entry.email;
          if (entry.dateofappointment) {
            adoxioLegalEntity.dateofappointment = new Date(entry.dateofappointment);
          }
          legalEntitiesList.push(adoxioLegalEntity);
        });

        return legalEntitiesList;
      })
      .catch(this.handleError);
  }

  getBusinessProfileSummary() {
    let apiPath = "api/adoxiolegalentity/business-profile-summary/";
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    // call API
    return this.http.get(apiPath, { headers: headers });

  }

  /**
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createLegalEntity(data: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    //console.log("===== AdoxioLegalEntityDataService.post: ", data);

    return this.http.post("api/adoxiolegalentity/", data, { headers: headers });
  
  }
  /**
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createShareholderLegalEntity(data: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    //console.log("===== AdoxioLegalEntityDataService.post: ", data);

    return this.http.post("api/adoxiolegalentity/shareholder", data, { headers: headers });
  }

  /**
   * Send a consent request to the emails received as parameter
   * @param data - array of emails
   */
  sendConsentRequestEmail(data: string[]) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    //console.log("===== AdoxioLegalEntityDataService.post: ", data);
    let legalEntityId:string = data[0];
    let apiPath = "api/adoxiolegalentity/" + legalEntityId + "/sendconsentrequests";

    return this.http.post(apiPath, data, { headers: headers });
  }

  /**
   * Handle error
   * @param error
   */
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
