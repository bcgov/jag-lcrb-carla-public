import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { filter } from 'rxjs/operators';
import { LicenseeChangeLog } from '@appmodels/legal-entity-change.model';

@Component({
  selector: 'app-shareholders-and-partners',
  templateUrl: './shareholders-and-partners.component.html',
  styleUrls: ['./shareholders-and-partners.component.scss']
})
export class ShareholdersAndPartnersComponent implements OnInit {
  form: FormGroup;

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareholdersAndPartnersComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, ) {


  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      nameNew: [''],
      firstNameNew: ['', Validators.required],
      lastNameNew: ['', Validators.required],
      dateofBirthNew: [''],
      emailNew: ['', Validators.email],
      numberofSharesNew: ['', Validators.required],
      partnerType: ['', Validators.required],
      isIndividual: [true],
      isShareholderNew: [true],
      legalentitytype: [''],
      dateIssued: [''],
    });

    if (this.data.shareholder) {
      this.form.patchValue(this.data.shareholder);
    }
  }

  save() {
    // console.log('shareholderForm', this.shareholderForm.value, this.shareholderForm.valid);
    if (!this.form.valid) {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
    let formData = this.data.shareholder || {};
    formData = (<any>Object).assign(new LicenseeChangeLog(), formData, this.form.value);
    if (formData.isIndividual === true) {
      formData.nameNew = `${formData.firstNameNew} ${formData.lastNameNew}`;
    }
    this.dialogRef.close(formData);
  }



  close() {
    this.dialogRef.close();
  }

}
