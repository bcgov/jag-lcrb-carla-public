import { Component, Input, ViewChild } from '@angular/core';
import { Router } from "@angular/router";
import { DynamicsDataService } from "../services/dynamics-data.service";
import { DynamicsAccount } from "../models/dynamics-account.model";
import { DynamicsContact } from "../models/dynamics-contact.model";
import { User } from "../models/user.model";
import { ReadVarExpr } from '@angular/compiler';
import { AccountDataService } from '../services/account-data.service';

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
  public showBceidUserContinue: boolean;
  businessType: string = "";
  finalBusinessType: string = "";
  busy: Promise<any>;

  /** bceid-confirmation ctor */
  constructor(private router: Router, private accountDataService: AccountDataService, private dynamicsDataService: DynamicsDataService) {
    // Load BCeID data from service
    this.accountDataService.getBCeID().subscribe((data) => {
      let temp = data.json();
      this.businessType = temp.businessTypeCode;
    }, err => {
      console.log(err);
    });
  }

  confirmBceidAccountYes() {
    // confirm BCeID
    if (this.businessType !== "Proprietorship" && this.businessType !== "Partnership") {
      this.bceidConfirmAccount = false;
      this.bceidConfirmBusinessType = true;
    }
    else {
      this.bceidConfirmAccount = false;
      this.confirmCorpType(this.businessType);
    }
  }

  confirmBceidAccountNo() {
    // confirm BCeID
    this.showBceidCorrection = true;
  }

  confirmBceidUser() {
    // confirm BCeID
    this.bceidConfirmContact = true;
  }

  confirmCorpType(propOrPartner) {
    this.bceidConfirmBusinessType = false;
    this.bceidConfirmContact = true;

    // Proprietorship and Partnership do not have radio buttons to chane the value of finalBusienssType
    if (propOrPartner) {
      this.finalBusinessType = propOrPartner;
    } else {
      if (!this.finalBusinessType) {
        if (this.businessType == "Proprietorship" || this.businessType == "Partnership") {
          this.finalBusinessType = this.businessType;
        }
      }
    }

  }

  confirmContactYes() {
    // create a contact
    let account = new DynamicsAccount();
    account.name = this.currentUser.businessname;
    account.id = this.currentUser.accountid;
    let contact = new DynamicsContact();
    contact.fullname = this.currentUser.name;
    contact.id = this.currentUser.contactid;
    account.primarycontact = contact;

    // Submit selected company type and sub-type to the account service
    account.businessType = this.finalBusinessType;
    let payload = JSON.stringify(account);
    this.busy = this.dynamicsDataService.createRecord('account', payload)
      .then((data) => {
        window.location.reload();
      });
  }

  confirmContactNo() {
    // confirm Contact
    this.showBceidUserContinue = true;
  }

}
