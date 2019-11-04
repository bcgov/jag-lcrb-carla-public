import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormArray, FormControl, FormGroup } from '@angular/forms';
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
    const updateRequest = {
      ...this.reportForm.value,
      inventorySalesReports: []
    };
    if (submit) {
      updateRequest.statusCode = this.monthlyReportStatusEnum.Submitted;
    }

    // add product fields
    let invalidProduct = false;
    this.productForms.forEach((f) => {
      if (!f.valid) {
        invalidProduct = true;
      }
      updateRequest.inventorySalesReports.push({...f.value});
    });
    if (!this.reportForm.valid || invalidProduct) {
      console.log("invalid report or product form");
      return false;
    }

    this.busy = forkJoin(
      this.monthlyReportDataService.updateMonthlyReport(updateRequest)
    )
    .subscribe(() => {
      console.log("saved!");
    });
  }

  periodChanged(event) {
    this.selectedMonthlyReport = this.monthlyReports.find(rep => rep.monthlyReportId === event.target.value);

    // Update product forms
    this.productForms = this.selectedMonthlyReport.inventorySalesReports.map((report) => {
      return new FormGroup({
        'inventoryReportId': new FormControl(report.inventoryReportId, []),
        'product': new FormControl(report.product, []),
        'openingInventory': new FormControl(report.openingInventory, []),
        'domesticAdditions': new FormControl(report.domesticAdditions, []),
        'returnsAdditions': new FormControl(report.returnsAdditions, []),
        'otherAdditions': new FormControl(report.otherAdditions, []),
        'domesticReductions': new FormControl(report.domesticReductions, []),
        'returnsReductions': new FormControl(report.returnsReductions, []),
        'destroyedReductions': new FormControl(report.destroyedReductions, []),
        'lostReductions': new FormControl(report.lostReductions, []),
        'otherReductions': new FormControl(report.otherReductions, []),
        'closingNumber': new FormControl(report.closingNumber, []),
        'closingValue': new FormControl(report.closingValue, []),
        'closingWeight': new FormControl(report.closingWeight, []),
        'totalSeeds': new FormControl(report.totalSeeds, []),
        'totalSalesToConsumerQty': new FormControl(report.totalSalesToConsumerQty, []),
        'totalSalesToConsumerValue': new FormControl(report.totalSalesToConsumerValue, []),
        'totalSalesToRetailerQty': new FormControl(report.totalSalesToRetailerQty, []),
        'totalSalesToRetailerValue': new FormControl(report.totalSalesToRetailerValue, []),
      });
    });

    // Update monthly report form
    this.reportForm.patchValue({
      ...this.selectedMonthlyReport
    });
  }
}
