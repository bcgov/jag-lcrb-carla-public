import { Component, OnInit } from '@angular/core';
import { KeyValue } from '@angular/common';
import { Application } from '@models/application.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription, Observable, Subject, forkJoin, of } from 'rxjs';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { UPLOAD_FILES_MODE } from '@components/licences/licences.component';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { ApplicationCancellationDialogComponent } from '@components/dashboard/applications-and-licences/applications-and-licences.component';
import { FormBase, ApplicationHTMLContent } from '@shared/form-base';
import { Account } from '@models/account.model';
import { License } from '@models/license.model';

const ValidationErrorMap = {
  renewalCriminalOffenceCheck: 'Please answer question 1',
  renewalDUI: 'Please answer question 2',
  renewalBusinessType: 'Please answer question 3',
  renewalShareholders: 'Please answer question 4',
  renewalThirdParty: 'Please answer question 5',
  renewalFloorPlan: 'Please answer question 6',
  renewalTiedhouse: 'Please answer question 7',
  renewalUnreportedSaleOfBusiness: 'Please answer question 8',
  renewalValidInterest: 'Please answer question 9',
  renewalkeypersonnel: 'Please answer question 10',

  contactPersonFirstName: 'Please enter the business contact\'s first name',
  contactPersonLastName: 'Please enter the business contact\'s last name',
  contactPersonEmail: 'Please enter the business contact\'s email address',
  contactPersonPhone: 'Please enter the business contact\'s 10-digit phone number',
  authorizedToSubmit: 'Please affirm that you are authorized to submit the application',
  signatureAgreement: 'Please affirm that all of the information provided for this application is true and complete',
}

@Component({
  selector: 'app-liquor-renewal',
  templateUrl: './liquor-renewal.component.html',
  styleUrls: ['./liquor-renewal.component.scss']
})
export class LiquorRenewalComponent extends FormBase implements OnInit {
  establishmentWatchWords: KeyValue<string, boolean>[];
  application: Application;

  form: FormGroup;
  savedFormData: any;
  applicationId: string;
  busy: Subscription;
  accountId: string;
  // need access to the licence to handle subcategory cases
  licenseSubCategory: string;
  payMethod: string;
  validationMessages: any[];
  showValidationMessages: boolean;
  submittedApplications = 8;
  tiedHouseFormData: TiedHouseConnection;
  possibleProblematicNameWarning = false;
  htmlContent: ApplicationHTMLContent = <ApplicationHTMLContent>{};
  readonly UPLOAD_FILES_MODE = UPLOAD_FILES_MODE;
  ApplicationTypeNames = ApplicationTypeNames;
  FormControlState = FormControlState;
  mode: string;
  account: Account;

  uploadedSupportingDocuments = 0;
  uploadedFinancialIntegrityDocuments: 0;
  uploadedAssociateDocuments: 0;
  window = window;

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
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
    this.route.paramMap.subscribe(pmap => this.mode = pmap.get('mode'));
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],

      renewalBusinessType: ['', Validators.required],
      renewalCriminalOffenceCheck: ['', Validators.required],
      renewalDUI: ['', Validators.required],
      renewalFloorPlan: ['', Validators.required],
      renewalkeypersonnel: ['', Validators.required],
      renewalShareholders: ['', Validators.required],
      renewalThirdParty: ['', Validators.required],
      renewalTiedhouse: ['', Validators.required],
      renewalUnreportedSaleOfBusiness: ['', Validators.required],
      renewalValidInterest: ['', Validators.required],

      contactPersonFirstName: ['', Validators.required],
      contactPersonLastName: ['', Validators.required],
      contactPersonRole: [''],
      contactPersonEmail: ['', Validators.required],
      contactPersonPhone: ['', Validators.required],

      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]]
    });

    this.applicationDataService.getSubmittedApplicationCount()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => this.submittedApplications = value);

    this.establishmentWatchWordsService.initialize();

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });

    this.busy = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        this.busy = this.licenceDataService.getLicenceById(data.assignedLicence.id)
          .pipe(takeWhile(() => this.componentActive))
          .subscribe((data: License) => {
            if (data.licenseSubCategory !== null &&
              data.licenseSubCategory !== 'Independent Wine Store' &&
              data.licenseSubCategory !== 'Tourist Wine Store' &&
              data.licenseSubCategory !== 'Special Wine Store') {
              this.form.addControl('ldbOrderTotals', this.fb.control('', [Validators.required, Validators.min(0), Validators.max(10000000), Validators.pattern("^[0-9]*$")]));
              this.form.addControl('ldbOrderTotalsConfirm', this.fb.control('', [Validators.required]));
            }
          });
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, '');
        }

        this.application = data;
        this.licenseSubCategory = this.application.assignedLicence.licenseSubCategory;

        this.addDynamicContent();

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          }, {});

        this.form.patchValue(noNulls);
        if (data.isPaid) {
          this.form.disable();
        }
        this.savedFormData = this.form.value;
      },
        () => {
          console.log('Error occured');
        }
      );
  }

  isTouchedAndInvalid(fieldName: string): boolean {
    return this.form.get(fieldName).touched
      && !this.form.get(fieldName).valid;
  }

  doAction(licenseId: string, actionName: string) {

    this.busy = this.licenceDataService.createApplicationForActionType(licenseId, actionName)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(data => {
        this.window.open(`/account-profile/${data.id}`, 'blank');
      },
        () => {
          this.snackBar.open(`Error running licence action for ${actionName}`, 'Fail',
            { duration: 3500, panelClass: ['red-snackbar'] });
          console.log('Error starting a Change Licence Application');
        }
      );
  }

  canDeactivate(): Observable<boolean> | boolean {
    const formDidntChange = JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value);
    if (formDidntChange) {
      return true;
    } else {
      const subj = new Subject<boolean>();
      this.busy = this.save(true).subscribe(res => {
        subj.next(res);
      });
      return subj;
    }
  }

  checkPossibleProblematicWords() {
    console.log(this.form.get('establishmentName').errors);
    this.possibleProblematicNameWarning =
      this.establishmentWatchWordsService.potentiallyProblematicValidator(this.form.get('establishmentName').value);
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    const saveData = this.form.value;

    if (this.form.get('ldbOrderTotals')) {
      this.licenceDataService.updateLicenceLDBOrders(this.application.assignedLicence.id, this.form.get('ldbOrderTotals').value)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(() => { });
    }

    return this.applicationDataService.updateApplication({ ...this.application, ...this.form.value })
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        this.savedFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true) {
          this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        }
        return of(true);
      }));
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
  submit_application() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
    } else if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
      this.submitPayment();
    } else {
      this.busy = this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result) {
            this.submitPayment();
          }
        });
    }
  }

  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   * */
  private submitPayment() {
    this.busy = this.paymentDataService.getPaymentSubmissionUrl(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(res => {
        const jsonUrl = res;
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      }, err => {
        if (err._body === 'Payment already made') {
          this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
  }

  isValid(): boolean {
    this.showValidationMessages = false;
    this.markConstrolsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, ValidationErrorMap);

    if (this.form.get('ldbOrderTotals') && this.form.get('ldbOrderTotals').value !== this.form.get('ldbOrderTotalsConfirm').value) {
      this.validationMessages.push('LDB Order Totals are required');
    }

    return this.validationMessages.length === 0;
  }

  /**
   * Dialog to confirm the application cancellation (status changed to "Termindated")
   */
  cancelApplication() {

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '400px',
      height: '200px',
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
          this.busy = this.applicationDataService.cancelApplication(this.applicationId)
            .pipe(takeWhile(() => this.componentActive))
            .subscribe(() => {
              this.savedFormData = this.form.value;
              this.router.navigate(['/dashboard']);
            },
              () => {
                this.snackBar.open('Error cancelling the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
                console.error('Error cancelling the application');
              });
        }
      });
  }

  businessTypeIsPartnership(): boolean {
    return this.account &&
      ['GeneralPartnership',
        'LimitedPartnership',
        'LimitedLiabilityPartnership',
        'Partnership'].indexOf(this.account.businessType) !== -1;
  }

  businessTypeIsPrivateCorporation(): boolean {
    return this.account &&
      ['PrivateCorporation',
        'UnlimitedLiabilityCorporation',
        'LimitedLiabilityCorporation'].indexOf(this.account.businessType) !== -1;
  }

  isCRSRenewalApplication(): boolean {
    return this.application
      && this.application.applicationType
      && [
        ApplicationTypeNames.CRSRenewal.toString(),
        ApplicationTypeNames.CRSRenewalLate30.toString(),
        ApplicationTypeNames.CRSRenewalLate6Months.toString(),
      ].indexOf(this.application.applicationType.name) !== -1;
  }

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.ReadOnly.toString()]
      .indexOf(state) !== -1;
  }

  saveForLater() {
    this.busy = this.save(true)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((result: boolean) => {
        if (result) {
          this.router.navigate(['/dashboard']);
        }
      });
  }

}
