import { Component, Input, ViewChild, Output, EventEmitter } from '@angular/core';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { Account } from '../models/account.model';
import { Contact } from '../models/contact.model';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { AccountDataService } from '../services/account-data.service';
import { Observable, Subscription } from '../../../node_modules/rxjs';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { filter, mergeMap, takeWhile } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';


@Component({
  selector: 'app-bceid-confirmation',
  templateUrl: './bceid-confirmation.component.html',
  styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent extends FormBase {
  @Input() currentUser: User;
  @Output() reloadUser = new EventEmitter();
  public bceidConfirmAccount = true;
  public bceidConfirmBusinessType = false;
  public bceidConfirmContact = false;
  public showBceidCorrection: boolean;
  public showBceidUserContinue = true;
  businessType = '';
  finalBusinessType = '';
  busy: Promise<any>;
  busySubscription: Subscription;
  termsAccepted = false;
  account: Account;

  constructor(private dynamicsDataService: DynamicsDataService,
    private userDataService: UserDataService,
    private store: Store<AppState>,
    private accountDataService: AccountDataService) {
    super();
    // if this passes, this means the user's account exists but it's contact information has not been created.
    // user will skip the BCeid confirmation.
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data) => {
        this.account = data;
        if (this.account) {
          this.termsAccepted = this.account.termsOfUseAccepted;
        }
      },
        error => { });

  }

  confirmBceidAccountYes() {
    this.bceidConfirmAccount = false;
    this.bceidConfirmBusinessType = true;
  }

  confirmBceidAccountNo() {
    // confirm BCeID
    this.showBceidCorrection = true;
  }

  confirmBceidUser() {
    // confirm BCeID
    this.bceidConfirmContact = true;
  }

  confirmCorpType() {
    this.bceidConfirmBusinessType = false;
    this.bceidConfirmContact = true;
  }

  confirmContactYes() {
    const account = <Account>{
      name: this.currentUser.businessname,
      id: this.currentUser.accountid,
      termsOfUseAccepted: true,
      termsOfUseAcceptedDate: new Date()
    };
    this.createContact(account);
  }

  createContact(account) {
    const contact = new Contact();
    contact.fullname = this.currentUser.name;
    contact.id = this.currentUser.contactid;
    account.primarycontact = contact;

    // Submit selected company type and sub-type to the account service
    account.businessType = this.businessType;
    const payload = JSON.stringify(account);
    this.busy = this.dynamicsDataService.createRecord('accounts', payload)
      .toPromise()
      .then((data) => {
        this.userDataService.loadUserToStore().then(res => { });
        this.reloadUser.emit();
      });
  }

  confirmContactNo() {
    // confirm Contact
    this.showBceidUserContinue = false;
  }

  onTermsAccepted(accepted: boolean) {
    this.termsAccepted = accepted;
    if (this.account) {
      const data = { ...this.account, termsOfUseAccepted: true, termsOfUseAcceptedDate: new Date() };
      this.dynamicsDataService.updateRecord('accounts', this.account.id, data)
        .subscribe(res => {
          this.accountDataService.loadCurrentAccountToStore(this.account.id)
            .subscribe(() => { });
        });
    }
  }

}
