import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Subscription, forkJoin } from 'rxjs';
import { MonthlyReport, monthlyReportStatus } from '@models/monthly-report.model';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { ActivatedRoute } from '@angular/router';

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
  selectedLicence: ApplicationLicenseSummary = null;
  visibleInventoryReports = [];
  busy: Subscription;
  reportIsDisabled = true;
  reportIsClosed = true;

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
    employeesManagement: ['', [Validators.min(0), Validators.max(10000), Validators.pattern('^[0-9]*$'), Validators.required]],
    employeesAdministrative: ['', [Validators.min(0), Validators.max(10000), Validators.pattern('^[0-9]*$'), Validators.required]],
    employeesSales: ['', [Validators.min(0), Validators.max(10000), Validators.pattern('^[0-9]*$'), Validators.required]],
    employeesProduction: ['', [Validators.min(0), Validators.max(10000), Validators.pattern('^[0-9]*$'), Validators.required]],
    employeesOther: ['', [Validators.min(0), Validators.max(10000), Validators.pattern('^[0-9]*$'), Validators.required]]
  });

  productForms: FormGroup[];
  defaultValue: any;

  constructor(
    public fb: FormBuilder,
    private licenceDataService: LicenseDataService,
    private route: ActivatedRoute,
    private monthlyReportDataService: MonthlyReportDataService
  ) {
    this.defaultValue = window.history.state.data;
    debugger;
  }

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
      this.selectedLicence = null;
      return;
    }
    this.selectedLicence = this.licenses.find(l => l.licenseId === event.target.value);

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
      updateRequest.inventorySalesReports.push({ ...f.value });
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

    this.selectedMonthlyReport.inventorySalesReports = this.selectedMonthlyReport.inventorySalesReports.sort((r1, r2) => {
      const nameA = r1.product.toLowerCase(), nameB = r2.product.toLowerCase();
      if (nameA < nameB) {
        return -1;
      } else if (nameA > nameB) {
        return 1;
      }
      return 0;
    });
    this.handleMonthlyReportChanged();
  }

  handleMonthlyReportChanged() {
    // Update product forms
    this.visibleInventoryReports = [];
    this.productForms = this.selectedMonthlyReport.inventorySalesReports.map((report) => {
      if ((report.openingInventory !== null && report.openingInventory !== 0) ||
          (report.domesticAdditions !== null && report.domesticAdditions !== 0) ||
          (report.returnsAdditions !== null && report.returnsAdditions !== 0) ||
          (report.otherAdditions !== null && report.otherAdditions !== 0) ||
          (report.domesticReductions !== null && report.domesticReductions !== 0) ||
          (report.returnsReductions !== null && report.returnsReductions !== 0) ||
          (report.destroyedReductions !== null && report.destroyedReductions !== 0) ||
          (report.lostReductions !== null && report.lostReductions !== 0) ||
          (report.otherReductions !== null && report.otherReductions !== 0) ||
          (report.closingNumber !== null && report.closingNumber !== 0) ||
          (report.closingValue !== null && report.closingValue !== 0) ||
          (report.closingWeight !== null && report.closingWeight !== 0) ||
          (report.totalSeeds !== null && report.totalSeeds !== 0) ||
          (report.totalSalesToConsumerQty !== null && report.totalSalesToConsumerQty !== 0) ||
          (report.totalSalesToConsumerValue !== null && report.totalSalesToConsumerValue !== 0) ||
          (report.totalSalesToRetailerQty !== null && report.totalSalesToRetailerQty !== 0) ||
          (report.totalSalesToRetailerValue !== null && report.totalSalesToRetailerValue !== 0)) {
            this.visibleInventoryReports.push(report.inventoryReportId);
      }
      return this.fb.group({
        inventoryReportId: [report.inventoryReportId, []],
        product: [report.product, []],
        openingInventory: [report.openingInventory, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        domesticAdditions: [report.domesticAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        returnsAdditions: [report.returnsAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        otherAdditions: [report.otherAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        domesticReductions: [report.domesticReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        returnsReductions: [report.returnsReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        destroyedReductions: [report.destroyedReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        lostReductions: [report.lostReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        otherReductions: [report.otherReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        closingNumber: [report.closingNumber, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        closingValue: [report.closingValue, [Validators.min(0), Validators.max(1000000000), Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$')]],
        closingWeight: [report.closingWeight, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]+(\.[0-9]{1,3})?$')]],
        totalSeeds: [report.totalSeeds, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        // tslint:disable-next-line: max-line-length
        totalSalesToConsumerQty: [report.totalSalesToConsumerQty, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        totalSalesToConsumerValue: [report.totalSalesToConsumerValue, [Validators.min(0), Validators.max(1000000000), Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$')]],
        totalSalesToRetailerQty: [report.totalSalesToRetailerQty, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
        totalSalesToRetailerValue: [report.totalSalesToRetailerValue, [Validators.min(0), Validators.max(1000000000), Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$')]],
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

  toggleProductVisibility(id: string) {
    if (this.visibleInventoryReports.indexOf(id) > -1) {
      this.visibleInventoryReports.splice(this.visibleInventoryReports.indexOf(id), 1);
      this.clearProductForm(id);
    } else {
      this.visibleInventoryReports.push(id);
    }
  }

  findProductForm(id: string): FormGroup {
    return this.productForms.find(f => f.value.inventoryReportId === id);
  }

  clearProductForm(id: string) {
    const index = this.productForms.findIndex(f => f.value.inventoryReportId === id);
    this.productForms[index].reset({
      inventoryReportId: this.productForms[index].value.inventoryReportId,
      product: this.productForms[index].value.product,
      openingInventory: 0,
      domesticAdditions: 0,
      returnsAdditions: 0,
      otherAdditions: 0,
      domesticReductions: 0,
      returnsReductions: 0,
      destroyedReductions: 0,
      lostReductions: 0,
      otherReductions: 0,
      closingNumber: 0,
      closingValue: 0,
      closingWeight: 0,
      totalSeeds: 0,
      totalSalesToConsumerQty: 0,
      totalSalesToConsumerValue: 0,
      totalSalesToRetailerQty: 0,
      totalSalesToRetailerValue: 0,
    });
  }

  isFieldInvalid(field: string) {
    return !this.reportForm.get(field).valid && (this.reportForm.get(field).dirty || this.reportForm.get(field).touched);
  }
}
