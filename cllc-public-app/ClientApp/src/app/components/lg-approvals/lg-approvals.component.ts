import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { MatSnackBar } from '@angular/material';
import * as moment from 'moment';

@Component({
  selector: 'app-lg-approvals',
  templateUrl: './lg-approvals.component.html',
  styleUrls: ['./lg-approvals.component.scss']
})
export class LgApprovalsComponent implements OnInit {

  account: Account;
  applications: Application[];
  applicationsDecisionNotMade: Application[] = [];
  applicationsForZoning: Application[] = [];
  applicationsDecisionMadeButNoDocs: Application[] = [];
  busy: any;
  dataLoaded = false; // this is set to true when all page data is loaded

  constructor(private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private applicationDataService: ApplicationDataService) {
  }

  ngOnInit() {
    // get account
    this.store.select(state => state.currentAccountState.currentAccount)
      .subscribe(account => {
        this.account = account;
      });

    // get approval applications
    this.busy = this.applicationDataService.getLGApprovalApplications()
      .subscribe(applications => {
        this.applications = applications || [];
        this.applicationsDecisionNotMade = this.applications.filter(app => !app.lGDecisionSubmissionDate && app.applicationType && app.applicationType.isShowLGINApproval);
        this.applicationsForZoning = this.applications.filter(app => !app.lGDecisionSubmissionDate && app.applicationType && app.applicationType.isShowLGZoningConfirmation);
        this.applicationsDecisionMadeButNoDocs = this.applications
          .filter(app => app.lGDecisionSubmissionDate
            && !app.resolutionDocsUploaded
            && app.lGApprovalDecision === 'Approved');
        this.dataLoaded = true;
      },
        error => {
          this.snackBar.open(`An error occured while getting approval applications`, 'Fail',
            { duration: 3500, panelClass: ['red-snackbar'] });
        });

  }

  get90dayCount(submissionDate: Date): number {
    let submission = moment(submissionDate)
      .startOf('day')
      .add(90, 'day');
    let current = moment().startOf('day');
    let count = submission.diff(current, 'days');
    return count;
  }

  noApplications(): boolean {
    let res = this.applicationsDecisionNotMade.length === 0
      && this.applicationsDecisionMadeButNoDocs.length === 0
      && this.applicationsForZoning.length === 0;
    return res;
  }
}
