import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Shareholder } from "../models/shareholder.model";
import { AdoxioLegalEntity } from "../models/adoxio-legalentities.model";
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AdoxioLegalEntityDataService {
   constructor(private http: Http) { }

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
          adoxioLegalEntity.firstname = entry.xxfirstname
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
          legalEntitiesList.push(adoxioLegalEntity);
        });

        return legalEntitiesList;
      })
      .catch(this.handleError);
  }

  post(data: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    //console.log("===== AdoxioLegalEntityDataService.post: ", data);

    return this.http.post("api/adoxiolegalentity/", data, {
      headers: headers
    })
      .subscribe(
        res => {
          //console.log(res);
        },
        err => {
          //console.log("Error occured");
          this.handleError(err);
        }
      );
  }

   
  //getShareholders()  {
  //  let headers = new Headers();
  //  headers.append("Content-Type", "application/json");

  //  return this.http.get("api/adoxiolegalentity/position/shareholder", {
  //    headers: headers
  //  })
  //    .toPromise()
  //    .then((res: Response) => {
  //        //console.log(res);
  //        let data = res.json();
  //        let allShareholders = [];

  //        data.forEach((entry) => {
  //          let shareholder = new Shareholder();
  //          shareholder.account = entry.account;
  //          shareholder.commonnonvotingshares = entry.commonnonvotingshares;
  //          shareholder.commonvotingshares = entry.commonvotingshares;
  //          shareholder.dateofbirth = entry.dateofbirth;
  //          shareholder.firstname = entry.xxfirstname
  //          shareholder.id = entry.id;
  //          shareholder.interestpercentage = entry.interestpercentage;
  //          shareholder.isindividual = entry.isindividual;
  //          shareholder.lastname = entry.lastname;
  //          shareholder.legalentitytype = entry.legalentitytype;
  //          shareholder.middlename = entry.middlename;
  //          shareholder.name = entry.name;
  //          shareholder.otherlegalentitytype = entry.otherlegalentitytype;
  //          shareholder.position = entry.position;
  //          shareholder.preferrednonvotingshares = entry.preferrednonvotingshares;
  //          shareholder.preferredvotingshares = entry.preferredvotingshares;
  //          shareholder.relatedentities = entry.relatedentities;
  //          shareholder.sameasapplyingperson = entry.sameasapplyingperson;
  //          shareholder.shareholderType = entry.shareholderType;
  //          allShareholders.push(shareholder);
  //        });
  //        return allShareholders;
  //    })
  //    .catch(this.handleError);
  //}

  //getDirectorsAndOfficers() {
  //  let headers = new Headers();
  //  headers.append("Content-Type", "application/json");

  //  return this.http.get("api/adoxiolegalentity/position/directorofficer", {
  //    headers: headers
  //  })
  //    .toPromise()
  //    .then((res: Response) => {
  //      //console.log(res);
  //      let data = res.json();
  //      let allDirectorsAndOfficers = [];

  //      data.forEach((entry) => {
  //        let directorofficer = new AdoxioLegalEntity();
  //        directorofficer.account = entry.account;
  //        directorofficer.commonnonvotingshares = entry.commonnonvotingshares;
  //        directorofficer.commonvotingshares = entry.commonvotingshares;
  //        directorofficer.dateofbirth = entry.dateofbirth;
  //        directorofficer.firstname = entry.xxfirstname
  //        directorofficer.id = entry.id;
  //        directorofficer.interestpercentage = entry.interestpercentage;
  //        directorofficer.isindividual = entry.isindividual;
  //        directorofficer.lastname = entry.lastname;
  //        directorofficer.legalentitytype = entry.legalentitytype;
  //        directorofficer.middlename = entry.middlename;
  //        directorofficer.name = entry.name;
  //        directorofficer.otherlegalentitytype = entry.otherlegalentitytype;
  //        directorofficer.position = entry.position;
  //        directorofficer.preferrednonvotingshares = entry.preferrednonvotingshares;
  //        directorofficer.preferredvotingshares = entry.preferredvotingshares;
  //        directorofficer.relatedentities = entry.relatedentities;
  //        directorofficer.sameasapplyingperson = entry.sameasapplyingperson;
  //        directorofficer.shareholderType = entry.shareholderType;
  //        allDirectorsAndOfficers.push(directorofficer);
  //      });
  //      return allDirectorsAndOfficers;
  //    })
  //    .catch(this.handleError);
  //}

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
