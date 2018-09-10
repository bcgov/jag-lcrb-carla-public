import { Component, OnInit } from '@angular/core';
import { PaymentDataService } from '../../services/payment-data.service';
import { UserDataService } from '../../services/user-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { User } from '../../models/user.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-spd-consent',
  templateUrl: './spd-consent.component.html',
  styleUrls: ['./spd-consent.component.scss']
})
export class SpdConsentComponent implements OnInit {
  currentUser: any;
  workerId: string;

  constructor(
    private paymentDataService: PaymentDataService,
    private workerDataService: WorkerDataService,
    private userDataService: UserDataService,
    private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.workerId = params.id;
    });
  }

  ngOnInit() {
    this.reloadUser();
  }

  reloadUser() {
    this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
      });
  }

}
