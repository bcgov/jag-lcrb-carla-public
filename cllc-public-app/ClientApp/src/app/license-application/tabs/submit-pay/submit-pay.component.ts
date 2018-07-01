import { Component, OnInit, Input } from '@angular/core';
import { PaymentDataService } from '../../../services/payment-data.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-submit-pay',
  templateUrl: './submit-pay.component.html',
  styleUrls: ['./submit-pay.component.scss']
})
export class SubmitPayComponent implements OnInit {
  @Input() applicationId: string;
  busy: Subscription;

  constructor() { }

  ngOnInit() {
  }

  submit_application() 
  {
  }

}
