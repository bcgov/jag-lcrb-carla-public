import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { AdoxioLicense } from '../models/adoxio-license.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class AdoxioLicenseDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

  getAdoxioLicenses(): Observable<AdoxioLicense[]> {
    return this.http.get<AdoxioLicense[]>('api/adoxiolicense/current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

}
