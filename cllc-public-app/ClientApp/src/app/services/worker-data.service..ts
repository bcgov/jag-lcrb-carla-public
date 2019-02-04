import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Worker } from '../models/worker.model';
import { Observable ,  of } from 'rxjs';

@Injectable()
export class WorkerDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param accountId
   */
  getWorkerByContactId(accountId: string): Observable<Worker[]> {
    const apiPath = `api/worker/contact/${accountId}`;
    return this.http.get<Worker[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError('getWorker', null)));
  }
  /**
   * Get legal entities from Dynamics filtered by position
   * @param id
   */
  getWorker(id: string): Observable<Worker> {
    const apiPath = `api/worker/${id}`;
    return this.http.get<Worker>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError('getWorker', null)));
  }

  /**
   * Create a new worker in Dynamics
   * @param data - worker data
   */
  createWorker(data: any) {
    return this.http.post<Worker>('api/worker/', data, { headers: this.headers })
      .pipe(catchError(this.handleError('createWorker', null)));
  }

  /**
   * update a  worker in Dynamics
   * @param data - worker data
   */
  updateWorker(data: any, id: string) {
    return this.http.put<Worker>(`api/worker/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError('updateWorker', null)));
  }

  /**
   * delete a  worker in Dynamics
   * @param data - worker data
   */
  deleteWorker(id: string) {
    return this.http.post<Worker>(`api/worker/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError('deleteWorker', null)));
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
