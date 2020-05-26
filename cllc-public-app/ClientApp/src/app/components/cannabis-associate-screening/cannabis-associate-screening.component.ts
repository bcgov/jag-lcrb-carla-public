import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Alias } from '@models/alias.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ContactDataService } from '@services/contact-data.service';
import { CASSContact } from '@models/contact.model';
import { FormBase } from '@shared/form-base';
import { takeWhile } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AppState } from '../../app-state/models/app-state';
import { User } from '../../models/user.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cannabis-associate-screening',
  templateUrl: './cannabis-associate-screening.component.html',
  styleUrls: ['./cannabis-associate-screening.component.scss']
})
export class CannabisAssociateScreeningComponent extends FormBase implements OnInit {
  busy: Subscription;
  user: User;
  aliasesToDelete: any;
  form: FormGroup;
  contactToken: string;
  contact: CASSContact;
  showErrors: boolean;
  fileCount: any = {};
  showForm = false;
  validationErrors: string[] = [];

  public get aliases(): FormArray {
    return this.form.get('contact.aliases') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private contactDataService: ContactDataService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute) {
    super();
    this.route.paramMap.subscribe(pmap => this.contactToken = pmap.get('token'));

  }

  ngOnInit() {
    this.form = this.fb.group({
      firstNameAtBirth: [''],
      lastNameAtBirth: [''],
      sameNameAtBirth: [true],
      contact: this.fb.group({
        id: [''],
        fullname: [''],
        shortName: [{ value: '', disabled: true }],
        emailaddress1: ['', [Validators.required, Validators.email]],
        telephone1: [''],
        address1_line1: [''],
        address1_city: [''],
        address1_stateorprovince: [''],
        address1_country: [''],
        address1_postalcode: [''],
        birthDate: [''],
        gender: [''],
        mobilePhone: ['', [Validators.required,]],
        aliases: this.fb.array([])
      }),
      consentToCollection: ['', [Validators.required]],
      privacyAgreement: ['', [Validators.required]]
    });
    this.store.select((state) => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((user) => {
        this.user = user;
      });

    this.form.get('sameNameAtBirth').valueChanges
      .subscribe(value => {
        if (value) {
          this.form.get('firstNameAtBirth').clearValidators();
          this.form.get('firstNameAtBirth').reset();
          this.form.get('lastNameAtBirth').clearValidators();
          this.form.get('lastNameAtBirth').reset();
        } else {
          this.form.get('firstNameAtBirth').setValidators([Validators.required]);
          this.form.get('lastNameAtBirth').setValidators([Validators.required]);
        }
      });

    this.busy = this.contactDataService.getContactByCassToken(this.contactToken)
      .subscribe(contact => {
        this.contact = contact;
        if (!contact.isWrongUser) {
          this.showForm = true;
          this.form.get('contact.shortName').setValue(contact.shortName);
          this.form.get('contact.birthDate').setValue(contact.dateOfBirth);
          this.form.get('contact.gender').setValue(contact.gender);
          this.form.get('contact.address1_line1').setValue(contact.streetAddress);
          this.form.get('contact.address1_city').setValue(contact.city);
          this.form.get('contact.address1_stateorprovince').setValue(contact.province);
          this.form.get('contact.address1_country').setValue(contact.country);
          this.form.get('contact.address1_postalcode').setValue(contact.postalCode);
        } else {
          this.showForm = false;
        }
        
        //if (this.contact.isComplete) {
        //  this.router.navigateByUrl('/security-screening/confirmation');
        //}
      });

  }

  addAlias(alias: Alias = null) {
    this.aliases.push(this.createAlias(alias));
  }

  deleteAlias(index: number) {
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
      firstname: [alias.firstname],
      middlename: [alias.middlename],
      lastname: [alias.lastname, Validators.required],
    });
  }



  updateUploadedFiles(uploadedNumber: number, docType: string) {
    this.fileCount[docType] = uploadedNumber;
  }

  uploadsValid(): boolean {
    this.validationErrors = [];
    if (this.showFinancialIntegrityForm() && !(this.fileCount['Associate_Fin'] > 0)) {
      this.validationErrors.push("Please Upload Your Completed Financial Integrity Form");
    }

    return this.validationErrors.length <= 0;
  }

  showFinancialIntegrityForm(): boolean {
    return true;
  }

  save() {
    this.showErrors = false;

    if (this.uploadsValid() && this.form.valid) {
      const contact = this.form.value.contact;
      contact.casDateSubmitted = new Date();
      contact.casComplete = 'Yes';
      contact.casConsentValidated = 'Yes';
      var today = new Date();
      contact.casConsentValidatedExpiryDate = new Date(today.setMonth(today.getMonth() + 3));

      if (this.form.value.firstNameAtBirth && this.form.value.lastNameAtBirth) {
        contact.aliases.push({
          firstname: this.form.value.firstNameAtBirth,
          lastname: this.form.value.lastNameAtBirth
        });
      }

      this.contactDataService.updateContactByToken(contact, this.contactToken)
        .subscribe(res => {
          this.router.navigateByUrl('/security-screening/confirmation');
        });
    } else {
      // show error messages
      this.showErrors = true;
      let controls = this.form.controls;
      for (let c in controls) {
        controls[c].markAsTouched();
      }
      controls = (<FormGroup>this.form.get('contact')).controls;
      for (let c in controls) {
        controls[c].markAsTouched();
      }

      this.aliases.controls.forEach(group => {
        group.get('lastname').markAsTouched();
      });
    }
  }

}
