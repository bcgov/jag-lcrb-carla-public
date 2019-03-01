import { Component, OnInit, Input, ViewChild, Inject } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatTableDataSource, MatPaginator, MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { LicenseApplicationSummary } from '@appmodels/license-application-summary.model';
import { AdoxioApplicationDataService } from '@appservices/adoxio-application-data.service';
import { Router } from '@angular/router';
import { AdoxioApplication } from '@appmodels/adoxio-application.model';
import { FileSystemItem } from '@appmodels/file-system-item.model';
import { UPLOAD_FILES_MODE } from '@applite-application-dashboard/lite-application-dashboard.component';
import { PaymentDataService } from '@appservices/payment-data.service';

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

      adoxioApplications.forEach((entry) => {
        const licAppSum: any = entry;


        if (!licAppSum.isPaid) {
          this.inProgressApplications.push(licAppSum);
          this.licencedApplications.push(licAppSum);
        } else {
          if (!entry.assignedLicence) {
            this.busy = this.adoxioApplicationDataService.getFileListAttachedToApplication(entry.id, 'Licence Application Main')
              .subscribe((files: FileSystemItem[]) => {
                if (files && files.length) {
                  licAppSum.applicationFormFileUrl = files[0].serverrelativeurl;
                  licAppSum.fileName = files[0].name;
                }
              });
          }
          this.licencedApplications.push(licAppSum);
        }
      });
    });
  }

  uploadMoreFiles(application: AdoxioApplication) {
    this.router.navigate([`/application-lite/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
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
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(
      cancelApplication => {
        if (cancelApplication) {
          // delete the application.
          this.busy = this.adoxioApplicationDataService.cancelApplication(applicationId).subscribe(
            res => {
              this.displayApplications();
            });
        }
      }
    );

  }

  getLicenceStatus(application: AdoxioApplication): string {
    let status = ACTIVE;
    if (application.licenceFeeInvoicePaid !== true) {
      status = PAYMENT_REQUIRED;
    }
    if (application.assignedLicence && (new Date() > new Date(application.assignedLicence.expiryDate))) {
      status = RENEWAL_DUE;
    }

    return status;
  }

  downloadLicence(application) {

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

  renewLicence(application) {

  }

}

@Component({
  selector: 'app-application-cancellation-dialog',
  templateUrl: 'application-cancellation-dialog.html',
})
export class ConfirmationDialogComponent {

  establishmentName: string;
  applicationName: string;

  constructor(
    public dialogRef: MatDialogRef<ConfirmationDialogComponent>,
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

