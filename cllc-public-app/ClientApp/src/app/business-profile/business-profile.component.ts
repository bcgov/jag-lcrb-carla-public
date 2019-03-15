
import { filter, zip } from 'rxjs/operators';
import { Component, OnInit, ViewChild } from '@angular/core';
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { ContactDataService } from '../services/contact-data.service';
import { DynamicsContact } from '../models/dynamics-contact.model';
import { AppState } from '../app-state/models/app-state';
import * as CurrentUserActions from '../app-state/actions/current-user.action';
import { Store } from '@ngrx/store';
import { Subscription, Observable, Subject, forkJoin } from 'rxjs';
import { FormBuilder, FormGroup, Validators, FormArray, ValidatorFn, AbstractControl, FormControl } from '@angular/forms';
import { PreviousAddressDataService } from '../services/previous-address-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { COUNTRIES } from './country-list';

import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import * as _moment from 'moment';
// tslint:disable-next-line:no-duplicate-imports
import { defaultFormat as _rollupMoment } from 'moment';
import { AccountDataService } from '../services/account-data.service';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { FormBase, postalRegex } from '../shared/form-base';
import { AdoxioLegalEntityDataService } from '../services/adoxio-legal-entity-data.service';
import { ConnectionToProducersComponent } from './tabs/connection-to-producers/connection-to-producers.component';
const moment = _rollupMoment || _moment;

// See the Moment.js docs for the meaning of these formats:
// https://momentjs.com/docs/#/displaying/format/
export const MY_FORMATS = {
  parse: {
    dateInput: 'LL',
  },
  display: {
    dateInput: 'YYYY-MM-DD',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'YYYY-MM-DD',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'app-business-profile',
  templateUrl: './business-profile.component.html',
  styleUrls: ['./business-profile.component.scss']
})
export class BusinessProfileComponent extends FormBase implements OnInit {
  currentUser: User;
  dataLoaded = false;
  busy: Promise<any>;
  busy2: Promise<any>;
  busy3: Promise<any>;
  form: FormGroup;
  countryList = COUNTRIES;
  maxDate = new Date();

  accountId: string;
  saveFormData: any;
  _showAdditionalAddress: boolean;
  _showAdditionalContact: boolean;
  legalEntityId: string;
  @ViewChild(ConnectionToProducersComponent) connectionsToProducers: ConnectionToProducersComponent;
  applicationId: any;


  public get contacts(): FormArray {
    return this.form.get('otherContacts') as FormArray;
  }

  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private contactDataService: ContactDataService,
    private legalEntityDataService: AdoxioLegalEntityDataService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,

  ) {
    super();
    this.applicationId = this.route.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.form = this.fb.group({
      businessProfile: this.fb.group({
        id: [''],
        _mailingSameAsPhysicalAddress: [],
        name: [{ value: '', disabled: true }],
        businessDBAName: [{ value: '', disabled: true }],
        bcIncorporationNumber: [''], // Validators.required
        dateOfIncorporationInBC: [''],
        businessNumber: ['', [Validators.required, Validators.minLength(9), Validators.maxLength(9)]],
        businessType: ['', Validators.required],
        contactPhone: ['', [Validators.required, /*Validators.minLength(10), Validators.maxLength(10)*/]],
        contactEmail: ['', [Validators.required, Validators.email]],
        // consentForEmailCommunication: [false, this.customRequiredCheckboxValidator()],
        // websiteAddress: [''],
        physicalAddressStreet: ['', Validators.required],
        physicalAddressStreet2: [''],
        physicalAddressCity: ['', Validators.required],
        physicalAddressPostalCode: ['', [Validators.required, /* Validators.pattern(postalRegex)*/]],
        physicalAddressProvince: [{ value: 'British Columbia' }],
        physicalAddressCountry: [{ value: 'Canada' }],
        mailingAddressStreet: ['', Validators.required],
        mailingAddressStreet2: [''],
        mailingAddressCity: ['', Validators.required],
        mailingAddressPostalCode: ['', [Validators.required, /* Validators.pattern(postalRegex)*/] ],
        mailingAddressProvince: ['', Validators.required],
        mailingAddressCountry: ['Canada', Validators.required],
      }),
      primarycontact: this.fb.group({
        id: [],
        firstname: ['', Validators.required],
        lastname: ['', Validators.required],
        jobTitle: [''],
        telephone1: ['', [Validators.required, /*Validators.minLength(10), Validators.maxLength(10)*/]],
        // phoneNumberAlt: [''],
        emailaddress1: ['', [Validators.required, Validators.email]],
      }),
      // additionalContact: this.fb.group({
      //   id: [],
      //   firstName: [''],
      //   lastName: [''],
      //   title: [''],
      //   telephone1: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10)]],
      //   // phoneNumberAlt: [''],
      //   emailaddress1: [''],
      // })
    });
    this.reloadUser();

    this.form.get('businessProfile._mailingSameAsPhysicalAddress').valueChanges.pipe(
      filter(value => value === true))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });

    this.form.get('businessProfile.physicalAddressStreet').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressStreet2').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressCity').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressPostalCode').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressProvince').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressCountry').valueChanges.pipe(
      filter(v => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(value => {
        this.copyPhysicalToMailingAddress();
      });

  }

  copyPhysicalToMailingAddress() {
    this.form.get('businessProfile.mailingAddressStreet').patchValue(this.form.get('businessProfile.physicalAddressStreet').value);
    this.form.get('businessProfile.mailingAddressStreet2').patchValue(this.form.get('businessProfile.physicalAddressStreet2').value);
    this.form.get('businessProfile.mailingAddressCity').patchValue(this.form.get('businessProfile.physicalAddressCity').value);
    this.form.get('businessProfile.mailingAddressPostalCode')
      .patchValue(this.form.get('businessProfile.physicalAddressPostalCode').value);
    this.form.get('businessProfile.mailingAddressProvince').patchValue(this.form.get('businessProfile.physicalAddressProvince').value);
    this.form.get('businessProfile.mailingAddressCountry').patchValue(this.form.get('businessProfile.physicalAddressCountry').value);
  }

  getBusinessTypeName() {
    if (!(this.saveFormData && this.saveFormData.businessProfile)) {
      return '';
    }
    let name = '';
    switch (this.saveFormData.businessProfile.businessType) {
      case 'GeneralPartnership':
      case 'LimitedPartnership"':
      case 'LimitedLiabilityPartnership':
        name = 'Partnership';
        break;
      case 'SoleProprietor':
        name = 'Sole Proprietor';
        break;
      case 'PublicCorporation':
      case 'PrivateCorporation':
      case 'UnlimitedLiabilityCorporation':
      case 'LimitedLiabilityCorporation':
        name = 'Corporation';
        break;
      default:
        name = this.saveFormData.businessProfile.businessType;
        break;
    }
    return name;
  }

  reloadUser() {
    this.busy = this.userDataService.getCurrentUser()
      .toPromise()
      .then((data: User) => {
        this.currentUser = data;

        this.store.dispatch(new CurrentUserActions.SetCurrentUserAction(data));
        this.dataLoaded = true;
        if (this.currentUser && this.currentUser.accountid) {
          this.busy2 = forkJoin(
            this.accountDataService.getAccount(this.currentUser.accountid),
            this.legalEntityDataService.getBusinessProfileSummary()
          ).toPromise().then(res => {
            const account: any = res[0];

            account.physicalAddressProvince = 'British Columbia';
            account.physicalAddressCountry = 'Canada';

            const legalEntities = res[1].filter(e => e.isApplicant === true);
            if (legalEntities.length) {
              this.legalEntityId = legalEntities[0].id;
            }


            this.form.patchValue({
              businessProfile: account,
              primarycontact: account.primarycontact || {}
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
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      this.busy = this.contactDataService.createWorkerContact(contact)
        .toPromise()
        .then(res => {
          this.reloadUser();
        }, error => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (// this.workerStatus !== 'Application Incomplete' ||
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Subject<boolean> {
    const subResult = new Subject<boolean>();
    this.form.get('businessProfile').patchValue({ physicalAddressCountry: 'Canada' });
    const value = <DynamicsAccount>{
      ...this.form.get('businessProfile').value
    };
    const saves = [
      this.accountDataService.updateAccount(value),
      this.contactDataService.updateContact(this.form.get('primarycontact').value)
    ];

    if (this.connectionsToProducers) {
      saves.push(this.connectionsToProducers.prepareSaveData());
    }

    this.busy = forkJoin(...saves)
      .toPromise()
      .then(res => {
        subResult.next(true);
        this.reloadUser();
      }, err => subResult.next(false));
    this.busy3 = Promise.resolve(this.busy);

    return subResult;
  }

  gotoReview() {
    if (this.form.valid && (!this.connectionsToProducers || this.connectionsToProducers.form.valid)) {
      this.save().subscribe(data => {
        if (this.applicationId) {
          this.router.navigate([`/application/${this.applicationId}`]);
        } else {
          this.router.navigate(['/dashboard']);
        }
      });
    } else {
      this.markAsTouched();
    }
  }

  // marking the form as touched makes the validation messages show
  markAsTouched() {
    this.form.markAsTouched();

    const businessProfileControls = (<FormGroup>(this.form.get('businessProfile'))).controls;
    for (const c in businessProfileControls) {
      if (typeof (businessProfileControls[c].markAsTouched) === 'function') {
        businessProfileControls[c].markAsTouched();
      }
    }

    const primarycontactControls = (<FormGroup>(this.form.get('primarycontact'))).controls;
    for (const c in primarycontactControls) {
      if (typeof (primarycontactControls[c].markAsTouched) === 'function') {
        primarycontactControls[c].markAsTouched();
      }
    }

  }
}
