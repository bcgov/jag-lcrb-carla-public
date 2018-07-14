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
  isApproved: boolean;

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

  verify_payment() 
  {
    this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var json = res.json();
    	console.log(json);

        switch (json.cardType) {
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
        		this.cardType = json.cardType;
        }
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
        this.invoice = json.invoice;

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
