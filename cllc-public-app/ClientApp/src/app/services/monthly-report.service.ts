import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';
import { MonthlyReport } from '@appmodels/monthly-report.model';

@Injectable()
export class MonthlyReportDataService extends DataService {

  apiPath = 'api/monthlyreports/';

  constructor(private http: HttpClient) {
    super();
  }

  getMonthlyReportById(monthlyReportId: string): Observable<MonthlyReport> {
    const url = `${this.apiPath}${monthlyReportId}`;
    return this.http.get<MonthlyReport>(url, { headers: this.headers });
  }

  getAllCurrentMonthlyReports(): Observable<MonthlyReport[]> {
    return this.http.get<MonthlyReport[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

}
