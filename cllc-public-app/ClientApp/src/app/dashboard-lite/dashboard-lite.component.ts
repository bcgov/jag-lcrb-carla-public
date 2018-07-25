import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { Router } from '@angular/router';
import { AdoxioApplication } from '../models/adoxio-application.model';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { PaymentDataService } from '../services/payment-data.service';

@Component({
  selector: 'app-dashboard-lite',
  templateUrl: './dashboard-lite.component.html',
  styleUrls: ['./dashboard-lite.component.css']
})
export class DashboardLiteComponent implements OnInit {

  constructor(private paymentDataService: PaymentDataService,
    private userDataService: UserDataService, private router: Router,
    private dynamicsDataService: DynamicsDataService,
    private applicationDataService: AdoxioApplicationDataService,
    public snackBar: MatSnackBar) { }

  public currentUser: User;
  applicationId: string;

  contactId: string = null;
  account: DynamicsAccount;
  busy: Subscription;
  isPaid: Boolean;

  ngOnInit(): void {
    this.userDataService.getCurrentUser()
      .subscribe((data) => {
        this.currentUser = data;

        if (this.currentUser.accountid != null) {
          // fetch the account to get the primary contact.
          this.dynamicsDataService.getRecord('account', this.currentUser.accountid)
            .then((result) => {
              this.account = result;
              if (result.primarycontact) {
                this.contactId = result.primarycontact.id;
              }
            });
        }

      });
  }

  verify_payment() {



    let newLicenceApplicationData: AdoxioApplication = new AdoxioApplication();
    newLicenceApplicationData.licenseType = 'Cannabis Retail Store';
    newLicenceApplicationData.applicantType = this.account.businessType;
    newLicenceApplicationData.account = this.account;
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      res => {
        const data = res.json();

        this.busy = this.paymentDataService.getPaymentSubmissionUrl(data.id).subscribe(
          res => {
            // console.log("applicationVM: ", res.json());
            const jsonUrl = res.json();
            // window.alert(jsonUrl['url']);
            window.location.href = jsonUrl['url'];
            return jsonUrl['url'];
          },
          err => {
            console.log('Error occured');
          }
        );

        // this.router.navigate(['./payment-confirmation'], { queryParams: { trnId: '0', SessionKey: data.id } });        
      },
      err => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );

  }
}
