import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { LicenseDataService } from '@services/license-data.service';
import { FormBase } from '@shared/form-base';
import { License } from '@models/license.model';


@Component({
  selector: 'app-offsite-storage',
  templateUrl: './offsite-storage.component.html',
  styleUrls: ['./offsite-storage.component.scss'],
})
export class OffsiteStorageComponent extends FormBase implements OnInit {
  isEditMode = true;
  isReadOnly = false;
  showValidationMessages = false;

  licence: License;

  busy: Subscription;

  form = this.fb.group({
    licenseId: ['', []],
    offsiteStorageLocations: ['', []],
    agreement: [false, [this.customRequiredCheckboxValidator()]]
  });

  constructor(
    private fb: FormBuilder,
    private licenceDataService: LicenseDataService,
    private snackBar: MatSnackBar,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      const licenceId = params.get('licenceId');
      this.form.get('licenseId').setValue(licenceId);
      if (licenceId) {
        this.retrieveLicence(licenceId);
      }
    });
  }

  ngOnInit() {
  }

  retrieveLicence(licenceId: string) {
    this.busy = this.licenceDataService
      .getLicenceById(licenceId)
      .subscribe((licence) => {
        this.licence = licence;
        this.setFormToLicence(licence);
      });
  }

  setFormToLicence(licence: License) {
    this.form.setValue({
      licenseId: licence.id,
      offsiteStorageLocations: licence.offsiteStorageLocations,
      agreement: false
    });

    if (this.isReadOnly) {
      this.form.disable();
    }
  }

  save() {
    // validate form before saving
    if (this.isFormInvalid()) {
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);
    } else {
      this.updateLicence();
    }
  }

  updateLicence() {
    const id = this.form.get('licenseId').value;
    this.busy = this.licenceDataService
      .updateLicenceOffsiteStorage(id, { ...this.form.value })
      .subscribe((result) => {
        if (result.licenseId) {
          this.snackBar.open('Off-Site Storage Successfully Updated', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
          this.router.navigateByUrl('/licences');
        } else {
          this.snackBar.open('Failed to Update Off-Site Storage', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
        }
      }, err => {
        this.snackBar.open('Failed to Update Off-Site Storage', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
      });
  }

  isFormInvalid() {
    return this.form.invalid || !this.form.get('agreement').value;
  }

  cancel() {
    this.router.navigate(['/licences']);
  }
}
