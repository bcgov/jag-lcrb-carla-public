import { Component, OnInit, Input, Inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { AdoxioApplicationDataService } from '@app/services/adoxio-application-data.service';
import { Router } from '@angular/router';
import { AdoxioApplication } from '@app/models/adoxio-application.model';
import { FileSystemItem } from '@app/models/file-system-item.model';
import { PaymentDataService } from '@services/payment-data.service';
import { DynamicsAccount } from './../models/dynamics-account.model';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';
export const TRANSFER_LICENCE_MODE = 'TransferLicenceMode';

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
  licencedApplications: any[] = [];

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;
  @Input() account: DynamicsAccount;
  dataLoaded = false;

  constructor(
    private applicationDataService: AdoxioApplicationDataService,
    private router: Router,
    private paymentService: PaymentDataService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog) { }

  ngOnInit() {
    this.displayApplications();
  }

  /**
   *
   * */
  private displayApplications() {
    this.inProgressApplications = [];
    this.licencedApplications = [];
    this.busy = this.applicationDataService.getAllCurrentApplications().subscribe((adoxioApplications: AdoxioApplication[]) => {
      adoxioApplications.forEach((application: AdoxioApplication | any) => {
        if (application.assignedLicence && application.applicationStatus === 'Approved') {
        this.licencedApplications.push(application);
        } else {
          this.inProgressApplications.push(application);
        }
        application.applicationStatus = this.transformStatus(application);
      });
    });
  }

  uploadMoreFiles(application: AdoxioApplication) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
  }

  transferLicence(application: AdoxioApplication) {
    const newLicenceApplicationData: AdoxioApplication = <AdoxioApplication>{
      licenseType: 'Cannabis Retail Store',
      applicantType: this.account.businessType,
      account: this.account,

      establishmentName: application.establishmentName,
      establishmentparcelid: application.establishmentparcelid,

      establishmentaddressstreet: application.establishmentaddressstreet,
      establishmentaddresscity: application.establishmentaddresscity,
      establishmentaddresspostalcode: application.establishmentaddresspostalcode,

      serviceHoursSundayOpen: application.serviceHoursSundayOpen,
      serviceHoursMondayOpen: application.serviceHoursMondayOpen,
      serviceHoursTuesdayOpen: application.serviceHoursTuesdayOpen,
      serviceHoursWednesdayOpen: application.serviceHoursWednesdayOpen,
      serviceHoursThursdayOpen: application.serviceHoursThursdayOpen,
      serviceHoursFridayOpen: application.serviceHoursFridayOpen,
      serviceHoursSaturdayOpen: application.serviceHoursSaturdayOpen,
      serviceHoursSundayClose: application.serviceHoursSundayClose,
      serviceHoursMondayClose: application.serviceHoursMondayClose,
      serviceHoursTuesdayClose: application.serviceHoursTuesdayClose,
      serviceHoursWednesdayClose: application.serviceHoursWednesdayClose,
      serviceHoursThursdayClose: application.serviceHoursThursdayClose,
      serviceHoursFridayClose: application.serviceHoursFridayClose,
      serviceHoursSaturdayClose: application.serviceHoursSaturdayClose,
    };
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.router.navigate([`/application/${application.id}`, { mode: TRANSFER_LICENCE_MODE }]);
      },
      () => {
        this.snackBar.open('Error starting a Licence Transfer', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a Licence Transfer');
      }
    );

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

  transformStatus(application: AdoxioApplication): string {
    const status = application.applicationStatus;
    let shownStatus = status;

    if (application.assignedLicence && application.applicationStatus === 'Approved') {
      shownStatus = ACTIVE;
      if (application.licenceFeeInvoicePaid !== true && application.licenseType === 'Cannabis Retail Store') {
        shownStatus = PAYMENT_REQUIRED;
      }
      if (application.assignedLicence && (new Date() > new Date(application.assignedLicence.expiryDate))) {
        shownStatus = RENEWAL_DUE;
      }
    } else {
      if (status === 'Intake' && !application.isPaid) {
        if (application.licenseType === 'CRS Transfer of Ownership') {
          shownStatus = 'Transfer Initiated';
        } else {
          shownStatus = 'Not Submitted';
        }
      } else if (status === 'In progress' || status === 'Under Review' || (status === 'Intake' && application.isPaid)) {
        if (application.licenseType === 'CRS Transfer of Ownership') {
          shownStatus = 'Transfer Application Under Review';
        } else {
          shownStatus = 'Application Under Review';
        }
      } else if (status === 'Incomplete') {
        shownStatus = 'Application Incomplete';
      } else if (status === 'PendingForLGFNPFeedback') {
        shownStatus = 'Pending External Review';
      }
    }
    return shownStatus;
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

