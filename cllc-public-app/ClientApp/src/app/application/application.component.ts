
import { filter } from 'rxjs/operators';
import { Component, OnInit, OnDestroy, ViewChild, ElementRef, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Subscription, Subject, Observable } from 'rxjs';
import { MatSnackBar, MatDialog } from '@angular/material';
import * as currentApplicationActions from '@app/app-state/actions/current-application.action';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { FileUploaderComponent } from '@shared/file-uploader/file-uploader.component';
import { Application } from '@models/application.model';
import { FormBase, CanadaPostalRegex } from '@shared/form-base';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { Title, DomSanitizer, SafeHtml } from '@angular/platform-browser';
import {
  ApplicationCancellationDialogComponent,
  UPLOAD_FILES_MODE
} from '@app/applications-and-licences/applications-and-licences.component';
import { Account } from '@appmodels/account.model';
import { ApplicationContentType } from '@models/application-content-type.model';
import { ApplicationTypeNames } from '@models/application-type.model';
import { CurrentAccountAction } from './../app-state/actions/current-account.action';
import { TiedHouseConnection } from '@models/tied-house-connection.model';

const ServiceHours = [
  // '00:00', '00:15', '00:30', '00:45', '01:00', '01:15', '01:30', '01:45', '02:00', '02:15', '02:30', '02:45', '03:00',
  // '03:15', '03:30', '03:45', '04:00', '04:15', '04:30', '04:45', '05:00', '05:15', '05:30', '05:45', '06:00', '06:15',
  // '06:30', '06:45', '07:00', '07:15', '07:30', '07:45', '08:00', '08:15', '08:30', '08:45',
  '09:00', '09:15', '09:30',
  '09:45', '10:00', '10:15', '10:30', '10:45', '11:00', '11:15', '11:30', '11:45', '12:00', '12:15', '12:30', '12:45',
  '13:00', '13:15', '13:30', '13:45', '14:00', '14:15', '14:30', '14:45', '15:00', '15:15', '15:30', '15:45', '16:00',
  '16:15', '16:30', '16:45', '17:00', '17:15', '17:30', '17:45', '18:00', '18:15', '18:30', '18:45', '19:00', '19:15',
  '19:30', '19:45', '20:00', '20:15', '20:30', '20:45', '21:00', '21:15', '21:30', '21:45', '22:00', '22:15', '22:30',
  '22:45', '23:00'
  // , '23:15', '23:30', '23:45'
];

class ApplicationHTMLContent {
  title: string;
  preamble: string;
  beforeStarting: string;
  nextSteps: string;
}


@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationComponent extends FormBase implements OnInit, OnDestroy {
  application: Application;
  @ViewChild('mainForm') mainForm: FileUploaderComponent;
  @ViewChild('financialIntegrityDocuments') financialIntegrityDocuments: FileUploaderComponent;
  @ViewChild('supportingDocuments') supportingDocuments: FileUploaderComponent;
  form: FormGroup;
  savedFormData: any;
  subscriptions: Subscription[] = [];
  applicationId: string;
  busy: Subscription;
  accountId: string;
  payMethod: string;
  validationMessages: any[];
  showValidationMessages: boolean;
  submittedApplications = 8;
  ServiceHours = ServiceHours;
  tiedHouseFormData: TiedHouseConnection;

  htmlContent: ApplicationHTMLContent = <ApplicationHTMLContent>{};



  readonly UPLOAD_FILES_MODE = UPLOAD_FILES_MODE;
  ApplicationTypeNames = ApplicationTypeNames;
  mode: string;
  account: Account;

  constructor(private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
    public router: Router,
    private applicationDataService: ApplicationDataService,
    private dynamicsDataService: DynamicsDataService,
    private sanitizer: DomSanitizer,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog) {
    super();
    this.applicationId = this.route.snapshot.params.applicationId;
    this.mode = this.route.snapshot.params.mode;
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      assignedLicence: this.fb.group({
        id: [''],
        establishmentAddressStreet: [''],
        establishmentAddressCity: [''],
        establishmentAddressPostalCode: [''],
        establishmentParcelId: ['']
      }),
      establishmentName: [''],
      establishmentParcelId: ['', [Validators.required, Validators.maxLength(9), Validators.minLength(9)]],
      contactPersonFirstName: ['', Validators.required],
      contactPersonLastName: ['', Validators.required],
      contactPersonRole: [''],
      contactPersonEmail: ['', Validators.required],
      contactPersonPhone: ['', Validators.required],
      establishmentAddressStreet: ['', Validators.required],
      establishmentAddressCity: ['', Validators.required],
      establishmentAddressPostalCode: ['', [Validators.required, Validators.pattern(CanadaPostalRegex)]],
      establishmentEmail: ['', Validators.email],
      establishmentPhone: [''],

      serviceHoursSundayOpen: ['', Validators.required],
      serviceHoursMondayOpen: ['', Validators.required],
      serviceHoursTuesdayOpen: ['', Validators.required],
      serviceHoursWednesdayOpen: ['', Validators.required],
      serviceHoursThursdayOpen: ['', Validators.required],
      serviceHoursFridayOpen: ['', Validators.required],
      serviceHoursSaturdayOpen: ['', Validators.required],
      serviceHoursSundayClose: ['', Validators.required],
      serviceHoursMondayClose: ['', Validators.required],
      serviceHoursTuesdayClose: ['', Validators.required],
      serviceHoursWednesdayClose: ['', Validators.required],
      serviceHoursThursdayClose: ['', Validators.required],
      serviceHoursFridayClose: ['', Validators.required],
      serviceHoursSaturdayClose: ['', Validators.required],
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
    });

    this.applicationDataService.getSubmittedApplicationCount()
      .subscribe(value => this.submittedApplications = value);

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });


    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, '');
        }
        this.application = data;
        this.hideFormControlByType();

        if (this.application.applicationType) {
          this.htmlContent = {
            title: this.application.applicationType.title,
            preamble: this.getApplicationContent('Preamble'),
            beforeStarting: this.getApplicationContent('BeforeStarting'),
            nextSteps: this.getApplicationContent('NextSteps'),
          };
        }

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
      err => {
        console.log('Error occured');
      }
    );

    const sub = this.store.select(state => state.currentApplicaitonState.currentApplication).pipe(
      filter(state => !!state))
      .subscribe(currentApplication => {
        this.form.patchValue(currentApplication);
        if (currentApplication.isPaid) {
          this.form.disable();
        }
        this.savedFormData = this.form.value;
      });
    this.subscriptions.push(sub);
  }
  private hideFormControlByType() {
    if (!this.application.applicationType.showPropertyDetails) {
      this.form.get('establishmentAddressStreet').disable();
      this.form.get('establishmentAddressCity').disable();
      this.form.get('establishmentAddressPostalCode').disable();
      this.form.get('establishmentName').disable();
      this.form.get('establishmentParcelId').disable();
      this.form.get('establishmentName').disable();
    }

    if (!this.application.applicationType.showHoursOfSale) {
      this.form.get('serviceHoursSundayOpen').disable();
      this.form.get('serviceHoursMondayOpen').disable();
      this.form.get('serviceHoursTuesdayOpen').disable();
      this.form.get('serviceHoursWednesdayOpen').disable();
      this.form.get('serviceHoursThursdayOpen').disable();
      this.form.get('serviceHoursFridayOpen').disable();
      this.form.get('serviceHoursSaturdayOpen').disable();
      this.form.get('serviceHoursSundayClose').disable();
      this.form.get('serviceHoursMondayClose').disable();
      this.form.get('serviceHoursTuesdayClose').disable();
      this.form.get('serviceHoursWednesdayClose').disable();
      this.form.get('serviceHoursThursdayClose').disable();
      this.form.get('serviceHoursFridayClose').disable();
      this.form.get('serviceHoursSaturdayClose').disable();
    }
  }

  private getApplicationContent(contentCartegory: string) {
    let body = '';
    const contents =
      this.application.applicationType.contentTypes
        .filter(t => t.category === contentCartegory && t.businessTypes.indexOf(this.account.businessType) !== -1);
    if (contents.length > 0) {
      body = contents[0].body;
    }
    return body;
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save(true);
    }
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const saveData = this.form.value;
    const subscription = this.applicationDataService.updateApplication(this.form.value).subscribe(
      res => {
        saveResult.next(true);
        this.savedFormData = saveData;
        this.updateApplicationInStore();
        if (showProgress === true) {
          this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured');
      });

    if (showProgress === true) {
      this.busy = subscription;
    }

    return saveResult;
  }

  updateApplicationInStore() {
    this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
      }
    );
  }

  isFieldError(field: string) {
    const isError = !this.form.get(field).valid && this.form.get(field).touched;
    return isError;
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
      this.save(true).subscribe((result: boolean) => {
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
    this.busy = this.paymentDataService.getPaymentSubmissionUrl(this.applicationId).subscribe(res => {
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
    // mark controls as touched
    for (const c in this.form.controls) {
      if (typeof (this.form.get(c).markAsTouched) === 'function') {
        this.form.get(c).markAsTouched();
      }
    }
    this.showValidationMessages = false;
    let valid = true;
    this.validationMessages = [];

    if (this.application.applicationType.showAssociatesFormUpload &&
      (!this.mainForm || !this.mainForm.files || this.mainForm.files.length < 1)) {
      valid = false;
      this.validationMessages.push('Associate form is required.');
    }

    if (this.application.applicationType.showFinancialIntegrityFormUpload &&
      (!this.financialIntegrityDocuments
        || !this.financialIntegrityDocuments.files
        || this.financialIntegrityDocuments.files.length < 1)) {
      valid = false;
      this.validationMessages.push('Financial Integrity form is required.');
    }

    if (this.application.applicationType.showSupportingDocuments &&
      (!this.supportingDocuments || !this.supportingDocuments.files || this.supportingDocuments.files.length < 1)) {
      valid = false;
      this.validationMessages.push('At least one supporting document is required.');
    }

    if (this.application.applicationType.showPropertyDetails && !this.form.get('establishmentName').value) {
      valid = false;
      this.validationMessages.push('Establishment name is required.');
    }
    if (this.application.applicationType.name === ApplicationTypeNames.CannabisRetailStore && this.submittedApplications >= 8) {
      valid = false;
      this.validationMessages.push('Only 8 applications can be submitted');
    }
    if (!this.form.valid) {
      this.validationMessages.push('Some required fields have not been completed');
    }
    return valid && this.form.valid;
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
    dialogRef.afterClosed().subscribe(
      cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.busy = this.applicationDataService.cancelApplication(this.applicationId).subscribe(
            res => {
              this.savedFormData = this.form.value;
              this.router.navigate(['/dashboard']);
            },
            err => {
              this.snackBar.open('Error cancelling the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
              console.error('Error cancelling the application');
            });
        }
      });
  }

}
