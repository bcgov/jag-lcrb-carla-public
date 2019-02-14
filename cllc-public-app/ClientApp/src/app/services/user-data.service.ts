import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

import { catchError, retry } from 'rxjs/operators';

import { User } from '../models/user.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DataService } from './data.service';

@Injectable()
export class UserDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getCurrentUser() {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json');

    return this.http.get<User>('api/user/current', {
      headers: headers
    }).pipe(catchError(this.handleError));
  }

}
