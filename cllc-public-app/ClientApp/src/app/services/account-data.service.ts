import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { DynamicsAccount } from '../models/dynamics-account.model';

@Injectable()
export class AccountDataService {

  apiPath = 'api/account/';
  jsonHeaders: Headers = new Headers({
    'Content-Type': 'application/json'
  });

  constructor(private http: Http) { }
  
  public getAccount(accountId: string) {
    return this.http.get(this.apiPath + accountId, { headers: this.jsonHeaders });
  }

  public getCurrentAccount() {
    return this.http.get(this.apiPath + 'current', { headers: this.jsonHeaders });
  }

  public getBusinessProfile(accountId: string) {
    return this.http.get(`${this.apiPath}business-profile/${accountId}`, { headers: this.jsonHeaders });
  }

  public getBCeID() {
    return this.http.get(this.apiPath + 'bceid', { headers: this.jsonHeaders });
  }

  public updateAccount(accountModel: DynamicsAccount) {
    return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: this.jsonHeaders });
  }

}
