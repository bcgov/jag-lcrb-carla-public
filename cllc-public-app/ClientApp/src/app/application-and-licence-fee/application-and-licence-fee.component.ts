import { Component, OnInit } from '@angular/core';
import { FormBase } from '@shared/form-base';
import { Application } from '@models/application.model';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription, Observable, Subject, of, forkJoin } from 'rxjs';
import {
  ApplicationCancellationDialogComponent
} from '@app/applications-and-licences/applications-and-licences.component';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { ApplicationHTMLContent } from '@app/application/application.component';
import { Account } from '@models/account.model';
import * as currentApplicationActions from '@app/app-state/actions/current-application.action';
import { DynamicsDataService } from '@services/dynamics-data.service';

@Component({
  selector: 'app-application-and-licence-fee',
  templateUrl: './application-and-licence-fee.component.html',
  styleUrls: ['./application-and-licence-fee.component.scss']
})
export class ApplicationAndLicenceFeeComponent extends FormBase implements OnInit {
  application: Application;
  form: FormGroup;
  savedFormData: any;
  applicationId: string;
  busy: Subscription;
  validationMessages: any[];
  showValidationMessages: boolean;
  submittedApplications = 8;
  htmlContent: ApplicationHTMLContent = <ApplicationHTMLContent>{};
  ApplicationTypeNames = ApplicationTypeNames;
  FormControlState = FormControlState;
  account: Account;
  minDate = new Date();


  constructor(private store: Store<AppState>,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
    public router: Router,
    public applicationDataService: ApplicationDataService,
    public dynamicsDataService: DynamicsDataService,
    public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog,
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      assignedLicence: this.fb.group({
        establishmentId: [''],
        establishmentPhone: [''],
        establishmentEmail: [''],
      }),
      description1: ['', [Validators.required, Validators.minLength(10)]],
      isReadyValidInterest: [''],
      isReadyWorkers: [''],
      isReadyNameBranding: [''],
      isReadyDisplays: [''],
      isReadyIntruderAlarm: [''],
      isReadyFireAlarm: [''],
      isReadyLockedCases: [''],
      isReadyLockedStorage: [''],
      isReadyPerimeter: [''],
      isReadyRetailArea: [''],
      isReadyStorage: [''],
      isReadyExtranceExit: [''],
      isReadySurveillanceNotice: [''],
      isReadyProductNotVisibleOutside: [''],
      establishmentopeningdate: ['', [Validators.required]],
    });

    this.applicationDataService.getSubmittedApplicationCount()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => this.submittedApplications = value);

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });

    this.busy = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      // .pipe(mergeMap(application))
      .subscribe((data: Application) => {
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, '');
        }

        this.application = data;

        this.addDynamicContent();

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          }, {});

        this.form.patchValue(noNulls);
        this.savedFormData = this.form.value;
      },
        () => {
          console.log('Error occured');
        }
      );
  }

  private addDynamicContent() {
    if (this.application.applicationType) {
      this.htmlContent = {
        title: this.application.applicationType.title,
        preamble: this.getApplicationContent('Preamble'),
        beforeStarting: this.getApplicationContent('BeforeStarting'),
        nextSteps: this.getApplicationContent('NextSteps'),
      };
    }
  }

  payLicenceFee() {
    this.busy = this.paymentDataService.getInvoiceFeePaymentSubmissionUrl(this.application.id)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(res => {
        const data = <any>res;
        window.location.href = data.url;
      }, err => {
        if (err._body === 'Payment already made') {
          this.snackBar.open('Licence Fee payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
  }

  private getApplicationContent(contentCartegory: string) {
    let body = '';
    const contents =
      this.application.applicationType.contentTypes
        .filter(t => t.category === contentCartegory && t.businessTypes.indexOf(this.application.applicantType) !== -1);
    if (contents.length > 0) {
      body = contents[0].body;
    }
    return body;
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

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    const saveData = this.form.value;
    const establishment = {
      id: saveData.assignedLicence.establishmentId,
      Phone: saveData.assignedLicence.establishmentPhone,
      email: saveData.assignedLicence.establishmentEmail,
    };

    return forkJoin(
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value }),
      this.dynamicsDataService.updateRecord('establishments', establishment.id, establishment)
    ).pipe(takeWhile(() => this.componentActive))
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
        this.store.dispatch(new currentApplicationActions.SetCurrentApplicationAction(data));
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
      this.payLicenceFee();
    } else {
      this.busy = this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result) {
            this.payLicenceFee();
          }
        });
    }
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


    if (!this.form.valid) {
      valid = false;
      this.validationMessages.push('Some required fields have not been completed');
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

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.Reaonly.toString()]
      .indexOf(state) !== -1;
  }

}
