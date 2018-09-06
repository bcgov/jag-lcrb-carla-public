import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { PreviousAddress } from '../models/previous-address.model';
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
  getPreviousAdderesses(contactId: string): Observable<PreviousAddress[]> {
    const apiPath = `api/previousaddress/by-contactid/${contactId}`;
    return this.http.get<PreviousAddress[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError('getPreviousAddress', null)));
  }

  /**
   * Create a new address in Dynamics
   * @param data - address data
   */
  createPreviousAdderess(data: any) {
    return this.http.post<PreviousAddress>('api/previousaddress/', data, { headers: this.headers })
      .pipe(catchError(this.handleError('createPreviousAddress', null)));
  }

  /**
   * update a  address in Dynamics
   * @param data - address data
   */
  updatePreviousAdderess(data: any, id: string) {
    return this.http.put<PreviousAddress>(`api/previousaddress/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError('updatePreviousAddress', null)));
  }

  /**
   * delete a  address in Dynamics
   * @param data - address data
   */
  deletePreviousAdderess(id: string) {
    return this.http.post<PreviousAddress>(`api/previousaddress/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError('deletePreviousAddress', null)));
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
