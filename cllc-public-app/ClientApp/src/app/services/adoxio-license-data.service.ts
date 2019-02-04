import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { AdoxioLicense } from '../models/adoxio-license.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class AdoxioLicenseDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  getAdoxioLicenses(): Observable<AdoxioLicense[]> {
    return this.http.get<AdoxioLicense[]>('api/adoxiolicense/current', {
      headers: this.headers
    });
  }

}
