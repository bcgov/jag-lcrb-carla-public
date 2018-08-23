import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { ContactDataService } from '../../services/contact-data.service';
import { DynamicsContact } from '../../models/dynamics-contact.model';

@Component({
  selector: 'app-associate-dashboard',
  templateUrl: './associates-dashboard.component.html',
  styleUrls: ['./associates-dashboard.component.scss']
})
export class AssociatesDashboardComponent implements OnInit {
  currentUser: User;
  isNewUser: boolean;
  dataLoaded = false;

  constructor(private userDataService: UserDataService,
    private contactDataService: ContactDataService) { }

  ngOnInit() {
    this.reloadUser();
  }

  reloadUser() {
    this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
        this.isNewUser = this.currentUser.isNewUser;
        this.dataLoaded = true;
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
