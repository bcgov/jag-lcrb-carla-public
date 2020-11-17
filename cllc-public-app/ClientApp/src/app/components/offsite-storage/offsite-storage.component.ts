import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { takeWhile } from 'rxjs/operators';
import { LicenseDataService } from '@services/license-data.service';
import { LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { AppState } from '@app/app-state/models/app-state';
import { User } from '@models/user.model';
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
  showErrorSection = false;

  licence: License;

  busy: Subscription;
  eventStatus = EventStatus;

  form = this.fb.group({
    status: ['', [Validators.required]],
    id: ['', []],
    name: ['', []],
    licenceId: ['', []],
    street1: ['', [Validators.required]],
    street2: ['', []],
    city: ['', [Validators.required]],
    province: ['BC', [Validators.required]],
    postalCode: ['', [Validators.required]],
    agreement: [false, [Validators.required]]
  });

  constructor(
    private fb: FormBuilder,
    private licenceDataService: LicenseDataService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      this.form.get('licenceId').setValue(params.get('licenceId'));
    });
  }

  ngOnInit() {

  }

  retrieveLicence(licenceId: string) {
    this.busy = this.licenceDataService.getLicenceById(licenceId)
      .subscribe((licence) => {
        this.licence = licence;
        this.setFormToLicence(licence);
      });
  }

  setFormToLicence(licence: License) {
    //TODO: Temporary test code
    const offsite = licence.offsiteStorageLocations.length > 0 ? licence.offsiteStorageLocations[0] : null;

    this.form.setValue({
      licenceId: licence.id,
      street1: licence.street1,
      street2: licence.street2,
      city: licence.city,
      province: licence.province,
      postalCode: licence.postalCode,
      agreement: false
    });

    if (this.isReadOnly) {
      this.form.disable();
    }
  }

  save(submit = false) {
    if (submit) {
      this.form.controls['status'].setValue(this.getOptionFromLabel(this.eventStatus, 'Submitted').value);
    }

    if (this.isEditMode) {
      this.updateLicence();
    } else {
      this.createLicence();
    }
  }

  updateLicence() {
    this.busy = this.licenceEvents.updateLicenceEvent(this.form.get('id').value, { ...this.form.value })
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
  }

  createLicence() {
    this.form.removeControl('id');
    this.busy = this.licenceEvents.createLicenceEvent({ ...this.form.value })
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
  }

  getOptionFromValue(options: any, value: number) {
    const idx = options.findIndex(opt => opt.value === value);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }

  getOptionFromLabel(options: any, label: string) {
    const idx = options.findIndex(opt => opt.label === label);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }


  isFormValid() {
    return this.form.invalid || !this.form.controls['agreement'].value;
  }

  cancel() {
    if (this.isEditMode) {
      const id = this.form.get('id').value;
      const status = this.getOptionFromLabel(this.eventStatus, 'Cancelled').value;
      this.busy = this.licenceEvents.updateLicenceEvent(id, { ...this.form.value, status: status, licenceId: this.form.get('licenceId').value })
        .subscribe((licenceEvent) => {
          this.router.navigate(['/licences']);
        });
    } else {
      this.router.navigate(['/licences']);
    }
  }
}
