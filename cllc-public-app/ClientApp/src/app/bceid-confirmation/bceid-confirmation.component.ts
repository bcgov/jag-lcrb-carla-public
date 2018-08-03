import { Component, Input, ViewChild } from '@angular/core';
import { DynamicsDataService } from "../services/dynamics-data.service";
import { DynamicsAccount } from "../models/dynamics-account.model";
import { DynamicsContact } from "../models/dynamics-contact.model";
import { User } from "../models/user.model";
import { UserDataService } from '../services/user-data.service';
import { AccountDataService } from '../services/account-data.service'
import { Observable, Subscription } from '../../../node_modules/rxjs';

@Component({
  selector: 'app-bceid-confirmation',
  templateUrl: './bceid-confirmation.component.html',
  styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent {
  @Input('currentUser') currentUser: User;
  public bceidConfirmAccount: boolean = true;
  public bceidConfirmBusinessType: boolean = false;
  public bceidConfirmContact: boolean = false;
  public showBceidCorrection: boolean;
  public showBceidUserContinue: boolean = true;
  businessType: string = "";
  finalBusinessType: string = "";
  busy: Promise<any>;
  busySubscription: Subscription;
  accountExists: boolean = true;
  termsAccepted: boolean = false;

  constructor(private dynamicsDataService: DynamicsDataService, private userDataService: UserDataService, private accountDataService: AccountDataService) {
    // if this passes, this means the user's account exists but it's contact information has not been created.
    // user will skip the BCeid confirmation.
    this.busySubscription = this.accountDataService.getCurrentAccount().subscribe((data) => {
      let account = data.json();
      this.createContact(account);
    },
    error => {
      // continue as normal
      this.accountExists = false
    });

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
    let account = new DynamicsAccount();
    account.name = this.currentUser.businessname;
    account.id = this.currentUser.accountid;
    this.createContact(account);
  }

  createContact(account) {
    let contact = new DynamicsContact();
    contact.fullname = this.currentUser.name;
    contact.id = this.currentUser.contactid;
    account.primarycontact = contact;

    // Submit selected company type and sub-type to the account service
    account.businessType = this.businessType;
    let payload = JSON.stringify(account);
    this.busy = this.dynamicsDataService.createRecord('account', payload)
      .then((data) => {
        this.currentUser.isNewUser = false;
      });
  }

  confirmContactNo() {
    // confirm Contact
    this.showBceidUserContinue = false;
  }

}
