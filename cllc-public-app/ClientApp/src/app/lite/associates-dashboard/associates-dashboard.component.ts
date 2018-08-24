import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { ContactDataService } from '../../services/contact-data.service';
import { DynamicsContact } from '../../models/dynamics-contact.model';
import { AppState } from '../../app-state/models/app-state';
import * as CurrentUserActions from '../../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-associate-dashboard',
  templateUrl: './associates-dashboard.component.html',
  styleUrls: ['./associates-dashboard.component.scss']
})
export class AssociatesDashboardComponent implements OnInit {
  currentUser: User;
  isNewUser: boolean;
  dataLoaded = false;
  contact: DynamicsContact;

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private contactDataService: ContactDataService) { }

  ngOnInit() {
    this.reloadUser();

  }

  reloadUser() {
    this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
        this.store.dispatch(new CurrentUserActions.SetCurrentUserAction(data));
        this.isNewUser = this.currentUser.isNewUser;
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.contactid) {
          this.contactDataService.getContact(this.currentUser.contactid).
            subscribe(res => {
              this.contact = res;
            });
        }
      });
  }

  confirmContact(confirm: boolean) {
    if (confirm) {
      //create contact here
      let contact = new DynamicsContact();
      contact.fullname = this.currentUser.name;
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      this.contactDataService.createContact(contact).subscribe(res => {
        this.reloadUser();
      })
    } else {
      window.location.href = 'logout';
    }
  }

}
