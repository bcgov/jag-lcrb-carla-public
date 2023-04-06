import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";
import { DataService } from "./data.service";
import { MonthlyReport } from "@models/monthly-report.model";

@Injectable()
export class MonthlyReportDataService extends DataService {

  apiPath = "api/monthlyreports/";

  constructor(private http: HttpClient) {
    super();
  }

  getMonthlyReportById(monthlyReportId: string): Observable<MonthlyReport> {
    const url = `${this.apiPath}${monthlyReportId}`;
    return this.http.get<MonthlyReport>(url, { headers: this.headers });
  }

  getMonthlyReportsByLicence(licenceId: string): Observable<MonthlyReport[]> {
    return this.http.get<MonthlyReport[]>(this.apiPath + "licence/" + licenceId,
        {
          headers: this.headers
        })
      .pipe(catchError(this.handleError));
  }
  getMonthlyReportByLicenceYearMonth(licenceId: string,year:string,month:string): Observable<MonthlyReport> {
    return this.http.get<MonthlyReport>(this.apiPath + "licenceYearMonth?licenceId="+licenceId+"&year="+year+"&month="+month,
      {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
  getAllCurrentMonthlyReports(expandInventoryReports: boolean): Observable<MonthlyReport[]> {
    return this.http.get<MonthlyReport[]>(this.apiPath + `current?expandInventoryReports=${expandInventoryReports}`,
        {
          headers: this.headers
        })
      .pipe(catchError(this.handleError));
  }

  updateMonthlyReport(monthlyReport: MonthlyReport) {
    return this.http.put<MonthlyReport>(
        this.apiPath + monthlyReport.monthlyReportId,
        monthlyReport,
        { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
