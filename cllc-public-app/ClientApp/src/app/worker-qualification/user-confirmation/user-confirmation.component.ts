import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { User } from '../../models/user.model';
import { Contact } from '../../models/contact.model';
import { UserDataService } from '../../services/user-data.service';
import { AppState } from '../../app-state/models/app-state';
import { Store } from '@ngrx/store';
import { AliasDataService } from '../../services/alias-data.service';
import { PreviousAddressDataService } from '../../services/previous-address-data.service';
import { ContactDataService } from '../../services/contact-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-confirmation',
  templateUrl: './user-confirmation.component.html',
  styleUrls: ['./user-confirmation.component.scss']
})
export class UserConfirmationComponent implements OnInit {
  @Input() currentUser: User;
  @Output() reloadUser: EventEmitter<any> = new EventEmitter<any>();
  busy: any;
  termsAccepted = false;

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private aliasDataService: AliasDataService,
    private previousAddressDataService: PreviousAddressDataService,
    private contactDataService: ContactDataService,
    private workerDataService: WorkerDataService,
    private fb: FormBuilder,
    private router: Router
  ) { }

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
      this.busy = this.contactDataService.createWorkerContact(contact).subscribe(res => {
        this.reloadUser.emit(true);
      }, error => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }
}
