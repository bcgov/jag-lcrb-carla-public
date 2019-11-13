import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { LicenseeChangeLog } from '@models/legal-entity-change.model';
import { FormBase } from '@shared/form-base';

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

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareholdersAndPartnersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, ) {
    super();
    this.shareholder = data.shareholder;
    this.action = data.action;
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      businessNameNew: [''],
      firstNameNew: ['', Validators.required],
      lastNameNew: ['', Validators.required],
      dateofBirthNew: ['', Validators.required],
      emailNew: ['', [Validators.email, Validators.required]],
      numberofSharesNew: ['', Validators.required],
      totalSharesNew: [],
      // partnerType: ['', Validators.required],
      isIndividual: [true],
      isShareholderNew: [true],
      legalentitytype: [''],
      dateIssued: [''],
    });

    this.form.get('isIndividual').valueChanges
      .subscribe(value => {
        if (value) {
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
          this.form.get('businessNameNew').setValidators([Validators.required]);
        }
      });

    if (this.data.shareholder) {
      this.form.patchValue(this.data.shareholder);
    }
  }

  save() {
    if ((!this.shareholder.isRoot && !this.form.valid) || (this.shareholder.isRoot && !this.form.get('totalSharesNew').value)) {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    } else {
      let formData = this.data.shareholder || {};
      formData = (<any>Object).assign(new LicenseeChangeLog(), formData, this.form.value);
      if (formData.isIndividual === true) {
        formData.businessNameNew = `${formData.firstNameNew} ${formData.lastNameNew}`;
      }
      this.dialogRef.close(formData);
    }
  }



  close() {
    this.dialogRef.close();
  }

}
