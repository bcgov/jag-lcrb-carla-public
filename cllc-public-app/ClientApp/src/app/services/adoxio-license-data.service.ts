import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { License } from '../models/license.model';
import { Application } from '../models/application.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class AdoxioLicenseDataService extends DataService {

  apiPath = 'api/adoxiolicense/';

  constructor(private http: HttpClient) {
    super();
  }

  getAdoxioLicenses(): Observable<License[]> {
    return this.http.get<License[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  createChangeOfLocationApplication(licenseId: string): Observable<Application> {
    return this.http.post<Application>(this.apiPath + licenseId + "/create-change-of-location", null , { headers: this.headers });
  }

}
