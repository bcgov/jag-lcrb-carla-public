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
   * Get all liquor tied house connections for a user.
   * - If `accountId` is provided, it filters results by that account.
   * - If `accountId` is not provided, it returns results for the current logged in user.
   *
   * @param {string} [accountId] An optional account ID to filter results by
   * @return {*}  {Observable<TiedHouseConnection[]>}
   */
  GetAllLiquorTiedHouseConnectionsForUser(accountId?: string): Observable<TiedHouseConnection[]> {
    const apiPath = `api/tiedhouseconnections/user/liquor/${accountId ?? ''}`;
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
   * Get the count of existing liquor tied house connections for a user.
   * - If `accountId` is provided, it filters results by that account.
   * - If `accountId` is not provided, it returns results for the current logged in user.
   *
   * @param {string} [accountId] An optional account ID to filter results by
   * @return {*}  {Observable<number>} The count of existing tied house connections.
   */
  GetExistingLiquorTiedHouseConnectionsCountForUser(accountId?: string): Observable<number> {
    const apiPath = `api/tiedhouseconnections/user/liquor/existing/count/${accountId ?? ''}`;
    return this.http.get<number>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Get all liquor tied house connections for a specific application.
   *
   * @param {string} applicationId
   * @return {*}  {Observable<TiedHouseConnection[]>}
   */
  GetAllLiquorTiedHouseConnectionsForApplication(applicationId: string): Observable<TiedHouseConnection[]> {
    const apiPath = `api/tiedhouseconnections/liquor/application/${applicationId ?? ''}`;
    return this.http.get<TiedHouseConnection[]>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Create or Update a liquor tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection
   * @param {string} applicationId
   * @return {*}
   */
  createLiquorTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, applicationId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/liquor/application/${applicationId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create or Update a liquor tied house connection for a user (account).
   *
   * Business Rules - This endpoint should only be called if:
   * - The user does not have any existing tied house connections of type `Liquor`.
   * - The user does not have any approved applications, of any type.
   *
   * @param {TiedHouseConnection} tiedHouseConnection
   * @param {string} accountId
   * @return {*}
   */
  createLiquorTiedHouseConnectionForUser(tiedHouseConnection: TiedHouseConnection, accountId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/liquor/user/${accountId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Creates or updates a cannabis tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection Optional tied house connection data used to create or update the
   * record.
   * @param {string} accountId The ID of the account to associate with the tied house connection.
   * @return {*}
   */
  upsertCannabisTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, accountId: string) {
    return this.http
      .post<TiedHouseConnection>(`api/tiedhouseconnections/cannabis/${accountId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  /**
   * Update an existing cannabis tied house connection.
   *
   * @param {TiedHouseConnection} tiedHouseConnection The tied house connection data used to update the existing record.
   * @param {string} tiedHouseConnectionId The ID of the tied house connection to update.
   * @return {*}
   */
  updateCannabisTiedHouseConnection(tiedHouseConnection: TiedHouseConnection, tiedHouseConnectionId: string) {
    return this.http
      .put<TiedHouseConnection>(`api/tiedhouseconnections/cannabis/${tiedHouseConnectionId}`, tiedHouseConnection, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
