import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-lg-approvals',
  templateUrl: './lg-approvals.component.html',
  styleUrls: ['./lg-approvals.component.scss']
})
export class LgApprovalsComponent implements OnInit {

  account: Account;
  applications: Application[];
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
        this.applications = applications;
        this.dataLoaded = true;
      },
        error => {
          this.snackBar.open(`An error occured while getting approval applications`, 'Fail',
            { duration: 3500, panelClass: ['red-snackbar'] });
        });
        
  }



}
