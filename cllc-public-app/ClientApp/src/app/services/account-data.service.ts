import { Injectable } from '@angular/core';

import { DynamicsAccount } from '../models/dynamics-account.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ProfileValidation } from '../models/profile-validation.model';
import { Observable } from 'rxjs';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';
import { TiedHouseConnection } from '@models/tied-house-connection.model';

@Injectable()
export class AccountDataService extends DataService {

  apiPath = 'api/account/';

  constructor(private http: HttpClient) {
    super();
  }

  public getAccount(accountId: string): Observable<DynamicsAccount> {
    return this.http.get<DynamicsAccount>(this.apiPath + accountId, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public getCurrentAccount() {
    return this.http.get<DynamicsAccount>(this.apiPath + 'current', { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public getBusinessProfile(accountId: string) {
    return this.http.get<ProfileValidation[]>(`${this.apiPath}business-profile/${accountId}`, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public getBCeID() {
    return this.http.get(this.apiPath + 'bceid', { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public updateAccount(accountModel: DynamicsAccount) {
    return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public createTiedHouseConnection(tiedHouse: TiedHouseConnection, accountId: string) {
    return this.http.post(this.apiPath + accountId + '/tiedhouseconnection', tiedHouse, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public deleteAccount(accountModel: DynamicsAccount) {
    return this.http.post(this.apiPath + accountModel.id + '/delete', accountModel, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public deleteCurrentAccount() {
    return this.http.post(this.apiPath + 'delete/current', {}, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

}
