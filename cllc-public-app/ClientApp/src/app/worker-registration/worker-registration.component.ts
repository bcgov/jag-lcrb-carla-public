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

  addressesToDelete: PreviousAddress[] =[];

  public get addresses(): FormArray {
    return this.form.get('previous_address') as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private aliasDataService: AliasDataService,
    private previousAddressDataService: PreviousAddressDataService,
    private contactDataService: ContactDataService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.reloadUser();
    this.form = this.fb.group({ //first level fields are for the alias
        id: [],
        firstname: ['', Validators.required],
        middlename: ['', Validators.required],
        lastname: ['', Validators.required],
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
          this.aliasDataService.getAlias(this.currentUser.contactid).
            subscribe(alias => {
              this.form.patchValue(alias);
            });
          this.previousAddressDataService.getPreviousAdderess(this.currentUser.contactid)
          .subscribe(addresses =>{
            addresses = addresses || [];
            addresses.forEach(a => {
                this.addAddress(a);  
            });
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
      this.busy = this.contactDataService.createWorkerContact(contact).subscribe(res => {
        this.reloadUser();
      }, error => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }

  createAddress(address: PreviousAddress = null) {
    return this.fb.group({
      id: [],
      name: ['', Validators.required],
      streetaddress: ['', Validators.required],
      city: ['', Validators.required],
      provstate: ['', Validators.required],
      country: ['', Validators.required],
      postalcode: ['', Validators.required],
      fromdate: ['', Validators.required],
      todate: ['', Validators.required]
    });
  }

  addAddress(address: PreviousAddress = null) {
    this.addresses.push(this.createAddress(address));
  }

  deletedddress(index: number) {
    var address = this.addresses[index];
    if(address.id){
      this.addressesToDelete.push(address);
    }
    this.addresses.removeAt(index);
  }

  save() {
    let saves = [this.aliasDataService.updateAlias( this.form.value,  this.form.value.id)];
    this.addressesToDelete.forEach(a => {
      let save = this.previousAddressDataService.deletePreviousAdderess(a.id);
      saves.push(save);
    });

    for(let i=0; i < this.addresses.length; i++){
      if(this.addresses[i].id){
        let save = this.previousAddressDataService.updatePreviousAdderess(this.addresses[i], this.addresses[i].id);
        saves.push(save);
      } else {
        let save = this.previousAddressDataService.createPreviousAdderess(this.addresses[i]);
        saves.push(save);
      }
    }

    Observable.zip(...saves).subscribe(res => {
      this.reloadUser();
    });
  }
}
