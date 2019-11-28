import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators, FormGroup, ValidatorFn } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Subscription, forkJoin } from 'rxjs';
import { MonthlyReport, monthlyReportStatus } from '@models/monthly-report.model';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material';
import { ModalComponent } from '@shared/components/modal/modal.component';

const ClosingInventoryValidator: ValidatorFn = (fg: FormGroup) => {
  const additions = [
    +fg.get('openingInventory').value,
    +fg.get('domesticAdditions').value,
    +fg.get('returnsAdditions').value,
    +fg.get('otherAdditions').value
  ];
  const reductions = [
    +fg.get('domesticReductions').value,
    +fg.get('returnsReductions').value,
    +fg.get('destroyedReductions').value,
    +fg.get('lostReductions').value,
    +fg.get('otherReductions').value
  ];

  const total = additions.reduce((n, curr) => n + curr, 0) - reductions.reduce((n, curr) => n + curr, 0);
  if (total !== +fg.get('closingNumber').value) {
    fg.get('closingNumber').setErrors({ closingNumberMismatch: true });
  } else {
    fg.get('closingNumber').setErrors(null);
  }
  return null;
};

const SalesValidator: ValidatorFn = (fg: FormGroup) => {
  if (+fg.get('totalSalesToConsumerQty').value !== +fg.get('domesticReductions').value) {
    fg.get('totalSalesToConsumerQty').setErrors({ salesMismatch: true });
  } else {
    fg.get('totalSalesToConsumerQty').setErrors(null);
  }
  return null;
};

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
  selectedMonthlyReportIndex = null;
  selectedLicenceIndex = null;
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
    private monthlyReportDataService: MonthlyReportDataService,
    public dialog: MatDialog
  ) {
    this.defaultValue = window.history.state.data;
  }

  ngOnInit() {
    this.busy = forkJoin(
      this.licenceDataService.getAllCurrentLicenses()
    )
      .subscribe(([licenses]) => {
        this.licenses = licenses;
        this.changeLicence(this.licenses[0].licenseId);
      });
  }

  handleLicenceChanged(event) {
    if (event.target.value === '') {
      this.shownMonthlyReports = [];
      this.selectedLicenceIndex = null;
      return;
    }
    this.changeLicence(event.target.value);
  }

  changeLicence(licenceId) {
    this.selectedLicenceIndex = this.licenses.findIndex(l => l.licenseId === licenceId);

    this.busy = forkJoin(
      this.monthlyReportDataService.getMonthlyReportsByLicence(licenceId)
    )
      .subscribe(([monthlyReports]) => {
        this.monthlyReports = monthlyReports;
        this.shownMonthlyReports = this.monthlyReports.filter((rep) => rep.licenseId === licenceId);
        if (this.shownMonthlyReports.length > 0) {
          this.selectMonthlyReport(0);
        }
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
    const invalidProduct = this.hasInvalidProductForm();
    this.productForms.forEach((f) => {
      updateRequest.inventorySalesReports.push({ ...f.value });
    });
    if ((!this.reportForm.valid || invalidProduct) && !this.reportIsDisabled) {
      return false;
    }

    this.busy = forkJoin(
      this.monthlyReportDataService.updateMonthlyReport(updateRequest)
    )
      .subscribe(([report]) => {
        const index = this.monthlyReports.findIndex(rep => rep.monthlyReportId === report.monthlyReportId);
        this.monthlyReports[index] = report;
        this.handleMonthlyReportChanged();
      });
  }

  handleMonthlyReportTabChanged(event) {
    this.selectMonthlyReport(event.index);
  }

  selectMonthlyReport(index) {
    this.productForms = [];
    this.selectedMonthlyReportIndex = index;
    if (this.selectedMonthlyReportIndex === undefined) {
      return;
    }

    this.handleMonthlyReportChanged();
  }

  handleMonthlyReportChanged() {
    // Update product forms
    this.visibleInventoryReports = [];
    this.productForms = this.monthlyReports[this.selectedMonthlyReportIndex].inventorySalesReports.map((report) => {
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
      return this.createProductForm(report);
    });

    switch (this.monthlyReports[this.selectedMonthlyReportIndex].statusCode) {
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

    // Sort inventory reports
    this.productForms = this.productForms.sort((r1, r2) => {
      const nameA = r1.value.product.toLowerCase(), nameB = r2.value.product.toLowerCase();
      if (nameA < nameB) {
        return -1;
      } else if (nameA > nameB) {
        return 1;
      }
      return 0;
    });

    // Update monthly report form
    this.reportForm.patchValue({
      ...this.monthlyReports[this.selectedMonthlyReportIndex]
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

  isProductSelected(id: string) {
    return this.visibleInventoryReports.indexOf(id) > -1;
  }

  hasInvalidProductForm() {
    let invalidProduct = false;
    this.productForms.forEach((f) => {
      if (!f.valid) {
        console.log(f);
        invalidProduct = true;
      }
    });
    return invalidProduct;
  }

  submitApplication() {
    const completedProductForms = this.productForms.filter(f => this.visibleInventoryReports.indexOf(f.value.inventoryReportId) > -1);
    const body = completedProductForms.length > 0 ?
      'Are you sure you want to submit this report?' :
      'You have not entered any inventory information. Are you sure you want to submit this report?';

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '400px',
      height: '200px',
      data: {
        title: 'Confirm Submission',
        body: body
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ModalComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe(submitApplication => {
        if (submitApplication) {
          this.save(true);
        }
      });
  }

  getMonthName(monthNumber): string {
    const monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
      'July', 'August', 'September', 'October', 'November', 'December'
    ];
    return monthNames[Number(monthNumber) - 1];
  }

  createProductForm(report) {
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
      totalSalesToConsumerQty: [report.totalSalesToConsumerQty, [Validators.min(0), Validators.max(10000000), Validators.pattern('^[0-9]*$')]],
      totalSalesToConsumerValue: [report.totalSalesToConsumerValue, [Validators.min(0), Validators.max(1000000000), Validators.pattern('^[0-9]+(\.[0-9]{1,2})?$')]]
    }, { validators: [ClosingInventoryValidator, SalesValidator] });
  }
}
