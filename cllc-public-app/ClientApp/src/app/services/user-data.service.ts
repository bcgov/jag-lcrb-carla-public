import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';
import { catchError, retry } from 'rxjs/operators';

import { User } from '../models/user.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class UserDataService {
  constructor(private http: HttpClient) { }

  getCurrentUser() {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.http.get<User>('api/user/current', {
      headers: headers
    }).pipe(
      catchError(this.handleError)
    );
  }


  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
    return Promise.reject(errMsg);
  }
}
