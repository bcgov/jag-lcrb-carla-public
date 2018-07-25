import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TiedHouseConnection } from '../models/tied-house-connection.model';
import { catchError, retry } from 'rxjs/operators';

@Injectable()
export class TiedHouseConnectionsDataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) { }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getTiedHouse(accountId: string) {
    const apiPath = `api/tiedhouseconnections/${accountId}`;
    return this.http.get<TiedHouseConnection>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


  /**
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createTiedHouse(data: any) {
    return this.http.post<TiedHouseConnection>('api/tiedhouseconnections/', data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a  legal entity in Dynamics
   * @param data - legal entity data
   */
  updateTiedHouse(data: any, id: string) {
    return this.http.put<TiedHouseConnection>(`api/tiedhouseconnections/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * delete a  legal entity in Dynamics
   * @param data - legal entity data
   */
  deleteTiedHouse(id: string) {
    return this.http.post<TiedHouseConnection>(`api/tiedhouseconnections/${id}/delete`, {}, { headers: this.headers })
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
