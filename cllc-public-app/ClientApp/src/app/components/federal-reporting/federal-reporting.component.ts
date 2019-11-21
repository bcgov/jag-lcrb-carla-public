import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Subscription, forkJoin } from 'rxjs';
import { MonthlyReport, monthlyReportStatus } from '@models/monthly-report.model';
import { MonthlyReportDataService } from '@services/monthly-report.service';

@Component({
  selector: 'app-federal-reporting',
  templateUrl: './federal-reporting.component.html',
  styleUrls: ['./federal-reporting.component.scss']
})
export class FederalReportingComponent implements OnInit {
  monthlyReportStatusEnum = monthlyReportStatus;
  licenses: ApplicationLicenseSummary[] = [];
  monthlyReports: MonthlyReport[] = [];
  shownMonthlyReports: MonthlyReport[] = [];
  selectedMonthlyReport: MonthlyReport = null;
  busy: Subscription;
  reportIsDisabled = false;
  reportIsClosed = false;

  metaForm = this.fb.group({
    licence: ['', [Validators.required]],
    period: ['', [Validators.required]]
  });

  reportForm = this.fb.group({
    monthlyReportId: ['', [Validators.required]],
    licenseId: ['', [Validators.required]],
    licenseNumber: ['', [Validators.required]],
    establishmentName: ['', [Validators.required]],
    city: ['', []],
    postalCode: ['', []],
    reportingPeriodYear: ['', [Validators.required]],
    reportingPeriodMonth: ['', [Validators.required]],
    statusCode: ['', [Validators.required]],
    employeesManagement: ['', [Validators.required]],
    employeesAdministrative: ['', [Validators.required]],
    employeesSales: ['', [Validators.required]],
    employeesProduction: ['', [Validators.required]],
    employeesOther: ['', [Validators.required]]
  });

  productForms: FormGroup[];

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
    if (event.target.value === '') {
      this.shownMonthlyReports = [];
      return;
    }
    this.shownMonthlyReports = this.monthlyReports.filter((rep) => rep.licenseId === event.target.value);
    this.busy = forkJoin(
      this.monthlyReportDataService.getMonthlyReportsByLicence(event.target.value)
    )
    .subscribe(([monthlyReports]) => {
      this.monthlyReports = monthlyReports;
    });
  }

  save(submit = false) {
    // main report fields
    const statusCode = submit ? this.monthlyReportStatusEnum.Submitted : this.monthlyReportStatusEnum.Draft;
    const updateRequest = {
      ...this.reportForm.value,
      inventorySalesReports: [],
      statusCode: statusCode
    };

    // add product fields
    let invalidProduct = false;
    this.productForms.forEach((f) => {
      if (!f.valid) {
        invalidProduct = true;
      }
      updateRequest.inventorySalesReports.push({...f.value});
    });
    if ((!this.reportForm.valid || invalidProduct) && !this.reportIsDisabled) {
      // TODO display validation errors
      console.log("invalid report or product form");
      return false;
    }

    this.busy = forkJoin(
      this.monthlyReportDataService.updateMonthlyReport(updateRequest)
    )
    .subscribe(([report]) => {
      const index = this.monthlyReports.findIndex(rep => rep.monthlyReportId === report.monthlyReportId);
      this.monthlyReports[index] = report;
      this.selectedMonthlyReport = report;
      this.handleMonthlyReportChanged();
    });
  }

  periodChanged(event) {
    this.productForms = [];
    this.selectedMonthlyReport = this.monthlyReports.find(rep => rep.monthlyReportId === event.target.value);

    if (this.selectedMonthlyReport === undefined) {
      return;
    }
    this.handleMonthlyReportChanged();
  }

  handleMonthlyReportChanged() {
    // Update product forms
    this.productForms = this.selectedMonthlyReport.inventorySalesReports.map((report) => {
      return this.fb.group({
        inventoryReportId: [report.inventoryReportId, []],
        product: [report.product, []],
        openingInventory: [report.openingInventory, []],
        domesticAdditions: [report.domesticAdditions, []],
        returnsAdditions: [report.returnsAdditions, []],
        otherAdditions: [report.otherAdditions, []],
        domesticReductions: [report.domesticReductions, []],
        returnsReductions: [report.returnsReductions, []],
        destroyedReductions: [report.destroyedReductions, []],
        lostReductions: [report.lostReductions, []],
        otherReductions: [report.otherReductions, []],
        closingNumber: [report.closingNumber, []],
        closingValue: [report.closingValue, []],
        closingWeight: [report.closingWeight, []],
        totalSeeds: [report.totalSeeds, []],
        totalSalesToConsumerQty: [report.totalSalesToConsumerQty, []],
        totalSalesToConsumerValue: [report.totalSalesToConsumerValue, []],
        totalSalesToRetailerQty: [report.totalSalesToRetailerQty, []],
        totalSalesToRetailerValue: [report.totalSalesToRetailerValue, []],
      });
    });

    switch (this.selectedMonthlyReport.statusCode) {
      case monthlyReportStatus.Closed:
        this.reportIsDisabled = true;
        this.reportIsClosed = true;
        this.reportForm.disable();
        this.productForms.forEach((report) => { report.disable(); });
        break;
      case monthlyReportStatus.Submitted:
        this.reportIsDisabled = true;
        this.reportIsClosed = false;
        this.reportForm.disable();
        this.productForms.forEach((report) => { report.disable(); });
        break;
      default:
          this.reportIsDisabled = false;
          this.reportIsClosed = false;
          this.reportForm.enable();
          this.productForms.forEach((report) => { report.enable(); });
        break;
    }

    // Update monthly report form
    this.reportForm.patchValue({
      ...this.selectedMonthlyReport
    });
  }
}
