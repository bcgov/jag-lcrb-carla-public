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

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  public currentUser: User;
  applicationId: string;
  submittedApplications = 8;

  contactId: string = null;
  account: any;
  busy: Subscription;
  isPaid: Boolean;
  orgType = '';

  constructor(private paymentDataService: PaymentDataService,
    private userDataService: UserDataService, private router: Router,
    private dynamicsDataService: DynamicsDataService,
    private applicationDataService: ApplicationDataService,
    public snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.userDataService.getCurrentUser()
      .subscribe((data) => {
        this.currentUser = data;
        if (this.currentUser.accountid != null) {
          // fetch the account to get the primary contact.
          this.dynamicsDataService.getRecord('accounts', this.currentUser.accountid)
            .subscribe((result) => {
              this.account = result;
              if (result.primarycontact) {
                this.contactId = result.primarycontact.id;
              }
            });
        }

      });

    this.applicationDataService.getSubmittedApplicationCount()
      .subscribe(value => this.submittedApplications = value);
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

  startNewLicenceApplication() {
    const newLicenceApplicationData: Application = <Application>{
      licenseType: 'Cannabis Retail Store',
      applicantType: this.account.businessType,
      account: this.account,
      servicehHoursStandardHours: false,
      serviceHoursSundayOpen: '09:00',
      serviceHoursMondayOpen: '09:00',
      serviceHoursTuesdayOpen: '09:00',
      serviceHoursWednesdayOpen: '09:00',
      serviceHoursThursdayOpen: '09:00',
      serviceHoursFridayOpen: '09:00',
      serviceHoursSaturdayOpen: '09:00',
      serviceHoursSundayClose: '23:00',
      serviceHoursMondayClose: '23:00',
      serviceHoursTuesdayClose: '23:00',
      serviceHoursWednesdayClose: '23:00',
      serviceHoursThursdayClose: '23:00',
      serviceHoursFridayClose: '23:00',
      serviceHoursSaturdayClose: '23:00',
    };
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      data => {
        this.router.navigateByUrl(`/account-profile/${data.id}`);
      },
      () => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );
  }
}
