import { Component, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ActivatedRoute, Router } from "@angular/router";
import { AppState } from "@app/app-state/models/app-state";
import { Account } from "@models/account.model";
import { ApplicationLicenseSummary } from "@models/application-license-summary.model";
import { Application } from "@models/application.model";
import { Store } from "@ngrx/store";
import { ApplicationDataService } from "@services/application-data.service";
import { PaymentDataService } from "@services/payment-data.service";
import { FormBase } from "@shared/form-base";
import { Observable, of } from "rxjs";
import { catchError, delay, filter, mergeMap, takeWhile } from "rxjs/operators";
import { ContactComponent, ContactData } from "@shared/components/contact/contact.component";
import { faIdCard } from "@fortawesome/free-regular-svg-icons";
import { faQuestionCircle } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-permanent-change-to-a-licensee",
  templateUrl: "./permanent-change-to-a-licensee.component.html",
  styleUrls: ["./permanent-change-to-a-licensee.component.scss"]
})
export class PermanentChangeToALicenseeComponent extends FormBase implements OnInit {
  isAmalgamated = false;
  faQuestionCircle = faQuestionCircle;
  faIdCard = faIdCard;
  value: any; // placeholder prop
  application: Application;
  liquorLicences: ApplicationLicenseSummary[] = [];
  cannabisLicences: ApplicationLicenseSummary[] = [];
  account: Account;
  businessType: string;
  saveComplete: any;
  submitApplicationInProgress: boolean;
  busyPromise: Promise<void>;
  showValidationMessages: boolean;
  savedFormData: any;
  invoiceType: any;
  dataLoaded: boolean;
  primaryPaymentInProgress: boolean;
  secondaryPaymentInProgress: boolean;
  applicationContact: ContactData;
  verifyRquestMade: boolean;
  validationMessages: string[];
  @ViewChild("appContact")
  appContact: ContactComponent;
  appContactDisabled: boolean;
  applicationId: string;
  canCreateNewApplication: boolean;
  createApplicationInProgress: boolean;
  primaryInvoice: any;
  secondaryInvoice: any;
  uploadedCentralSecuritiesRegister = 0;
  uploadedNOA = 0;
  uploadedNameChangeDocuments = 0;
  uploadedCertificateOfNameChange = 0;
  uploadedPartnershipRegistration = 0;
  uploadedSocietyNameChange = 0;
  uploadedExecutorDocuments = 0;
  showDirectorsAndOfficersNotice: boolean = false;


  get hasLiquor(): boolean {
    return this.liquorLicences.length > 0;
  }

  get hasCannabis(): boolean {
    return this.cannabisLicences.length > 0;
  }

  changeList = [];

  get selectedChangeList() {
    return this.changeList
      .filter(item => this.form && this.form.get(item.formControlName).value === true);
  }

  form: FormGroup;

  constructor(private applicationDataService: ApplicationDataService,
    private paymentDataService: PaymentDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private store: Store<AppState>) {
    super();
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(filter(account => !!account))
      .subscribe(account => {
        this.account = account;
        this.changeList = masterChangeList.filter(item => !!item.availableTo.find(bt => bt === account.businessType));
      });

    this.route.paramMap
      .subscribe(pmap => {
        this.invoiceType = pmap.get("invoiceType");
        this.applicationId = pmap.get("applicationId");
      });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      csInternalTransferOfShares: [""],
      csExternalTransferOfShares: [""],
      csChangeOfDirectorsOrOfficers: [""],
      csNameChangeLicenseeCorporation: [""],
      csNameChangeLicenseePartnership: [""],
      csNameChangeLicenseeSociety: [""],
      csNameChangeLicenseePerson: [""],
      csAdditionalReceiverOrExecutor: [""],
      firstNameOld: [""],
      firstNameNew: [""],
      lastNameOld: [""],
      lastNameNew: [""],
      description2: [""],
      description3: [""],
      authorizedToSubmit: ["", [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ["", [this.customRequiredCheckboxValidator()]],
    });

    this.loadData();
  }

  private loadData() {
    const sub = this.applicationDataService.getPermanentChangesToLicenseeData(this.applicationId)
      .subscribe((data) => {
        this.setFormData(data);
      });
    this.subscriptionList.push(sub);
  }

  createApplication() {
    this.router.navigateByUrl(`/permanent-change-to-a-licensee`);
  }

  private setFormData({ application, licences, primary, secondary }) {
    this.liquorLicences = licences.filter(item => item.licenceTypeCategory === "Liquor" && item.status === "Active");
    this.cannabisLicences =
      licences.filter(item => item.licenceTypeCategory === "Cannabis" && item.status === "Active");
      // Check if licensee holds any licences that require a Personal History Summary
      licences.forEach(item => {
      if (
        item.licenceTypeName === this.ApplicationTypeNames.FP
        || item.licenceTypeName === this.ApplicationTypeNames.LP
        || item.licenceTypeName === this.ApplicationTypeNames.LPC
        || item.licenceTypeName === this.ApplicationTypeNames.Catering
        || item.licenceTypeName === this.ApplicationTypeNames.UBV
        || item.licenceTypeName === this.ApplicationTypeNames.RLRS
      ) {
        this.showDirectorsAndOfficersNotice = true;
      }
    })
    this.application = application;
    this.applicationContact = {
      contactPersonFirstName: this.application.contactPersonFirstName,
      contactPersonLastName: this.application.contactPersonLastName,
      contactPersonRole: this.application.contactPersonRole,
      contactPersonPhone: this.application.contactPersonPhone,
      contactPersonEmail: this.application.contactPersonEmail
    };

    this.primaryInvoice = primary;
    this.secondaryInvoice = secondary;

    const primaryInvoiceInfoMissing = primary && primary.isApproved && !this.application.primaryInvoicePaid;
    const secondaryInvoiceInfoMissing = secondary && secondary.isApproved && !this.application.secondaryInvoicePaid;

    // if all required payments are made, go to the dashboard
    if ((!this.hasCannabis || this.application.primaryInvoicePaid) &&
      (!this.hasLiquor || this.application.secondaryInvoicePaid)) {
      this.canCreateNewApplication = true;
    }

    if (primaryInvoiceInfoMissing || secondaryInvoiceInfoMissing) {
      // the asynchonous workflow in dynamics has not yet run. Pause for a second and get data again
      setTimeout(() => {
        this.loadData();
      }, 1000);
    } else {
      // if any payment was made, disable the form
      if (this.application.primaryInvoicePaid || this.application.secondaryInvoicePaid) {
        this.form.disable();
        this.appContactDisabled = true;
      }
      this.form.patchValue(application);
     
      this.dataLoaded = true;
    }
  }

  /**
 * Save form data
 * @param showProgress
 */
  save(showProgress: boolean = false, appData: Application = {} as Application): Observable<[boolean, Application]> {
    const saveData = this.form.value;

    return this.applicationDataService.updateApplication({
      ...this.application,
      ...this.form.value,
      ...this.appContact.form.value,
      ...appData
    })
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        const res: [boolean, Application] = [false, null];
        return of(res);
      }))
      .pipe(mergeMap((data) => {
        if (showProgress === true) {
          this.snackBar.open("Application has been saved",
            "Success",
            { duration: 2500, panelClass: ["green-snackbar"] });
        }
        const res: [boolean, Application] = [true, data as Application];
        return of(res);
      }));
  }

  isValid(): boolean {
    this.showValidationMessages = false;
    this.validationMessages = [];
    this.validationMessages = this.listControlsWithErrors(this.form, this.getValidationErrorMap());
    let valid = this.form.disabled || this.form.valid;

    const securitiesDocIsRequired = (this.form.get('csInternalTransferOfShares').value
      || this.form.get('csExternalTransferOfShares').value);
    if (securitiesDocIsRequired && this.uploadedCentralSecuritiesRegister < 1) {
      this.validationMessages.push('At least one Central Securities Register document is required.');
      valid = false;
    }

    const noticeOfArticlesDocIsRequired = (this.form.get('csChangeOfDirectorsOrOfficers').value);
    if (noticeOfArticlesDocIsRequired && this.uploadedNOA < 1) {
      this.validationMessages.push('The Notice of Articles document is required.');
      valid = false;
    }

    const partnershipRegistrationDocIsRequired = (this.form.get('csNameChangeLicenseeSociety').value);
    if (partnershipRegistrationDocIsRequired && this.uploadedSocietyNameChange < 1) {
      this.validationMessages.push('A Partnership Registration document is required.');
      valid = false;
    }

    const corpNameChangeDocIsRequired = (this.form.get('csNameChangeLicenseeCorporation').value);
    if (corpNameChangeDocIsRequired && this.uploadedCertificateOfNameChange < 1) {
      this.validationMessages.push('A copy of the Certificate of Change of Name Form is required.');
      valid = false;
    }

    const partnerLicenseeNameChangeDocIsRequired = (this.form.get('csNameChangeLicenseePartnership').value);
    if (partnerLicenseeNameChangeDocIsRequired && this.uploadedPartnershipRegistration < 1) {
      this.validationMessages.push('A Change of Name document is required.');
      valid = false;
    }

    const nameChangeDocIsRequired = (this.form.get('csNameChangeLicenseePerson').value);
    if (nameChangeDocIsRequired && this.uploadedNameChangeDocuments < 1) {
      this.validationMessages.push('A Change of Name document is required.');
      valid = false;
    }

    const executorDocIsRequired = (this.form.get('csAdditionalReceiverOrExecutor').value && this.form.get(''));
    if (executorDocIsRequired && this.uploadedExecutorDocuments < 1) {
      this.validationMessages.push('Please upload a copy of Assignment of Executor, a copy of the last will(s) and testament(s) or a copy of the Death Certificate.');
      valid = false;
    }

    return valid;
  }

  isScreeningRequired(): boolean {
    return true;
  }

  hasChanges(): boolean {
    return true;
  }

  /**
   * Submit the application for payment
   * */
  submit_application(invoiceType: "primary" | "secondary") {
    // Only save if the data is valid
    if (this.isValid()) {
      if (invoiceType === "primary") {
        this.primaryPaymentInProgress = true;
      } else {
        this.secondaryPaymentInProgress = true;
      }
      this.submitApplicationInProgress = true;
      var trigInv = 0;
      if (this.application.licenceFeeInvoice == null) {
        trigInv = 1;
      }
      else {
        if (this.application.licenceFeeInvoice.statuscode == 3) //cancelled
        {
          trigInv = 1;
        }
      }
      this
        .save(!this.application.applicationType.isFree,
          { invoiceTrigger: trigInv } as Application) // trigger invoice generation when saving LCSD6564 Only if not present or cancelled.
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(([saveSucceeded, app]) => {
          if (saveSucceeded) {
            if (app) {
              this.submitPayment(invoiceType)
                .subscribe(res => {
                  this.saveComplete.emit(true);
                  if (invoiceType === "primary") {
                    this.primaryPaymentInProgress = false;
                  } else {
                    this.secondaryPaymentInProgress = false;
                  }
                });
            }
          } else if (this.application.applicationType.isFree
          ) { // show error message the save failed and the application is free
            this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
            if (invoiceType === "primary") {
              this.primaryPaymentInProgress = false;
            } else {
              this.secondaryPaymentInProgress = false;
            }
          }
        });
    } else {
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);
      this.markControlsAsTouched(this.appContact.form);
    }
  }

  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   * */
  private submitPayment(invoiceType: "primary" | "secondary") {
    let payMethod = this.paymentDataService.getPaymentURI("primaryInvoice", this.application.id);
    if (invoiceType === "secondary") {
      payMethod = this.paymentDataService.getPaymentURI("secondaryInvoice", this.application.id);
    }
    return payMethod
      .pipe(takeWhile(() => this.componentActive))
      .pipe(mergeMap(jsonUrl => {
        window.location.href = jsonUrl["url"];
        return jsonUrl["url"];
      },
        (err: any) => {
          if (err === "Payment already made") {
            this.snackBar.open("Application payment has already been made, please refresh the page.",
              "Fail",
              { duration: 3500, panelClass: ["red-snackbar"] });
          }
        }));
  }

  showLiquorCostColumn(item: any){
    //const show = this.form.get(item.formControlName).value === true
    const show = true
    && (item.name !== 'Internal Transfer of Shares')
    && !(item.name === 'Change of Directors or Officers' && this.account.businessType  === 'PrivateCorporation');
    return show;
  }

  getValidationErrorMap() {
    const errorMap = {
      signatureAgreement:
        "Please affirm that all of the information provided for this application is true and complete",
      authorizedToSubmit: "Please affirm that you are authorized to submit the application",
      contactPersonEmail: "Please enter the business contact's email address",
      contactPersonFirstName: "Please enter the business contact's first name",
      contactPersonLastName: "Please enter the business contact's last name",
      contactPersonPhone: "Please enter the business contact's 10-digit phone number",
      contactPersonRole: "Please enter the contact person role"
    };
    return errorMap;
  }

}

const masterChangeList = [
  {
    name: "Internal Transfer of Shares",
    formControlName: "csInternalTransferOfShares",
    availableTo: ["PrivateCorporation", "LimitedLiabilityPartnership"],
    CannabisFee: 110,
    LiquorFee: 110,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When shares or partnership units are redistributed between existing shareholders/partners  but no new shareholders/partners are added to the business structure (if new shareholders are added, see external transfer of shares)',
      'Removal of shareholders/unit holders',
      'Amalgamations that do not add new shareholders or legal entities to the licensee  corporation',
      'Holding companies within the licensee corporation and/or third party operators should also complete this section when an internal share transfer or an amalgamation occurs'
    ],
    ////helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "External Transfer of Shares",
    formControlName: "csExternalTransferOfShares",
    availableTo: ["PrivateCorporation", "LimitedLiabilityPartnership"],
    CannabisFee: 330,
    LiquorFee: 330,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When new shareholders (companies or individuals) have been added to the licensee corporation or holding companies as a result of a transfer of existing shares or the issuance of new shares',
      'Amalgamations that add new shareholders or legal entities to the licensee corporation',
      'Third party operators should also complete this section when an external transfer occurs'
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "Change of Directors or Officers",
    formControlName: "csChangeOfDirectorsOrOfficers",
    availableTo: ["PrivateCorporation", "PublicCorporation", "Society", "Coop", "MilitaryMess","LocalGovernment", "University"],
    CannabisFee: 500,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['For liquor licensees - when there are changes in directors or officers of a public corporation or society that holds a licence, or of a public corporation or society within the licensee legal entity',
      'For cannabis licensees – when there are changes in directors or officers of a private or public  corporation or society that holds a licence, or of a public or private corporation or society within the licensee legal entity'
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "Name Change, Licensee -- Corporation",
    otherName: "Name Change, Licensee -- Organization",
    formControlName: "csNameChangeLicenseeCorporation",
    availableTo: ["PrivateCorporation", "PublicCorporation", "SoleProprietorship", "Coop", "MilitaryMess","University"],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: "Use this option to report:",
    helpText: [
      "When a corporation with an interest in a licence has legally changed its name, but existing corporate shareholders, directors and officers, and certificate number on the certificate of incorporation have not changed"
    ],

    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "Name Change, Licensee -- Partnership",
    formControlName: "csNameChangeLicenseePartnership",
    availableTo: ["GeneralPartnership", "Partnership", "LimitedLiabilityPartnership"],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When a person holding an interest in a licence has legally changed their name'
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',

  },
  {
    name: "Name Change, Licensee -- Society",
    formControlName: "csNameChangeLicenseeSociety",
    availableTo: ["Society"],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: "Use this option to report:",
    helpText: [
      "When the legal name of a society is changed, but the society structure, membership and certification number on the certificate of incorporation does not change"
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "Name Change, Person",
    formControlName: "csNameChangeLicenseePerson",
    availableTo: [
      "PrivateCorporation", "PublicCorporation", "GeneralPartnership", "Partnership",
      "LimitedLiabilityPartnership", "IndigenousNation", "LocalGovernment", "Society",
      "Coop", "MilitaryMess"
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: "Use this option to report:",
    helpText: [
      "when a person holding an interest in a licence has legally changed their name"
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: "Addition of Receiver or Executor",
    formControlName: "csAdditionalReceiverOrExecutor",
    availableTo: [
      "PrivateCorporation", "PublicCorporation", "GeneralPartnership", "Partnership",
      "LimitedLiabilityPartnership", "Society", "MilitaryMess"
    ],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['Upon the death, bankruptcy or receivership of a licensee'
    ],
    //helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  }
];
