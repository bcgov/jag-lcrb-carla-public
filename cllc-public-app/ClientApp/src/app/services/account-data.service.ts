import { Injectable } from '@angular/core';

import { Account } from '../models/account.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ProfileValidation } from '../models/profile-validation.model';
import { Observable, forkJoin } from 'rxjs';
import { DataService } from './data.service';
import { catchError, map } from 'rxjs/operators';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { SetCurrentAccountAction } from '@app/app-state/actions/current-account.action';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';

@Injectable()
export class AccountDataService extends DataService {

  apiPath = 'api/accounts/';

  constructor(private http: HttpClient,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private store: Store<AppState>) {
    super();
  }

  public getAccount(accountId: string): Observable<Account> {
    return this.http.get<Account>(this.apiPath + accountId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  public getCurrentAccount() {
    return this.http.get<Account>(this.apiPath + 'current', { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  public loadCurrentAccountToStore(id: string) {
    return forkJoin(this.getAccount(id), this.tiedHouseService.getTiedHouse(id))
      .pipe(map(data => {
        const account = data[0];
        account.tiedHouse = data[1];
        this.store.dispatch(new SetCurrentAccountAction({ ...account }));
        return account;
      }));
}

  public getBusinessProfile(accountId: string) {
  return this.http.get<ProfileValidation[]>(`${this.apiPath}business-profile/${accountId}`, { headers: this.headers })
    .pipe(catchError(this.handleError));
}

  public getBCeID() {
  return this.http.get(this.apiPath + 'bceid', { headers: this.headers })
    .pipe(catchError(this.handleError));
}

  public updateAccount(accountModel: Account) {
  return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: this.headers })
    .pipe(catchError(this.handleError));
}

  public createTiedHouseConnection(tiedHouse: TiedHouseConnection, accountId: string) {
  return this.http.post(this.apiPath + accountId + '/tiedhouseconnection', tiedHouse, { headers: this.headers })
    .pipe(catchError(this.handleError));
}

  public deleteAccount(accountModel: Account) {
  return this.http.post(this.apiPath + accountModel.id + '/delete', accountModel, { headers: this.headers })
    .pipe(catchError(this.handleError));
}

  public deleteCurrentAccount() {
  return this.http.post(this.apiPath + 'delete/current', {}, { headers: this.headers })
    .pipe(catchError(this.handleError));
}

}
