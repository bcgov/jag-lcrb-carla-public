import { Component, Input, ViewChild } from '@angular/core';
import { Router } from "@angular/router";
import { DynamicsDataService } from "../services/dynamics-data.service";
import { DynamicsAccount } from "../models/dynamics-account.model";
import { DynamicsContact } from "../models/dynamics-contact.model";
import { User } from "../models/user.model";
import { ReadVarExpr } from '@angular/compiler';

// class BusinessType{
//   value: string;
// }

@Component({
    selector: 'app-bceid-confirmation',
    templateUrl: './bceid-confirmation.component.html',
    styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent {
  @Input('currentUser') currentUser: User;
  public bceidConfirmAccount: boolean=true;
  public bceidConfirmBusinessType: boolean=false;
  public bceidConfirmContact: boolean=false;
  public showBceidCorrection: boolean;
  public showBceidUserContinue: boolean;
  corp: boolean;
  businessType: string = "";
  prefix: string = "a";
  businessValue: number;
  busy: Promise<any>;

  /** bceid-confirmation ctor */
  constructor(private router: Router, private dynamicsDataService: DynamicsDataService) {
    // TODO load BCeID data from service
    this.businessType = "Corporation";
  }

  onTypeChange(select) {
   switch (select.value) {
     case "void":
     case "proprietorship":
     case "partnership":
     case "corporation":
       this.prefix = "a";
       break;
     case "extra provincially registered company":
     case "other":
       this.prefix = "an";
       break;
   
     default:
       break;
   }
  }
  
  confirmBceid() {
    // confirm BCeID
    this.currentUser.isBceidConfirmed = true;
  }

   confirmBceidAccountYes() {
    // confirm BCeID
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
      this.corp = true;
      this.bceidConfirmBusinessType = false;
      this.bceidConfirmContact = true;
    }

    confirmContactYes() {
      // create a contact
      var account = new DynamicsAccount();
      account.name = this.currentUser.businessname;
      account.id = this.currentUser.accountid;
      var contact = new DynamicsContact();
      contact.fullname = this.currentUser.name;
      contact.id = this.currentUser.contactid;
      account.primarycontact = contact;
      // account.adoxio_businesstype = this.corp;

      // TODO submit selected comany type and sub-type to the account service
      var payload = JSON.stringify(account);
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
