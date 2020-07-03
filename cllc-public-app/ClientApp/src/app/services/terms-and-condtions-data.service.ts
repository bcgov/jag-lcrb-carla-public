import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';
import { TermsAndConditions } from '@models/terms-and-conditions.model';

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
    return this.http.get<TermsAndConditions[]>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
