import { Component, OnInit, Input, Inject, Output, EventEmitter } from '@angular/core';
import { forkJoin, Subscription } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { ApplicationDataService } from '@app/services/application-data.service';
import { LicenseDataService } from '@app/services/license-data.service';
import { Router } from '@angular/router';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { Account } from '@models/account.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { FormBase } from '@shared/form-base';
import { takeWhile } from 'rxjs/operators';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { SetIndigenousNationModeAction } from '@app/app-state/actions/app-state.action';
import * as moment from 'moment';
import { PaymentDataService } from '@services/payment-data.service';
import { LegalEntity } from '../../models/legal-entity.model';
import { LicenseeChangeLog } from '../../models/licensee-change-log.model';
import { LegalEntityDataService } from '../../services/legal-entity-data.service';
import { ApplicationCancellationDialogComponent } from '../dashboard/applications-and-licences/applications-and-licences.component';

export const UPLOAD_FILES_MODE = 'UploadFilesMode';
// export const TRANSFER_LICENCE_MODE = 'TransferLicenceMode';
// export const CHANGE_OF_LOCATION_MODE = 'ChangeOfLocationMode';
export const CATERING_MODE = 'catering';

const ACTIVE = 'Active';
const PAYMENT_REQUIRED = 'Payment Required';
const RENEWAL_DUE = 'Renewal Due';

@Component({
  selector: 'app-catering-demo',
  templateUrl: './catering-demo.component.html',
  styleUrls: ['./catering-demo.component.scss']
})
export class CateringDemoComponent extends FormBase  implements OnInit {
  account: Account;
  indigenousNationModeActive: boolean;
  currentLegalEntities: LegalEntity;
  tree: LicenseeChangeLog;


  inProgressApplications: ApplicationSummary[] = [];
  licensedApplications: ApplicationLicenseSummary[] = [];

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;
  // readonly TRANSFER_LICENCE_MODE = TRANSFER_LICENCE_MODE;
  // readonly CHANGE_OF_LOCATION_MODE = CHANGE_OF_LOCATION_MODE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;

  @Output() marketerApplicationExists: EventEmitter<boolean> = new EventEmitter<boolean>();
  dataLoaded = false;
  licencePresentLabel: string;
  licenceAbsentLabel: string;
  submittedApplications = 8;
  marketerExists: boolean;
  nonMarketerExists: boolean;
  ApplicationTypeNames = ApplicationTypeNames;
  licenceTransferFeatureOn = false;

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private paymentService: PaymentDataService,
    public featureFlagService: FeatureFlagService,
    public dialog: MatDialog,
    private legalEntityDataService: LegalEntityDataService
    
  ) {
    super();
    if (featureFlagService.featureOn('Marketer')) {
      this.licencePresentLabel = '';
      this.licenceAbsentLabel = '';
    } else {
      this.licencePresentLabel = '';
      this.licenceAbsentLabel = '';
    }
    featureFlagService.featureOn('LicenceTransfer')
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((featureOn: boolean) => {
        this.licenceTransferFeatureOn = featureOn;
      });
  }

  ngOnInit() {
    this.store.select((state) => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((account) => {
        this.account = account;
      });

    this.store.select((state) => state.indigenousNationState.indigenousNationModeActive)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((active) => {
        this.indigenousNationModeActive = active;
      });

    this.legalEntityDataService.getCurrentHierachy()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: LegalEntity) => {
        this.tree = LicenseeChangeLog.processLegalEntityTree(data);
        this.tree.isRoot = true;
      },
        () => {
          console.log('Error occured');
        }
      );

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
          // filter out approved applications
          applications.filter(app => ['Approved', 'Renewal Due', 'Payment Required', 'Active'].indexOf(app.applicationStatus) === -1)
            .forEach((application: ApplicationSummary) => {
              this.inProgressApplications.push(application);
            });

          licenses.forEach((licence: ApplicationLicenseSummary) => {
            licence.actionApplications = [];
            const relatedApplications = applications.filter(l => l.licenceId === licence.licenseId);
            relatedApplications.forEach(app => {
              licence.actionApplications.push({
                applicationId: app.id,
                applicationTypeName: app.applicationTypeName,
                applicationStatus: app.applicationStatus,
                isPaid: app.isPaid
              });
            });
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

  doAction(licence: ApplicationLicenseSummary, actionName: string) {
    const actionApplication = licence.actionApplications.find(app => app.applicationTypeName === actionName && app.applicationStatus !== 'Active');
    if (actionApplication && !actionApplication.isPaid) {
      this.router.navigateByUrl('/account-profile/' + actionApplication.applicationId);
    } else if (actionApplication && actionApplication.isPaid) {
      this.snackBar.open('Application already submitted', 'Fail',
        { duration: 3500, panelClass: ['red-snackbar'] });
    } else {
      this.busy = this.licenceDataService.createApplicationForActionType(licence.licenseId, actionName)
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
  }

  planStoreOpening(licence: ApplicationLicenseSummary) {
    const crsApplication = licence.actionApplications.find(app => app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore);
    if (crsApplication) {
      this.router.navigate([`/store-opening/${crsApplication.applicationId}`]);
    } else {
      this.snackBar.open('Unable to find CRS Application', 'Fail',
        { duration: 3500, panelClass: ['red-snackbar'] });
    }
  }

  payLicenceFee(licence: ApplicationLicenseSummary) {
    const crsApplication = licence.actionApplications.find(app => app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore);
    if (crsApplication) {
      this.busy = this.paymentService.getInvoiceFeePaymentSubmissionUrl(crsApplication.applicationId)
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
  }

  startCatering() {
    const newLicenceApplicationData: Application = <Application>{
      licenseType: 'Catering',
      applicantType: this.account.businessType,
      applicationType: <ApplicationType>{ name: ApplicationTypeNames.Catering },
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
        this.router.navigateByUrl(`/account-profile/${data.id};mode=catering`);
      },
      () => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );
  }

  startRenewal(licence: ApplicationLicenseSummary) {
    const renewalApplication = licence.actionApplications.find(app => app.applicationTypeName === 'CRS Renewal');
    if (renewalApplication && !renewalApplication.isPaid) {
      this.router.navigateByUrl('/renew-crs-licence/application/' + renewalApplication.applicationId);
    } else if (renewalApplication && renewalApplication.isPaid) {
      this.snackBar.open('Renewal application already submitted', 'Fail',
        { duration: 3500, panelClass: ['red-snackbar'] });
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

  startLicenseeChangeApplication() {
    const newLicenceApplicationData: Application = <Application>{
      // licenseType: ApplicationTypeNames.LeaderhsipChange,
      applicantType: this.account.businessType,
      applicationType: <ApplicationType>{ name: ApplicationTypeNames.LicenseeChanges },
      account: this.account,
    };

    this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.router.navigateByUrl(`/licensee-changes/${data.id}`);
      },
      () => {
        this.snackBar.open('Error starting a New Licensee Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Licensee Application');
      }
    );
  }



  isAboutToExpire(expiryDate: string) {
    const now = moment(new Date()).startOf('day');
    const expiry = moment(expiryDate).startOf('day');
    const diff = expiry.diff(now, 'days') + 1;
    return diff <= 60 || expiry < now;
  }

  licenceHasExpired(expiryDate: string) {
    const now = moment(new Date()).startOf('day');
    const expiry = moment(expiryDate).startOf('day');
    return expiry < now;
  }

}



