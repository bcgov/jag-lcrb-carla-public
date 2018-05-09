import { Component, Input, ViewChild } from '@angular/core';
import { Router } from "@angular/router";
import { DynamicsFormComponent } from '../dynamics-form/dynamics-form.component';
import { User } from "../models/user.model";

@Component({
    selector: 'app-bceid-confirmation',
    templateUrl: './bceid-confirmation.component.html',
    styleUrls: ['./bceid-confirmation.component.scss']
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent {
  @Input('currentUser') currentUser: User;

  @ViewChild(DynamicsFormComponent)
  private dynamicsFormComponent: DynamicsFormComponent;

    /** bceid-confirmation ctor */
  constructor(private router: Router) {

  }
    confirmBceid() {
      // confirm BCeID
      this.currentUser.isBceidConfirmed = true;
    }

    confirmContact() {
      // confirm Contact
      this.dynamicsFormComponent.onSubmit();
      this.currentUser.isContactCreated = true;      
    }

    confirmAccount() {
      // confirm Account
      this.dynamicsFormComponent.onSubmit();
      this.currentUser.isAccountCreated = true;

      this.router.navigate( [this.router.url] );
    }
    
}
