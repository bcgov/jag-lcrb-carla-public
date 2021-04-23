import { Component, Input, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { Contact } from '@models/contact.model';
import { User } from '@models/user.model';
import { ContactDataService } from '@services/contact-data.service';
import { UserDataService } from '@services/user-data.service';

@Component({
  selector: 'app-servicecard-user-confirmation',
  templateUrl: './servicecard-user-confirmation.component.html',
  styleUrls: ['./servicecard-user-confirmation.component.scss']
})
export class ServicecardUserConfirmationComponent implements OnInit {
  @Input() currentUser: User;
  busy: Subscription;
  termsAccepted = false;

  constructor(private userDataService: UserDataService, private contactDataService: ContactDataService) { }

  ngOnInit() {
  }

  confirmContact(confirm: boolean) {
    if (confirm) {
      // create contact here
      const contact = new Contact();
      contact.fullname = this.currentUser.name;
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      contact.secondaryIdentificationType = "BCidCard";
      contact.primaryIdentificationType = "DriversLicence";
      this.busy = this.contactDataService.createContact(contact).subscribe(
        () => this.userDataService.loadUserToStore(),
        () => alert("Failed to create contact")
      );
    } else {
      window.location.href = "logout";
    }
  }
}
