import { Component, Input, ViewChild } from '@angular/core';
import { Router } from "@angular/router";
import { DynamicsDataService } from "../services/dynamics-data.service";
import { DynamicsAccount } from "../models/dynamics-account.model";
import { User } from "../models/user.model";

@Component({
    selector: 'app-bceid-confirmation',
    templateUrl: './bceid-confirmation.component.html',
    styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent {
  @Input('currentUser') currentUser: User;
  public bceidConfirmAccount: boolean;
  public bceidConfirmContact: boolean;
  public showBceidCorrection: boolean;
  public showBceidUserContinue: boolean;

    /** bceid-confirmation ctor */
  constructor(private router: Router, private dynamicsDataService: DynamicsDataService) {

  }
  
  confirmBceid() {
    // confirm BCeID
    this.currentUser.isBceidConfirmed = true;
  }


   confirmBceidAccountYes() {
    // confirm BCeID
    this.bceidConfirmAccount = true;
  }

   confirmBceidAccountNo() {
     // confirm BCeID
     this.showBceidCorrection = true;
   }

    confirmBceidUser() {
      // confirm BCeID

      this.bceidConfirmContact = true;
    }

    confirmContactYes() {
      // create a contact.
      var account = new DynamicsAccount();
      var payload = JSON.stringify(account);
      this.dynamicsDataService.createRecord('account', payload)
        .then((data) => {          
            window.location.reload();
        });          
    }

    confirmContactNo() {
      // confirm Contact
      this.showBceidUserContinue = true;
    }
    
}
