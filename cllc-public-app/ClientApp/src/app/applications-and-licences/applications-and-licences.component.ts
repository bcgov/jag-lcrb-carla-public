import { Component, OnInit, Input, Inject } from '@angular/core';
import { forkJoin, Subscription } from 'rxjs';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatSnackBar } from '@angular/material';
import { AdoxioApplicationDataService } from '@app/services/adoxio-application-data.service';
import { AdoxioLicenseDataService } from '@app/services/adoxio-license-data.service';
import { Router } from '@angular/router';
import { Application } from '@app/models/application.model';
import { License } from '@app/models/license.model';
import { FileSystemItem } from '@app/models/file-system-item.model';
import { PaymentDataService } from '@services/payment-data.service';
import { DynamicsAccount } from './../models/dynamics-account.model';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';
export const TRANSFER_LICENCE_MODE = 'TransferLicenceMode';
export const CHANGE_OF_LOCATION_MODE = 'ChangeOfLocationMode';


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
  licenses: any[] = [];

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;
  readonly TRANSFER_LICENCE_MODE = TRANSFER_LICENCE_MODE;
  readonly CHANGE_OF_LOCATION_MODE = CHANGE_OF_LOCATION_MODE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;
  @Input() account: DynamicsAccount;
  dataLoaded = false;

  constructor(
    private applicationDataService: AdoxioApplicationDataService,
    private licenceDataService: AdoxioLicenseDataService,
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
    this.licenses = [];
    this.busy =
      forkJoin(this.applicationDataService.getAllCurrentApplications(), this.licenceDataService.getAllCurrentLicenses()
        ).subscribe(([applications, licenses]) => {
      applications.forEach((application: Application | any) => {
          application.applicationStatus = this.transformStatus(application);
          this.inProgressApplications.push(application);
      });

      licenses.forEach((licence: License | any) => {        
        this.licenses.push(licence);
      });

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

  changeLicenceLocation(application: Application) {
     // create an application for relocation, linked to the given licence.
    
    var licenceId = application.assignedLicence.id;

    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.licenceDataService.createChangeOfLocationApplication(licenceId).subscribe(
        data => {
          this.router.navigateByUrl('/account-profile/' + data.id + ';mode=ChangeOfLocationMode');
        },
        () => {
          this.snackBar.open('Error starting a Change Licence Location Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          console.log('Error starting a Change Licence Location Application');
        }
      );
    
  }

  transformStatus(application: Application): string {
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
        } else
          if (application.licenseType === 'CRS Location Change') {
            shownStatus = 'Relocation Initiated';
          } else
        {
          shownStatus = 'Not Submitted';
        }
      } else if (status === 'In progress' || status === 'Under Review' || (status === 'Intake' && application.isPaid)) {
        if (application.licenseType === 'CRS Transfer of Ownership') {
          shownStatus = 'Transfer Application Under Review';
        } else if (application.licenseType === 'CRS Location Change') {
          shownStatus = 'Relocation Application Under Review';
        } else
        {
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

