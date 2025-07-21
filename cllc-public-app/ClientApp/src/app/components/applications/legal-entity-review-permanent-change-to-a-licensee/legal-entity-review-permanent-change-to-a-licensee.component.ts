import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import {
  permanentChangeTypesOfChanges,
  PermanentChangeTypesOfChangesOption
} from '@app/constants/permanent-change-types-of-changes';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { FormBase } from '@shared/form-base';
import { EMPTY, Observable, of } from 'rxjs';
import { catchError, filter, mergeMap, takeWhile } from 'rxjs/operators';

export const SharepointNameRegex = /^[^~#%&*{}\\:<>?/+|""]*$/;

/**
 * A component that displays a form page for a legal entity review permanent change to a licensee application.
 *
 * This is step 2 of the legal entity review process, where the user pays for the legal entity review changes.
 * For step 1, see `LegalEntityReviewComponent`.
 *
 * @export
 * @class LegalEntityReviewPermanentChangeToALicenseeComponent
 * @extends {FormBase}
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-permanent-change-to-a-licensee',
  templateUrl: './legal-entity-review-permanent-change-to-a-licensee.component.html',
  styleUrls: ['./legal-entity-review-permanent-change-to-a-licensee.component.scss']
})
export class LegalEntityReviewPermanentChangeToALicenseeComponent extends FormBase implements OnInit {
  account: Account;
  application: Application;
  liquorLicences: ApplicationLicenseSummary[] = [];
  cannabisLicences: ApplicationLicenseSummary[] = [];

  applicationId: string;
  primaryInvoice: any;
  secondaryInvoice: any;

  invoiceType: string;

  hasDataLoaded: boolean = false;

  validationMessages: string[] = [];
  showValidationMessages: boolean = false;

  submitApplicationInProgress: boolean = false;
  primaryPaymentInProgress: boolean = false;
  secondaryPaymentInProgress: boolean = false;
  changeList: any;

  get hasLiquor(): boolean {
    return this.liquorLicences.length > 0;
  }

  get hasCannabis(): boolean {
    return this.cannabisLicences.length > 0;
  }

  get selectedChangeList() {
    return this.changeList?.filter((item) => this.form && this.form.get(item.formControlName).value === true);
  }

  /**
   * The list of changes required for this legal entity review permanent change application.
   *
   * @type {PermanentChangeTypesOfChangesOption[]}
   */
  changesRequired: PermanentChangeTypesOfChangesOption[] = [];

  form: FormGroup;

  constructor(
    private applicationDataService: ApplicationDataService,
    private paymentDataService: PaymentDataService,
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
      });

    this.route.paramMap.subscribe((pmap) => {
      this.invoiceType = pmap.get('invoiceType');
      this.applicationId = pmap.get('applicationId');
    });
  }

  ngOnInit(): void {
    this.initForm();
    this.loadData();
  }

  /**
   * Initializes the form with required controls and validators.
   *
   */
  initForm(): void {
    this.form = this.fb.group({
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
      csInternalTransferOfShares: [''],
      csExternalTransferOfShares: [''],
      csChangeOfDirectorsOrOfficers: [''],
      csNameChangeLicenseeCorporation: [''],
      csNameChangeLicenseePartnership: [''],
      csNameChangeLicenseeSociety: [''],
      csNameChangeLicenseePerson: [''],
      csAdditionalReceiverOrExecutor: [''],
      csTiedHouseDeclaration: ['']
    });
  }

  /**
   * Loads the form data.
   *
   * @private
   * @memberof PermanentChangeToALicenseeComponent
   */
  private loadData() {
    const sub = this.applicationDataService.getLegalEntityPermanentChangesToLicenseeData(this.applicationId).subscribe({
      next: (data) => {
        this.setFormData(data);
        this.getChangeList();
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
   * Sets the form data based on the provided application, licences, and invoice information.
   *
   * @private
   * @param {*} { application, licences, primary, secondary }
   * @memberof PermanentChangeToALicenseeComponent
   */
  private setFormData({ application, licences, primary, secondary }) {
    this.application = application;

    this.liquorLicences = licences.filter((item) => item.licenceTypeCategory === 'Liquor' && item.status === 'Active');
    this.cannabisLicences = licences.filter(
      (item) => item.licenceTypeCategory === 'Cannabis' && item.status === 'Active'
    );

    this.primaryInvoice = primary;
    this.secondaryInvoice = secondary;

    // TODO Update this to pull the list of changes from the correct properties
    this.changesRequired = application.permanentChangeTypesOfChangesRequired || [];

    const isPrimaryInvoiceInfoMissing: boolean = primary?.isApproved && !this.application.primaryInvoicePaid;
    const isSecondaryInvoiceInfoMissing: boolean = secondary?.isApproved && !this.application.secondaryInvoicePaid;

    if (isPrimaryInvoiceInfoMissing || isSecondaryInvoiceInfoMissing) {
      // The asynchronous workflow in dynamics has not yet run. Pause for a second and get data again.
      setTimeout(() => {
        this.loadData();
      }, 1000);

      return;
    }

    // If any payment was made, disable the form
    if (this.application.primaryInvoicePaid || this.application.secondaryInvoicePaid) {
      this.form.disable();
    }

    this.application.csChangeOfDirectorsOrOfficers = true;
    this.application.csTiedHouseDeclaration = true;

    this.form.patchValue(application);

    this.hasDataLoaded = true;
  }

  /**
   * Submit the application for payment.
   *
   * @param {('primary' | 'secondary')} invoiceType
   * @return {*}
   */
  onSubmit(invoiceType: 'primary' | 'secondary') {
    if (!this.isValid()) {
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);

      return;
    }

    if (invoiceType === 'primary') {
      this.primaryPaymentInProgress = true;
    } else {
      this.secondaryPaymentInProgress = true;
    }

    this.submitApplicationInProgress = true;

    let invoiceTrigger = 0;

    if (this.application.licenceFeeInvoice == null) {
      invoiceTrigger = 1;
    } else if (this.application.licenceFeeInvoice.statuscode == 3) {
      // Cancelled
      invoiceTrigger = 1;
    }

    this.save(!this.application.applicationType.isFree, { invoiceTrigger: invoiceTrigger }) // trigger invoice generation when saving LCSD6564 Only if not present or cancelled.
      .pipe(takeWhile(() => this.componentActive))
      .subscribe({
        next: ([saveSucceeded, app]) => {
          if (saveSucceeded && app) {
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
   * Checks if the form is valid and collects validation messages.
   *
   * @private
   * @return {*}  {boolean}
   */
  private isValid(): boolean {
    this.showValidationMessages = false;

    this.validationMessages = this.listControlsWithErrors(this.form, this.getValidationErrorMap());

    let isValid: boolean = this.form.disabled || this.form.valid;

    return isValid;
  }

  /**
   * Saves the application data.
   *
   * Merges the initial application data with the form data values and any additional data provided.
   *
   * @private
   * @param {boolean} [showProgress=false]
   * @param {Partial<Application>} applicationData Additional application data to save, which will override any matching
   * values from the initial application data OR from the form data.
   * @return {*}  {Observable<[boolean, Application]>}
   */
  private save(
    showProgress: boolean = false,
    applicationData: Partial<Application>
  ): Observable<[boolean, Application]> {
    return this.applicationDataService
      .updateApplication({
        ...this.application,
        ...this.form.value,
        ...applicationData
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
   * Redirect to payment processing page (Express Pay / Bambora service)
   *
   * @private
   * @param {('primary' | 'secondary')} invoiceType
   * @return {*}
   */
  private submitPayment(invoiceType: 'primary' | 'secondary') {
    let payMethod: Observable<object>;

    if (invoiceType === 'primary') {
      payMethod = this.paymentDataService.getLegalEntityPaymentURI('primaryInvoice', this.application.id);
    } else if (invoiceType === 'secondary') {
      payMethod = this.paymentDataService.getLegalEntityPaymentURI('secondaryInvoice', this.application.id);
    }

    return payMethod.pipe(takeWhile(() => this.componentActive)).pipe(
      mergeMap((jsonUrl) => {
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      }),
      catchError((error: any) => {
        if (error === 'Payment already made') {
          this.snackBar.open('Application payment has already been made, please refresh the page.', 'Fail', {
            duration: 3500,
            panelClass: ['red-snackbar']
          });
        }

        return EMPTY;
      })
    );
  }

  /**
   * Returns a map of validation error messages for the form controls.
   *
   * @return {*}
   */
  getValidationErrorMap() {
    const errorMap = {
      signatureAgreement:
        'Please affirm that all of the information provided for this application is true and complete',
      authorizedToSubmit: 'Please affirm that you are authorized to submit the application'
    };

    return errorMap;
  }

  private getChangeList() {
    this.changeList = permanentChangeTypesOfChanges.filter(
      (item) => !!item.availableTo.find((bt) => bt === this.account.businessType)
    );
  }
}
