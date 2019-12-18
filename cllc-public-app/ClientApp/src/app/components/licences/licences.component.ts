import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { forkJoin, Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { ApplicationDataService } from '@app/services/application-data.service';
import { LicenseDataService } from '@app/services/license-data.service';
import { Router } from '@angular/router';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationTypeNames } from '@models/application-type.model';
import { Account } from '@models/account.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { FormBase } from '@shared/form-base';
import { takeWhile } from 'rxjs/operators';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import * as moment from 'moment';
import { PaymentDataService } from '@services/payment-data.service';
import { EstablishmentDataService } from '@services/establishment-data.service';
import { FormBuilder } from '@angular/forms';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';


const ACTIVE = 'Active';
const PAYMENT_REQUIRED = 'Payment Required';
const RENEWAL_DUE = 'Renewal Due';

@Component({
  selector: 'app-licences',
  templateUrl: './licences.component.html',
  styleUrls: ['./licences.component.scss']
})
export class LicencesComponent extends FormBase implements OnInit {
  applications: ApplicationSummary[] = [];
  licensedApplications: ApplicationLicenseSummary[] = [];
  licenceForms = {};

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;
  @Input() account: Account;
  @Output() marketerApplicationExists: EventEmitter<boolean> = new EventEmitter<boolean>();
  dataLoaded = false;
  ApplicationTypeNames = ApplicationTypeNames;
  licenceTransferFeatureOn = false;

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private paymentService: PaymentDataService,
    private establishmentService: EstablishmentDataService,
    public featureFlagService: FeatureFlagService,
    public fb: FormBuilder) {
    super();
    featureFlagService.featureOn('LicenceTransfer')
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((featureOn: boolean) => {
        this.licenceTransferFeatureOn = featureOn;
      });
  }

  ngOnInit() {
    this.displayApplications();
  }

  /**
   *
   * */
  private displayApplications() {
    this.licensedApplications = [];
    this.busy =
      forkJoin(this.applicationDataService.getAllCurrentApplications(), this.licenceDataService.getAllCurrentLicenses()
      ).pipe(takeWhile(() => this.componentActive))
        .subscribe(([applications, licenses]) => {
          this.applications = applications;
          licenses.forEach((licence: ApplicationLicenseSummary) => {
            this.addOrUpdateLicence(licence);
          });
        });
  }

  uploadMoreFiles(application: Application) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
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

  submitFieldUpdate(licenceId, establishmentId, establishmentFieldName, licenceFieldName, event) {
    const establishment = {
      id: establishmentId,
    };
    establishment[establishmentFieldName] = event.target.value;

    const licence = Object.assign( new ApplicationLicenseSummary(), {
      licenseId: licenceId
    });
    licence[licenceFieldName] = event.target.value;

    this.busy = forkJoin(
      this.establishmentService.upEstablishment(establishment),
      this.licenceDataService.updateLicenceEstablishment(licenceId, licence)
      )
    .subscribe(([establishmentResp, licenceResp]) => {
      this.addOrUpdateLicence(licenceResp);
    });
  }

  addOrUpdateLicence(licence: ApplicationLicenseSummary) {
    licence.actionApplications = [];
    const relatedApplications = this.applications.filter(l => l.licenceId === licence.licenseId);
    relatedApplications.forEach(app => {
      licence.actionApplications.push({
        applicationId: app.id,
        applicationTypeName: app.applicationTypeName,
        applicationStatus: app.applicationStatus,
        isPaid: app.isPaid
      });
    });

    const licenceIndex = this.licensedApplications.findIndex(l => l.licenseId === licence.licenseId);
    if (licenceIndex >= 0) {
      this.licensedApplications[licenceIndex] = licence;
    } else {
      this.licensedApplications.push(licence);
    }
    this.licenceForms[licence.licenseId] = this.fb.group({
      phone: [licence.establishmentPhoneNumber],
      email: [licence.establishmentEmail]
    });
  }

  toggleStoreOpen(index: number, establishmentId: string, isOpen: boolean) {
    const establishment = {
      id: establishmentId,
      isOpen: isOpen
    };

    this.busy = forkJoin(
      this.establishmentService.upEstablishment(establishment)
      )
    .subscribe(([establishmentResp]) => {
      this.licensedApplications[index].establishmentIsOpen = establishmentResp.isOpen;
    });
  }
}
