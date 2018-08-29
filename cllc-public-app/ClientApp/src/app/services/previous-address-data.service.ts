import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { AdoxioPreviousAddress } from '../models/adoxio-previous-address.model';
import { Observable } from 'rxjs';
import { of } from 'rxjs/observable/of';


@Injectable()
export class PreviousAddressDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getPreviousAdderess(accountId: string) {
    const apiPath = `api/previous-address/${accountId}`;
    return this.http.get<AdoxioPreviousAddress>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError("getPreviousAddress", null)));
  }

  /**
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createPreviousAdderess(data: any) {
    return this.http.post<AdoxioPreviousAddress>('api/previous-address/', data, { headers: this.headers })
      .pipe(catchError(this.handleError("createPreviousAddress", null)));
  }

  /**
   * update a  legal entity in Dynamics
   * @param data - legal entity data
   */
  updatePreviousAdderess(data: any, id: string) {
    return this.http.put<AdoxioPreviousAddress>(`api/previous-address/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError("updatePreviousAddress", null)));
  }

  /**
   * delete a  legal entity in Dynamics
   * @param data - legal entity data
   */
  deletePreviousAdderess(id: string) {
    return this.http.post<AdoxioPreviousAddress>(`api/previous-address/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError("deletePreviousAddress", null)));
  }

  /**
   * Handle error
   * @param error
   */

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(`${operation} failed: ${error.message}`); // log to console instead


      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
