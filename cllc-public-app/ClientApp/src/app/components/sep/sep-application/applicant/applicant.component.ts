import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { AppState } from '@app/app-state/models/app-state';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { Account } from '@models/account.model';
import { Contact } from '@models/contact.model';
import { SepApplication } from '@models/sep-application.model';
import { Store } from '@ngrx/store';
import { ContactDataService } from '@services/contact-data.service';
import { IndexedDBService } from '@services/indexed-db.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-applicant',
  templateUrl: './applicant.component.html',
  styleUrls: ['./applicant.component.scss']
})
export class ApplicantComponent extends FormBase implements OnInit {
  policySlug = 'sep-terms-and-conditions';
  validationMessages: string[] = [];
  @ViewChild('policyDocs', { static: true })
  policyDocs: PolicyDocumentComponent;
  @Input() account: Account;
  _app: SepApplication = {} as SepApplication;
  contact: Contact;
  showValidationMessages: boolean;
  @Input()
  set sepApplication(value) {
    this._app = value;
    if (this.form) {
      this.form.patchValue(value);
    }
  }
  get sepApplication() {
    return this._app;
  }
  @Output()
  saveComplete = new EventEmitter<SepApplication>();
  form: FormGroup;

  constructor(private fb: FormBuilder,
    private store: Store<AppState>,
    private contactDataService: ContactDataService,
    private db: IndexedDBService) {
    super();
    store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
            if (this.form) {
              this.form.get('telephone1').patchValue(contact.telephone1);
              this.form.get('emailaddress1').patchValue(contact.emailaddress1);
            }
          });
      });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      eventName: [this?.sepApplication?.eventName, [Validators.required]],
      applicantInfo: [],
      isAgreeTsAndCs: [this?.sepApplication?.isAgreeTsAndCs, [this.customRequiredCheckboxValidator()]],
      dateAgreedToTsAndCs: [this?.sepApplication?.dateAgreedToTsAndCs],
      telephone1: [this?.contact?.telephone1, [Validators.required]],
      emailaddress1: [this?.contact?.emailaddress1, [Validators.required, Validators.email]],
    });

    this.form.get('isAgreeTsAndCs').valueChanges
      .subscribe((agree: boolean) => {
        if (agree) {
          this.form.get('dateAgreedToTsAndCs').setValue(new Date());
        }
      });

    this.policyDocs.setSlug(this.policySlug);
  }

  customRequiredCheckboxValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (control.value === true) {
        return null;
      } else {
        return { 'shouldBeTrue': 'But value is false' };
      }
    };
  }

  isValid() {
    this.showValidationMessages = false;
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, {
      eventName: 'Please enter the Event Name',
      isAgreeTsAndCs: 'Please indicate agreement to the general terms and conditions',
      telephone1: 'Please enter the Telephone',
      emailaddress1: 'Please enter the Email Address'
    });
    if (!this.form.valid) {
      this.showValidationMessages = true;
    }
    return this.form.valid;
  }

  next() {
    const data = {
      ...this.application,
      lastUpdated: new Date(),
      eventStatus: 'Draft',
      lastStepCompleted: 'applicant',
      ...this.form.value
    } as SepApplication;
    if (this.isValid()) {
      this.saveContactInfo()
        .subscribe(result => {
          this.saveComplete.emit(data);
        });
    }
  }

  saveContactInfo() {
    const patchContact = {
      ...this.contact,
      telephone1: this.form.get('telephone1').value,
      emailaddress1: this.form.get('emailaddress1').value,
    };
    return this.contactDataService.updateContact(patchContact);
  }


}
