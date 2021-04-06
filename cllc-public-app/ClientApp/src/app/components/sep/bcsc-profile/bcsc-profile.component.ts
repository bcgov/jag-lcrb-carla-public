import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { faAddressCard, faChevronRight, faEnvelope, faExclamationTriangle, faPhone, faTrash } from '@fortawesome/free-solid-svg-icons';
import { AppState } from '@app/app-state/models/app-state';
import { ContactDataService } from '@services/contact-data.service';
import { UserDataService } from '@services/user-data.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-bcsc-profile',
  templateUrl: './bcsc-profile.component.html',
  styleUrls: ['./bcsc-profile.component.scss']
})
export class BcscProfileComponent extends FormBase implements OnInit {
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

  // account profile form
  form = this.fb.group({
    contact: this.fb.group({
      _mailingSameAsPhysicalAddress: [],
      id: [],
      firstname: [{ value: "", disabled: true }, Validators.required],
      lastname: [{ value: "", disabled: true }, Validators.required],
      jobTitle: [""],
      telephone1: ["", [Validators.required]],
      emailaddress1: ["", [Validators.required, Validators.email]],
      // current (physical) address
      address1_line1: ["", Validators.required],
      address1_city: ["", Validators.required],
      address1_stateorprovince: ["British Columbia", Validators.required],
      address1_postalcode: ["", [Validators.required, this.customZipCodeValidator("address1_country")]],
      address1_country: ["Canada", Validators.required],
      // mailing (secondary) address
      address2_line1: ["", Validators.required],
      address2_city: ["", Validators.required],
      address2_stateorprovince: ["British Columbia", Validators.required],
      address2_postalcode: ["", [Validators.required, this.customZipCodeValidator("address2_country")]],
      address2_country: ["Canada", Validators.required],
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
      .updateContact(this.form.get("contact").value)
      .subscribe(() => {
        this.router.navigate(["/sep/dashboard"]);
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
      'contact.id': "Contact ID",
      'contact.firstname': "Contact First Name",
      'contact.lastname': "Contact LastName",
      'contact.jobTitle': "Contact Job Title",
      'contact.telephone1': "Contact Telephone",
      'contact.emailaddress1': "Contact Email",
    };
  }
}
