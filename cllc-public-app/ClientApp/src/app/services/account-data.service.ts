import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";
import { DynamicsAccount } from "../models/dynamics-account.model";

@Injectable()
export class AccountDataService {

  apiPath = "api/account/";

  constructor(private http: Http) { }

  getAccount(accountId: string) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    //call API
    //console.log("===== AccountDataService.getAccount: ", accountId);
    return this.http.get(this.apiPath + accountId, { headers: headers });
  }

  updateAccount(accountModel: DynamicsAccount) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");

    //call API
    //console.log("===== AccountDataService.updateAccount: ", accountModel);
    return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: headers });
  }

}
