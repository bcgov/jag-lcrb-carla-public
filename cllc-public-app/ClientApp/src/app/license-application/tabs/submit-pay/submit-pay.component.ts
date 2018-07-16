import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { PaymentDataService } from '../../../services/payment-data.service';
import { Router, ActivatedRoute } from '@angular/router'
import { Subscription, Subject } from 'rxjs';
import { MatSnackBar } from '@angular/material';


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
  validationMessage: string;

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
      res => {
        const data = res.json();
        this.isSubmitted = data.isSubmitted;
        this.isPaid = data.isPaid;
        this.prevPaymentFailed = data.prevPaymentFailed;
      },
      err => {
        this.snackBar.open('Error getting Application Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
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
      res => {
        const data = res.json();
        let isApplicationValid = true;
        this.validationMessage = '';
        // validate contact details
        if (this.isNullOrEmpty(data.contactpersonfirstname)
          || this.isNullOrEmpty(data.contactpersonlastname)
          || this.isNullOrEmpty(data.contactpersonrole)
          || this.isNullOrEmpty(data.contactpersonemail)
          || this.isNullOrEmpty(data.contactpersonphone)) {
          isApplicationValid = false;
          this.validationMessage = 'Contact details are not complete.\r\n';
        }
        // validate property details
        if (this.isNullOrEmpty(data.establishmentaddressstreet)
          || this.isNullOrEmpty(data.establishmentaddresscity)
          || this.isNullOrEmpty(data.establishmentaddresspostalcode)
          || this.isNullOrEmpty(data.establishmentparcelid)) {
          isApplicationValid = false;
          this.validationMessage += 'Property details are not complete.\r\n';
        }
        // validate store info
        if (this.isNullOrEmpty(data.establishmentName)) {
          isApplicationValid = false;
          this.validationMessage += 'Store Information is not complete.\r\n';
        }
        // validate declaration
        if (this.isNullOrEmpty(data.authorizedtosubmit)
          || this.isNullOrEmpty(data.signatureagreement)) {
          isApplicationValid = false;
          this.validationMessage += 'Declarations are not complete.\r\n';
        }
        if (!isApplicationValid) {
          alert(this.validationMessage + 'Please complete the application before you can submit.');
        }
        result.next(isApplicationValid);
      },
      err => {
        this.snackBar.open('Error getting Application Details for validation', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
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
    this.busy = this.validateApplication(this.applicationId).subscribe(isValid => {
      if (isValid) {
        this.busy = this.paymentDataService.getPaymentSubmissionUrl(this.applicationId).subscribe(
          res => {
            // console.log("applicationVM: ", res.json());
            const jsonUrl = res.json();
            // window.alert(jsonUrl['url']);
            window.location.href = jsonUrl['url'];
            return jsonUrl['url'];
          },
          err => {
            console.log('Error occured');
          }
        );
      }
    });

  }

  verify_payment() {
    this.router.navigate(['./payment-confirmation'], { queryParams: { trnId: '0', SessionKey: this.applicationId } });
  }
}
