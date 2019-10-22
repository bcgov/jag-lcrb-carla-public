import { Injectable } from '@angular/core';
import { ApplicationLicenseSummary } from '../models/application-license-summary.model';
import { Application } from '../models/application.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';
import { License } from '@models/license.model';

@Injectable()
export class LicenseDataService extends DataService {

  apiPath = 'api/licenses/';

  constructor(private http: HttpClient) {
    super();
  }

  getLicenceById(licenseId: string): Observable<License> {
    const url = `${this.apiPath}${licenseId}`;
    return this.http.get<License>(url, { headers: this.headers });
  }

  initiateTransfer(licenceId: string, accountId: string) {
    const url = `${this.apiPath}initiate-transfer`;
    return this.http.post<Application>(url, {licenceId, accountId}, { headers: this.headers });
  }

  getAllCurrentLicenses(): Observable<ApplicationLicenseSummary[]> {
    return this.http.get<ApplicationLicenseSummary[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  createApplicationForActionType(licenseId: string, applicationType: string): Observable<Application> {
    const url = `${this.apiPath}${licenseId}/create-action-application/${encodeURIComponent(applicationType)}`;
    return this.http.post<Application>(url, null, { headers: this.headers });
  }

}
