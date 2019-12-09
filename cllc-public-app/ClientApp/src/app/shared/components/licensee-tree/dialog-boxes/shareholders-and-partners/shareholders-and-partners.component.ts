import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ValidatorFn, AbstractControl } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/licensee-change-log.model';
import { FormBase } from '@shared/form-base';
import * as moment from 'moment';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-shareholders-and-partners',
  templateUrl: './shareholders-and-partners.component.html',
  styleUrls: ['./shareholders-and-partners.component.scss']
})
export class ShareholdersAndPartnersComponent extends FormBase implements OnInit {
  form: FormGroup;
  parentName: any;
  shareholder: any;
  action = 'add';
  maxDate19: Date;
  availableParentShares: number = 0;
  Account = Account;
  formType: 'shareholder' | 'partnership';

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareholdersAndPartnersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, ) {
    super();
    this.shareholder = data.shareholder;
    this.action = data.action;

    this.formType = data.rootBusinessType;
    this.maxDate19 = moment(new Date()).startOf('day').subtract(19, 'year').toDate();

    if (this.shareholder && this.shareholder.parentLinceseeChangeLog) {
      this.availableParentShares = this.shareholder.parentLinceseeChangeLog.totalSharesNew
        - this.shareholder.parentLinceseeChangeLog.totalChildShares
        + (this.shareholder.numberofSharesNew || 0);
    }

  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      businessNameNew: [''],
      firstNameNew: ['', Validators.required],
      lastNameNew: ['', Validators.required],
      dateofBirthNew: ['', Validators.required],
      emailNew: ['', [Validators.email, Validators.required]],
      numberofSharesNew: [''],
      totalSharesNew: [],
      interestPercentageNew: [''],
      isIndividual: [true],
      isShareholderNew: [true],
      businessType: [''],
      dateIssued: [''],
    });

    this.addFormValidation();

    if (this.data.shareholder) {
      this.form.patchValue(this.data.shareholder);
    }
  }

  /**
   * Adds dynamic (based on data) validataion to the form
   */
  addFormValidation() {
    // partnership vs shaholder validation
    if (this.formType === 'partnership') {
      this.form.get('interestPercentageNew').setValidators([Validators.required]);
    } else if (this.formType === 'shareholder') {
      this.form.get('numberofSharesNew').setValidators([Validators.required, Validators.max(this.availableParentShares)]);
    }

    this.form.get('businessType').valueChanges
      .subscribe(value => {
        if (value === 'PublicCorporation' || value === 'PrivateCorporation') {
          this.form.get('totalSharesNew').setValidators([Validators.required, Validators.max(this.availableParentShares)]);
        } else {
          this.form.get('totalSharesNew').clearValidators();
          this.form.get('totalSharesNew').reset();
        }
      });

    // individual vs corporation/business validation
    this.form.get('isIndividual').valueChanges
      .subscribe(value => {
        if (value) {
          this.form.get('businessType').clearValidators();
          this.form.get('businessType').reset();
          this.form.get('businessNameNew').clearValidators();
          this.form.get('businessNameNew').reset();
          this.form.get('firstNameNew').setValidators([Validators.required]);
          this.form.get('lastNameNew').setValidators([Validators.required]);
          this.form.get('dateofBirthNew').setValidators([Validators.required]);
        } else {
          this.form.get('firstNameNew').clearValidators();
          this.form.get('firstNameNew').reset();
          this.form.get('lastNameNew').clearValidators();
          this.form.get('lastNameNew').reset();
          this.form.get('dateofBirthNew').clearValidators();
          this.form.get('dateofBirthNew').reset();
          this.form.get('businessType').setValidators([Validators.required]);
          this.form.get('businessNameNew').setValidators([Validators.required]);
        }
      });
  }

  /**
   * returns true if the form is valid
   */
  isValid(): boolean {
    let valid = (!this.shareholder.isRoot && this.form.valid)
      || (this.shareholder.isRoot && this.form.get('totalSharesNew').value);

    const formData = this.data.shareholder || {};
    if (this.shareholder.parentLinceseeChangeLog) {
      const value = Object.assign(this.shareholder, new LicenseeChangeLog(), formData, this.form.value);
      if (this.formType === 'shareholder' && value.percentageShares > 100) {
        valid = false;
      }
    }
    return valid;
  }

  /**
   * returns form data on dialog close
   */
  save() {
    if (!this.isValid()) {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    } else {
      let formData = this.data.shareholder || {};
      formData = (<any>Object).assign(new LicenseeChangeLog(), formData, this.form.value);
      if (formData.isRoot) {
        formData.isIndividual = false;
      }
      if (formData.isIndividual === true) {
        formData.businessNameNew = `${formData.firstNameNew} ${formData.lastNameNew}`;
      }
      formData.isShareholderNew = true;
      this.dialogRef.close(formData);
    }
  }

  /**
   * closes the dialog box and returns no data (cancels the add or edit)
   */
  close() {
    this.dialogRef.close();
  }

}
