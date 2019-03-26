import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { AdoxioLicense } from '../models/adoxio-license.model';
import { AdoxioApplication } from '../models/adoxio-application.model';
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

  getAdoxioLicenses(): Observable<AdoxioLicense[]> {
    return this.http.get<AdoxioLicense[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  createChangeOfLocationApplication(licenseId: string): Observable<AdoxioApplication> {
    return this.http.post<AdoxioApplication>(this.apiPath + licenseId + "/create-change-of-location", null , { headers: this.headers });
  }

}
