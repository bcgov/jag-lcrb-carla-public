import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormBuilder, Validators } from '@angular/forms';
import { Alias } from '@models/alias.model';
import { ActivatedRoute, Router } from '@angular/router';
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
  showErrors: boolean;
  fileCount: any = {};
  validationErrors: string[] = [];

  public get aliases(): FormArray {
    return this.form.get('contact.aliases') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private contactDataService: ContactDataService,
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
        phsHasFive: ['', Validators.required],
        phsHasLivedInCanada: ['', Validators.required],
        phsExclusiveMFG:['', Validators.required],
        phsExclusiveDetails: [''],
        phsFinancialInt:['', Validators.required],
        phsFinancialIntDetails: [''],
        phsProfitAgreement:['', Validators.required],
        phsProfitAgreementDetails: [''],
        // phsExpired: [''],
        // phsComplete: [''],
        phsConnectionsToOtherLicences: ['', Validators.required],
        phsCanadianDrugAlchoholDrivingOffence: ['', Validators.required],
        phsForeignDrugAlchoholOffence: ['', Validators.required],
        aliases: this.fb.array([])
      })
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

    this.form.get('contact.phsLivesInCanada').valueChanges
      .subscribe(value => {
        if (value === 'Yes') {
          this.form.get('contact.phsHasLivedInCanada').clearValidators();
          this.form.get('contact.phsHasLivedInCanada').reset();
          this.form.get('contact.phsHasFive').setValidators([Validators.required]);
          
        } else {
          this.form.get('contact.phsHasLivedInCanada').setValidators([Validators.required]);
          this.form.get('contact.phsHasFive').clearValidators();
          this.form.get('contact.phsHasFive').reset();
        }
      });


    this.form.get('contact.phsConnectionsToOtherLicences').valueChanges
      .subscribe(value => {
        if (value === 'No') {
          this.form.get('contact.phsConnectionsDetails').clearValidators();
          this.form.get('contact.phsConnectionsDetails').reset();
        } else {
          this.form.get('contact.phsConnectionsDetails').setValidators([Validators.required]);
        }
      });

    this.contactDataService.getContactByPhsToken(this.contactToken)
      .subscribe(contact => {
        this.contact = contact;
        this.form.get('contact.shortName').setValue(contact.shortName);
        if (this.contact.isComplete) {
          this.router.navigateByUrl('/security-screening/confirmation');
        }
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

  showStatutoryDeclaration(): boolean {
    let show = (
      this.form.get('contact.phsHasFive').value === 'No' ||
      this.form.get('contact.phsHasLivedInCanada').value === 'No' ||      
      this.form.get('contact.phsCanadianDrugAlchoholDrivingOffence').value === 'Yes' ||
      this.form.get('contact.phsForeignDrugAlchoholOffence').value === 'Yes'
    );
    return show;
  }

  showCRCUpload(): boolean {
    let show = (
      this.form.get('contact.phsLivesInCanada').value === 'Yes' ||
      this.form.get('contact.phsHasLivedInCanada').value === 'Yes'
    );
    return show;
  }

  showDriversAbstract(): boolean {
    let show = (
      this.form.get('contact.phsCanadianDrugAlchoholDrivingOffence').value === 'Yes'
    );
    return show;

  }

  updateUploadedFiles(uploadedNumber: number, docType: string) {
    this.fileCount[docType] = uploadedNumber;
  }

  uploadsValid(): boolean {
    this.validationErrors = [];
    if (this.showCRCUpload() && !(this.fileCount['CRC'] > 0)) {
      this.validationErrors.push("Please Upload Your Completed Criminal Record Check ");
    }
    if (this.showStatutoryDeclaration() && !(this.fileCount['StatDeclaration'] > 0)) {
      this.validationErrors.push("Please Upload Your Statutory Declaration");
    }

    if (this.showDriversAbstract() && (this.fileCount['DriverExtract'] > 0)) {
      this.validationErrors.push("Please Upload Your Driver's Extract");
    }
    return this.validationErrors.length <= 0;
  }

  save() {
    this.showErrors = false;

    if (this.uploadsValid() && this.form.valid) {
      const contact = this.form.value.contact;
      contact.phsDateSubmitted = new Date();
      contact.phsComplete = 'Yes';

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
