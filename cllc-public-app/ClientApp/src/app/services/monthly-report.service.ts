import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';
import { MonthlyReport } from '@models/monthly-report.model';

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

  getMonthlyReportsByLicence(licenceId: string): Observable<MonthlyReport[]> {
    return this.http.get<MonthlyReport[]>(this.apiPath + 'licence/' + licenceId, {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  getAllCurrentMonthlyReports(): Observable<MonthlyReport[]> {
    return this.http.get<MonthlyReport[]>(this.apiPath + 'current', {
      headers: this.headers
    })
      .pipe(catchError(this.handleError));
  }

  updateMonthlyReport(monthlyReport: MonthlyReport) {
    return this.http.put(
      this.apiPath + monthlyReport.monthlyReportId,
      monthlyReport,
      { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
