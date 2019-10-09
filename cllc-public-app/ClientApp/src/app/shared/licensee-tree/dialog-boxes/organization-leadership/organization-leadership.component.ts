import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { LicenseeChangeLog } from '@models/legal-entity-change.model';

@Component({
  selector: 'app-organization-leadership',
  templateUrl: './organization-leadership.component.html',
  styleUrls: ['./organization-leadership.component.scss']
})
export class OrganizationLeadershipComponent {
  directorOfficerForm: FormGroup;
  businessType: string;

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<OrganizationLeadershipComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.directorOfficerForm = fb.group({
      id: [''],
      leaderType: ['key-personnel'],
      isDirectorNew: [false],
      isOfficerNew: [false],
      isSeniorManagementNew: [false],
      firstNameNew: ['', Validators.required],
      lastNameNew: ['', Validators.required],
      emailNew: ['', Validators.email],
      titleNew: [''],
      dateofappointment: ['', Validators.required]
    }, { validator: this.dateLessThanToday('dateofappointment') }
    );

    if (data && data.person) {
      this.directorOfficerForm.patchValue(data.person);
    }
    this.businessType = data.businessType;
  }

  dateLessThanToday(field1) {
    return form => {
      const d1 = form.controls[field1].value;
      if (!d1) {
        return {};
      }
      const d1Date = new Date(d1.year, d1.month, d1.day);
      if (d1Date < new Date()) {
        return { dateLessThanToday: true };
      }
      return {};
    };
  }

  save() {
    let formData = this.data.person || {};
    formData = (<any>Object).assign(new LicenseeChangeLog(), formData, this.directorOfficerForm.value);
    formData.nameNew = `${formData.firstNameNew} ${formData.lastNameNew}`;

    this.dialogRef.close(formData);

    if (!this.directorOfficerForm.valid) {
      Object.keys(this.directorOfficerForm.controls).forEach(field => {
        const control = this.directorOfficerForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  close() {
    this.dialogRef.close();
  }

  isFieldError(field: string) {
    const isError = !this.directorOfficerForm.get(field).valid && this.directorOfficerForm.get(field).touched;
    return isError;
  }
}
