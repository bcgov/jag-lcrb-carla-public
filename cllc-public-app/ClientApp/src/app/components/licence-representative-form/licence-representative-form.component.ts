import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators, FormGroup, ValidatorFn } from '@angular/forms';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';
import { LicenseDataService } from '@services/license-data.service';
import { MatSnackBar } from '@angular/material';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

const FormValidationErrorMap = {
  representativeFullName: 'Representative Name',
  representativePhoneNumber: 'Telephone',
  representativeEmail: 'E-mail Address',
  signatureAgreement: 'Declaration Checkbox',
  hasAtLeastOneScope: 'One Scope is Required'
};

const hasAtLeastOneScopeValidator: ValidatorFn = (fg: FormGroup) => {
  return (
    fg.controls['representativeCanSubmitPermanentChangeApplications'].value ||
    fg.controls['representativeCanSignTemporaryChangeApplications'].value ||
    fg.controls['representativeCanObtainLicenceInformation'].value ||
    fg.controls['representativeCanSignGroceryStoreProofOfSale'].value ||
    fg.controls['representativeCanAttendEducationSessions'].value ||
    fg.controls['representativeCanAttendComplianceMeetings'].value ||
    fg.controls['representativeCanRepresentAtHearings'].value
  ) ? null : { 'hasAtLeastOneScope': true };
};

@Component({
  selector: 'app-licence-representative-form',
  templateUrl: './licence-representative-form.component.html',
  styleUrls: ['./licence-representative-form.component.scss'],
})
export class LicenceRepresentativeFormComponent extends FormBase implements OnInit {
  busy: Subscription;
  form: FormGroup;
  validationMessages: string[] = [];
  licenceId: string;
  showValidationMessages: boolean;
  hasRepresentativeOnFile = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private licenceDataService: LicenseDataService,
    private snackBar: MatSnackBar
    ) {
      super();
      this.route.paramMap.subscribe(params => {
        this.licenceId = params.get('licenceId');
      });
      
    }

  ngOnInit() {
    this.form = this.fb.group({
      licenseId: ['', []],
      representativeFullName: ['', [Validators.required]],
      representativePhoneNumber: ['', [Validators.required]],
      representativeEmail: ['', [Validators.required]],
      representativeCanSubmitPermanentChangeApplications: [false, []],
      representativeCanSignTemporaryChangeApplications: [false, []],
      representativeCanObtainLicenceInformation: [false, []],
      representativeCanSignGroceryStoreProofOfSale: [false, []],
      representativeCanAttendEducationSessions: [false, []],
      representativeCanAttendComplianceMeetings: [false, []],
      representativeCanRepresentAtHearings: [false, []],
      signatureAgreement: [false, [this.customRequiredCheckboxValidator()]]
    }, { validator: hasAtLeastOneScopeValidator });
    this.getRepresentativeData();
  }

  getRepresentativeData() {
    this.busy = this.licenceDataService.getLicenceById(this.licenceId).pipe()
      .subscribe(result => {
        if (result.id) {
          this.form.controls['representativeFullName'].setValue(result.representativeFullName);
          this.form.controls['representativePhoneNumber'].setValue(result.representativePhoneNumber);
          this.form.controls['representativeEmail'].setValue(result.representativeEmail);
          this.form.controls['representativeCanSubmitPermanentChangeApplications'].setValue(result.representativeCanSubmitPermanentChangeApplications);
          this.form.controls['representativeCanSignTemporaryChangeApplications'].setValue(result.representativeCanSignTemporaryChangeApplications);
          this.form.controls['representativeCanObtainLicenceInformation'].setValue(result.representativeCanObtainLicenceInformation);
          this.form.controls['representativeCanSignGroceryStoreProofOfSale'].setValue(result.representativeCanSignGroceryStoreProofOfSale);
          this.form.controls['representativeCanAttendEducationSessions'].setValue(result.representativeCanAttendEducationSessions);
          this.form.controls['representativeCanAttendComplianceMeetings'].setValue(result.representativeCanAttendComplianceMeetings);
          this.form.controls['representativeCanRepresentAtHearings'].setValue(result.representativeCanRepresentAtHearings);
          
          if (result.representativeFullName) {
            this.hasRepresentativeOnFile = true;
          }
        }
      })
  }

  save() {
    if (!this.form.valid) {
      this.validationMessages = this.listControlsWithErrors(this.form, FormValidationErrorMap)
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);  
    }
    else {
      this.validationMessages = [];
      this.form.controls['licenseId'].setValue(this.licenceId);
      this.busy = this.licenceDataService.updateLicenseeRepresentative(this.licenceId, this.form.value).pipe()
        .subscribe(result => {
          if (result.licenseId) {
            this.snackBar.open('Representative Successfully Updated', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
            this.router.navigateByUrl('/licences');
          } else {
            this.snackBar.open('Failed to Update Representative', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
          }
        }, err => {
          this.snackBar.open('Failed to Update Representative', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
        });
    }
  }

  remove() {
    var emptyRep = new ApplicationLicenseSummary();
    emptyRep.licenseId = this.licenceId;
    emptyRep.representativeFullName = '';
    emptyRep.representativePhoneNumber = '';
    emptyRep.representativeEmail = '';
    emptyRep.representativeCanSubmitPermanentChangeApplications = false;
    emptyRep.representativeCanSignTemporaryChangeApplications = false;
    emptyRep.representativeCanObtainLicenceInformation = false;
    emptyRep.representativeCanSignGroceryStoreProofOfSale = false;
    emptyRep.representativeCanAttendEducationSessions = false;
    emptyRep.representativeCanAttendComplianceMeetings = false;
    emptyRep.representativeCanRepresentAtHearings = false;

    this.busy = this.licenceDataService.updateLicenseeRepresentative(this.licenceId, emptyRep).pipe()
      .subscribe(result => {
        if (result.licenseId) {
          this.snackBar.open('Representative Successfully Removed', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
          this.router.navigateByUrl('/licences');
        } else {
          this.snackBar.open('Failed to Remove Representative', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
        }
      }, err => {
        this.snackBar.open('Failed to Remove Representative', 'Fail', { duration: 2500, panelClass: ['red-snackbar'] });
      });
  }
}
