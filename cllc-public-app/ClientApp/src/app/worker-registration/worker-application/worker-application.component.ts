import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../services/user-data.service';
import { User } from '../../models/user.model';
import { ContactDataService } from '../../services/contact-data.service';
import { DynamicsContact } from '../../models/dynamics-contact.model';
import { AppState } from '../../app-state/models/app-state';
import * as CurrentUserActions from '../../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs/Subscription';
import { FormBuilder, FormGroup, Validators, FormArray, ValidatorFn, AbstractControl } from '@angular/forms';
import { AliasDataService } from '../../services/alias-data.service';
import { PreviousAddressDataService } from '../../services/previous-address-data.service';
import { Observable, Subject } from 'rxjs';
import { WorkerDataService } from '../../services/worker-data.service.';
import { Alias } from '../../models/alias.model';
import { PreviousAddress } from '../../models/previous-address.model';
import { ActivatedRoute, Router } from '@angular/router';

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
    return this.form.get('worker.aliases') as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private aliasDataService: AliasDataService,
    private previousAddressDataService: PreviousAddressDataService,
    private contactDataService: ContactDataService,
    private workerDataService: WorkerDataService,
    private fb: FormBuilder,
    private router: Router,
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
        firstname: [''],
        middlename: [''],
        lastname: [''],
        emailaddress1: [''],
        telephone1: [''],
        address1_line1: ['', Validators.required],
        address1_city: ['', Validators.required],
        address1_stateorprovince: ['', Validators.required],
        address1_country: ['', Validators.required],
        address1_postalcode: ['', Validators.required],
      }),
      worker: this.fb.group({
        id: [],
        isldbworker: [false],
        firstname: [''],
        middlename: [''],
        lastname: [''],
        dateofbirth: [''],
        gender: [''],
        birthplace: ['', Validators.required],
        driverslicencenumber: [''],
        bcidcardnumber: [''],
        phonenumber: ['', Validators.required],
        email: ['', [Validators.required, Validators.email]],
        selfdisclosure: [''],
        // triggerphs: ['', Validators.required],
        aliases: this.fb.array([
        ]),
      }),
      addresses: this.fb.array([
      ])
    });
    this.reloadUser();
  }

  reloadUser() {
    this.busy = this.userDataService.getCurrentUser()
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
    for (let i = this.addresses.controls.length; i > 0; i--) {
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
    for (let i = this.aliases.controls.length; i > 0; i--) {
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
      middlename: [alias.middlename],
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

  validatePastAddresses() {
    let valid = true;
    let dts = [];
    const addressControls = this.addresses.controls;
    for (let i = 0; i < addressControls.length; i++) {
      const fromDate = new Date(addressControls[i].value.fromdate);
      const toDate = new Date(addressControls[i].value.todate);
      dts.push({ fd: fromDate, td: toDate });
      if (fromDate > toDate) {
        valid = false;
      }
    }

    if (dts.length < 1) {
      return false;
    }

    dts = dts.sort((a, b) => {
      let res = 0;
      if (a.fd < b.fd) {
        res = -1;
      }
      if (a.fd > b.fd) {
        res = 1;
      }
      return res;
    });

    // verify there is no gap between dates
    let isContinuous = true;
    for (let i = 1; i < dts.length; i++) {
      const element = dts[i];
      if (element.fd >= dts[i - 1].td && this.daysBetween(element.fd, dts[i - 1].td) > 1) {
        isContinuous = false;
        valid = false;
        break;
      }
    }

    if (isContinuous) {
      const daysIn5years = 365 * 5 + 1;
      // verify that the dates form a range >=  5 years
      if (this.daysBetween(dts[0].fd, dts[dts.length - 1].td) < daysIn5years) {
        valid = false;
      }
    }
    return valid;
  }

  private daysBetween(firstDate: Date, secondDate: Date) {
    const oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    const diffDays = Math.round(Math.abs((firstDate.getTime() - secondDate.getTime()) / (oneDay)));
    return diffDays;
  }

  isAscending(fromDate: string, toDate: string) {
    return new Date(toDate) >= new Date(fromDate);
  }

  gotoStep2() {
    if (this.form.valid && this.isBCIDValid()) {
      this.router.navigate([`/worker-registration/spd-consent/${this.workerId}`]);
    } else {
      this.markAsTouched();
    }
  }

  // marking the form as touched makes the validation messages show
  markAsTouched() {
    this.form.markAsTouched();

    const workerControls = (<FormGroup>(this.form.get('worker'))).controls;
    for (const c in workerControls) {
      if (typeof (workerControls[c].markAsTouched) === 'function') {
        workerControls[c].markAsTouched();
      }
    }

    const contactControls = (<FormGroup>(this.form.get('contact'))).controls;
    for (const c in contactControls) {
      if (typeof (contactControls[c].markAsTouched) === 'function') {
        contactControls[c].markAsTouched();
      }
    }

    (<FormGroup[]>this.addresses.controls).forEach(address => {
      for (const c in address.controls) {
        if (typeof (address.controls[c].markAsTouched) === 'function') {
          address.controls[c].markAsTouched();
        }
      }
    });
    (<FormGroup[]>this.aliases.controls).forEach(alias => {
      for (const c in alias.controls) {
        if (typeof (alias.controls[c].markAsTouched) === 'function') {
          alias.controls[c].markAsTouched();
        }
      }
    });
  }

  isBCIDValid(): boolean {
    const valid = !!(this.form.get('worker.driverslicencenumber').value
      || this.form.get('worker.bcidcardnumber').value);
    return valid;
  }
}
