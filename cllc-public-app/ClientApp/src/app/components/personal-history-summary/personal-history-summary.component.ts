import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Alias } from '@models/alias.model';
import { ActivatedRoute } from '@angular/router';
import { ContactDataService } from '@services/contact-data.service';

@Component({
  selector: 'app-personal-history-summary',
  templateUrl: './personal-history-summary.component.html',
  styleUrls: ['./personal-history-summary.component.scss']
})
export class PersonalHistorySummaryComponent implements OnInit {

  aliasesToDelete: any;
  form: FormGroup;
  contactToken: string;
  contactId: string;

  public get aliases(): FormArray {
    return this.form.get('contact.aliases') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private contactDataService: ContactDataService,
    private route: ActivatedRoute) {
    this.route.paramMap.subscribe(pmap => this.contactToken = pmap.get('token'));

   }

  ngOnInit() {
    this.form = this.fb.group({
      firstNameAtBirth: [''],
      lastNameAtBirth: [''],
      contact: this.fb.group({
        id: [''],
        fullname: [''],
        firstname: [''],
        lastname: [''],
        emailaddress1: [''],
        telephone1: [''],
        address1_line1: [''],
        address1_city: [''],
        address1_stateorprovince: [''],
        address1_country: [''],
        address1_postalcode: [''],
        jobTitle: [''],
        birthDate: [''],
        birthPlace: [''],
        gender: [''],
        mobilePhone: [''],
        primaryIdNumber: [''],
        secondaryIdNumber: [''],
        isWorker: [''],
        selfDisclosure: [''],
        secondaryIdentificationType: [''],
        primaryIdentificationType: [''],
        phsConnectionsDetails: [''],
        phsLivesInCanada: [''],
        phsHasLivedInCanada: [''],
        phsExpired: [''],
        phsComplete: [''],
        phsConnectionsToOtherLicences: [''],
        phsCanadianDrugAlchoholDrivingOffence: [''],
        phsDateSubmitted: [''],
        phsForeignDrugAlchoholOffence: [''],
        aliases: this.fb.array([])
      })
    });

    this.contactDataService.getContactByPhsToken(this.contactToken)
    .subscribe(contact => {
      this.contactId = contact.id;
      this.form.get('contact.firstname').setValue(contact.firstname);
      this.form.get('contact.lastname').setValue(contact.lastname);
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


}
