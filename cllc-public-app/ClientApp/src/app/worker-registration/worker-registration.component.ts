import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { ContactDataService } from '../services/contact-data.service';
import { DynamicsContact } from '../models/dynamics-contact.model';
import { AppState } from '../app-state/models/app-state';
import * as CurrentUserActions from '../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs/Subscription';
import { Form, FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';

@Component({
  selector: 'app-worker-registration',
  templateUrl: './worker-registration.component.html',
  styleUrls: ['./worker-registration.component.scss']
})
export class WorkerRegistrationComponent implements OnInit {
  currentUser: User;
  isNewUser: boolean;
  dataLoaded = false;
  contact: DynamicsContact;
  busy: Subscription;
  form: FormGroup;

  public get addresses(): FormArray {
    return this.form.get('previous_address') as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private contactDataService: ContactDataService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.reloadUser();
    this.form = this.fb.group({
      contact: this.fb.group({
        firstname: ['', Validators.required],
        middlename: ['', Validators.required],
        lastname: ['', Validators.required],
        emailaddress1: ['', Validators.required],
        telephone1: ['', Validators.required],
        address1_street1: ['', Validators.required],
        address1_city: ['', Validators.required],
        address1_stateorprovince: ['', Validators.required],
        address1_country: ['', Validators.required],
        address1_postalcode: ['', Validators.required],
      }),
      worker: this.fb.group({
        adoxio_isldbworker: [false],
        adoxio_firstname: ['', Validators.required],
        adoxio_middlename: ['', Validators.required],
        adoxio_lastname: ['', Validators.required],
        adoxio_dateofbirth: ['', Validators.required],
        adoxio_gender: ['', Validators.required],
        adoxio_birthplace: ['', Validators.required],
        adoxio_driverslicencenumber: ['', Validators.required],
        adoxio_bcidcardnumber: ['', Validators.required],
        adoxio_phonenumber: ['', Validators.required],
        adoxio_email: ['', Validators.required],
        adoxio_selfdisclosure: ['', Validators.required],
        adoxio_triggerphs: ['', Validators.required]
      }),
      alias: this.fb.group({
        adoxio_firstname: ['', Validators.required],
        adoxio_middlename: ['', Validators.required],
        adoxio_lastname: ['', Validators.required],
      }),
      previous_address: this.fb.array([
        this.createAddress()
      ])
    });
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
      this.busy = this.contactDataService.createContact(contact).subscribe(res => {
        this.reloadUser();
      }, error => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }

  createAddress() {
    return this.fb.group({
      adoxio_name: ['', Validators.required],
      adoxio_streetaddress: ['', Validators.required],
      adoxio_city: ['', Validators.required],
      adoxio_provstate: ['', Validators.required],
      adoxio_country: ['', Validators.required],
      adoxio_postalcode: ['', Validators.required],
      adoxio_fromdate: ['', Validators.required],
      adoxio_todate: ['', Validators.required]
    });
  }

  addAddress() {
    this.addresses.push(
      this.createAddress()
    );
  }

  deleteAddress(index: number) {
    this.addresses.removeAt(index);
  }
}
