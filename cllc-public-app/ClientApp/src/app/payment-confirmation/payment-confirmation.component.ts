import { Component, OnInit, Input } from '@angular/core';
import {Router} from '@angular/router'
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

    /** payment-confirmation ctor */
    constructor(private router: Router, private paymentDataService: PaymentDataService) {
    }

    ngOnInit() {
    	// TODO get slug from URL parameters and find associated Application
    	this.verify_payment();

	    setTimeout(function () {
	    }, 5000);

        this.router.navigate(['./license-application/9591326e-6b7c-e811-814a-480fcfe9cf31']);
    }

  verify_payment() 
  {
    this.paymentDataService.verifyPaymentSubmission("fake-id").subscribe(
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
