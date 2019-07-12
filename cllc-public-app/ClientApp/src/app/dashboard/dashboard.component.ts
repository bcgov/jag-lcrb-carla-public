import { Component, OnInit } from '@angular/core';
import { User } from '../models/user.model';
import { Subscription } from 'rxjs';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { takeWhile } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends FormBase implements OnInit {
  account: Account;
  indigenousNationModeActive: boolean;

  constructor(private store: Store<AppState>) {
    super();
  }

  ngOnInit(): void {
    this.store.select((state) => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((account) => {
        this.account = account;
      });

    this.store.select((state) => state.indigenousNationState.indigenousNationModeActive)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((active) => {
        this.indigenousNationModeActive = active;
      });
  }
}
