import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';
import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { PreviousAddress } from '../models/previous-address.model';
import { Observable, of } from 'rxjs';
import { DataService } from './data.service';


@Injectable()
export class PreviousAddressDataService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get legal entities from Dynamics filtered by position
   * @param positionType
   */
  getPreviousAdderesses(contactId: string): Observable<PreviousAddress[]> {
    const apiPath = `api/previousaddress/by-contactid/${contactId}`;
    return this.http.get<PreviousAddress[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Create a new address in Dynamics
   * @param data - address data
   */
  createPreviousAdderess(data: any) {
    return this.http.post<PreviousAddress>('api/previousaddress/', data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * update a  address in Dynamics
   * @param data - address data
   */
  updatePreviousAdderess(data: any, id: string) {
    return this.http.put<PreviousAddress>(`api/previousaddress/${id}`, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * delete a  address in Dynamics
   * @param data - address data
   */
  deletePreviousAddress(id: string) {
    return this.http.post<PreviousAddress>(`api/previousaddress/${id}/delete`, {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
