import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { Account } from '@models/account.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';

@Component({
  selector: 'app-police-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  currentUser: User;
  isNewUser = false;
  dataLoaded = false;
  userLoaded = false;

  assignedJobsInProgress: number;
  assignedJobsInProgressWithExceptions: number;
  assignedApplicationsIssued: number;
  hasLinkedPoliceAccount: boolean;
  account: Account;
  terminatedApplicationExists: boolean;
  nonTerminatedApplicationExists: boolean;

  constructor(private store: Store<AppState>,
    private applicationDataService: ApplicationDataService,
    private sepDataService: SpecialEventsDataService,) { }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .subscribe(user => this.loadUser(user));
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => this.account = account);
    this.loadData();
  }

  loadUser(user: User) {
    this.currentUser = user;
    this.isNewUser = this.currentUser.isNewUser;
    this.userLoaded = true;
  }

  loadData() {
    this.sepDataService.getPoliceHome()
      .subscribe(data => {
        this.assignedJobsInProgress = data.assignedJobsInProgress;
        this.assignedJobsInProgressWithExceptions = data.assignedJobsInProgressWithExceptions;
        this.assignedApplicationsIssued = data.assignedApplicationsIssued;
        this.dataLoaded = true;
        this.hasLinkedPoliceAccount = true;
      },
        error => {
          this.hasLinkedPoliceAccount = false;
          this.dataLoaded = true;
        }
      );

      this.applicationDataService.getApplicationsByType(ApplicationTypeNames.PoliceClaim)
      .subscribe(applications => {
        // check whether there is an application that is not terminated
        this.nonTerminatedApplicationExists =
          applications.filter(app => app.applicationStatus !== "Terminated")
          .length >
          0;

        // check whether there is an application that is terminated
        this.terminatedApplicationExists =
          applications.filter(app => app.applicationStatus === "Terminated")
          .length >
          0;
      });
  }

  requestClaimApproval() {
    const application = {
      applicantType: this.account.businessType,
      applicationType: { name: ApplicationTypeNames.PoliceClaim } as ApplicationType,
      account: this.account,
    } as Application;
    this.applicationDataService.createApplication(application)
      .subscribe(result => {
        this.loadData();
      });
  }

}
