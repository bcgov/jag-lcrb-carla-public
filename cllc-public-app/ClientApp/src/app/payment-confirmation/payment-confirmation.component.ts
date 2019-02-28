import { Component, OnInit, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentDataService } from '../services/payment-data.service';
import { Subscription } from 'rxjs';
import { AlertModule } from 'ngx-bootstrap/alert';

@Component({
  selector: 'app-payment-confirmation',
  templateUrl: './payment-confirmation.component.html',
  styleUrls: ['./payment-confirmation.component.scss']
})
/** payment-confirmation component*/
export class PaymentConfirmationComponent implements OnInit {
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
  isApproved = false;

  paymentTransactionTitle: string;
  paymentTransactionMessage: string;
  loaded = false;
  @Input() inputApplicationId: string;

  /** payment-confirmation ctor */
  constructor(private router: Router,
    private route: ActivatedRoute,
    private paymentDataService: PaymentDataService
  ) {
    this.route.queryParams.subscribe(params => {
      this.transactionId = params['trnId'];
      this.applicationId = params['SessionKey'];
    });
  }

  ngOnInit() {
    if (!this.applicationId) {
      this.applicationId = this.inputApplicationId;
    }
    this.verify_payment();
  }

  /**
   * Payment verification
   * */
  verify_payment() {
    this.busy = this.paymentDataService.verifyPaymentSubmission(this.applicationId).subscribe(
      res => {
        const verifyPayResponse = <any>res;
        // console.log(verifyPayResponse);
        switch (verifyPayResponse.cardType) {
          case 'VI':
            this.cardType = 'Visa';
            break;
          case 'PV':
            this.cardType = 'Visa Debit';
            break;
          case 'MC':
            this.cardType = 'MasterCard';
            break;
          case 'AM':
            this.cardType = 'American Express';
            break;
          case 'MD':
            this.cardType = 'Debit MasterCard';
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

        if (this.trnApproved === '1') {
          this.isApproved = true;
        } else {
          this.isApproved = false;
          if (this.messageId === '559') {
            this.paymentTransactionTitle = 'Cancelled';
            this.paymentTransactionMessage = 'Your payment transaction was cancelled. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>';
          } else if (this.messageId === '7') {
            this.paymentTransactionTitle = 'Declined';
            this.paymentTransactionMessage = 'Your payment transaction was declined. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>';
          } else {
            this.paymentTransactionTitle = 'Declined';
            this.paymentTransactionMessage = 'Your payment transaction was declined. Please contact your bank for more information. <br><br> <p>Please note, your application remains listed under Applications In Progress. </p>';
          }
        }

        this.loaded = true;
      },
      err => {
        console.log('Error occured');
      }
    );
  }

  /**
   * Return to dashboard
   * */
  return_to_application() {
    if (this.trnApproved === '1') {
      this.router.navigate(['./dashboard']);
    } else {
      this.router.navigate(['./application-lite/' + this.applicationId]);
    }
  }

  /**
   * Print current page
   * */
  printPage() {
    window.print();
  }
}
