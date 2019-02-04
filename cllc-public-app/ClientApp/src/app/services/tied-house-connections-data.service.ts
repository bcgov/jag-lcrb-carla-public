import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { TiedHouseConnection } from '../models/tied-house-connection.model';
import { catchError, retry } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class TiedHouseConnectionsDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

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

}
