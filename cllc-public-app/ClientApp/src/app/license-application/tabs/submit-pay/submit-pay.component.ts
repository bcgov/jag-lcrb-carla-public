import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { PaymentDataService } from '../../../services/payment-data.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Subscription, Subject ,  Observable ,  zip } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { Application } from '../../../models/application.model';


@Component({
  selector: 'app-submit-pay',
  templateUrl: './submit-pay.component.html',
  styleUrls: ['./submit-pay.component.scss']
})
export class SubmitPayComponent implements OnInit {

  @Input() accountId: string;
  @Input() applicationId: string;
  busy: Subscription;
  isSubmitted: boolean;
  isPaid: boolean;
  prevPaymentFailed: boolean;
  validationMessages = [];
  isLoaded = false;
  isApplicationValid = true;
  displayValidationMessages = false;

  constructor(private paymentDataService: PaymentDataService,
    private applicationDataService: AdoxioApplicationDataService,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private router: Router) {
      this.applicationId =  this.route.parent.snapshot.params.applicationId;
    }

  ngOnInit() {
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        this.isSubmitted = data.isSubmitted;
        this.isPaid = data.isPaid;
        this.prevPaymentFailed = data.prevPaymentFailed;
        this.isLoaded = true;
      },
      err => {
        this.snackBar.open('Error getting Application Details', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured getting Application Details');
      }
    );
  }

  /**
   * Validate the application before it can be submitted
   * @param applicationId
   */
  validateApplication(applicationId: string): Subject<boolean> {
    const result = new Subject<boolean>();
    // get application data and validate all required fields are entered
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      (data: Application) => {
        // validate contact details
        if (this.isNullOrEmpty(data.contactPersonFirstName)
          || this.isNullOrEmpty(data.contactPersonLastName)
          || this.isNullOrEmpty(data.contactPersonRole)
          || this.isNullOrEmpty(data.contactPersonEmail)
          || this.isNullOrEmpty(data.contactPersonPhone)) {
          this.isApplicationValid = false;
          this.validationMessages.push('Contact details are not complete.');
        }
        // validate property details
        if (this.isNullOrEmpty(data.establishmentAddressStreet)
          || this.isNullOrEmpty(data.establishmentAddressCity)
          || this.isNullOrEmpty(data.establishmentAddressPostalCode)
          || this.isNullOrEmpty(data.establishmentParcelId)) {
          this.isApplicationValid = false;
          this.validationMessages.push('Property details are not complete.');
        }
        // validate store info
        if (this.isNullOrEmpty(data.establishmentName)) {
          this.isApplicationValid = false;
          this.validationMessages.push('Store Information details are not complete.');
        }
        // validate declaration
        // if (this.isNullOrEmpty(data.authorizedToSubmit)
        //   || this.isNullOrEmpty(data.signatureagreement)) {
        //   this.isApplicationValid = false;
        //   this.validationMessages.push('Declaration details are not complete.');
        // }

        const fileList: Observable<any>[] = [];
        // get application documents and verify that at least one file has been uploaded per screen
        fileList.push(this.applicationDataService.getFileListAttachedToApplication(this.applicationId, 'Store Information'));
        fileList.push(this.applicationDataService.getFileListAttachedToApplication(this.applicationId, 'Floor Plan'));
        fileList.push(this.applicationDataService.getFileListAttachedToApplication(this.applicationId, 'Site Map'));
        this.busy = zip(...fileList).subscribe(
          response => {
            response.forEach((resp, i) => {
            const files = resp;
            if (files && files.length < 1) {
              this.isApplicationValid = false;
              if (i === 0) {
                this.validationMessages.push('Store Information documents have not been uploaded.');
              } else if (i === 1) {
                this.validationMessages.push('Floor Plan documents have not been uploaded.');
              } else if (i === 2) {
                this.validationMessages.push('Site Plan documents have not been uploaded');
              }
            }
            });
            result.next(this.isApplicationValid);
          });
      },
      err => {
        this.snackBar.open('Error getting Application Details for validation', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error occured getting Application Details for validation');
        result.next(false);
      }
    );
    return result;
  }

  isNullOrEmpty(item: string): boolean {
    if (!item || item.length < 1) {
      return true;
    }
    return false;
  }

  /**
   *
   * */
  submit_application() {
    this.validateApplication(this.applicationId).subscribe(isValid => {
      if (isValid) {
        this.busy = this.paymentDataService.getPaymentSubmissionUrl(this.applicationId).subscribe(
          res => {
            // console.log("applicationVM: ", res;
            const jsonUrl = res;
            // window.alert(jsonUrl['url']);
            window.location.href = jsonUrl['url'];
            return jsonUrl['url'];
          },
          err => {
            if (err._body === 'Payment already made') {
              this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
            }
          }
        );
      } else {
        this.displayValidationMessages = true;
      }
    });

  }

  verify_payment() {
    this.router.navigate(['./payment-confirmation'], { queryParams: { trnId: '0', SessionKey: this.applicationId } });
  }
}
