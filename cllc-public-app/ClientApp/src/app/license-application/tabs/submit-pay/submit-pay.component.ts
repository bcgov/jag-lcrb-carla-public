import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { PaymentDataService } from '../../../services/payment-data.service';
import { Router, ActivatedRoute } from '@angular/router'
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';


@Component({
  selector: 'app-submit-pay',
  templateUrl: './submit-pay.component.html',
  styleUrls: ['./submit-pay.component.scss']
})
export class SubmitPayComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;
  //@Input() applicationId: string;
  busy: Subscription;
  isSubmitted: boolean;
  isPaid: boolean;
  prevPaymentFailed: boolean;

  constructor(private paymentDataService: PaymentDataService, private applicationDataService: AdoxioApplicationDataService, 
  				public snackBar: MatSnackBar, private router: Router) { }

  ngOnInit() {
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        let data = res.json();
        this.isSubmitted = data['isSubmitted']
        this.isPaid = data['isPaid']
        this.prevPaymentFailed = data['prevPaymentFailed']
      },
      err => {
        this.snackBar.open('Error getting Application Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log("Error occured getting Application Details");
      }
    );
  }

  submit_application() 
  {
    this.paymentDataService.getPaymentSubmissionUrl(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var jsonUrl = res.json();
        //window.alert(jsonUrl['url']);
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      },
      err => {
        console.log("Error occured");
      }
    );
  }

  verify_payment()
  {
     this.router.navigate(['./payment-confirmation'], { queryParams: { trnId: '0', SessionKey: this.applicationId } });
  }
}
