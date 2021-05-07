import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faChevronRight, faTrash } from '@fortawesome/free-solid-svg-icons';
import { filter, takeWhile } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { User } from '@models/user.model';
import { AccountDataService } from '@services/account-data.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-approval-settings',
  templateUrl: './approval-settings.component.html',
  styleUrls: ['./approval-settings.component.scss']
})
export class ApprovalSettingsComponent extends FormBase implements OnInit {
  // icons
  faTrash = faTrash;
  faChevronRight = faChevronRight;

  // component state
  showErrorSection = false;
  validationMessages: string[];
  currentUser: User;

  // form
  form = this.fb.group({
    id: [],
    isLateHoursApproval: [false],
    maxGuestsForPublicEvents: ['', Validators.required],
    maxGuestsForPrivateEvents: ['', Validators.required],
    maxGuestsForFamilyEvents: ['', Validators.required],
  });

  constructor(
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private fb: FormBuilder,
    private router: Router,
  ) {
    super();
  }

  ngOnInit() {
    this.subscribeForData();
  }

  private subscribeForData() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => this.loadAccount(account));
  }

  private loadAccount(account: Account): void {
    this.account = account;
    this.form.patchValue({
      id: account.id,
      isLateHoursApproval: account.isLateHoursApproval,
      maxGuestsForPublicEvents: account.maxGuestsForPublicEvents,
      maxGuestsForPrivateEvents: account.maxGuestsForPrivateEvents,
      maxGuestsForFamilyEvents: account.maxGuestsForFamilyEvents,
    });
  }

  save() {
    const validForm = this.validateForm();
    if (!validForm) {
      this.showErrorSection = true;
      return;
    }

    // Do not show validation errors when form is valid
    this.showErrorSection = false;

    // Update Police account record
    const data = { ...this.form.value } as Account;
    this.accountDataService.updateAccount(data)
      .subscribe(() => this.router.navigate(['/sep/dashboard']));
  }

  validateForm(): boolean {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.form, this.validationErrorMap))];
    this.markControlsAsTouched(this.form);
    const isValid = this.validationMessages.length == 0;
    return isValid;
  }

  get validationErrorMap() {
    return {
      'maxGuestsForPublicEvents': 'Please enter the maximum number of guests permitted before Police Approval is required for Public Events',
      'maxGuestsForPrivateEvents': 'Please enter the maximum number of guests permitted before Police Approval is required for Private Events',
      'maxGuestsForFamilyEvents': 'Please enter the maximum number of guests permitted before Police Approval is required for Family Events',
    };
  }
}
