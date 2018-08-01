import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { Observable } from 'rxjs/Observable';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { debounce, catchError } from 'rxjs/operators';

@Injectable()
export class ClientConfigDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  /**
   * Get client application Configuration
   */
  getConfig() {
    const apiPath = `api/appconfig`;
    return this.http.get<any>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  /**
   * Handle error
   * @param error
   */
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
