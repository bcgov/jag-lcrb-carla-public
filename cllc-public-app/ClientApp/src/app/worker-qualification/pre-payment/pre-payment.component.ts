import { Component, OnInit } from '@angular/core';
import { PaymentDataService } from '../../services/payment-data.service';
import { User } from '../../models/user.model';
import { UserDataService } from '../../services/user-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { Worker } from '../../models/worker.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-pre-payment',
  templateUrl: './pre-payment.component.html',
  styleUrls: ['./pre-payment.component.scss']
})
export class PrePaymentComponent implements OnInit {
  currentUser: User;
  worker: Worker;
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
        if (this.currentUser && this.currentUser.contactid) {
            this.workerDataService.getWorker(this.workerId).subscribe(res => {
            this.worker = res;
          });
        }
      });
  }

  /**
  * Submit the application for payment
  * */
  submit_application() {
    // if (!this.isValid()) {
    //   this.showValidationMessages = true;
    // } else if (JSON.stringify(this.savedFormData) === JSON.stringify(this.form.value)) {
    //   this.submitPayment();
    // }
    // else {
    //   this.save(true).subscribe((result: boolean) => {
    //     if (result) {
          this.submitPayment();
    //     }
    //   });
    // }
  }


  /**
   * Redirect to payment processing page (Express Pay / Bambora service)
   * */
  private submitPayment() {
    this.paymentDataService.getWorkerPaymentSubmissionUrl(this.worker.id).subscribe(res => {
      const jsonUrl = res;
      window.location.href = jsonUrl['url'];
      return jsonUrl['url'];
    }, err => {
      console.log('Error occured');
    });
  }

}
