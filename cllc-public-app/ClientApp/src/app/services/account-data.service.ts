import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";
import { DynamicsAccount } from "../models/dynamics-account.model";

@Injectable()
export class AccountDataService {

  constructor(private http: Http) { }

  getAccount(accountId: string) {

    let apiPath = "api/account/" + accountId;
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    // call API
    return this.http.get(apiPath, { headers: headers });

      //.toPromise()
      //.then((res: Response) => {
      //  //console.log(res);
      //  let data = res.json();
      //  let legalEntitiesList = [];

      //  data.forEach((entry) => {
      //    let adoxioLegalEntity = new AdoxioLegalEntity();
      //    adoxioLegalEntity.account = entry.account;
      //    legalEntitiesList.push(adoxioLegalEntity);
      //  });

      //  return legalEntitiesList;
      //})
  }

}
