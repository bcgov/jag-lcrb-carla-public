import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

import { catchError, retry, map } from 'rxjs/operators';

import { User } from '../models/user.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DataService } from './data.service';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { SetCurrentUserAction } from '@app/app-state/actions/current-user.action';
import { Observable, of } from 'rxjs';

@Injectable()
export class UserDataService extends DataService {
  constructor(private http: HttpClient,
    private store: Store<AppState>) {
    super();
  }

  getCurrentUser() {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.http.get<User>('api/user/current', {
      headers: headers
    }).pipe(catchError(this.handleError));
  }

  public loadUserToStore() {
    return this.getCurrentUser()
    // handle the error before converting to a promise
      .pipe(catchError(e => of(<User>null)))
      .toPromise().then(user => {
        this.store.dispatch(new SetCurrentUserAction({ ...user }));
      }).catch(e => { });
  }

}
