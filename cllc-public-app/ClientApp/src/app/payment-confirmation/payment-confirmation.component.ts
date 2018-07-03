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
    orderNum: string;
    applicationId: string;

    /** payment-confirmation ctor */
    constructor(private router: Router, private route: ActivatedRoute, private paymentDataService: PaymentDataService) {
	    this.route.queryParams.subscribe(params => {
	        this.orderNum = params['trnId'];
	        this.applicationId = params['SessionKey'];
	    });
    }

    ngOnInit() {
    	// TODO get slug from URL parameters and find associated Application
        window.alert("trnId=" + this.orderNum + ", SessionKey=" + this.applicationId);

    	this.verify_payment();

	    setTimeout(function () {
	    }, 5000);

        //this.router.navigate(['./license-application/' + this.applicationId]);
    }

  verify_payment() 
  {
    this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var jsonUrl = res.json();
        return jsonUrl['url'];
      },
      err => {
        console.log("Error occured");
      }
    );
  }
}
