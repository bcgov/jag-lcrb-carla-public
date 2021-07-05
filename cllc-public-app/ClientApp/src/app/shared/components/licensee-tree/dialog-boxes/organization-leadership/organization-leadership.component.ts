import { Component, Inject } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { LicenseeChangeLog } from "@models/licensee-change-log.model";
import { FormBase } from "@shared/form-base";
import { startOfToday, subDays } from "date-fns";

@Component({
  selector: "app-organization-leadership",
  templateUrl: "./organization-leadership.component.html",
  styleUrls: ["./organization-leadership.component.scss"]
})
export class OrganizationLeadershipComponent extends FormBase {
  form: FormGroup;
  businessType: string;
  parentName: string;
  maxDate19: Date; // used to limit the birth date control to make sure the user is 19 years or older

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<OrganizationLeadershipComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    super();
    // max date is 19 years ago
    this.maxDate19 = subDays(startOfToday(), 19);
    this.parentName = data.parentName;
    this.form = fb.group({
      id: [""],
      isDirectorNew: [false],
      isOfficerNew: [false],
      isManagerNew: [false],
      firstNameNew: ["", Validators.required],
      lastNameNew: ["", Validators.required],
      emailNew: ["", [Validators.email, Validators.required]],
      isIndividual: [true],
      dateofBirthNew: ["", Validators.required],
      titleNew: [""],
      dateofappointment: ["", Validators.required]
    },
      { validator: this.dateLessThanToday("dateofappointment") }
    );

    if (data && data.person) {
      this.form.patchValue(data.person);
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

  /**
   * returns form data on dialog close
   */
  save() {
    let formData = this.data.person || {};
    formData = (Object as any).assign(new LicenseeChangeLog(), formData, this.form.value);
    formData.businessNameNew = `${formData.firstNameNew} ${formData.lastNameNew}`;

    this.dialogRef.close(formData);

    if (!this.form.valid) {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  /**
   * returns true if the form is valid
   */
  close() {
    this.dialogRef.close();
  }
}
