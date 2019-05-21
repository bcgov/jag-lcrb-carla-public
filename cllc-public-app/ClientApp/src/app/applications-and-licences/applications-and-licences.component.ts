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
import { FileSystemItem } from '@models/file-system-item.model';
import { PaymentDataService } from '@services/payment-data.service';
import { Account } from '@models/account.model';
import { FeatureFlagService } from '@services/feature-flag.service';


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
export class ApplicationsAndLicencesComponent implements OnInit {
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

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private paymentService: PaymentDataService,
    private snackBar: MatSnackBar,
    private featureFlagService: FeatureFlagService,
    public dialog: MatDialog) {
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
  }

  /**
   *
   * */
  private displayApplications() {
    this.inProgressApplications = [];
    this.licensedApplications = [];
    this.busy =
      forkJoin(this.applicationDataService.getAllCurrentApplications(), this.licenceDataService.getAllCurrentLicenses()
      ).subscribe(([applications, licenses]) => {
        applications.forEach((application: ApplicationSummary | any) => {
          this.inProgressApplications.push(application);
        });

        licenses.forEach((licence: License | any) => {
          this.licensedApplications.push(licence);
        });

        // let marketerApplicationExists = !!applications.find((a: any) => a.applicationType.name === ApplicationTypeNames.Marketer);
        // marketerApplicationExists = marketerApplicationExists
        //   || !!licenses.find((a: any) => a.licenseType === ApplicationTypeNames.Marketer);
        //   debugger;
        // this.marketerApplicationExists.emit(marketerApplicationExists);

      });
  }

  uploadMoreFiles(application: Application) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
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
    dialogRef.afterClosed().subscribe(
      cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.busy = this.applicationDataService.cancelApplication(applicationId).subscribe(
            () => {
              this.displayApplications();
            });
        }
      }
    );

  }

  doAction(licenceId: string, actionName: string) {
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.licenceDataService.createApplicationForActionType(licenceId, actionName).subscribe(
      data => {
        this.router.navigateByUrl('/account-profile/' + data.id);
      },
      () => {
        this.snackBar.open('Error starting a Change Licence Location Application', 'Fail',
          { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a Change Licence Location Application');
      }
    );
  }

  downloadLicence() {

  }


  payLicenceFee(application) {
    this.busy = this.paymentService.getInvoiceFeePaymentSubmissionUrl(application.id).subscribe(res => {
      const data = <any>res;
      window.location.href = data.url;
    }, err => {
      if (err._body === 'Payment already made') {
        this.snackBar.open('Application Fee payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
      }
    });
  }

  renewLicence() {

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

