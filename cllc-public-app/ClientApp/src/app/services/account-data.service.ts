import { Injectable } from '@angular/core';

import { DynamicsAccount } from '../models/dynamics-account.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ProfileValidation } from '../models/profile-validation.model';
import { Observable } from 'rxjs';

@Injectable()
export class AccountDataService {

  apiPath = 'api/account/';
  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });


  constructor(private http: HttpClient) { }

  public getAccount(accountId: string): Observable<DynamicsAccount>{
    return this.http.get<DynamicsAccount>(this.apiPath + accountId, { headers: this.headers });
  }

  public getCurrentAccount() {
    return this.http.get<DynamicsAccount>(this.apiPath + 'current', { headers: this.headers });
  }

  public getBusinessProfile(accountId: string) {
    return this.http.get<ProfileValidation[]>(`${this.apiPath}business-profile/${accountId}`, { headers: this.headers });
  }

  public getBCeID() {
    return this.http.get(this.apiPath + 'bceid', { headers: this.headers });
  }

  public updateAccount(accountModel: DynamicsAccount) {
    return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: this.headers });
  }

  public deleteAccount(accountModel: DynamicsAccount) {
    return this.http.post(this.apiPath + accountModel.id + '/delete', accountModel, { headers: this.headers });
  }

}
