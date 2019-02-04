
import {filter} from 'rxjs/operators';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AppState } from '../../app-state/models/app-state';
import { Subscription ,  Subject ,  Observable } from 'rxjs';
import { MatSnackBar, MatDialog } from '@angular/material';
import * as currentApplicationActions from '../../app-state/actions/current-application.action';
import { ActivatedRoute, Router } from '@angular/router';
import { AdoxioApplicationDataService } from '../../services/adoxio-application-data.service';
import { PaymentDataService } from '../../services/payment-data.service';
import { FileUploaderComponent } from '../../file-uploader/file-uploader.component';
import { AdoxioApplication } from '../../models/adoxio-application.model';
import { debug } from 'util';
import { ConfirmationDialogComponent } from '../../lite-application-dashboard/lite-application-dashboard.component';

export const UPLOAD_FILES_MODE = 'UploadFilesMode';

@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
  styleUrls: ['./application.component.scss']
})
export class ApplicationComponent implements OnInit, OnDestroy {
  application: AdoxioApplication;
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

  UPLOAD_FILES_MODE = UPLOAD_FILES_MODE;
  mode: string;

  constructor(private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
    public router: Router,
    private applicationDataService: AdoxioApplicationDataService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog) {
    this.applicationId = this.route.snapshot.params.applicationId;
    this.mode = this.route.snapshot.params.mode;
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      establishmentName: [''], // Validators.required
    });

    this.applicationDataService.getSubmittedApplicationCount()
      .subscribe(value => this.submittedApplications = value);

    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: AdoxioApplication) => {
        this.application = data;
        this.form.patchValue(data);
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
      (data: AdoxioApplication) => {
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
      const jsonUrl = res.json();
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
    let valid = true;
    this.validationMessages = [];
    if (!this.mainForm || !this.mainForm.files || this.mainForm.files.length < 1) {
      valid = false;
      this.validationMessages.push('Application form is required.');
    }
    if (!this.financialIntegrityDocuments || !this.financialIntegrityDocuments.files || this.financialIntegrityDocuments.files.length < 1) {
      valid = false;
      this.validationMessages.push('Financial Integrity form is required.');
    }
    if (!this.supportingDocuments || !this.supportingDocuments.files || this.supportingDocuments.files.length < 1) {
      valid = false;
      this.validationMessages.push('At least one supporting document is required.');
    }
    if (!this.form.get('establishmentName').value) {
      valid = false;
      this.validationMessages.push('Establishment name is required.');
    }
    if (this.submittedApplications >= 8) {
      valid = false;
      this.validationMessages.push('Only 8 applications can be submitted');
    }
    return valid;
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
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(
      cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.busy = this.applicationDataService.cancelApplication(this.applicationId).subscribe(
            res => {
              this.savedFormData = this.form.value;
              this.router.navigate(['/dashboard-lite']);
            },
            err => {
              this.snackBar.open('Error cancelling the application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
              console.error('Error cancelling the application');
            });
        }
      });
  }

}
