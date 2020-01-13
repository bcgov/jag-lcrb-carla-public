
import { filter, map, catchError, takeWhile } from 'rxjs/operators';
import { Component, OnInit, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { UserDataService } from '@services/user-data.service';
import { User } from '@models/user.model';
import { ContactDataService } from '@services/contact-data.service';
import { Contact } from '@models/contact.model';
import { Store } from '@ngrx/store';
import { Subscription, Observable, forkJoin, of } from 'rxjs';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { COUNTRIES } from './country-list';

import * as moment from 'moment';
// tslint:disable-next-line:no-duplicate-imports
import { defaultFormat as _rollupMoment } from 'moment';
import { AccountDataService } from '@services/account-data.service';
import { Account } from '@models/account.model';
import { FormBase } from '@shared/form-base';
import { ConnectionToProducersComponent } from './tabs/connection-to-producers/connection-to-producers.component';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { AppState } from '@app/app-state/models/app-state';

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
  selector: 'app-account-profile',
  templateUrl: './account-profile.component.html',
  styleUrls: ['./account-profile.component.scss']
})
export class AccountProfileComponent extends FormBase implements OnInit {
  @Input() useInStepperMode = false;
  @Output() saveComplete: EventEmitter<boolean> = new EventEmitter<boolean>();
  currentUser: User;
  dataLoaded = false;
  busy: Subscription;
  busy2: Promise<any>;
  busy3: Promise<any>;
  form: FormGroup;
  countryList = COUNTRIES;
  maxDate = moment().endOf('day').toDate();

  accountId: string;
  saveFormData: any;
  _showAdditionalAddress: boolean;
  _showAdditionalContact: boolean;
  legalEntityId: string;
  @ViewChild(ConnectionToProducersComponent, { static: false }) connectionsToProducers: ConnectionToProducersComponent;
  applicationId: string;
  applicationMode: string;
  account: Account;
  tiedHouseFormData: Observable<TiedHouseConnection>;

  public get contacts(): FormArray {
    return this.form.get('otherContacts') as FormArray;
  }

  constructor(private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private contactDataService: ContactDataService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private tiedHouseService: TiedHouseConnectionsDataService
  ) {
    super();
    this.route.paramMap.subscribe(params => this.applicationId = params.get('applicationId'));
    this.route.paramMap.subscribe(params => this.applicationMode = params.get('mode'));
  }

  ngOnInit() {
    this.form = this.fb.group({
      businessProfile: this.fb.group({
        id: [''],
        _mailingSameAsPhysicalAddress: [],
        // name: [''],
        // businessDBAName: [''],
        bcIncorporationNumber: [''],
        dateOfIncorporationInBC: [''],
        businessNumber: ['', [Validators.required, Validators.minLength(9), Validators.maxLength(9)]],
        businessType: ['', Validators.required],
        contactPhone: ['', [Validators.required, /*Validators.minLength(10), Validators.maxLength(10)*/]],
        contactEmail: ['', [Validators.required, Validators.email]],

        physicalAddressStreet: ['', Validators.required],
        physicalAddressStreet2: [''],
        physicalAddressCity: ['', Validators.required],
        physicalAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('physicalAddressCountry')]],
        physicalAddressProvince: [{ value: 'British Columbia' }],
        physicalAddressCountry: [{ value: 'Canada' }],
        mailingAddressStreet: ['', Validators.required],
        mailingAddressStreet2: [''],
        mailingAddressCity: ['', Validators.required],
        mailingAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('mailingAddressCountry')]],
        mailingAddressProvince: ['', Validators.required],
        mailingAddressCountry: ['Canada', Validators.required],
      }),
      primarycontact: this.fb.group({
        id: [],
        firstname: ['', Validators.required],
        lastname: ['', Validators.required],
        jobTitle: [''],
        telephone1: ['', [Validators.required, /*Validators.minLength(10), Validators.maxLength(10)*/]],
        emailaddress1: ['', [Validators.required, Validators.email]],
      }),
    });
    this.subscribeForData();

    this.form.get('businessProfile._mailingSameAsPhysicalAddress').valueChanges.pipe(
      filter(value => value === true))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });

    this.form.get('businessProfile.physicalAddressStreet').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressStreet2').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressCity').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressPostalCode').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressProvince').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get('businessProfile.physicalAddressCountry').valueChanges.pipe(
      filter(() => this.form.get('businessProfile._mailingSameAsPhysicalAddress').value))
      .subscribe(() => {
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
      case 'IndigenousNation':
        name = 'Indigenous Nation';
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

  legalNameLabel() {
    const businessType = this.getBusinessTypeName();
    let label = `${businessType} - Legal Name`;
    if (businessType === 'IndigenousNation') {
      label = 'Full name of Indigenous Nation';
    }
    return label;
  }

  subscribeForData() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => {
        this.account = account;
        account.physicalAddressProvince = 'British Columbia';
        account.physicalAddressCountry = 'Canada';

        this.form.patchValue({
          businessProfile: account,
          primarycontact: account.primarycontact || {}
        });

        this.saveFormData = this.form.value;

        // normalize postal codes
        this.form.get('businessProfile.mailingAddressPostalCode').setValue(
          (this.form.get('businessProfile.mailingAddressPostalCode').value || '').replace(/\s+/g, '')
        );
        this.form.get('businessProfile.physicalAddressPostalCode').setValue(
          (this.form.get('businessProfile.physicalAddressPostalCode').value || '').replace(/\s+/g, '')
        );

        if (this.account.isPrivateCorporation()) {
          this.form.get('businessProfile.bcIncorporationNumber')
          .setValidators([Validators.pattern('^[A-Za-z][A-Za-z]?[0-9][0-9][0-9][0-9][0-9][0-9][0-9]$')]);
        } else if (this.account.businessType === 'Society') {
          this.form.get('businessProfile.bcIncorporationNumber')
          .setValidators([Validators.pattern('^[A-Za-z][A-Za-z]?[0-9][0-9][0-9][0-9][0-9][0-9][0-9]$')]);
        } else {
          this.form.get('businessProfile.bcIncorporationNumber').clearValidators();
        }
      });
  }

  confirmContact(confirm: boolean) {
    if (confirm) {
      // create contact here
      const contact = new Contact();
      contact.firstname = this.currentUser.firstname;
      contact.lastname = this.currentUser.lastname;
      contact.emailaddress1 = this.currentUser.email;
      this.busy = this.contactDataService.createWorkerContact(contact)
        .subscribe(() => {
          this.subscribeForData();
        }, () => alert('Failed to create contact'));
    } else {
      window.location.href = 'logout';
    }
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (!this.connectionsToProducers.formHasChanged() &&
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Observable<boolean> {
    const _tiedHouse = this.tiedHouseFormData || {};
    this.form.get('businessProfile').patchValue({ physicalAddressCountry: 'Canada' });
    const value = <Account>{
      ...this.form.get('businessProfile').value
    };
    const saves = [
      this.accountDataService.updateAccount(value),
      this.contactDataService.updateContact(this.form.get('primarycontact').value)
    ];

    if (this.connectionsToProducers) {
      saves.push(
        this.prepareTiedHouseSaveRequest({ ...this.account.tiedHouse, ..._tiedHouse })
      );
    }

    return forkJoin(...saves)
      .pipe(catchError(() => of(false)),
        map(() => {
          this.accountDataService.loadCurrentAccountToStore(this.account.id).subscribe(() => { });
          return true;
        }));
  }

  gotoReview() {
    if (this.form.valid && (!this.connectionsToProducers || this.connectionsToProducers.form.valid)) {
      this.busy = this.save().subscribe(() => {
        if (this.useInStepperMode) {
          this.saveComplete.emit(true);
        } else if (this.applicationId) {
          // divert catering.
          if (this.applicationMode === 'catering') {
            const route: any[] = [`/application/catering/${this.applicationId}`];
            if (this.applicationMode) {
              route.push({ mode: this.applicationMode });
            }
            this.router.navigate(route);
          }
          else {
            const route: any[] = [`/application/${this.applicationId}`];
            if (this.applicationMode) {
              route.push({ mode: this.applicationMode });
            }
            this.router.navigate(route);
          }
        } else {
          this.router.navigate(['/dashboard']);
        }
      });
    } else {
      this.markAsTouched();
    }
  }

  prepareTiedHouseSaveRequest(_tiedHouseData) {
    const data = { ...this.account.tiedHouse, ..._tiedHouseData };

    if (data.id) {
      return this.tiedHouseService.updateTiedHouse(data, data.id);
    } else {
      return this.accountDataService.createTiedHouseConnection(data, this.accountId);
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
