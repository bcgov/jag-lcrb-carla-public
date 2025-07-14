import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { permanentChangeTypesOfChanges } from '@app/constants/permanent-change-types-of-changes';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';
import { TiedHouseViewMode } from '@models/tied-house-connection.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import {
  ContactData,
  PermanentChangeContactComponent
} from '@shared/components/permanent-change/permanent-change-contact/permanent-change-contact.component';
import { FormBase } from '@shared/form-base';
import { Observable, of } from 'rxjs';
import { catchError, filter, mergeMap, takeWhile } from 'rxjs/operators';
import { TiedHouseDeclarationComponent } from '../tied-house-decleration/tied-house-declaration.component';

export const SharepointNameRegex = /^[^~#%&*{}\\:<>?/+|""]*$/;

@Component({
  selector: 'app-permanent-change-to-a-licensee',
  templateUrl: './permanent-change-to-a-licensee.component.html',
  styleUrls: ['./permanent-change-to-a-licensee.component.scss']
})
export class PermanentChangeToALicenseeComponent extends FormBase implements OnInit {
  faQuestionCircle = faQuestionCircle;
  faIdCard = faIdCard;

  application: Application;
  liquorLicences: ApplicationLicenseSummary[] = [];
  cannabisLicences: ApplicationLicenseSummary[] = [];
  account: Account;
  applicationContact: ContactData;

  businessType: string;
  submitApplicationInProgress: boolean;
  showValidationMessages: boolean;
  invoiceType: any;
  dataLoaded: boolean;
  primaryPaymentInProgress: boolean;
  secondaryPaymentInProgress: boolean;
  verifyRquestMade: boolean;
  validationMessages: string[];

  @ViewChild('appContact')
  appContact: PermanentChangeContactComponent;

  @ViewChild('tiedHouseDeclaration')
  tiedHouseDeclaration: TiedHouseDeclarationComponent;

  appContactDisabled: boolean;
  applicationId: string;
  canCreateNewApplication: boolean;
  createApplicationInProgress: boolean;
  primaryInvoice: any;
  secondaryInvoice: any;

  uploadedCentralSecuritiesRegister: 0;
  uploadedIndividualsWithLessThan10: 0;
  uploadedCertificateOfAmalgamation: 0;
  uploadedNOAAmalgamation: 0;
  uploadedRegisterOfDirectorsAndOfficers: 0;
  uploadedNOA: 0;
  uploadedNameChangeDocuments: 0;
  uploadedCertificateOfNameChange: 0;
  uploadedPartnershipRegistration: 0;
  uploadedSocietyNameChange: 0;
  uploadedExecutorDocuments: 0;
  uploadedlDeathCertificateDocuments: 0;
  uploadedletterOfIntentDocuments: 0;
  uploadedCAS: 0;
  uploadedFinancialIntegrity: 0;

  get hasLiquor(): boolean {
    return this.liquorLicences.length > 0;
  }

  get hasCannabis(): boolean {
    return this.cannabisLicences.length > 0;
  }

  changeList = [];

  get selectedChangeList() {
    return this.changeList.filter((item) => this.form && this.form.get(item.formControlName).value === true);
  }

  form: FormGroup;

  constructor(
    private applicationDataService: ApplicationDataService,
    private paymentDataService: PaymentDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private store: Store<AppState>,
    private matDialog: MatDialog
  ) {
    super();
    this.store
      .select((state) => state.currentAccountState.currentAccount)
      .pipe(filter((account) => !!account))
      .subscribe((account) => {
        this.account = account;
        this.changeList = permanentChangeTypesOfChanges.filter(
          (item) => !!item.availableTo.find((bt) => bt === account.businessType)
        );
      });

    this.route.paramMap.subscribe((pmap) => {
      this.invoiceType = pmap.get('invoiceType');
      this.applicationId = pmap.get('applicationId');
    });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      csInternalTransferOfShares: [''],
      csExternalTransferOfShares: [''],
      csChangeOfDirectorsOrOfficers: [''],
      csNameChangeLicenseeCorporation: [''],
      csNameChangeLicenseePartnership: [''],
      csNameChangeLicenseeSociety: [''],
      csNameChangeLicenseePerson: [''],
      csAdditionalReceiverOrExecutor: [''],
      csTiedHouseDeclaration: [''],
      firstNameOld: [''],
      firstNameNew: [''],
      lastNameOld: [''],
      lastNameNew: [''],
      description2: [''],
      description3: ['', Validators.pattern(SharepointNameRegex)],
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]]
    });

    this.loadData();
  }

  /**
   * Loads the form data.
   *
   * @private
   * @memberof PermanentChangeToALicenseeComponent
   */
  private loadData() {
    const sub = this.applicationDataService.getPermanentChangesToLicenseeData(this.applicationId).subscribe({
      next: (data) => {
        this.setFormData(data);
      },
      error: (error) => {
        console.error('Error loading form data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Form Data',
            message: 'Failed to load form data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
    this.subscriptionList.push(sub);
  }

  /**
   * Navigates to the application create page.
   *
   * @memberof PermanentChangeToALicenseeComponent
   */
  onNewApplication(_invoiceType: 'primary' | 'secondary') {
    this.router.navigateByUrl(`/permanent-change-to-a-licensee`);
  }

  /**
   * Sets the form data based on the provided application, licences, and invoice information.
   *
   * @private
   * @param {*} { application, licences, primary, secondary }
   * @memberof PermanentChangeToALicenseeComponent
   */
  private setFormData({ application, licences, primary, secondary }) {
    this.liquorLicences = licences.filter((item) => item.licenceTypeCategory === 'Liquor' && item.status === 'Active');
    this.cannabisLicences = licences.filter(
      (item) => item.licenceTypeCategory === 'Cannabis' && item.status === 'Active'
    );
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
    if (
      (!this.hasCannabis || this.application.primaryInvoicePaid) &&
      (!this.hasLiquor || this.application.secondaryInvoicePaid)
    ) {
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
   * Saves the application data.
   *
   * @param {boolean} [showProgress=false]
   * @param {Application} [appData={} as Application]
   * @return {*}  {Observable<[boolean, Application]>}
   * @memberof PermanentChangeToALicenseeComponent
   */
  private save(
    showProgress: boolean = false,
    appData: Application = {} as Application
  ): Observable<[boolean, Application]> {
    return this.applicationDataService
      .updateApplication({
        ...this.application,
        ...this.form.value,
        ...this.appContact.form.value,
        ...appData,
        tiedHouseConnections: this.tiedHouseDeclaration.tiedHouseDeclarations
      })
      .pipe(takeWhile(() => this.componentActive))
      .pipe(
        catchError(() => {
          this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          const res: [boolean, Application] = [false, null];
          return of(res);
        })
      )
      .pipe(
        mergeMap((data) => {
          if (showProgress === true) {
            this.snackBar.open('Application has been saved', 'Success', {
              duration: 2500,
              panelClass: ['green-snackbar']
            });
          }
          const res: [boolean, Application] = [true, data as Application];
          return of(res);
        })
      );
  }

  /**
   * Checks if the form is valid and collects validation messages.
   *
   * @return {*}  {boolean}
   * @memberof PermanentChangeToALicenseeComponent
   */
  private isValid(): boolean {
    this.showValidationMessages = false;
    this.validationMessages = [];
    this.validationMessages = this.listControlsWithErrors(this.form, this.getValidationErrorMap());
    let valid = this.form.disabled || this.form.valid;

    const securitiesDocIsRequired =
      this.form.get('csInternalTransferOfShares').value || this.form.get('csExternalTransferOfShares').value;
    if (securitiesDocIsRequired && this.uploadedCentralSecuritiesRegister < 1) {
      this.validationMessages.push('At least one Central Securities Register document is required.');
      valid = false;
    }

    const noticeOfArticlesDocIsRequired = this.form.get('csChangeOfDirectorsOrOfficers').value;
    if (noticeOfArticlesDocIsRequired && this.uploadedNOA < 1) {
      this.validationMessages.push('The Notice of Articles document is required.');
      valid = false;
    }

    const partnershipRegistrationDocIsRequired = this.form.get('csNameChangeLicenseeSociety').value;
    if (partnershipRegistrationDocIsRequired && this.uploadedSocietyNameChange < 1) {
      this.validationMessages.push('A Partnership Registration document is required.');
      valid = false;
    }

    const corpNameChangeDocIsRequired = this.form.get('csNameChangeLicenseeCorporation').value;
    if (corpNameChangeDocIsRequired && this.uploadedCertificateOfNameChange < 1) {
      this.validationMessages.push('A copy of the Certificate of Change of Name Form is required.');
      valid = false;
    }

    const partnerLicenseeNameChangeDocIsRequired = this.form.get('csNameChangeLicenseePartnership').value;
    if (partnerLicenseeNameChangeDocIsRequired && this.uploadedPartnershipRegistration < 1) {
      this.validationMessages.push('A Change of Name document is required.');
      valid = false;
    }

    const nameChangeDocIsRequired = this.form.get('csNameChangeLicenseePerson').value;
    if (nameChangeDocIsRequired && this.uploadedNameChangeDocuments < 1) {
      this.validationMessages.push('A Change of Name document is required.');
      valid = false;
    }

    const executorDocIsRequired = this.form.get('csAdditionalReceiverOrExecutor').value && this.form.get('');
    if (executorDocIsRequired && this.uploadedExecutorDocuments < 1) {
      this.validationMessages.push(
        'Please upload a copy of Assignment of Executor, a copy of the last will(s) and testament(s) or a copy of the Death Certificate.'
      );
      valid = false;
    }

    const tiedHouseDeclarationIsRequired = this.form.get('csTiedHouseDeclaration').value;
    if (tiedHouseDeclarationIsRequired && this.isTiedHouseValid()) {
      this.validationMessages.push('A Tide House Declaration is required.');
      valid = false;
    }

    return valid;
  }

  isTiedHouseValid() {
    return (
      this.tiedHouseDeclaration.tiedHouseDeclarations.length < 1 &&
      !this.tiedHouseDeclaration.tiedHouseDeclarations.find((th) =>
        // If any declarations are in a new or edit mode, then they have not yet been saved, and the tied house form
        // is therefore not valid (and not ready to be submitted as part of the larger form).
        [TiedHouseViewMode.new, TiedHouseViewMode.addNewRelationship, TiedHouseViewMode.editExistingRecord].includes(
          th.viewMode
        )
      )
    );
  }

  /**
   * Submit the application for payment.
   *
   * @param {('primary' | 'secondary')} invoiceType
   * @memberof PermanentChangeToALicenseeComponent
   */
  onSubmit(invoiceType: 'primary' | 'secondary') {
    if (!this.isValid()) {
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);
      this.markControlsAsTouched(this.appContact.form);

      return;
    }

    if (invoiceType === 'primary') {
      this.primaryPaymentInProgress = true;
    } else {
      this.secondaryPaymentInProgress = true;
    }
    this.submitApplicationInProgress = true;
    var trigInv = 0;
    if (this.application.licenceFeeInvoice == null) {
      trigInv = 1;
    } else {
      if (this.application.licenceFeeInvoice.statuscode == 3) {
        //cancelled
        trigInv = 1;
      }
    }
    this.save(!this.application.applicationType.isFree, { invoiceTrigger: trigInv } as Application) // trigger invoice generation when saving LCSD6564 Only if not present or cancelled.
      .pipe(takeWhile(() => this.componentActive))
      .subscribe({
        next: ([saveSucceeded, app]) => {
          if (saveSucceeded) {
            if (app) {
              this.submitPayment(invoiceType).subscribe({
                next: () => {
                  if (invoiceType === 'primary') {
                    this.primaryPaymentInProgress = false;
                  } else {
                    this.secondaryPaymentInProgress = false;
                  }
                },
                error: (error) => {
                  console.error('Error submitting payment', error);
                  this.matDialog.open(GenericMessageDialogComponent, {
                    data: {
                      title: 'Error Submitting Payment',
                      message:
                        'Failed to submit payment. Please try again. If the problem persists, please contact support.',
                      closeButtonText: 'Close'
                    }
                  });
                }
              });
            }
          } else if (this.application.applicationType.isFree) {
            // show error message the save failed and the application is free
            this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
            if (invoiceType === 'primary') {
              this.primaryPaymentInProgress = false;
            } else {
              this.secondaryPaymentInProgress = false;
            }
          }
        },
        error: (error) => {
          console.error('Error saving form data', error);
          this.matDialog.open(GenericMessageDialogComponent, {
            data: {
              title: 'Error Saving Form Data',
              message: 'Failed to save form data. Please try again. If the problem persists, please contact support.',
              closeButtonText: 'Close'
            }
          });
        }
      });
  }

  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   *
   * @private
   * @param {('primary' | 'secondary')} invoiceType
   * @return {*}
   * @memberof PermanentChangeToALicenseeComponent
   */
  private submitPayment(invoiceType: 'primary' | 'secondary') {
    let payMethod = this.paymentDataService.getPaymentURI('primaryInvoice', this.application.id);
    if (invoiceType === 'secondary') {
      payMethod = this.paymentDataService.getPaymentURI('secondaryInvoice', this.application.id);
    }
    return payMethod.pipe(takeWhile(() => this.componentActive)).pipe(
      mergeMap(
        (jsonUrl) => {
          window.location.href = jsonUrl['url'];
          return jsonUrl['url'];
        },
        (err: any) => {
          if (err === 'Payment already made') {
            this.snackBar.open('Application payment has already been made, please refresh the page.', 'Fail', {
              duration: 3500,
              panelClass: ['red-snackbar']
            });
          }
        }
      )
    );
  }

  /**
   * Returns a map of validation error messages for the form controls.
   *
   * @return {*}
   * @memberof PermanentChangeToALicenseeComponent
   */
  getValidationErrorMap() {
    const errorMap = {
      signatureAgreement:
        'Please affirm that all of the information provided for this application is true and complete',
      authorizedToSubmit: 'Please affirm that you are authorized to submit the application',
      contactPersonEmail: "Please enter the business contact's email address",
      contactPersonFirstName: "Please enter the business contact's first name",
      contactPersonLastName: "Please enter the business contact's last name",
      contactPersonPhone: "Please enter the business contact's 10-digit phone number",
      contactPersonRole: 'Please enter the contact person role',
      description3: 'The following characters are not allowed in a Company Name: ~ # % & * { } \\ : < > ? / + | "'
    };
    return errorMap;
  }
}
