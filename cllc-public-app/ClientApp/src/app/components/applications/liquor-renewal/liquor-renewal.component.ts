import { Component, OnInit } from "@angular/core";
import { KeyValue } from "@angular/common";
import { Application } from "@models/application.model";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Observable, Subject, forkJoin, of } from "rxjs";
import { UPLOAD_FILES_MODE } from "@components/licences/licences.component";
import { ApplicationTypeNames, FormControlState } from "@models/application-type.model";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { PaymentDataService } from "@services/payment-data.service";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router, ActivatedRoute } from "@angular/router";
import { ApplicationDataService } from "@services/application-data.service";
import { LicenseDataService } from "@services/license-data.service";
import { FeatureFlagService } from "@services/feature-flag.service";
import { EstablishmentWatchWordsService } from "@services/establishment-watch-words.service";
import { takeWhile, filter, catchError, mergeMap } from "rxjs/operators";
import { ApplicationCancellationDialogComponent } from
  "@components/dashboard/applications-and-licences/applications-and-licences.component";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { Account } from "@models/account.model";
import { License } from "@models/license.model";
import { AnnualVolumeService } from "@services/annual-volume.service";
import { faBolt, faExchangeAlt, faPencilAlt, faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";
import { faSave } from "@fortawesome/free-regular-svg-icons";

const ValidationErrorMap = {
  renewalCriminalOffenceCheck: "Please answer question 1",
  renewalDUI: "Please answer question 2",
  renewalBusinessType: "Please answer question 3",
  renewalShareholders: "Please answer question 4",
  renewalThirdParty: "Please answer question 5",
  renewalFloorPlan: "Please answer question 6",
  renewalTiedhouse: "Please answer question 7",
  renewalUnreportedSaleOfBusiness: "Please answer question 8",
  renewalValidInterest: "Please answer question 9",
  renewalkeypersonnel: "Please answer question 10",

  contactPersonFirstName: "Please enter the business contact's first name",
  contactPersonLastName: "Please enter the business contact's last name",
  contactPersonEmail: "Please enter the business contact's email address",
  contactPersonPhone: "Please enter the business contact's 10-digit phone number",
  authorizedToSubmit: "Please affirm that you are authorized to submit the application",
  signatureAgreement: "Please affirm that all of the information provided for this application is true and complete",
  readRefundPolicy: "Please affirm that you have read and understand the refund policy",

  ldbOrderTotals: "Please provide LDB Order Totals ($0 - $10,000,000)",
  ldbOrderTotalsConfirm: "Please confirm LDB Order Totals matches",
  volumeProduced: "Please provide the total volume produced (in litres)",
  volumeDestroyed: "Please provide the total volume destroyed (in litres and included in production volume)",

};

// Wineries are required to manufacture at least 4500 litres on-site per year to keep their licence
const WINERY_MINIMUM_PRODUCTION = 4500;

@Component({
  selector: "app-liquor-renewal",
  templateUrl: "./liquor-renewal.component.html",
  styleUrls: ["./liquor-renewal.component.scss"]
})
export class LiquorRenewalComponent extends FormBase implements OnInit {
  faPenclilAlt = faPencilAlt;
  faBolt = faBolt;
  faPlus = faPlus;
  faExchangeAlt = faExchangeAlt;
  faTrash = faTrash;
  faSave = faSave;
  establishmentWatchWords: KeyValue<string, boolean>[];
  application: Application;

  form: FormGroup;
  savedFormData: any;
  applicationId: string;
  accountId: string;
  // need access to the licence to handle subcategory cases
  licenseSubCategory: string;
  licenseType: string;
  payMethod: string;
  validationMessages: any[];
  showValidationMessages: boolean;
  possibleProblematicNameWarning = false;
  htmlContent = {} as ApplicationHTMLContent;
  readonly UPLOAD_FILES_MODE = UPLOAD_FILES_MODE;
  ApplicationTypeNames = ApplicationTypeNames;
  FormControlState = FormControlState;
  mode: string;
  account: Account;

  uploadedSupportingDocuments = 0;
  uploadedFinancialIntegrityDocuments: 0;
  uploadedAssociateDocuments: 0;
  uploadedDiscretionLetter: 0;
  window = window;
  previousYear: string;
  dataLoaded: boolean;
  saveForLaterInProgress: boolean;
  submitReqInProgress: boolean;
  cancelReqInprogress: boolean;
  applicationNotLoaded = false;
  constructor(private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
    public router: Router,
    public applicationDataService: ApplicationDataService,
    public licenceDataService: LicenseDataService,
    public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog,
    public annualVolumeService: AnnualVolumeService,
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get("applicationId"));
    this.route.paramMap.subscribe(pmap => this.mode = pmap.get("mode"));
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [""],


      renewalCriminalOffenceCheck: ["", Validators.required], // #1
      renewalDUI: ["", Validators.required],                  // #2
      renewalBusinessType: ["", Validators.required],         // #3
      renewalShareholders: ["", Validators.required],         // #4
      renewalThirdParty: ["", Validators.required],           // #5
      renewalFloorPlan: ["", Validators.required],            // #6
      renewalTiedhouse: ["", Validators.required],                  //#7
      renewalUnreportedSaleOfBusiness: ["", Validators.required],   //#8
      renewalValidInterest: ["", Validators.required],              //#9
      renewalkeypersonnel: ["", Validators.required],               // #10

      contactPersonFirstName: ["", Validators.required],
      contactPersonLastName: ["", Validators.required],
      contactPersonRole: [""],
      contactPersonEmail: ["", Validators.required],
      contactPersonPhone: ["", Validators.required],

      authorizedToSubmit: ["", [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ["", [this.customRequiredCheckboxValidator()]],
      isManufacturedMinimum: ["", []],

      readRefundPolicy: ["", [this.customRequiredCheckboxValidator()]],
    });

    this.previousYear = (new Date().getFullYear() - 1).toString();

    this.establishmentWatchWordsService.initialize();

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });

    const sub = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        this.licenceDataService.getLicenceById(data.assignedLicence.id)
          .pipe(takeWhile(() => this.componentActive))
          .subscribe((data: License) => {
            if (data.licenseType !== "Manufacturer" &&
              data.licenseSubCategory !== null &&
              data.licenseSubCategory !== "Independent Wine Store" &&
              data.licenseSubCategory !== "Tourist Wine Store" &&
              data.licenseSubCategory !== "Special Wine Store") {
              this.form.addControl("ldbOrderTotals",
                this.fb.control("",
                  [
                    Validators.required, Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")
                  ]));
              this.form.addControl("ldbOrderTotalsConfirm", this.fb.control("", [Validators.required]));
              this.form.get('ldbOrderTotals').setValue(this.application.ldbOrderTotals);
              this.form.get('ldbOrderTotalsConfirm').setValue(this.application.ldbOrderTotals);

            }
            if (data.licenseType === "Manufacturer" &&
              (data.licenseSubCategory === "Winery" || data.licenseSubCategory === "Brewery")) {
              this.form.addControl("volumeProduced", this.fb.control("", [Validators.required]));
              this.form.get('volumeProduced').setValue(this.application.volumeProduced);
            }
            if (data.licenseType === "Manufacturer" && data.licenseSubCategory === "Winery") {
              this.form.addControl("volumeDestroyed", this.fb.control("", [Validators.required]));
              this.form.get('volumeDestroyed').setValue(this.application.volumeDestroyed);
            }
            this.dataLoaded = true;
          },

            error => {
              this.dataLoaded = true;
              this.applicationNotLoaded = true;
            }
            );
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, "");
        }

        this.application = data;
        this.licenseSubCategory = this.application.assignedLicence.licenseSubCategory;
        this.licenseType = this.application.assignedLicence.licenseType;

        // if (this.isSubcategory('Winery'))

        this.addDynamicContent();

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          },
            {});

        this.form.patchValue(noNulls);
        if (data.isPaid) {
          this.form.disable();
        }
        this.savedFormData = this.form.value;
      },
        () => {
          this.snackBar.open("Failed to load the application, Please try again",
            "Fail",
            { duration: 180000, panelClass: ["red-snackbar"] });
          console.log("Error occured");
          this.applicationNotLoaded = true;
          this.dataLoaded = true;
        }
      );
  }

  isSubcategory(subcategoryName: string) {
    return this.licenseSubCategory == subcategoryName;
  }

  getVolumeLabel() {
    const label = this.isSubcategory("Brewery") ? "hectolitres" : "litres";
    return `What was your ${this.licenseSubCategory}'s total volume produced (in ${label
      }) between January 1 and December 31?`;
  }

  isAgent() {
    return false;
    //return this.licenseType == 'Agent';
  }

  canDeactivate(): Observable<boolean> | boolean {
    const formDidntChange = JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value);
    if (formDidntChange) {
      return true;
    } else {
      const subj = new Subject<boolean>();
      this.save(true).subscribe(res => {
        subj.next(res);
      });
      return subj;
    }
  }

  checkPossibleProblematicWords() {
    this.possibleProblematicNameWarning =
      this.establishmentWatchWordsService.potentiallyProblematicValidator(this.form.get("establishmentName").value);
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    const saveData = this.form.value;
    var saved = true;
    const reqs = [];
    //LCSD-6608&LCSD-6610 Refactoring volumeProduced,volumeDestroyed and ldbOrderTotals to be saved in the application entity
    if (this.form.get("ldbOrderTotals")) {
      this.application.ldbOrderTotals = Number(this.form.value.ldbOrderTotals != null && this.form.value.ldbOrderTotals != undefined ? this.form.value.ldbOrderTotals : 0);
    }
    if (this.licenseSubCategory === "Winery" || this.licenseSubCategory === "Brewery") {
      this.application.volumeProduced = this.form.get("volumeProduced") ? this.form.get("volumeProduced").value : 0;
      this.application.volumeDestroyed = this.form.get("volumeDestroyed") ? this.form.get("volumeDestroyed").value : 0;
    }

    return forkJoin([
      ...reqs,
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value })
    ]
    )
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        saved = false;
        return of(false);
      }))
      .pipe(mergeMap(() => {
        this.savedFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true && saved === true) {
          this.snackBar.open("Application has been saved",
            "Success",
            { duration: 2500, panelClass: ["green-snackbar"] });
        }
        if (saved !== true) {
          return of(false);
        }
        return of(true);
      }));
  }

  isMFG(): boolean {
    return this.licenseType === "Manufacturer";
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        // this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
      );
  }

  /**
   * Submit the application for payment
   * */
  submitApplication() {
    this.submitReqInProgress = true;
    if (!this.isValid()) {
      this.showValidationMessages = true;
      this.submitReqInProgress = false;
    } else if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
      this.submitPayment();
    } else {
      this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result==true) {
            this.submitPayment();
          } else {
            this.submitReqInProgress = false;
          }
        },
          error => { this.submitReqInProgress = false });
    }
  }

  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   * */
  private submitPayment() {
    this.paymentDataService.getPaymentSubmissionUrl(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(res => {
        this.submitReqInProgress = false;
        const jsonUrl = res;
        window.location.href = jsonUrl["url"];
        return jsonUrl["url"];
      },
        err => {
          if (err === "Payment already made") {
            this.snackBar.open("Application payment has already been made, please refresh the page.",
              "Fail",
              { duration: 3500, panelClass: ["red-snackbar"] });
          }
          this.submitReqInProgress = false;
        });
  }

  isValid(): boolean {
    this.showValidationMessages = false;

    if(this.isAgent()){
      this.form.get("renewalFloorPlan").setValidators([]);
      this.form.get("renewalFloorPlan").updateValueAndValidity();
      this.form.get("renewalTiedhouse").setValidators([]);
      this.form.get("renewalTiedhouse").updateValueAndValidity();
      this.form.get("renewalUnreportedSaleOfBusiness").setValidators([]);
      this.form.get("renewalUnreportedSaleOfBusiness").updateValueAndValidity();
      this.form.get("renewalValidInterest").setValidators([]);
      this.form.get("renewalValidInterest").updateValueAndValidity();
    }

    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, ValidationErrorMap);

    if (this.form.get("ldbOrderTotals") &&
      this.form.get("ldbOrderTotals").value !== this.form.get("ldbOrderTotalsConfirm").value) {
      this.validationMessages.push("LDB Order Totals are required");
    }

    // Wineries only
    //  - enforce minimum production (4500L per year)
    //  - discretion letter is required when winery manufactured less than the minimum production.
    if (this.isSubcategory("Winery")) {
      const volumeProduced = parseInt(this.form.get("volumeProduced").value, 10);
      const isMinimumChecked = !!this.form.get("isManufacturedMinimum").value;
      if (!isNaN(volumeProduced) && volumeProduced < WINERY_MINIMUM_PRODUCTION && isMinimumChecked) {
        this.validationMessages.push(
          `You have not indicated that you have produced less than the required minimum production. The value of ${volumeProduced} is less than the required minimum production.`);
      }
      if (!isMinimumChecked && (this.uploadedDiscretionLetter || 0) < 1) {
        this.validationMessages.push(
          "You have indicated that you have produced less than the required minimum production. Please upload a discretion letter.");
      }
    }

    if (this.applicationNotLoaded) {
      this.validationMessages.push("Failed to load the application, Please try again");
    }

    return this.validationMessages.length === 0;
  }

  /**
   * Dialog to confirm the application cancellation (status changed to "Terminated")
   */
  cancelApplication() {

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "400px",
      height: "200px",
      data: {
        establishmentName: this.application.establishmentName,
        applicationName: this.application.name
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ApplicationCancellationDialogComponent, dialogConfig);
    dialogRef.afterClosed()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.cancelReqInprogress = true;
          this.applicationDataService.cancelApplication(this.applicationId)
            .pipe(takeWhile(() => this.componentActive))
            .subscribe(() => {
              this.cancelReqInprogress = false;
              this.savedFormData = this.form.value;
              this.router.navigate(["/dashboard"]);
            },
              () => {
                this.snackBar.open("Error cancelling the application",
                  "Fail",
                  { duration: 3500, panelClass: ["red-snackbar"] });
                console.error("Error cancelling the application");
                this.cancelReqInprogress = false;
              });
        }
      });
  }

  businessTypeIsPartnership(): boolean {
    return this.account &&
      [
        "GeneralPartnership",
        "LimitedPartnership",
        "LimitedLiabilityPartnership",
        "Partnership"
      ].indexOf(this.account.businessType) !==
      -1;
  }

  businessTypeIsPrivateCorporation(): boolean {
    return this.account &&
      [
        "PrivateCorporation",
        "UnlimitedLiabilityCorporation",
        "LimitedLiabilityCorporation"
      ].indexOf(this.account.businessType) !==
      -1;
  }

  isCRSRenewalApplication(): boolean {
    return this.application &&
      this.application.applicationType &&
      [
        ApplicationTypeNames.CRSRenewal.toString(),
        ApplicationTypeNames.CRSRenewalLate30.toString(),
        ApplicationTypeNames.CRSRenewalLate6Months.toString(),
      ].indexOf(this.application.applicationType.name) !==
      -1;
  }

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.ReadOnly.toString()]
      .indexOf(state) !==
      -1;
  }

  saveForLater() {
    this.saveForLaterInProgress = true;
    this.save(true)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((result: boolean) => {
        if (result) {
          this.router.navigate(["/dashboard"]);
        }
        this.saveForLaterInProgress = false;
      },
        () => {
          this.saveForLaterInProgress = false;
        });
  }
}
