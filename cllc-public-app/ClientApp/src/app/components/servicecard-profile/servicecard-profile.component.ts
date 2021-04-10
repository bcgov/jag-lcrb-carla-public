import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { faAddressCard, faChevronRight, faEnvelope, faExclamationTriangle, faPhone, faTrash } from '@fortawesome/free-solid-svg-icons';
import { AppState } from '@app/app-state/models/app-state';
import { ContactDataService } from '@services/contact-data.service';
import { UserDataService } from '@services/user-data.service';
import { FormBase } from '@shared/form-base';
import { Contact } from '@models/contact.model';
import { filter, takeWhile } from 'rxjs/operators';
import { User } from '@models/user.model';

@Component({
  selector: 'app-servicecard-profile',
  templateUrl: './servicecard-profile.component.html',
  styleUrls: ['./servicecard-profile.component.scss']
})
export class ServiceCardProfileComponent extends FormBase implements OnInit {
  // icons
  faExclamationTriangle = faExclamationTriangle;
  faTrash = faTrash;
  faChevronRight = faChevronRight;
  faAddressCard = faAddressCard;
  faEnvelope = faEnvelope;
  faPhone = faPhone;

  // component state
  showErrorSection = false;
  validationMessages: string[];
  currentUser: User;

  // account profile form
  form = this.fb.group({
    contact: this.fb.group({
      _mailingSameAsPhysicalAddress: [],
      id: [],
      firstname: [{ value: '', disabled: true }, Validators.required],
      lastname: [{ value: '', disabled: true }, Validators.required],
      jobTitle: [''],
      mobilePhone: ['', [Validators.required]],
      emailaddress1: ['', [Validators.required, Validators.email]],
      // current (physical) address
      address1_line1: ['', Validators.required],
      address1_city: ['', Validators.required],
      address1_stateorprovince: ['British Columbia', Validators.required],
      address1_postalcode: ['', [Validators.required, this.customZipCodeValidator('address1_country')]],
      address1_country: ['Canada', Validators.required],
      // mailing (secondary) address
      address2_line1: ['', Validators.required],
      address2_city: ['', Validators.required],
      address2_stateorprovince: ['British Columbia', Validators.required],
      address2_postalcode: ['', [Validators.required, this.customZipCodeValidator('address2_country')]],
      address2_country: ['Canada', Validators.required],
    }),
  });

  constructor(
    private store: Store<AppState>,
    private contactDataService: ContactDataService,
    private userDataService: UserDataService,
    private fb: FormBuilder,
    private router: Router,
  ) {
    super();
  }

  ngOnInit() {
    this.initializeForm();
    this.subscribeForData();
  }

  private subscribeForData() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(user => !!user))
      .subscribe(user => this.loadUser(user));
  }

  private loadUser(user: User) {
    this.currentUser = user;
    if (this.currentUser && this.currentUser.contactid) {
      this.contactDataService.getContact(this.currentUser.contactid)
        .subscribe(contact => {
          this.form.patchValue({ contact: contact });
        });
    }
  }

  private initializeForm() {
    // copy physical address when checkbox is checked
    this.form.get('contact._mailingSameAsPhysicalAddress').valueChanges.pipe(
      filter(value => value === true)
    ).subscribe(() => {
      this.copyPhysicalToMailingAddress();
    });

    // keep mailing address up-to-date with changes to the main address (when checkbox is checked)
    this.applyChangesFrom('contact.address1_line1');
    this.applyChangesFrom('contact.address1_city');
    this.applyChangesFrom('contact.address1_stateorprovince');
    this.applyChangesFrom('contact.address1_postalcode');
    this.applyChangesFrom('contact.address1_country');
  }

  private applyChangesFrom(field: string) {
    this.form.get(field).valueChanges.pipe(
      filter(() => this.form.get('contact._mailingSameAsPhysicalAddress').value)
    ).subscribe(() => {
      this.copyPhysicalToMailingAddress();
    });
  }

  private copyPhysicalToMailingAddress() {
    // address1 == physical address
    // address2 == mailing address
    this.form.get('contact').patchValue({
      address2_line1: this.form.get('contact.address1_line1').value,
      address2_city: this.form.get('contact.address1_city').value,
      address2_stateorprovince: this.form.get('contact.address1_stateorprovince').value,
      address2_postalcode: this.form.get('contact.address1_postalcode').value,
      address2_country: this.form.get('contact.address1_country').value
    });
  }

  save() {
    const validForm = this.validateForm();
    if (!validForm) {
      this.showErrorSection = true;
      return;
    }

    // Do not show validation errors when form is valid
    this.showErrorSection = false;

    // Save contact
    this.contactDataService
      .updateContact(this.form.get('contact').value)
      .subscribe(() => {
        // reload the user to fetch updated contact information
        this.userDataService.loadUserToStore().then(() => this.router.navigate(['/sep/dashboard']));
      });
  }

  validateForm(): boolean {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.form, this.validationErrorMap))];
    this.markControlsAsTouched(this.form);
    const isValid = this.validationMessages.length == 0;
    return isValid;
  }

  get validationErrorMap() {
    return {
      'contact.id': 'Please enter the contact ID',
      'contact.firstname': 'Please enter the contact first name',
      'contact.lastname': 'Please enter the contact last name',
      'contact.jobTitle': 'Please enter the contact job title',
      'contact.telephone1': 'Please enter the contact phone number',
      'contact.emailaddress1': 'Please enter the contact email address',
      'contact.address1_line1': 'Please enter the physical address street',
      'contact.address1_city': 'Please enter the physical address city',
      'contact.address1_stateorprovince': 'Please enter the physical address province',
      'contact.address1_postalcode': 'Please enter the physical address postal code',
      'contact.address1_country': 'Please enter the physical address country',
      'contact.address2_line1': 'Please enter the mailing address street',
      'contact.address2_city': 'Please enter the mailing address city',
      'contact.address2_stateorprovince': 'Please enter the mailing address province',
      'contact.address2_postalcode': 'Please enter the mailing address postal code',
      'contact.address2_country': 'Please enter the mailing address country',
    };
  }
}
