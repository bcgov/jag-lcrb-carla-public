


import { filter, takeWhile, catchError, mergeMap } from "rxjs/operators";
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, ValidatorFn, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { Subject, Observable, forkJoin, of } from "rxjs";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import * as currentApplicationActions from "@app/app-state/actions/current-application.action";
import { ActivatedRoute, Router } from "@angular/router";
import { ApplicationDataService } from "@services/application-data.service";
import { PaymentDataService } from "@services/payment-data.service";
import { Application } from "@models/application.model";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { Account } from "@models/account.model";
import { ApplicationTypeNames, FormControlState } from "@models/application-type.model";
import { LicenceTypeNames} from "@models/license-type.model";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { EstablishmentWatchWordsService } from "@services/establishment-watch-words.service";
import { FeatureFlagService } from "@services/feature-flag.service";
import { LicenseDataService } from "@app/services/license-data.service";
import { UPLOAD_FILES_MODE, ApplicationCancellationDialogComponent } from
  "@components/dashboard/applications-and-licences/applications-and-licences.component";
import { faExclamationCircle, faQuestionCircle, faTrash } from "@fortawesome/free-solid-svg-icons";
import { faSave } from "@fortawesome/free-regular-svg-icons";

const ValidationErrorMap = {
  renewalCriminalOffenceCheck: "Please answer question 1",
  renewalUnreportedSaleOfBusiness: "Please answer question 2",
  renewalBusinessType: "Please answer question 3",
  renewalTiedhouse: "Please answer question 4",
  tiedhouseFederalInterest: "Please answer question 5",
  renewalOrgLeadership: "Please answer question 6",
  renewalkeypersonnel: "Please answer question 7",
  renewalShareholders: "Please answer question 8",
  renewalOutstandingFines: "Please answer question 9",
  renewalBranding: "Please answer question 10",
  renewalSignage: "Please answer question 11",
  renewalEstablishmentAddress: "Please answer question 12",
  renewalValidInterest: "Please answer question 13",
  renewalZoning: "Please answer question 14",
  renewalFloorPlan: "Please answer question 15",
  renewalFederalLicence: "Please answer question 16",
  renewalFederalSecurity: "Please answer question 17",
  contactPersonFirstName: "Please enter the business contact's first name",
  contactPersonLastName: "Please enter the business contact's last name",
  contactPersonEmail: "Please enter the business contact's email address",
  contactPersonPhone: "Please enter the business contact's 10-digit phone number",
  authorizedToSubmit: "Please affirm that you are authorized to submit the application",
  signatureAgreement: "Please affirm that all of the information provided for this application is true and complete",
};


@Component({
  selector: "app-application-renewal",
  templateUrl: "./application-renewal.component.html",
  styleUrls: ["./application-renewal.component.scss"]
})
export class ApplicationRenewalComponent extends FormBase implements OnInit {
  faExclamationCircle = faExclamationCircle;
  faQuestionCircle = faQuestionCircle;
  faSave = faSave;
  faTrash = faTrash;
  //establishmentWatchWords: KeyValue<string, boolean>[];
  application: Application;

  form: FormGroup;
  savedFormData: any;
  applicationId: string;
  accountId: string;
  payMethod: string;
  validationMessages: any[];
  showValidationMessages: boolean;
  submittedApplications = 8;
  tiedHouseFormData: TiedHouseConnection;
  possibleProblematicNameWarning = false;
  htmlContent = {} as ApplicationHTMLContent;
  readonly UPLOAD_FILES_MODE = UPLOAD_FILES_MODE;
  ApplicationTypeNames = ApplicationTypeNames;
  LicenceTypeNames = LicenceTypeNames;
  FormControlState = FormControlState;
  mode: string;
  account: Account;

  uploadedSupportingDocuments = 0;
  uploadedFinancialIntegrityDocuments: 0;
  uploadedAssociateDocuments: 0;
  window = window;
  dataLoaded: boolean;
  submitReqInProgress: boolean;
  cancelReqInProgress: boolean;
  saveForLateInProgress: boolean;

  constructor(private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
    public router: Router,
    public applicationDataService: ApplicationDataService,
    public licenceDataService: LicenseDataService,
    public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private tiedHouseService: TiedHouseConnectionsDataService,
    public dialog: MatDialog,
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get("applicationId"));
    this.route.paramMap.subscribe(pmap => this.mode = pmap.get("mode"));
  }


  holder(): string {
    if(this.application.assignedLicence.licenseType === LicenceTypeNames.S119){
      return "authorized retailer";
    } else {
    return "licensee";
    }
  }

  typeOf(): string {
    if(this.application.assignedLicence.licenseType === LicenceTypeNames.S119){
      return "authorization";
    } else {
    return "licence";
    }

  }

  titleOf(): string {
    if(this.application.assignedLicence.licenseType === LicenceTypeNames.S119){
      return "Section 119 Authorization";
    } else if(this.application.assignedLicence.licenseType === LicenceTypeNames.PRS) {
      return "Producer Retail Store License";
    }
    return "Cannabis Retail Store Licence";
  }

  requiredAlternateQuestionValidator(): ValidatorFn {
    if(this.showAlternateQuestions()) {
      return Validators.required;
    } 

    return Validators.nullValidator;
  }

  ngOnInit() {

    let sub = this.applicationDataService.getSubmittedApplicationCount()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => this.submittedApplications = value);
    this.subscriptionList.push(sub);

    //this.establishmentWatchWordsService.initialize();

    sub = this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });
    this.subscriptionList.push(sub);

    sub = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, "");
        }

        this.application = data;

        this.hideFormControlByType();

        this.addDynamicContent();

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          },
            {});

        this.form = this.fb.group({
          id: [""],
    
          // #1
          renewalCriminalOffenceCheck: ["", Validators.required],
          // #2
          renewalUnreportedSaleOfBusiness: ["", Validators.required],
          // #3
          renewalBusinessType: ["", Validators.required],
          // #4
          renewalTiedhouse: ["", Validators.required],
          // #5
          tiedhouseFederalInterest: ["", Validators.required],
          // #6
          renewalOrgLeadership: ["", Validators.required],
          // #7
          renewalkeypersonnel: ["", Validators.required],
          // #8
          renewalShareholders: ["", Validators.required],
          // #9
          renewalOutstandingFines: ["", Validators.required],
          // # 10
          renewalBranding: ["", Validators.required],
          // #11
          renewalSignage: ["", Validators.required],
          // #12
          renewalEstablishmentAddress: ["", Validators.required],
          // #13
          renewalValidInterest: ["", Validators.required],
          // #14
          renewalZoning: ["", Validators.required],
          // #15
          renewalFloorPlan: ["", Validators.required],
          // #16
          renewalFederalLicence: ["", [this.requiredAlternateQuestionValidator()]],
          // #17
          renewalFederalSecurity: ["", [this.requiredAlternateQuestionValidator()]],
    
          contactPersonFirstName: ["", Validators.required],
          contactPersonLastName: ["", Validators.required],
          contactPersonRole: [""],
          contactPersonEmail: ["", Validators.required],
          contactPersonPhone: ["", Validators.required],
    
          authorizedToSubmit: ["", [this.customRequiredCheckboxValidator()]],
          signatureAgreement: ["", [this.customRequiredCheckboxValidator()]],
    
          assignedLicence: this.fb.group({
            id: [""],
            establishmentAddressStreet: [""],
            establishmentAddressCity: [""],
            establishmentAddressPostalCode: [""],
            establishmentParcelId: [""]
          }),
        });

        this.form.patchValue(noNulls);
        if (data.isPaid) {
          this.form.disable();
        }
        this.savedFormData = this.form.value;
        this.dataLoaded = true;
      },
        () => {
          console.log("Error occured");
          this.dataLoaded = true;
        }
      );
    this.subscriptionList.push(sub);
  }

  private hideFormControlByType() {
    //add guard
    if (!(this.application && this.application.applicationType)) {
      return;
    }
    if (this.application.applicationType.name === ApplicationTypeNames.MarketingRenewal) {
      this.form.get("renewalBranding").clearValidators();
      this.form.get("renewalSignage").clearValidators();
      this.form.get("renewalEstablishmentAddress").clearValidators();
      this.form.get("renewalValidInterest").clearValidators();
      this.form.get("renewalZoning").clearValidators();
      this.form.get("renewalFloorPlan").clearValidators();

      this.form.get("renewalBranding").updateValueAndValidity();
      this.form.get("renewalSignage").updateValueAndValidity();
      this.form.get("renewalEstablishmentAddress").updateValueAndValidity();
      this.form.get("renewalValidInterest").updateValueAndValidity();
      this.form.get("renewalZoning").updateValueAndValidity();
      this.form.get("renewalFloorPlan").updateValueAndValidity();

    }
  }

  isMarketing(): boolean {
    return this.application.applicationType.name === ApplicationTypeNames.MarketingRenewal;
  }

  showAlternateQuestions = (): boolean => {
    const licenceType = this.application.assignedLicence.licenseType;
    if(licenceType === LicenceTypeNames.S119 || licenceType === LicenceTypeNames.PRS) {
      return true;
    }
    
    return false;
  }

  isTouchedAndInvalid(fieldName: string): boolean {
    return this.form.get(fieldName).touched && !this.form.get(fieldName).valid;
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
    console.log(this.form.get("establishmentName").errors);
    this.possibleProblematicNameWarning =
      this.establishmentWatchWordsService.potentiallyProblematicValidator(this.form.get("establishmentName").value);
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    const saveData = this.form.value;

    return forkJoin([
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value }),
      this.prepareTiedHouseSaveRequest(this.tiedHouseFormData)
    ]).pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        this.savedFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true) {
          this.snackBar.open("Application has been saved",
            "Success",
            { duration: 2500, panelClass: ["green-snackbar"] });
        }
        return of(true);
      }));
  }

  prepareTiedHouseSaveRequest(_tiedHouseData) {
    if (!this.application.tiedHouse) {
      return of(null);
    }
    let data = (Object as any).assign(this.application.tiedHouse, _tiedHouseData);
    data = { ...data };
    return this.tiedHouseService.updateTiedHouse(data, data.id);
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
      );
  }

  /**
   * Submit the application for payment
   * */
  submitApplication() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
    } else if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
      this.submitPayment();
    } else {
      this.submitReqInProgress = true;
      this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result) {
            this.submitPayment();
          } else {
            this.submitReqInProgress = false;
          }
        },
          error => { this.submitReqInProgress = false; });
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
          if (err._body === "Payment already made") {
            this.snackBar.open("Application payment has already been made.",
              "Fail",
              { duration: 3500, panelClass: ["red-snackbar"] });
          }
          this.submitReqInProgress = false;
        });
  }

  isValid(): boolean {
    this.markControlsAsTouched(this.form);
    this.showValidationMessages = false;
    this.validationMessages = this.listControlsWithErrors(this.form, ValidationErrorMap);
    return this.form.valid;
  }

  /**
   * Dialog to confirm the application cancellation (status changed to "Termindated")
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
          this.cancelReqInProgress = true;
          this.applicationDataService.cancelApplication(this.applicationId)
            .pipe(takeWhile(() => this.componentActive))
            .subscribe(() => {
              this.cancelReqInProgress = false;
              this.savedFormData = this.form.value;
              this.router.navigate(["/dashboard"]);
            },
              () => {
                this.snackBar.open("Error cancelling the application",
                  "Fail",
                  { duration: 3500, panelClass: ["red-snackbar"] });
                console.error("Error cancelling the application");
                this.cancelReqInProgress = false;
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
    this.saveForLateInProgress = true;
    this.save(true)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((result: boolean) => {
        this.saveForLateInProgress = false;
        if (result) {
          this.router.navigate(["/dashboard"]);
        }

      },
        error => {
          this.saveForLateInProgress = false;
          console.error(error);
        });
  }

}
