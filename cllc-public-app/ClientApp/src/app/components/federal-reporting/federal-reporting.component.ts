import { Component, OnInit } from "@angular/core";
import { FormBuilder, Validators, FormGroup } from "@angular/forms";
import { LicenseDataService } from "@services/license-data.service";
import { ApplicationLicenseSummary } from "@models/application-license-summary.model";
import { Subscription, forkJoin, of, Observable } from "rxjs";
import { MonthlyReport, monthlyReportStatus } from "@models/monthly-report.model";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { MatDialog } from "@angular/material/dialog";
import { ModalComponent } from "@shared/components/modal/modal.component";
import { ClosingInventoryValidator, SalesValidator } from "./federal-reporting-validation";
import { switchMap } from "rxjs/operators";
import { faChevronLeft, faExclamationTriangle, faQuestionCircle, faTrash, faInfoCircle } from "@fortawesome/free-solid-svg-icons";
import { faSave } from "@fortawesome/free-regular-svg-icons";
"@fortawesome/free-solid-svg-icons";
import { InventorySalesReport } from "@models/inventory-sales-report.model";
import getMonth from "date-fns/fp/getMonth/index.js";

interface FederalReportingParams {
  licenceId: string;
  monthlyReportId: string;
}

@Component({
  selector: "app-federal-reporting",
  templateUrl: "./federal-reporting.component.html",
  styleUrls: ["./federal-reporting.component.scss"]
})
export class FederalReportingComponent implements OnInit {
  faSave = faSave;
  faChevronLeft = faChevronLeft;
  faQuestionCircle = faQuestionCircle;
  faExclamationTriangle = faExclamationTriangle;
  faTrash = faTrash;
  faInfoCircle = faInfoCircle;
  monthlyReportStatusEnum = monthlyReportStatus;
  licenses: ApplicationLicenseSummary[] = [];
  selectedLicense: ApplicationLicenseSummary;

  monthlyReports: MonthlyReport[] = [];
  selectedMonthlyReport: MonthlyReport;
  shownMonthlyReports: MonthlyReport[] = [];
  selectedMonthlyReportIndex = null;
  selectedLicenceIndex = null;
  selectedLicenseId: string;

  busy: Subscription;
  reportIsDisabled = true;
  reportIsClosed = true;
  routeParams: Observable<FederalReportingParams>;
  loadingMonthlyReports = false;

  reportYears: string[] = [];
  selectedYear: string;  
  reportMonths: string[] = [];
  selectedMonth: string;

  metaForm = this.fb.group({
    licence: ["", [Validators.required]],
    period: ["", [Validators.required]]
  });

  reportForm = this.fb.group({
    monthlyReportId: ["", [Validators.required]],
    licenseId: ["", [Validators.required]],
    licenseNumber: ["", [Validators.required]],
    establishmentName: ["", [Validators.required]],
    city: ["", []],
    postalCode: ["", []],
    reportingPeriodYear: ["", [Validators.required]],
    reportingPeriodMonth: ["", [Validators.required]],
    statusCode: ["", [Validators.required]],
    employeesManagement: [
      "", [Validators.min(0), Validators.max(10000), Validators.pattern("^[0-9]*$"), Validators.required]
    ],
    employeesAdministrative: [
      "", [Validators.min(0), Validators.max(10000), Validators.pattern("^[0-9]*$"), Validators.required]
    ],
    employeesSales: [
      "", [Validators.min(0), Validators.max(10000), Validators.pattern("^[0-9]*$"), Validators.required]
    ],
    employeesProduction: [
      "", [Validators.min(0), Validators.max(10000), Validators.pattern("^[0-9]*$"), Validators.required]
    ],
    employeesOther: [
      "", [Validators.min(0), Validators.max(10000), Validators.pattern("^[0-9]*$"), Validators.required]
    ]
  });

  productForms: FormGroup[];
  defaultValue: any;

  constructor(
    public fb: FormBuilder,
    private licenceDataService: LicenseDataService,
    private route: ActivatedRoute,
    private router: Router,
    private monthlyReportDataService: MonthlyReportDataService,
    public dialog: MatDialog
  ) {
    this.routeParams = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>
        of({
          licenceId: params.get("licenceId"),
          monthlyReportId: params.get("monthlyReportId")
        })
      )
    );
  }

  ngOnInit() {
    this.selectedYear = "";
    this.selectedMonth = "";
    this.busy = forkJoin([
      this.licenceDataService.getAllCurrentLicenses()      
    ])
      .subscribe(results => {
        this.licenses = results[0].filter(l => l.licenceTypeName === "Cannabis Retail Store"
          || l.licenceTypeName === "Section 119 Authorization"
          || l.licenceTypeName === "S119 CRS Authorization"
          || l.licenceTypeName === "Producer Retail Store");

        if (this.licenses?.length > 0) 
        {
          this.setYearMonthDropDownListDataSource();
          this.selectedLicense = this.licenses[0];         
          this.getMonthlyReport(this.licenses[0].licenseId, this.selectedYear, this.selectedMonth);
          this.renderMonthlyReport();
        }       
      });
  }

  viewReport() {
    this.getMonthlyReport(this.selectedLicense?.licenseId, this.selectedYear, this.selectedMonth);
  }

  getMonthlyReport(licenceId, year = null, month = null) {
    this.selectedMonthlyReport = null;
    this.loadingMonthlyReports = true;   
    return forkJoin([
      this.monthlyReportDataService.getMonthlyReportByLicenceYearMonth(licenceId, year, month)
    ])
      .subscribe(([monthlyReport]) => {       
        this.selectedMonthlyReport = monthlyReport;       
        this.loadingMonthlyReports = false;
        this.renderMonthlyReport();
      });
  }


  private setYearMonthDropDownListDataSource() {
    const currentYear = new Date().getFullYear();
    this.reportYears = [];
    this.selectedYear = currentYear.toString();
    for (let i = 0; i < 5; i++) {
       var temp = currentYear - i;
       this.reportYears.push(temp.toString());
    }
    this.reportMonths = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
    var tmpMonth = new Date().getMonth() + 1;
    if (tmpMonth < 10) {
      this.selectedMonth = "0" + tmpMonth.toString();

    } else {
      this.selectedMonth = tmpMonth.toString();
    }
    
  }  
  

  handleLicenceSelectedChanged() {    
    this.getMonthlyReport(this.selectedLicense?.licenseId, this.selectedYear, this.selectedMonth);
    this.renderMonthlyReport();
  }

 
  save(submit = false) {
    // main report fields
    const statusCode = submit ? this.monthlyReportStatusEnum.Submitted : this.monthlyReportStatusEnum.Draft;
    const updateRequest: MonthlyReport = {
      ...this.reportForm.value,
      inventorySalesReports: [],
      statusCode: statusCode
    };

    // add product fields
    for (const f of this.productForms) {
      let inventorySales = { ...f.value };
      delete inventorySales.selected;  // Remove field that is only relevant to UI
      updateRequest.inventorySalesReports.push(inventorySales);
    }

    this.loadingMonthlyReports = true;
    this.busy = forkJoin(
      this.monthlyReportDataService.updateMonthlyReport(updateRequest)
    )
      .subscribe(([report]) => {
        //const fullListIndex = this.monthlyReports.findIndex(rep => rep.monthlyReportId === report.monthlyReportId);
        //this.monthlyReports[fullListIndex] = report;       
        this.selectedMonthlyReport = report;
        this.renderMonthlyReport();
        this.loadingMonthlyReports = false;
      });
  }
    
  renderMonthlyReport() {
    this.loadingMonthlyReports = true;
    if (this.selectedMonthlyReport==null) {
      this.loadingMonthlyReports = false;
      return;
    }   
    // Update product forms
    this.productForms = this.selectedMonthlyReport.inventorySalesReports.map(
      (report) => {
        let selected = false;
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
          (report.totalSalesToRetailerValue !== null && report.totalSalesToRetailerValue !== 0) ||
          (report.otherDescription !== null)) {
          selected = true;
        }
        return this.createProductForm(report, selected);
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
      ...this.selectedMonthlyReport
    });
    this.loadingMonthlyReports = false;
  }

  // Shows or hides product forms when relevant checkboxes are toggled
  toggleProductVisibility(report: FormGroup) {
    if (this.reportIsDisabled || !report) {
      return;
    }

    // Reset form when product checkbox is unchecked
    const selected = !!report.value.selected;
    if (selected) {
      this.clearProductForm(report);
    }

    report.get("selected").setValue(!selected);
  }

  findProductForm(id: string): FormGroup {
    return this.productForms.find(f => f.value.inventoryReportId === id);
  }

  clearProductForm(report: FormGroup) {
    const { inventoryReportId, product, productDescription } = report.value as InventorySalesReport;
    report.reset({
      selected: false,
      inventoryReportId: inventoryReportId,
      product: product,
      productDescription: productDescription,
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
      otherDescription: null
    });
  }

  isFieldInvalid(field: string) {
    return !this.reportForm.get(field).valid &&
      (this.reportForm.get(field).dirty || this.reportForm.get(field).touched);
  }

  get isProductInventorySelected() {
    return this.productForms?.some(report => report?.value?.selected);
  }

  hasInvalidProductForm() {
    let invalidProduct = false;
    this.productForms.forEach((f) => {
      if (!f.valid && f.value.selected) {
        invalidProduct = true;
      }
    });
    return invalidProduct;
  }

  submitApplication() {
    const completedProductForms = this.productForms.filter(f => f.value.selected);
    const body = completedProductForms.length > 0
      ? "Are you sure you want to submit this report?"
      : "You have not entered any inventory information. Are you sure you want to submit this report?";

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "400px",
      height: "200px",
      data: {
        title: "Confirm Submission",
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

  getName(report): string {
    if (report == null) return "";
    let temp = report as MonthlyReport;
    let monthNumber = temp.reportingPeriodMonth;
    const monthNames = [
      "January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[Number(monthNumber) - 1]; //+"  "+temp.reportingPeriodYear;
  }

  getMonthName(monthNumber): string {  
    const monthNames = [
      "January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[Number(monthNumber) - 1]; //+"  "+temp.reportingPeriodYear;
  }

  createProductForm(report: InventorySalesReport, selected = false) {
    const closingWeightValidators =
      [Validators.min(0), Validators.max(1000), Validators.pattern("^[0-9]+(\.[0-9]{1,3})?$")];
    if (report.product !== "Seeds" && report.product !== "Vegetative Cannabis") {
      closingWeightValidators.push(Validators.required);
    }
    const totalSeedsValidators = [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")];
    if (report.product === "Seeds") {
      totalSeedsValidators.push(Validators.required);
    }
    return this.fb.group({
      selected: [selected, []],
      inventoryReportId: [report.inventoryReportId, []],
      product: [report.product, []],
      productDescription: [report.productDescription, []],
      openingInventory: [
        report.openingInventory, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      domesticAdditions: [
        report.domesticAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      returnsAdditions: [
        report.returnsAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      otherAdditions: [
        report.otherAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      domesticReductions: [
        report.domesticReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      returnsReductions: [
        report.returnsReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      destroyedReductions: [
        report.destroyedReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      lostReductions: [
        report.lostReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      otherReductions: [
        report.otherReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      closingNumber: [
        report.closingNumber,
        [Validators.required, Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      closingValue: [
        report.closingValue,
        [
          Validators.required, Validators.min(0), Validators.max(1000000000),
          Validators.pattern("^[0-9]+(\.[0-9]{1,2})?$")
        ]
      ],
      closingWeight: [report.closingWeight, closingWeightValidators],
      totalSeeds: [report.totalSeeds, totalSeedsValidators],
      totalSalesToConsumerQty: [
        report.totalSalesToConsumerQty, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      totalSalesToConsumerValue: [
        report.totalSalesToConsumerValue,
        [Validators.min(0), Validators.max(1000000000), Validators.pattern("^[0-9]+(\.[0-9]{1,2})?$")]
      ],
      otherDescription: ["Extracts - Other", "Other"].indexOf(report.product) >= 0
        ? [report.otherDescription, [Validators.required]]
        : null
    },
      { validators: [ClosingInventoryValidator, SalesValidator] });
  }

  checkIfReportValid() {
    return this.reportForm.valid && !this.hasInvalidProductForm() || this.reportIsDisabled;
  }

  checkIfReportSaveable() {
    if (!this.checkIfReportValid()) {
      let hasFieldError = false;
      this.productForms.forEach((form) => {
        Object.keys(form.controls).forEach(key => {
          if (form.controls[key].errors !== null) {
            hasFieldError = true;
          }
        });
        if (hasFieldError) {
          return;
        }
      });
      if (hasFieldError) {
        return false;
      }
    }

    return this.reportForm.valid;
  }


  /***
   * The following code is Federal Report Tab moudle before LCSD-6499
   *
    ngOnInit() {
    this.busy = forkJoin([
      this.licenceDataService.getAllCurrentLicenses(),
      this.monthlyReportDataService.getAllCurrentMonthlyReports(true)
    ])
      .subscribe(results => {
        this.licenses = results[0].filter(l => l.licenceTypeName === "Cannabis Retail Store"
          || l.licenceTypeName === "Section 119 Authorization"
          || l.licenceTypeName === "S119 CRS Authorization"
          || l.licenceTypeName === "Producer Retail Store");
        this.monthlyReports = results[1];

        this.routeParams.subscribe(params => {
          this.updateStateFromParams(params);
        });
      });

    this.routeParams.subscribe(params => {
      this.updateStateFromParams(params);
    });
  }

  updateStateFromParams(params: FederalReportingParams) {
    const licenceIndex = (params.licenceId === null) ? 0 : this.licenses.findIndex(l => l.licenseId === params.licenceId);
    if (licenceIndex !== -1) {
      this.selectedLicenceIndex = licenceIndex;
    } else {
      this.selectedLicenceIndex = 0;
    }

    this.shownMonthlyReports = this.monthlyReports.filter((rep) => rep.licenseId === params.licenceId);

    const reportIndex = (params.monthlyReportId === null || params.monthlyReportId === "default")
      ? 0
      : this.shownMonthlyReports.findIndex(r => r.monthlyReportId === params.monthlyReportId);
    if (reportIndex !== -1) {
      this.selectedMonthlyReportIndex = reportIndex;
    } else {
      this.selectedMonthlyReportIndex = 0;
    }

    this.renderMonthlyReport();
  }


  getLicences(licenceId: string, monthlyReportId = null) {
    this.busy = forkJoin([
      this.licenceDataService.getAllCurrentLicenses()
    ])
      .subscribe(([licenses]) => {
        this.licenses = licenses;
        this.selectedLicenceIndex = this.licenses.findIndex(l => l.licenseId === licenceId);
        // no monthly report chosen
        if (monthlyReportId === null) {
          this.busy = this.getMonthlyReports(licenceId);
        } else {
          this.busy = this.getMonthlyReports(licenceId, monthlyReportId);
        }
      });
  }

  getMonthlyReports(licenceId, monthlyReportId = null) {
    this.loadingMonthlyReports = true;
    return forkJoin([
      this.monthlyReportDataService.getMonthlyReportsByLicence(licenceId)
    ])
      .subscribe(([monthlyReports]) => {
        this.monthlyReports = monthlyReports;
        this.shownMonthlyReports = this.monthlyReports.filter((rep) => rep.licenseId === licenceId);
        if (monthlyReportId !== null) {
          this.selectedMonthlyReportIndex =
            this.shownMonthlyReports.findIndex(r => r.monthlyReportId === monthlyReportId);
        } else if (this.monthlyReports.length > 0) {
          this.selectedMonthlyReportIndex = 0;
          this.router.navigate([
            `/federal-reporting/${this.licenses[this.selectedLicenceIndex].licenseId}/${this.monthlyReports[0]
              .monthlyReportId}`
          ]);
        }
        this.renderMonthlyReport();
        this.loadingMonthlyReports = false;
      });
  }

  save(submit = false) {
    // main report fields
    const statusCode = submit ? this.monthlyReportStatusEnum.Submitted : this.monthlyReportStatusEnum.Draft;
    const updateRequest: MonthlyReport = {
      ...this.reportForm.value,
      inventorySalesReports: [],
      statusCode: statusCode
    };

    // add product fields
    for (const f of this.productForms) {
      let inventorySales = { ...f.value };
      delete inventorySales.selected;  // Remove field that is only relevant to UI
      updateRequest.inventorySalesReports.push(inventorySales);
    }

    this.loadingMonthlyReports = true;
    this.busy = forkJoin(
      this.monthlyReportDataService.updateMonthlyReport(updateRequest)
    )
      .subscribe(([report]) => {
        const fullListIndex = this.monthlyReports.findIndex(rep => rep.monthlyReportId === report.monthlyReportId);
        this.monthlyReports[fullListIndex] = report;
        this.shownMonthlyReports[this.selectedMonthlyReportIndex] = report;
        this.renderMonthlyReport();
        this.loadingMonthlyReports = false;
      });
  }

  handleMonthlyReportTabChanged(event) {
    this.selectedMonthlyReportIndex = event.index;
    this.router.navigate([
      `/federal-reporting/${this.licenses[this.selectedLicenceIndex].licenseId}/${this.shownMonthlyReports[event.index]
        .monthlyReportId}`
    ]);
  }

  handleLicenceTabChanged(event) {
    if (this.selectedLicenceIndex !== event.index) {
      this.router.navigate([
        `/federal-reporting/${this.licenses[event.index].licenseId}/default`
      ]);
    }
  }

  renderMonthlyReport() {
    if (this.shownMonthlyReports.length < 1) {
      return;
    }
    // Update product forms
    this.productForms = this.shownMonthlyReports[this.selectedMonthlyReportIndex].inventorySalesReports.map(
      (report) => {
        let selected = false;
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
          (report.totalSalesToRetailerValue !== null && report.totalSalesToRetailerValue !== 0) ||
          (report.otherDescription !== null)) {
          selected = true;
        }
        return this.createProductForm(report, selected);
      });

    switch (this.shownMonthlyReports[this.selectedMonthlyReportIndex].statusCode) {
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
      ...this.shownMonthlyReports[this.selectedMonthlyReportIndex]
    });
  }

  // Shows or hides product forms when relevant checkboxes are toggled
  toggleProductVisibility(report: FormGroup) {
    if (this.reportIsDisabled || !report) {
      return;
    }

    // Reset form when product checkbox is unchecked
    const selected = !!report.value.selected;
    if (selected) {
      this.clearProductForm(report);
    }

    report.get("selected").setValue(!selected);
  }

  findProductForm(id: string): FormGroup {
    return this.productForms.find(f => f.value.inventoryReportId === id);
  }

  clearProductForm(report: FormGroup) {
    const { inventoryReportId, product, productDescription } = report.value as InventorySalesReport;
    report.reset({
      selected: false,
      inventoryReportId: inventoryReportId,
      product: product,
      productDescription: productDescription,
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
      otherDescription: null
    });
  }

  isFieldInvalid(field: string) {
    return !this.reportForm.get(field).valid &&
      (this.reportForm.get(field).dirty || this.reportForm.get(field).touched);
  }

  get isProductInventorySelected() {
    return this.productForms.some(report => report?.value?.selected);
  }

  hasInvalidProductForm() {
    let invalidProduct = false;
    this.productForms.forEach((f) => {
      if (!f.valid && f.value.selected) {
        invalidProduct = true;
      }
    });
    return invalidProduct;
  }

  submitApplication() {
    const completedProductForms = this.productForms.filter(f => f.value.selected);
    const body = completedProductForms.length > 0
      ? "Are you sure you want to submit this report?"
      : "You have not entered any inventory information. Are you sure you want to submit this report?";

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "400px",
      height: "200px",
      data: {
        title: "Confirm Submission",
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
    const monthNames = [
      "January", "February", "March", "April", "May", "June",
      "July", "August", "September", "October", "November", "December"
    ];
    return monthNames[Number(monthNumber) - 1];
  }

  createProductForm(report: InventorySalesReport, selected = false) {
    const closingWeightValidators =
      [Validators.min(0), Validators.max(1000), Validators.pattern("^[0-9]+(\.[0-9]{1,3})?$")];
    if (report.product !== "Seeds" && report.product !== "Vegetative Cannabis") {
      closingWeightValidators.push(Validators.required);
    }
    const totalSeedsValidators = [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")];
    if (report.product === "Seeds") {
      totalSeedsValidators.push(Validators.required);
    }
    return this.fb.group({
      selected: [selected, []],
      inventoryReportId: [report.inventoryReportId, []],
      product: [report.product, []],
      productDescription: [report.productDescription, []],
      openingInventory: [
        report.openingInventory, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      domesticAdditions: [
        report.domesticAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      returnsAdditions: [
        report.returnsAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      otherAdditions: [
        report.otherAdditions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      domesticReductions: [
        report.domesticReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      returnsReductions: [
        report.returnsReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      destroyedReductions: [
        report.destroyedReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      lostReductions: [
        report.lostReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      otherReductions: [
        report.otherReductions, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      closingNumber: [
        report.closingNumber,
        [Validators.required, Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      closingValue: [
        report.closingValue,
        [
          Validators.required, Validators.min(0), Validators.max(1000000000),
          Validators.pattern("^[0-9]+(\.[0-9]{1,2})?$")
        ]
      ],
      closingWeight: [report.closingWeight, closingWeightValidators],
      totalSeeds: [report.totalSeeds, totalSeedsValidators],
      totalSalesToConsumerQty: [
        report.totalSalesToConsumerQty, [Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]
      ],
      totalSalesToConsumerValue: [
        report.totalSalesToConsumerValue,
        [Validators.min(0), Validators.max(1000000000), Validators.pattern("^[0-9]+(\.[0-9]{1,2})?$")]
      ],
      otherDescription: ["Extracts - Other", "Other"].indexOf(report.product) >= 0
        ? [report.otherDescription, [Validators.required]]
        : null
    },
      { validators: [ClosingInventoryValidator, SalesValidator] });
  }

  checkIfReportValid() {
    return this.reportForm.valid && !this.hasInvalidProductForm() || this.reportIsDisabled;
  }

  checkIfReportSaveable() {
    if (!this.checkIfReportValid()) {
      let hasFieldError = false;
      this.productForms.forEach((form) => {
        Object.keys(form.controls).forEach(key => {
          if (form.controls[key].errors !== null) {
            hasFieldError = true;
          }
        });
        if (hasFieldError) {
          return;
        }
      });
      if (hasFieldError) {
        return false;
      }
    }

    return this.reportForm.valid;
  }
   */
}
