import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { ContactDataService } from '../../services/contact-data.service';
import { DynamicsContact } from '../../models/dynamics-contact.model';
import { AppState } from '../../app-state/models/app-state';
import * as CurrentUserActions from '../../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs/Subscription';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { AliasDataService } from '../../services/alias-data.service';
import { PreviousAddressDataService } from '../../services/previous-address-data.service';
import { Observable, Subject } from 'rxjs';
import { WorkerDataService } from '../../services/worker-data.service.';
import { Alias } from '../../models/alias.model';
import { PreviousAddress } from '../../models/previous-address.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-worker-application',
  templateUrl: './worker-application.component.html',
  styleUrls: ['./worker-application.component.scss']
})
export class WorkerApplicationComponent implements OnInit {
  currentUser: User;
  dataLoaded = false;
  busy: Subscription;
  form: FormGroup;

  addressesToDelete: PreviousAddress[] = [];
  aliasesToDelete: Alias[] = [];
  workerId: string;
  saveFormData: any;

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
    private fb: FormBuilder,
    private route: ActivatedRoute
  ) {
    this.route.params.subscribe(params => {
      this.workerId = params.id;
    });
   }

  ngOnInit() {
    this.form = this.fb.group({
      contact: this.fb.group({
        id: [],
        firstname: ['', Validators.required],
        middlename: ['', Validators.required],
        lastname: ['', Validators.required],
        emailaddress1: ['', [Validators.required, Validators.email]],
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
        email: ['', [Validators.required, Validators.email]],
        selfdisclosure: ['', Validators.required],
        // triggerphs: ['', Validators.required]
      }),
      aliases: this.fb.array([
      ]),
      addresses: this.fb.array([
      ])
    });
    this.reloadUser();
  }

  reloadUser() {
    this.busy =  this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
        this.store.dispatch(new CurrentUserActions.SetCurrentUserAction(data));
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.contactid) {
         Observable.zip(
            this.workerDataService.getWorker(this.workerId),
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
              contact: contact,
            });

            this.clearAliases();
            aliases.forEach(alias => {
              this.addAlias(alias);
            });

            this.clearAddresses();
            addresses.forEach(address => {
              this.addAddress(address);
            });

            this.saveFormData = this.form.value;
          });
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

  clearAddresses() {
    for (let i = this.addresses.controls.length; i > 0;  i--) {
      this.addresses.removeAt(0);
    }
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

  clearAliases() {
    for (let i = this.aliases.controls.length; i > 0;  i--) {
      this.aliases.removeAt(0);
    }
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

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Subject<boolean> {
    const subResult = new Subject<boolean>();
    const value = this.form.value;
    const saves = [
      this.contactDataService.updateContact(value.contact),
      this.workerDataService.updateWorker(value.worker, value.worker.id)
    ];

    this.addressesToDelete.forEach(a => {
      const save = this.previousAddressDataService.deletePreviousAdderess(a.id);
      saves.push(save);
    });

    const addressControls = this.addresses.controls;
    for (let i = 0; i < addressControls.length; i++) {
      if (addressControls[i].value.id) {
        const save = this.previousAddressDataService.updatePreviousAdderess(addressControls[i].value, addressControls[i].value.id);
        saves.push(save);
      } else {
        const newAddress = addressControls[i].value;
        newAddress.contactId = value.contact.id;
        newAddress.workerId = value.worker.id;
        const save = this.previousAddressDataService.createPreviousAdderess(newAddress);
        saves.push(save);
      }
    }

    this.aliasesToDelete.forEach(a => {
      const save = this.aliasDataService.deleteAlias(a.id);
      saves.push(save);
    });

    const aliasControls = this.aliases.controls;
    for (let j = 0; j < aliasControls.length; j++) {
      if (aliasControls[j].value.id) {
        const save = this.aliasDataService.updateAlias(aliasControls[j].value, aliasControls[j].value.id);
        saves.push(save);
      } else {
        const alias = aliasControls[j].value;
        alias.contact = { id: value.contact.id };
        alias.worker = { id: value.worker.id };
        const save = this.aliasDataService.createAlias(alias);
        saves.push(save);
      }
    }

    this.busy = Observable.zip(...saves).subscribe(res => {
      subResult.next(true);
      this.reloadUser();
    }, err => subResult.next(false));

    return subResult;
  }
}
