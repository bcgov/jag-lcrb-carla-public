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
  busy: Subscription;

  form = this.fb.group({
    agreement: [false, [this.customRequiredCheckboxValidator()]]
  });

  constructor(
    private store: Store<AppState>,
    private fb: FormBuilder,
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
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => {
        this.account = account;
        // default to BC if no province found
        account.physicalAddressProvince = account.physicalAddressProvince || 'British Columbia';
        account.physicalAddressCountry = 'Canada';
        this.setFormToAccount(account);
      });
  }

  setFormToAccount(account: Account) {
    // TODO: put some values on the form...
    this.form.patchValue({});

    if (this.isReadOnly) {
      this.form.disable();
    }
  }
}
