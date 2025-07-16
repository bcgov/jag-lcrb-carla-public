import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { catchError } from "rxjs/operators";
import { DataService } from "./data.service";
import { Observable } from "rxjs";

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
   * Create a new legal entity in Dynamics
   * @param data - legal entity data
   */
  createTiedHouse(data: any) {
    return this.http.post<TiedHouseConnection>("api/tiedhouseconnections/", data, { headers: this.headers })
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
