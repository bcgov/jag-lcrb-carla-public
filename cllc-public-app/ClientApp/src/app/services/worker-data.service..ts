import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Worker } from '../models/worker.model';
import { Observable, of } from 'rxjs';
import { DataService } from './data.service';

@Injectable()
export class WorkerDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param accountId
   */
  getWorkerByContactId(accountId: string): Observable<Worker[]> {
    const apiPath = `api/worker/contact/${accountId}`;
    return this.http.get<Worker[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  /**
   * Get legal entities from Dynamics filtered by position
   * @param id
   */
  getWorker(id: string): Observable<Worker> {
    const apiPath = `api/worker/${id}`;
    return this.http.get<Worker>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a new worker in Dynamics
   * @param data - worker data
   */
  createWorker(data: any) {
    return this.http.post<Worker>('api/worker/', data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a  worker in Dynamics
   * @param data - worker data
   */
  updateWorker(data: any, id: string) {
    return this.http.put<Worker>(`api/worker/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * delete a  worker in Dynamics
   * @param data - worker data
   */
  deleteWorker(id: string) {
    return this.http.post<Worker>(`api/worker/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
