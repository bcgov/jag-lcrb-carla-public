import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Observable, of, Subscription } from 'rxjs';
import { filter, takeWhile } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { AccountDataService } from '@services/account-data.service';
import { Account } from '@models/account.model';
import { AppState } from '@app/app-state/models/app-state';

@Component({
  selector: 'app-notices',
  templateUrl: './notices.component.html',
  styleUrls: ['./notices.component.scss'],
})
export class NoticesComponent extends FormBase implements OnInit {
  isEditMode = true;
  isReadOnly = false;
  showValidationMessages = false;

  account: Account;
  notices: any[]; // TODO: improve typing here

  busy: Subscription;
  dataLoaded = false; // this is set to true when all page data is loaded


  constructor(
    private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute,
    private accountDataService: AccountDataService,
  ) {
    super();
  }

  ngOnInit() {
    this.retrieveAccount();
  }

  retrieveAccount() {
    this.store
      .select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => this.fetchData(account));
  }

  fetchData(account: Account) {
    this.account = account;
    this.busy = this.retrieveNotices(account)
      .subscribe(notices => {
        this.notices = notices;
        this.dataLoaded = true;
      })

  }

  retrieveNotices(account: Account) {
    // TODO: implement this mocked function
    return of([]);
  }
}
