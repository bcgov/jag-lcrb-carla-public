import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-lg-approvals',
  templateUrl: './lg-approvals.component.html',
  styleUrls: ['./lg-approvals.component.scss']
})
export class LgApprovalsComponent implements OnInit {

  account: Account;

  constructor(store: Store<AppState>) { 
    store.select(state => state.currentAccountState.currentAccount)
    .subscribe(account =>{
      this.account = account;
    });
  }

  ngOnInit() {
  }

}
