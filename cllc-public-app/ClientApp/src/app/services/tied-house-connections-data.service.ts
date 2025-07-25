import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class TiedHouseConnectionsDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get all tied house connections for a user.
   * - If `accountId` is provided, it filters results by that account.
   * - If `accountId` is not provided, it returns results for the current logged in user.
   *
   * @param {string} [accountId] An optional account ID to filter results by
   * @return {*}  {Observable<TiedHouseConnection[]>}
   */
  GetAllTiedHouseConnectionsForUser(accountId?: string): Observable<TiedHouseConnection[]> {
    const apiPath = `api/tiedhouseconnections/user/${accountId ?? ''}`;
    return this.http.get<TiedHouseConnection[]>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Get the cannabis Tied House Connection for a user.
   * - If `accountId` is provided, it filters results by that account.
   * - If `accountId` is not provided, it returns results for the current logged in user.
   *
   * @param {string} [accountId] An optional account ID to filter results by
   * @return {*}  {Observable<TiedHouseConnection>}
   */
  GetCannabisTiedHouseConnectionForUser(accountId?: string): Observable<TiedHouseConnection> {
    const apiPath = `api/tiedhouseconnections/user/cannabis/${accountId ?? ''}`;
    return this.http.get<TiedHouseConnection>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Get the count of existing tied house connections for a user.
   * - If `accountId` is provided, it filters results by that account.
   * - If `accountId` is not provided, it returns results for the current logged in user.
   *
   * @param {string} [accountId] An optional account ID to filter results by
   * @return {*}  {Observable<number>} The count of existing tied house connections.
   */
  GetExistingTiedHouseConnectionsCountForUser(accountId?: string): Observable<number> {
    const apiPath = `api/tiedhouseconnections/user/existing/count/${accountId ?? ''}`;
    return this.http.get<number>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Get all tied house connections for a specific application.
   *
   * @param {string} applicationId
   * @return {*}  {Observable<TiedHouseConnection[]>}
   */
  GetAllTiedHouseConnectionsForApplication(applicationId: string): Observable<TiedHouseConnection[]> {
    const apiPath = `api/tiedhouseconnections/application/${applicationId ?? ''}`;
    return this.http.get<TiedHouseConnection[]>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Create a liquor tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection
   * @param {string} applicationId
   * @return {*}
   */
  createLiquorTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, applicationId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/application/${applicationId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a new cannabis tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection
   * @param {string} accountId The ID of the account to associate with the tied house connection.
   * @return {*}
   */
  createCannabisTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, accountId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/cannabis/${accountId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Update an existing cannabis tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection
   * @param {string} tiedHouseConnectionId The ID of the tied house connection to update.
   * @return {*}
   */
  updateCannabisTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, tiedHouseConnectionId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/cannabis/${tiedHouseConnectionId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
