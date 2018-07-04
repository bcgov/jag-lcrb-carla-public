import { Component, OnInit, Input } from '@angular/core';
import { PaymentDataService } from '../../../services/payment-data.service';
import { Subscription } from 'rxjs';

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

  constructor(private paymentDataService: PaymentDataService) { }

  ngOnInit() {
  }

  submit_application() 
  {
    this.paymentDataService.getPaymentSubmissionUrl(this.applicationId).subscribe(
      res => {
        //console.log("applicationVM: ", res.json());
        var jsonUrl = res.json();
        window.alert(jsonUrl['url']);
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      },
      err => {
        console.log("Error occured");
      }
    );
  }
}
