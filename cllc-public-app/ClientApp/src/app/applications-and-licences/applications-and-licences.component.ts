import { Component, OnInit, Input, Inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { AdoxioApplicationDataService } from '@app/services/adoxio-application-data.service';
import { Router } from '@angular/router';
import { AdoxioApplication } from '@app/models/adoxio-application.model';
import { FileSystemItem } from '@app/models/file-system-item.model';
import { PaymentDataService } from '@services/payment-data.service';


const UPLOAD_FILES_MODE = 'UploadFilesMode';

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
  dataLoaded = false;

  constructor(
    private adoxioApplicationDataService: AdoxioApplicationDataService,
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
    this.busy = this.adoxioApplicationDataService.getAllCurrentApplications().subscribe((adoxioApplications: AdoxioApplication[]) => {
      adoxioApplications.forEach((licAppSum: AdoxioApplication | any) => {
        licAppSum.applicationStatus = this.transformStatus(licAppSum);
        if (!licAppSum.assignedLicence) {
          this.inProgressApplications.push(licAppSum);
        } else {
          this.busy = this.adoxioApplicationDataService.getFileListAttachedToApplication(licAppSum.id, 'Licence Application Main')
            .subscribe((files: FileSystemItem[]) => {
              if (files && files.length) {
                licAppSum.applicationFormFileUrl = files[0].serverrelativeurl;
                licAppSum.fileName = files[0].name;
              }
            });
          this.licencedApplications.push(licAppSum);
        }
      });
    });
  }

  uploadMoreFiles(application: AdoxioApplication) {
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
          this.busy = this.adoxioApplicationDataService.cancelApplication(applicationId).subscribe(
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
    if (!application.assignedLicence) {
      if (status === 'Intake' && !application.isPaid) {
        shownStatus = 'Not Submitted';
      } else if (status === 'In Progress' || status === 'Under Review' || (status === 'Intake' && application.isPaid)) {
        shownStatus = 'Application Under Review';
      } else if (status === 'Incomplete') {
        shownStatus = 'Application Incomplete';
      } else if (status === 'PendingForLGFNPFeedback') {
        shownStatus = 'Pending External Review';
      }
    } else {
      shownStatus = ACTIVE;
      if (application.licenceFeeInvoicePaid !== true) {
        shownStatus = PAYMENT_REQUIRED;
      }
      if (application.assignedLicence && (new Date() > new Date(application.assignedLicence.expiryDate))) {
        shownStatus = RENEWAL_DUE;
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

