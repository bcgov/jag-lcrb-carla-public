import { Component, OnInit, Input } from '@angular/core';
import { User } from '@models/user.model';
import { Contact } from '@models/contact.model';
import { ContactDataService } from '@services/contact-data.service';
import { UserDataService } from '@services/user-data.service';

@Component({
  selector: 'app-user-confirmation',
  templateUrl: './user-confirmation.component.html',
  styleUrls: ['./user-confirmation.component.scss']
})
export class UserConfirmationComponent implements OnInit {
  @Input() currentUser: User;
  busy: any;
  termsAccepted = false;

  constructor(private userDataService: UserDataService,
    private contactDataService: ContactDataService  ) { }

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
      contact.isWorker = true;
      contact.secondaryIdentificationType = 'BCidCard';
      contact.primaryIdentificationType = 'DriversLicence';
      this.busy = this.contactDataService.createWorkerContact(contact).subscribe(() => {
        this.userDataService.loadUserToStore();
      }, () => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }
}
