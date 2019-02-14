import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Alias } from '../models/alias.model';
import { Observable ,  of } from 'rxjs';
import { DataService } from './data.service';

@Injectable()
export class AliasDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
   }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getAliases(contactId: string): Observable<Alias[]> {
    const apiPath = `api/alias/by-contactid/${contactId}`;
    return this.http.get<Alias[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a new alias in Dynamics
   * @param data - alias data
   */
  createAlias(data: any) {
    return this.http.post<Alias>('api/alias/', data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a  alias in Dynamics
   * @param data - alias data
   */
  updateAlias(data: any, id: string) {
    return this.http.put<Alias>(`api/alias/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * delete a  alias in Dynamics
   * @param data - alias data
   */
  deleteAlias(id: string) {
    return this.http.post<Alias>(`api/alias/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


}
