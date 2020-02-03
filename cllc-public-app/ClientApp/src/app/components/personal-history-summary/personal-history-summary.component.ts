import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Alias } from '@models/alias.model';
import { ActivatedRoute } from '@angular/router';
import { ContactDataService } from '@services/contact-data.service';
import { PHSContact } from '@models/contact.model';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-personal-history-summary',
  templateUrl: './personal-history-summary.component.html',
  styleUrls: ['./personal-history-summary.component.scss']
})
export class PersonalHistorySummaryComponent extends FormBase implements OnInit {

  aliasesToDelete: any;
  form: FormGroup;
  contactToken: string;
  contact: PHSContact;

  public get aliases(): FormArray {
    return this.form.get('contact.aliases') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private contactDataService: ContactDataService,
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
        emailaddress1: [''],
        telephone1: [''],
        address1_line1: ['', [Validators.required, Validators.minLength(5)]],
        address1_city: ['', Validators.required],
        address1_stateorprovince: ['', [Validators.required]],
        address1_country: ['', Validators.required],
        address1_postalcode: ['', Validators.required],
        jobTitle: [''],
        birthDate: ['', Validators.required],
        birthPlace: [''],
        gender: ['', Validators.required],
        mobilePhone: ['', [Validators.required,]],
        primaryIdNumber: [''],
        secondaryIdNumber: [''],
        selfDisclosure: [''],
        secondaryIdentificationType: [''],
        primaryIdentificationType: [''],
        phsConnectionsDetails: [''],
        phsLivesInCanada: ['', Validators.required],
        phsHasLivedInCanada: ['', Validators.required],
        // phsExpired: [''],
        // phsComplete: [''],
        phsConnectionsToOtherLicences: ['', Validators.required],
        phsCanadianDrugAlchoholDrivingOffence: ['', Validators.required],
        phsForeignDrugAlchoholOffence: ['', Validators.required],
        aliases: this.fb.array([])
      })
    });

    this.contactDataService.getContactByPhsToken(this.contactToken)
      .subscribe(contact => {
        this.contact = contact;
        this.form.get('contact.shortName').setValue(contact.shortName);
      })

  }

  addAlias(alias: Alias = null) {
    this.aliases.push(this.createAlias(alias));
  }

  deleteAlias(index: number) {
    const alias = this.aliases.controls[index];
    // if (alias.value.id) {
    //   this.aliasesToDelete.push(alias.value);
    // }
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

  showStatutoryDeclaration() {
    let show = (
      this.form.get('contact.phsLivesInCanada').value === 'No' ||
      this.form.get('contact.phsCanadianDrugAlchoholDrivingOffence').value === 'Yes' ||
      this.form.get('contact.phsForeignDrugAlchoholOffence').value === 'Yes'
    );
    return show;
  }


  isQuestionnaireValid(): boolean {
    const valid = false;
    return valid;
  }

  save() {
    const contact = this.form.value.contact;
    contact.phsDateSubmitted = new Date();
    if (this.form.value.firstNameAtBirth && this.form.value.lastNameAtBirth) {
      contact.aliases.push({
        firstname: this.form.value.firstNameAtBirth,
        lastname: this.form.value.lastNameAtBirth
      });
    }

    this.contactDataService.updatePHSContact(contact, this.contactToken)
      .subscribe(res => {
      })
  }

}
