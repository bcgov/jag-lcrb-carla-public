import { Component, OnInit } from '@angular/core';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { AdoxioApplication } from '../models/adoxio-application.model';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: User;
  isApplicant: boolean = false;
  isAssociate: boolean = false;
  accountId: string = null;
  contactId: string = null;
  account: DynamicsAccount;
  busy: Subscription;

  constructor(private userDataService: UserDataService, private dynamicsDataService: DynamicsDataService,
    private applicationDataService: AdoxioApplicationDataService, public snackBar: MatSnackBar,
    private router: Router) {
}

ngOnInit(): void {
  // TODO - pass currentUser in as router data rather than doing another call to getCurrentUser.
  if (!this.user) {
    this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.user = data;

        this.isApplicant = (this.user.businessname != null);
        this.isAssociate = (this.user.businessname == null);
        // console.log("isApplicant = " + this.isApplicant);
        // console.log("isAssociate = " + this.isAssociate);

        if (!this.accountId && this.user) {
          this.accountId = this.user.accountid;
        }
        if (this.accountId != null && !this.isAssociate) {
          // fetch the account to get the primary contact.
          this.dynamicsDataService.getRecord('account', this.accountId)
            .then((result) => {
              this.account = result;
              if (result.primarycontact) {
                this.contactId = result.primarycontact.id;
              }
            });
         }
      });
    }
  }

  /**
   * Start a new Dynamics License Application
   * */
  startNewLicenceApplication() {
    let newLicenceApplicationData: AdoxioApplication = new AdoxioApplication();
    newLicenceApplicationData.licenseType = 'Cannabis Retail Store';
    newLicenceApplicationData.applicantType = this.account.businessType;
    newLicenceApplicationData.account = this.account;
    // newLicenceApplicationData. = this.account.businessType;
    this.busy = this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      res => {
        const data = res.json();
        this.router.navigateByUrl(`/license-application/${data.id}/contact-details`);
      },
      err => {
        this.snackBar.open('Error starting a New Licence Application', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error starting a New Licence Application');
      }
    );
  }

}
