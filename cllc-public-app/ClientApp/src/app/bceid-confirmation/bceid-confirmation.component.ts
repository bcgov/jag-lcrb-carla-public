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
  businessType: string = "";
  businessValue: number;
  busy: Promise<any>;

  /** bceid-confirmation ctor */
  constructor(private router: Router, private dynamicsDataService: DynamicsDataService) {
    // TODO load BCeID data from service
    this.businessType = "Corporation";
  }
  
  confirmBceid() {
    // confirm BCeID
    this.currentUser.isBceidConfirmed = true;
  }

   confirmBceidAccountYes() {
    // confirm BCeID
    this.bceidConfirmAccount = false;
    this.bceidConfirmBusinessType = true;
    this.dynamicsDataService.getRecord("account", this.currentUser.accountid)
      .then((data) => {
        console.log(data);
        // this.account = data;
        // if (data.primarycontact) {
        //   this.contactId = data.primarycontact.id;
        // }              
      });
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
      // create a contact
      let account = new DynamicsAccount();
      account.name = this.currentUser.businessname;
      account.id = this.currentUser.accountid;
      let contact = new DynamicsContact();
      contact.fullname = this.currentUser.name;
      contact.id = this.currentUser.contactid;
      account.primarycontact = contact;

      // TODO submit selected comany type and sub-type to the account service
      // account.adoxio_business_type = 
      // account.adoxio_business_subtype = 

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
