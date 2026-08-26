import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { TermsAndConditions } from '@models/terms-and-conditions.model';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class TermsAndConditionsDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get terms and conditions from Dynamics filtered by licence
   * @param positionType
   */
  getTermsAndCondtions(licenceId: string) {
    const apiPath = `api/termsandconditions/${licenceId}`;
    return this.http.get<TermsAndConditions[]>(apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }

  /**
   * Get terms and conditions from Dynamics for multiple licences in a single request
   */
  getTermsAndCondtionsBatch(licenceIds: string[]) {
    const apiPath = `api/termsandconditions/batch`;
    return this.http
      .post<TermsAndConditions[]>(apiPath, licenceIds, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
