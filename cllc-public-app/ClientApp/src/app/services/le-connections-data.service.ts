import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { Observable } from 'rxjs';
import { SecurityScreeningSummary } from '@models/security-screening-summary.model';

@Injectable()
export class LEConnectionsDataService extends DataService {

  headers: HttpHeaders = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Gets the list of security screening records
   */
  getCurrentSecurityScreeningItems(): Observable<SecurityScreeningSummary> {
    const apiPath = 'api/le-connections/current-security-summary';
    return this.http.get<SecurityScreeningSummary>(apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
