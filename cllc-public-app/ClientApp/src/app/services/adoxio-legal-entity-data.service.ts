import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import { Shareholder } from "../models/shareholder.model";
import { Observable } from 'rxjs/Observable';

@Injectable()
export class AdoxioLegalEntityDataService {
   constructor(private http: Http) { }

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

  getShareholders()  {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    return this.http.get("api/adoxiolegalentity/?shareholder=true", {
      headers: headers
    })
      .toPromise()
      .then((res: Response) => {
          //console.log(res);
          let data = res.json();
          let allShareholders = [];

          data.forEach((entry) => {
            let shareholder = new Shareholder();
            shareholder.account = entry.account;
            shareholder.commonnonvotingshares = entry.commonnonvotingshares;
            shareholder.commonvotingshares = entry.commonvotingshares;
            shareholder.dateofbirth = entry.dateofbirth;
            shareholder.firstname = entry.xxfirstname
            shareholder.id = entry.id;
            shareholder.interestpercentage = entry.interestpercentage;
            shareholder.isindividual = entry.isindividual;
            shareholder.lastname = entry.lastname;
            shareholder.legalentitytype = entry.legalentitytype;
            shareholder.middlename = entry.middlename;
            shareholder.name = entry.name;
            shareholder.otherlegalentitytype = entry.otherlegalentitytype;
            shareholder.position = entry.position;
            shareholder.preferrednonvotingshares = entry.preferrednonvotingshares;
            shareholder.preferredvotingshares = entry.preferredvotingshares;
            shareholder.relatedentities = entry.relatedentities;
            shareholder.sameasapplyingperson = entry.sameasapplyingperson;
            shareholder.shareholderType = entry.shareholderType;
            allShareholders.push(shareholder);
          });
          return allShareholders;
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
