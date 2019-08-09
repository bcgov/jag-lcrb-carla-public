import { Component, OnInit, Input, Inject, Output, EventEmitter } from '@angular/core';
import { forkJoin, Subscription } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { ApplicationDataService } from '@app/services/application-data.service';
import { LicenseDataService } from '@app/services/license-data.service';
import { Router } from '@angular/router';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { License } from '@models/license.model';
import { PaymentDataService } from '@services/payment-data.service';
import { Account } from '@models/account.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { FormBase } from '@shared/form-base';
import { takeWhile, filter } from 'rxjs/operators';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { SetIndigenousNationModeAction } from '@app/app-state/actions/app-state.action';
import * as moment from 'moment';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';
// export const TRANSFER_LICENCE_MODE = 'TransferLicenceMode';
// export const CHANGE_OF_LOCATION_MODE = 'ChangeOfLocationMode';


const ACTIVE = 'Active';
const PAYMENT_REQUIRED = 'Payment Required';
const RENEWAL_DUE = 'Renewal Due';

@Component({
  selector: 'app-applications-and-licences',
  templateUrl: './applications-and-licences.component.html',
  styleUrls: ['./applications-and-licences.component.scss']
})
export class ApplicationsAndLicencesComponent extends FormBase implements OnInit {
  inProgressApplications: any[] = [];
  licensedApplications: any[] = [];

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;
  // readonly TRANSFER_LICENCE_MODE = TRANSFER_LICENCE_MODE;
  // readonly CHANGE_OF_LOCATION_MODE = CHANGE_OF_LOCATION_MODE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;
  @Input() account: Account;
  @Output() marketerApplicationExists: EventEmitter<boolean> = new EventEmitter<boolean>();
  dataLoaded = false;
  licencePresentLabel: string;
  licenceAbsentLabel: string;
  submittedApplications = 8;
  marketerExists: boolean;
  nonMarketerExists: boolean;

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private paymentService: PaymentDataService,
    private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private featureFlagService: FeatureFlagService,
    public dialog: MatDialog) {
    super();
    if (featureFlagService.featureOn('Marketer')) {
      this.licencePresentLabel = '';
      this.licenceAbsentLabel = '';
    } else {
      this.licencePresentLabel = '';
      this.licenceAbsentLabel = '';
    }
  }

  ngOnInit() {
    this.displayApplications();

    this.applicationDataService.getSubmittedApplicationCount()
      .subscribe(value => this.submittedApplications = value);
  }

  /**
   *
   * */
  private displayApplications() {
    this.inProgressApplications = [];
    this.licensedApplications = [];
    this.busy =
      forkJoin(this.applicationDataService.getAllCurrentApplications(), this.licenceDataService.getAllCurrentLicenses()
      ).pipe(takeWhile(() => this.componentActive))
        .subscribe(([applications, licenses]) => {
          this.checkIndigenousNationState(applications);
          applications.forEach((application: ApplicationSummary | any) => {
            this.inProgressApplications.push(application);
          });

          licenses.forEach((licence: License | any) => {
            const relatedApplications = applications.filter(l => l.licenceId === licence.licenseId);
            if (relatedApplications.length > 0) {
              licence.relatedApplicationId = relatedApplications[0].id;
              if (relatedApplications[0].isPaid) {
                licence.relatedApplicationPaid = true;
              }
            }
            this.licensedApplications.push(licence);
          });

          this.marketerExists = applications.filter(item => item.applicationTypeName === ApplicationTypeNames.Marketer)
            .map(item => <any>item)
            .concat(licenses.filter(item => item.licenceTypeName === ApplicationTypeNames.Marketer)).length > 0;

          this.nonMarketerExists = applications.filter(item => item.applicationTypeName !== ApplicationTypeNames.Marketer)
            .map(item => <any>item)
            .concat(licenses.filter(item => item.licenceTypeName !== ApplicationTypeNames.Marketer)).length > 0;

        });
  }

  uploadMoreFiles(application: Application) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
  }

  checkIndigenousNationState(applications: ApplicationSummary[]) {
    if (applications.find((a) => a.isIndigenousNation)) {
      this.store.dispatch(new SetIndigenousNationModeAction(true));
    }
  }

  /**
   *
   * @param applicationId
   * @param establishmentName
   * @param applicationName
   */
  cancelApplication(applicationId: string, establishmentName: string, applicationName: string) {

    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '400px',
      height: '200px',
      data: {
        establishmentName: establishmentName,
        applicationName: applicationName
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ApplicationCancellationDialogComponent, dialogConfig);
    dialogRef.afterClosed()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.busy = this.applicationDataService.cancelApplication(applicationId)
            .pipe(takeWhile(() => this.componentActive))
            .subscribe(() => {
              this.displayApplications();
            });
        }
      }
      );

  }

  doAction(licenceId: string, actionName: string) {
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.licenceDataService.createApplicationForActionType(licenceId, actionName)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(data => {
        this.router.navigateByUrl('/account-profile/' + data.id);
      },
        () => {
          this.snackBar.open(`Error running licence action for ${actionName}`, 'Fail',
            { duration: 3500, panelClass: ['red-snackbar'] });
          console.log('Error starting a Change Licence Location Application');
        }
      );
  }

  downloadLicence() {

  }


  payLicenceFee(licence: ApplicationLicenseSummary) {
    this.busy = this.paymentService.getInvoiceFeePaymentSubmissionUrl(licence.applicationId)
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

  startNewLicenceApplication() {
    const newLicenceApplicationData: Application = <Application>{
      licenseType: 'Cannabis Retail Store',
      applicantType: this.account.businessType,
      applicationType: <ApplicationType>{ name: ApplicationTypeNames.CannabisRetailStore },
      account: this.account,
      servicehHoursStandardHours: false,
      serviceHoursSundayOpen: '09:00',
      serviceHoursMondayOpen: '09:00',
      serviceHoursTuesdayOpen: '09:00',
      serviceHoursWednesdayOpen: '09:00',
      serviceHoursThursdayOpen: '09:00',
      serviceHoursFridayOpen: '09:00',
      serviceHoursSaturdayOpen: '09:00',
      serviceHoursSundayClose: '23:00',
      serviceHoursMondayClose: '23:00',
      serviceHoursTuesdayClose: '23:00',
      serviceHoursWednesdayClose: '23:00',
      serviceHoursThursdayClose: '23:00',
      serviceHoursFridayClose: '23:00',
      serviceHoursSaturdayClose: '23:00',
    };
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.router.navigateByUrl(`/account-profile/${data.id}`);
      },
      () => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );
  }

  startNewMarketerApplication() {
    const newLicenceApplicationData: Application = <Application>{
      licenseType: 'Marketer',
      applicantType: this.account.businessType,
      applicationType: <ApplicationType>{ name: ApplicationTypeNames.Marketer },
      account: this.account,
    };
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.router.navigateByUrl(`/account-profile/${data.id}`);
      },
      () => {
        this.snackBar.open('Error starting a New Marketer Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Marketer Application');
      }
    );
  }

  startRenewal(licence) {
    if (licence.relatedApplicationId) {
      this.router.navigateByUrl('/renew-crs-licence/application/' + licence.relatedApplicationId);
    } else {
      const actionName = 'CRS Renewal';
      this.busy = this.licenceDataService.createApplicationForActionType(licence.licenseId, actionName)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(data => {
          this.router.navigateByUrl('/renew-crs-licence/application/' + data.id);
        },
          () => {
            this.snackBar.open(`Error running licence action for ${actionName}`, 'Fail',
              { duration: 3500, panelClass: ['red-snackbar'] });
            console.log('Error starting a Change Licence Location Application');
          }
        );
    }
  }

  isAboutToExpire(expiryDate: string) {
    const now = moment(new Date()).startOf('day');
    const expiry = moment(expiryDate).startOf('day');
    const diff = expiry.diff(now, 'days') + 1;
    return diff <= 60 || expiry < now;
  }

}

@Component({
  selector: 'app-application-cancellation-dialog',
  templateUrl: 'application-cancellation-dialog.html',
})
export class ApplicationCancellationDialogComponent {

  establishmentName: string;
  applicationName: string;

  constructor(
    public dialogRef: MatDialogRef<ApplicationCancellationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.applicationName = data.applicationName;
    this.establishmentName = data.establishmentName;
  }

  close() {
    this.dialogRef.close(false);
  }

  cancel() {
    this.dialogRef.close(true);
  }

}

