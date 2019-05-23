import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { Router } from '@angular/router';
import { Application } from '../models/application.model';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { ApplicationDataService } from '../services/application-data.service';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { PaymentDataService } from '../services/payment-data.service';
import { ApplicationType, ApplicationTypeNames } from '@app/models/application-type.model';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { takeWhile } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends FormBase implements OnInit {
  public currentUser: User;
  applicationId: string;

  contactId: string = null;
  account: any;
  busy: Subscription;
  isPaid: Boolean;
  orgType = '';
  console = console;

  constructor(private paymentDataService: PaymentDataService,
    private userDataService: UserDataService, private router: Router,
    private dynamicsDataService: DynamicsDataService,
    private applicationDataService: ApplicationDataService,
    private store: Store<AppState>,
    public snackBar: MatSnackBar) {
      super();
     }

  ngOnInit(): void {
    this.store.select((state) => state.currentAccountState.currentAccount)
    .pipe(takeWhile(() => this.componentActive))
      .subscribe((account) => {
        this.account = account;
        if (account && account.primarycontact) {
          this.contactId = account.primarycontact.id;
        }
      });
  }

  verify_payment() {
    const newLicenceApplicationData: Application = new Application();
    newLicenceApplicationData.licenseType = 'Cannabis Retail Store';
    newLicenceApplicationData.applicantType = this.account.businessType;
    newLicenceApplicationData.account = this.account;
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.busy = this.paymentDataService.getPaymentSubmissionUrl(data.id).subscribe(
          res2 => {
            // console.log("applicationVM: ", res);
            const jsonUrl = res2;
            // window.alert(jsonUrl['url']);
            window.location.href = jsonUrl['url'];
            return jsonUrl['url'];
          },
          err => {
            if (err._body === 'Payment already made') {
              this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
            }
          }
        );

        // this.router.navigate(['./payment-confirmation'], { queryParams: { trnId: '0', SessionKey: data.id } });
      },
      () => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );

  }
}
