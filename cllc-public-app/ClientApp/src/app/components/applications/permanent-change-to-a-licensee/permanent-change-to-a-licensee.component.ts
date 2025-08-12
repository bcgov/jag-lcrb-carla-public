import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import {
  AccountType,
  PCLFormControlDefinitionOption,
  PCLFormControlName,
  PCLMatrixLicenceGroup
} from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-types';
import {
  getPCLMatrixConditionalGroup,
  getPCLMatrixGroup,
  getPCLMatrixSectionBusinessRules
} from '@components/applications/permanent-change-to-a-licensee/pcl-business-rules/pcl-bussiness-rules-utils';
import { faIdCard } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';
import { TiedHouseViewMode } from '@models/tied-house-connection.model';
import { AccountDataService } from '@services/account-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import {
  ContactData,
  PermanentChangeContactComponent
} from '@shared/components/permanent-change/permanent-change-contact/permanent-change-contact.component';
import { FormBase } from '@shared/form-base';
import { debounce } from 'lodash';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, map, mergeMap, takeWhile } from 'rxjs/operators';
import { TiedHouseDeclarationComponent } from '../tied-house-decleration/tied-house-declaration.component';

export const SharepointNameRegex = /^[^~#%&*{}\\:<>?/+|""]*$/;

@Component({
  selector: 'app-permanent-change-to-a-licensee',
  templateUrl: './permanent-change-to-a-licensee.component.html',
  styleUrls: ['./permanent-change-to-a-licensee.component.scss']
})
export class PermanentChangeToALicenseeComponent extends FormBase implements OnInit, OnDestroy {
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
  validationMessages: string[];

  @ViewChild('appContact')
  appContact: PermanentChangeContactComponent;

  @ViewChild('tiedHouseDeclaration')
  tiedHouseDeclaration: TiedHouseDeclarationComponent;

  appContactDisabled: boolean;
  formDisabled: boolean;
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

  /**
   * `true` if the user has any liquor licenses.
   *
   * @readonly
   * @type {boolean}
   */
  get hasLiquor(): boolean {
    return this.liquorLicences.length > 0;
  }

  /**
   * `true` if the user has any cannabis licenses.
   *
   * @readonly
   * @type {boolean}
   */
  get hasCannabis(): boolean {
    return this.cannabisLicences.length > 0;
  }

  changeList: PCLFormControlDefinitionOption[] = [];

  get selectedChangeList() {
    return this.changeList.filter((item) => this.form && this.form.get(item.formControlName).value === true);
  }

  form: FormGroup;

  // PCL Matrix business rules variables
  _PCLMatrixAccountType: AccountType;
  _PCLMatrixLicenceGroup: PCLMatrixLicenceGroup;
  _PCLMatrixEnabledSections: PCLFormControlName[] = [];

  constructor(
    private applicationDataService: ApplicationDataService,
    private paymentDataService: PaymentDataService,
    private accountDataService: AccountDataService,
    private legalEntityDataService: LegalEntityDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private matDialog: MatDialog
  ) {
    super();

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

    const _PCLSectionFormControlNames = Object.values(PCLFormControlName);

    this.form.valueChanges.subscribe((values) => {
      if (_PCLSectionFormControlNames.some((key) => key in values)) {
        // If any of the PCL section form controls change, update the business rules
        this._PCLMatrixOnFormControlChanges();
      }

      if (!this.form.disabled) {
        // Quietly auto save the application data when form changes, if it is still active/editable.
        this.saveDebounced({ invoiceTrigger: 0 } as Partial<Application>).subscribe({
          next: ([saveSucceeded, app]) => {
            if (saveSucceeded) {
              this.application = app;
            }
          },
          error: (error) => {
            console.error('Error saving form data', error);
          }
        });
      }
    });

    this.loadData();
  }

  /**
   * Updates the PCL business rules based on the selected form controls.
   */
  _PCLMatrixOnFormControlChanges() {
    const _PCLSectionSelectedFormControlNames = (Object.entries(this.form.value) as [PCLFormControlName, any][])
      .filter(([_, value]) => value === true)
      .map(([key]) => key);

    const pclMatrixConditionalGroup = getPCLMatrixConditionalGroup({
      selectedPCLSections: _PCLSectionSelectedFormControlNames
    });

    const businessRules = getPCLMatrixSectionBusinessRules({
      accountType: this._PCLMatrixAccountType,
      conditionalGroup: pclMatrixConditionalGroup,
      licenceGroup: this._PCLMatrixLicenceGroup
    });

    this.changeList = businessRules;
  }

  /**
   * Loads the form data.
   *
   * @private
   * @memberof PermanentChangeToALicenseeComponent
   */
  private loadData() {
    const accountData$ = forkJoin({
      accountData: this.accountDataService.getCurrentAccount(),
      legalEntityData: this.legalEntityDataService.getBusinessProfileSummary()
    }).pipe(
      map(({ accountData, legalEntityData }) => {
        const account = accountData;
        account.legalEntity = legalEntityData?.length ? legalEntityData[0] : null;
        return Object.assign(new Account(), account);
      })
    );
    const permanentChangeData$ = this.applicationDataService.getPermanentChangesToLicenseeData(this.applicationId);
    const accountSummary$ = this.accountDataService.getAccountSummary();

    const sub = forkJoin({
      accountData: accountData$,
      permanentChangeData: permanentChangeData$,
      accountSummaryData: accountSummary$
    }).subscribe({
      next: ({ accountData, permanentChangeData, accountSummaryData }) => {
        this.account = accountData;

        this.setFormData(permanentChangeData);

        this._PCLMatrixAccountType = this.account.businessType as AccountType;
        this._PCLMatrixLicenceGroup = getPCLMatrixGroup(accountSummaryData);
        this._PCLMatrixOnFormControlChanges();
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
        this.formDisabled = true;
      }
      this.form.patchValue(application);
      this.dataLoaded = true;
    }
  }

  private _debouncedSave = debounce(
    (applicationDataOverrides: Partial<Application> = {}, observer: (value: [boolean, Application]) => void) => {
      this._save(false, applicationDataOverrides).subscribe(observer);
    },
    2000 // ms debounce delay
  );

  private _save(
    showProgress: boolean = false,
    applicationDataOverrides: Partial<Application> = {}
  ): Observable<[boolean, Application]> {
    const applicationDataForSave = this._getApplicationDataForSave(applicationDataOverrides);

    return this.applicationDataService
      .updateApplication(applicationDataForSave)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(
        catchError(() => {
          if (showProgress) {
            this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          }

          return of<[boolean, Application]>([false, null]);
        })
      )
      .pipe(
        mergeMap((data) => {
          if (data[0] !== false && showProgress) {
            this.snackBar.open('Application has been saved', 'Success', {
              duration: 2500,
              panelClass: ['green-snackbar']
            });
          }

          return of<[boolean, Application]>([true, data as Application]);
        })
      );
  }

  /**
   * Prepares the application data for saving.
   *
   * @private
   * @param {Partial<Application>} [applicationDataOverrides={}] Optional data that will override any existing
   * application data.
   * @return {*}  {Application}
   */
  private _getApplicationDataForSave(applicationDataOverrides: Partial<Application> = {}): Application {
    let formData = this.form.value;

    /*
     * Business Rule:
     * Tied House Declaration changes, like all changes, normally cost the user a fee. However, if the user is
     * submitting Internal Transfer of Shares, or External Transfer of Shares, or Change of Directors or Officers
     * changes, then the fee for the Tied House Declaration changes is waived (it is covered under the fee for the other
     * changes). In order to accommodate this, we must ensure that `csTiedHouseDeclaration` is `false`. These booleans
     * control which changes dynamics generates invoices for.
     *
     * See related business rules: `isTiedHouseDeclarationVisible`
     */
    if (
      formData.csInternalTransferOfShares === true ||
      formData.csExternalTransferOfShares === true ||
      formData.csChangeOfDirectorsOrOfficers === true
    ) {
      formData.csTiedHouseDeclaration = false;
    }

    return {
      ...this.application,
      ...formData,
      ...this.appContact.form.value,
      ...applicationDataOverrides
    };
  }

  /**
   * Saves the application data.
   *
   * @param {boolean} [showProgress=false]
   * @param {Partial<Application>} [applicationDataOverrides={}]
   * @return {*}  {Observable<[boolean, Application]>}
   */
  public save(
    showProgress: boolean = false,
    applicationDataOverrides: Partial<Application> = {}
  ): Observable<[boolean, Application]> {
    return this._save(showProgress, applicationDataOverrides);
  }

  /**
   * Debounced save function to prevent excessive API calls.
   *
   * @param {Partial<Application>} [applicationDataOverrides={}]
   * @return {*}  {Observable<[boolean, Application]>}
   */
  public saveDebounced(applicationDataOverrides: Partial<Application> = {}): Observable<[boolean, Application]> {
    return new Observable<[boolean, Application]>((subscriber) => {
      this._debouncedSave(applicationDataOverrides, (result) => {
        subscriber.next(result);
        subscriber.complete();
      });
    });
  }

  /**
   * Checks if the form is valid and collects validation messages.
   *
   * @return {*}  {boolean}
   * @memberof PermanentChangeToALicenseeComponent
   */
  private isValid(): boolean {
    this.showValidationMessages = false;
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

    if (!this.areTiedHouseDeclarationsValid()) {
      this.validationMessages.push('Tide House Declaration(s) have not been saved.');
      valid = false;
    }

    return valid;
  }

  /**
   * Checks if the tied house declarations are valid.
   *
   * @return {*}  {boolean} `true` if valid, `false` otherwise.
   */
  areTiedHouseDeclarationsValid(): boolean {
    if (
      this.tiedHouseDeclaration.tiedHouseDeclarations.find((item) =>
        [TiedHouseViewMode.new, TiedHouseViewMode.editExistingRecord, TiedHouseViewMode.addNewRelationship].includes(
          item.viewMode
        )
      )
    ) {
      // One or more declarations are in an unsaved state.
      return false;
    }

    return true;
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
    let payMethod: Observable<object>;

    if (invoiceType === 'primary') {
      payMethod = this.paymentDataService.getPermanentChangePaymentURI('primaryInvoice', this.application.id);
    } else if (invoiceType === 'secondary') {
      payMethod = this.paymentDataService.getPermanentChangePaymentURI('secondaryInvoice', this.application.id);
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

  /**
   * Indicates whether the Internal Transfer of Shares section is visible.
   *
   * @readonly
   */
  get isInternalTransferOfSharesVisible() {
    return this.form.get('csInternalTransferOfShares').value;
  }

  /**
   * Indicates whether the External Transfer of Shares section is visible.
   *
   * @readonly
   */
  get isExternalTransferOfSharesVisible() {
    return this.form.get('csExternalTransferOfShares').value;
  }

  /**
   * Indicates whether the Change of Directors or Officers section is visible.
   *
   * @readonly
   */
  get isChangeOfDirectorsOrOfficersVisible() {
    return this.form.get('csChangeOfDirectorsOrOfficers').value;
  }

  /**
   * Indicates whether the Name Change (Person) section is visible.
   *
   * @readonly
   */
  get isNameChangeLicenseePersonVisible() {
    return this.form.get('csNameChangeLicenseePerson').value;
  }

  /**
   * Indicates whether the Name Change (Corporation) section is visible.
   *
   * @readonly
   */
  get isNameChangeLicenseeCorporationVisible() {
    return this.form.get('csNameChangeLicenseeCorporation').value;
  }

  /**
   * Indicates whether the Name Change (Partnership) section is visible.
   *
   * @readonly
   */
  get isNameChangeLicenseePartnershipVisible() {
    return this.form.get('csNameChangeLicenseePartnership').value;
  }

  /**
   * Indicates whether the Name Change (Society) section is visible.
   *
   * @readonly
   */
  get isNameChangeLicenseeSocietyVisible() {
    return this.form.get('csNameChangeLicenseeSociety').value;
  }

  /**
   * Indicates whether the Additional Receiver or Executor section is visible.
   *
   * @readonly
   */
  get isAdditionalReceiverOrExecutorVisible() {
    return this.form.get('csAdditionalReceiverOrExecutor').value;
  }

  /**
   * Indicates whether the Tied House Declaration section is visible.
   *
   * Business Rule:
   * The Tied House Declaration section is visible if `csTiedHouseDeclaration` is `true` OR if any of the
   * following are `true`:
   * - csInternalTransferOfShares
   * - csExternalTransferOfShares
   * - csChangeOfDirectorsOrOfficers
   *
   * See related business rules: `_getApplicationDataForSave`
   *
   * @readonly
   */
  get isTiedHouseDeclarationVisible() {
    return (
      this.form.get('csTiedHouseDeclaration').value ||
      this.form.get('csExternalTransferOfShares').value ||
      this.form.get('csInternalTransferOfShares').value ||
      this.form.get('csChangeOfDirectorsOrOfficers').value
    );
  }

  /**
   * Indicates whether the payment section is visible.
   *
   * @readonly
   */
  get isPaymentVisible() {
    return (
      this.selectedChangeList?.length !== 0 &&
      (this.cannabisLicences?.length !== 0 || this.liquorLicences?.length !== 0)
    );
  }

  ngOnDestroy(): void {
    this._debouncedSave.cancel();
  }
}
