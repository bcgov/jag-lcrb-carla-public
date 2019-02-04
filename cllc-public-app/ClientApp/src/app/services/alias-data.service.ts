import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Alias } from '../models/alias.model';
import { Observable ,  of } from 'rxjs';

@Injectable()
export class AliasDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getAliases(contactId: string): Observable<Alias[]> {
    const apiPath = `api/alias/by-contactid/${contactId}`;
    return this.http.get<Alias[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError('getAlias', null)));
  }

  /**
   * Create a new alias in Dynamics
   * @param data - alias data
   */
  createAlias(data: any) {
    return this.http.post<Alias>('api/alias/', data, { headers: this.headers })
      .pipe(catchError(this.handleError('createAlias', null)));
  }

  /**
   * update a  alias in Dynamics
   * @param data - alias data
   */
  updateAlias(data: any, id: string) {
    return this.http.put<Alias>(`api/alias/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError('updateAlias', null)));
  }

  /**
   * delete a  alias in Dynamics
   * @param data - alias data
   */
  deleteAlias(id: string) {
    return this.http.post<Alias>(`api/alias/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError('deleteAlias', null)));
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
