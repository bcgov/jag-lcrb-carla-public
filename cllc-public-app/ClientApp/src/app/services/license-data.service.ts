import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { ApplicationLicenseSummary } from '../models/application-license-summary.model';
import { Application } from '../models/application.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class LicenseDataService extends DataService {

  apiPath = 'api/licenses/';

  constructor(private http: HttpClient) {
    super();
  }

  getAllCurrentLicenses(): Observable<ApplicationLicenseSummary[]> {
    return this.http.get<ApplicationLicenseSummary[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  createApplicationForActionType(licenseId: string, applicationType: string): Observable<Application> {
    const url = `${this.apiPath}${licenseId}/create-action-application/${applicationType}`;
    return this.http.post<Application>(url, null, { headers: this.headers });
  }

}
