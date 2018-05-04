import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-bceid-confirmation',
    templateUrl: './bceid-confirmation.component.html',
    styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent {
  @Input('currentUser') currentUser: any;

    /** bceid-confirmation ctor */
    constructor() {

  }
    confirmBceid() {
      // confirm BCeID
      this.currentUser.isBceidConfirmed = true;
    }

    confirmContact() {
      // confirm BCeID
      this.currentUser.isContactCreated = true;
    }

    confirmAccount() {
      // confirm BCeID
      this.currentUser.isAccountCreated = true;
    }
    
}
