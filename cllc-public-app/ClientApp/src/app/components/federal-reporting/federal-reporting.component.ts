import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Subscription, forkJoin } from 'rxjs';
import { MonthlyReport } from '@models/monthly-report.model';
import { MonthlyReportDataService } from '@services/monthly-report.service';

@Component({
  selector: 'app-federal-reporting',
  templateUrl: './federal-reporting.component.html',
  styleUrls: ['./federal-reporting.component.scss']
})
export class FederalReportingComponent implements OnInit {
  licenses: ApplicationLicenseSummary[] = [];
  monthlyReports: MonthlyReport[] = [];
  shownMonthlyReports: MonthlyReport[] = [];
  selectedMonthlyReport: MonthlyReport = null;
  busy: Subscription;

  reportForm = this.fb.group({
    licence: ['', [Validators.required]],
    period: ['', [Validators.required]],
    employeesManagement: ['', [Validators.required]],
    employeesAdministrative: ['', [Validators.required]],
    employeesSales: ['', [Validators.required]],
    employeesProduction: ['', [Validators.required]],
    employeesOther: ['', [Validators.required]]
  });

  constructor(
    public fb: FormBuilder,
    private licenceDataService: LicenseDataService,
    private monthlyReportDataService: MonthlyReportDataService
  ) { }

  ngOnInit() {
    this.busy = forkJoin(
      this.licenceDataService.getAllCurrentLicenses()
    )
    .subscribe(([licenses]) => {
      this.licenses = licenses;
    });

  }

  licenceChanged(event) {
    this.shownMonthlyReports = this.monthlyReports.filter((rep) => rep.licenseId === event.target.value);

    this.busy = forkJoin(
      this.monthlyReportDataService.getMonthlyReportsByLicence(event.target.value)
    )
    .subscribe(([monthlyReports]) => {
      this.monthlyReports = monthlyReports;
    });
  }

  periodChanged(event) {
    this.selectedMonthlyReport = this.monthlyReports.find(rep => rep.monthlyReportId === event.target.value);
    this.reportForm.patchValue({
      'employeesManagement': this.selectedMonthlyReport.employeesManagement,
      'employeesAdministrative': this.selectedMonthlyReport.employeesAdministrative,
      'employeesSales': this.selectedMonthlyReport.employeesSales,
      'employeesProduction': this.selectedMonthlyReport.employeesProduction,
      'employeesOther': this.selectedMonthlyReport.employeesOther
    });
  }
}
