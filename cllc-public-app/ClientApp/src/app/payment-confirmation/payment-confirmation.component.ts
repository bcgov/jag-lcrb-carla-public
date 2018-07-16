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
  cardType: string;
  trnAmount: string;
  trnApproved: string;
  trnDate: string;
  trnId: string;
  trnOrderNumber: string;
  invoice: string;
  isApproved: boolean = false;

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
    }

  /**
   * Payment verification
   * */
  verify_payment()
  {
    this.busy = this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var verifyPayResponse = res.json();
        //console.log(verifyPayResponse);
        switch (verifyPayResponse.cardType) {
        	case 'VI': 
        		this.cardType = "Visa";
        		break;
        	case 'PV': 
        		this.cardType = "Visa Debit";
        		break;
        	case 'MC': 
        		this.cardType = "MasterCard";
        		break;
        	case 'AM': 
        		this.cardType = "American Express";
        		break;
        	case 'MD': 
        		this.cardType = "Debit MasterCard";
        		break;
        	default:
        		this.cardType = verifyPayResponse.cardType;
        }
        this.authCode = verifyPayResponse.authCode;
        this.avsMessage = verifyPayResponse.avsMessage;
        this.avsAddrMatch = verifyPayResponse.avsAddrMatch;
        this.messageId = verifyPayResponse.messageId;
        this.messageText = verifyPayResponse.messageText;
        this.paymentMethod = verifyPayResponse.paymentMethod;
        this.trnAmount = verifyPayResponse.trnAmount;
        this.trnApproved = verifyPayResponse.trnApproved;
        this.trnDate = verifyPayResponse.trnDate;
        this.trnId = verifyPayResponse.trnId;
        this.trnOrderNumber = verifyPayResponse.trnOrderNumber;
        this.invoice = verifyPayResponse.invoice;

        if (this.trnApproved == "1") {
          this.isApproved = true;
        } else {
          this.isApproved = false;
        }


      },
      err => {
        console.log("Error occured");
      }
    );
  }

  return_to_application()
  {
    this.router.navigate(['./license-application/' + this.applicationId + '/submit-pay']);
  }
}
