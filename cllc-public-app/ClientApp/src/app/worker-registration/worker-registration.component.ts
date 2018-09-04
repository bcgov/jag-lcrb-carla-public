import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { ContactDataService } from '../services/contact-data.service';
import { DynamicsContact } from '../models/dynamics-contact.model';
import { AppState } from '../app-state/models/app-state';
import * as CurrentUserActions from '../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs/Subscription';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { AliasDataService } from '../services/alias-data.service';
import { Alias } from '../models/alias.model';
import { PreviousAddress } from '../models/previous-address.model';
import { PreviousAddressDataService } from '../services/previous-address-data.service';
import { Observable } from 'rxjs';
import { WorkerDataService } from '../services/worker-data.service.';

@Component({
  selector: 'app-worker-registration',
  templateUrl: './worker-registration.component.html',
  styleUrls: ['./worker-registration.component.scss']
})
export class WorkerRegistrationComponent implements OnInit {
  currentUser: User;
  isNewUser: boolean;
  dataLoaded = false;
  busy: Subscription;
  form: FormGroup;

  addressesToDelete: PreviousAddress[] = [];
  aliasesToDelete: Alias[] = [];

  public get addresses(): FormArray {
    return this.form.get('addresses') as FormArray;
  }

  public get aliases(): FormArray {
    return this.form.get('aliases') as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private aliasDataService: AliasDataService,
    private previousAddressDataService: PreviousAddressDataService,
    private contactDataService: ContactDataService,
    private workerDataService: WorkerDataService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.form = this.fb.group({
      contact: this.fb.group({
        id: [],
        firstname: ['', Validators.required],
        middlename: ['', Validators.required],
        lastname: ['', Validators.required],
        emailaddress1: ['', Validators.required],
        telephone1: ['', Validators.required],
        address1_line1: ['', Validators.required],
        address1_city: ['', Validators.required],
        address1_stateorprovince: ['', Validators.required],
        address1_country: ['', Validators.required],
        address1_postalcode: ['', Validators.required],
      }),
      worker: this.fb.group({
        id: [],
        isldbworker: [false],
        firstname: ['', Validators.required],
        middlename: ['', Validators.required],
        lastname: ['', Validators.required],
        dateofbirth: ['', Validators.required],
        gender: ['', Validators.required],
        birthplace: ['', Validators.required],
        driverslicencenumber: ['', Validators.required],
        bcidcardnumber: ['', Validators.required],
        phonenumber: ['', Validators.required],
        email: ['', Validators.required],
        selfdisclosure: ['', Validators.required],
        triggerphs: ['', Validators.required]
      }),
      aliases: this.fb.array([
        this.createAlias()
      ]),
      addresses: this.fb.array([
        this.createAddress()
      ])
    });
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
          Observable.zip(
            this.workerDataService.getWorker(this.currentUser.contactid),
            this.aliasDataService.getAliases(this.currentUser.contactid),
            this.previousAddressDataService.getPreviousAdderesses(this.currentUser.contactid)
          ).subscribe(res => {
            const worker = res[0];
            const contact = worker.contact;
            delete worker.contact;
            const aliases = res[1];
            const addresses = res[2];
            this.form.patchValue({
              worker: worker,
              contact:  contact,
              aliases: aliases,
              addresses: addresses
            });
          });
          // this.aliasDataService.getAliases(this.currentUser.contactid).
          //   subscribe(alias => {
          //     this.form.patchValue(alias);
          //   });
          // this.previousAddressDataService.getPreviousAdderesses(this.currentUser.contactid)
          //   .subscribe(addresses => {
          //     addresses = addresses || [];
          //     addresses.forEach(a => {
          //       this.addAddress(a);
          //     });
          //   });
        }
      });
  }

  confirmContact(confirm: boolean) {
    if (confirm) {
      // create contact here
      const contact = new DynamicsContact();
      contact.fullname = this.currentUser.name;
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      this.busy = this.contactDataService.createWorkerContact(contact).subscribe(res => {
        this.reloadUser();
      }, error => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }

  createAddress(address: PreviousAddress = null) {
    address = address || <PreviousAddress>{
      id: undefined,
      streetaddress: '',
      city: '',
      provstate: '',
      country: '',
      postalcode: '',
      fromdate: '',
      todate: ''
    };
    return this.fb.group({
      id: [address.id],
      streetaddress: [address.streetaddress, Validators.required],
      city: [address.city, Validators.required],
      provstate: [address.provstate, Validators.required],
      country: [address.country, Validators.required],
      postalcode: [address.postalcode, Validators.required],
      fromdate: [address.fromdate, Validators.required],
      todate: [address.todate, Validators.required]
    });
  }

  addAddress(address: PreviousAddress = null) {
    this.addresses.push(this.createAddress(address));
  }

  deleteAddress(index: number) {
    const address = this.addresses.controls[index];
    if (address.value.id) {
      this.addressesToDelete.push(address.value);
    }
    this.addresses.removeAt(index);
  }

  addAlias(alias: Alias = null) {
    this.aliases.push(this.createAlias(alias));
  }

  deleteAlias(index: number) {
    const alias = this.aliases.controls[index];
    if (alias.value.id) {
      this.aliasesToDelete.push(alias.value);
    }
    this.aliases.removeAt(index);
  }

  createAlias(alias: Alias = null) {
    alias = alias || <Alias>{
      firstname: '',
      middlename: '',
      lastname: ''
    };
    return this.fb.group({
      id: [alias.id],
      firstname: [alias.firstname, Validators.required],
      middlename: [alias.middlename, Validators.required],
      lastname: [alias.lastname, Validators.required],
    });
  }

  save() {
    const saves = [this.aliasDataService.updateAlias(this.form.value, this.form.value.id)];
    this.addressesToDelete.forEach(a => {
      const save = this.previousAddressDataService.deletePreviousAdderess(a.id);
      saves.push(save);
    });

    for (let i = 0; i < this.addresses.length; i++) {
      if (this.addresses[i].id) {
        const save = this.previousAddressDataService.updatePreviousAdderess(this.addresses[i], this.addresses[i].id);
        saves.push(save);
      } else {
        const save = this.previousAddressDataService.createPreviousAdderess(this.addresses[i]);
        saves.push(save);
      }
    }

    Observable.zip(...saves).subscribe(res => {
      this.reloadUser();
    });
  }
}
