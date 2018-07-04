import { Component, OnInit, Input } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router'
import { PaymentDataService } from '../services/payment-data.service';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-payment-confirmation',
    templateUrl: './payment-confirmation.component.html',
    styleUrls: ['./payment-confirmation.component.scss']
})
/** payment-confirmation component*/
export class PaymentConfirmationComponent {
    busy: Subscription;
    transactionId: string;
    applicationId: string;
    authCode: string;
    avsMessage: string;
    avsAddrMatch: string;
    messageId: string;
    messageText: string;
    paymentMethod: string;
    trnAmount: string;
    trnApproved: string;
    trnDate: string;
    trnId: string;
    trnOrderNumber: string;

    /** payment-confirmation ctor */
    constructor(private router: Router, private route: ActivatedRoute, private paymentDataService: PaymentDataService) {
	    this.route.queryParams.subscribe(params => {
	        this.transactionId = params['trnId'];
	        this.applicationId = params['SessionKey'];
	    });
    }

    ngOnInit() {
    	// TODO get slug from URL parameters and find associated Application
    	this.verify_payment();

	    //setTimeout(function () {
	    //}, 5000);

        //this.router.navigate(['./license-application/' + this.applicationId]);
    }

  verify_payment() 
  {
    this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var json = res.json();
    	console.log(json);

        // TODO do something with it!!!
        this.authCode = json.authCode;
        this.avsMessage = json.avsMessage;
        this.avsAddrMatch = json.avsAddrMatch;
        this.messageId = json.messageId;
        this.messageText = json.messageText;
        this.paymentMethod = json.paymentMethod;
        this.trnAmount = json.trnAmount;
        this.trnApproved = json.trnApproved;
        this.trnDate = json.trnDate;
        this.trnId = json.trnId;
        this.trnOrderNumber = json.trnOrderNumber;

      },
      err => {
        console.log("Error occured");
      }
    );
  }
}
