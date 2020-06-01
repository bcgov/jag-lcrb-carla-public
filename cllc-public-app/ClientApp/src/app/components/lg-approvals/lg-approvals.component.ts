import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';

@Component({
  selector: 'app-lg-approvals',
  templateUrl: './lg-approvals.component.html',
  styleUrls: ['./lg-approvals.component.scss']
})
export class LgApprovalsComponent implements OnInit {

  account: Account;
  applications: Application[];

  constructor(private store: Store<AppState>,
    private applicationDataService: ApplicationDataService) { 
    store.select(state => state.currentAccountState.currentAccount)
    .subscribe(account =>{
      this.account = account;
    });
  }

  ngOnInit() {
    this.applicationDataService.getLGApprovalApplications()
    .subscribe(applications => this.applications = applications)
  }



}
