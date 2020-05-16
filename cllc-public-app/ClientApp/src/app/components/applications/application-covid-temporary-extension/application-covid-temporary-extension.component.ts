import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { filter } from 'rxjs/operators';
import { ApplicationDataService } from '@services/application-data.service';
import { MatSnackBar } from '@angular/material';
import { DelayedFileUploaderComponent } from '@shared/components/delayed-file-uploader/delayed-file-uploader.component';

const FormValidationErrorMap = {
  description1: 'Licence Number',
  licenceType: 'Licence Type',
  nameOfApplicant: 'Licensee name',
  establishmentName: 'Establishment name',
  establishmentAddressStreet: 'Establishmen Address Street',
  establishmentAddressCity: 'Establishment Address City',
  establishmentAddressPostalCode: 'Establishment Address city',
  contactPersonPhone: 'Business Telephone',
  contactPersonEmail: 'Business Email',
  contactPersonFirstName: 'Contact First Name',
  contactPersonLastName: 'Contact Last Name',
  contactPersonRole: 'Contact Title/Position',
  addressStreet: 'Mailing Address Street',
  addressCity: 'Mailing Address City',
  addressPostalCode: 'Mailing Address Postal Code',

  receivedLGPermission: 'Local Government Permission Receipt checkbox',
  signatureAgreement: 'Declaration checkbox',

  currentTotalCapicityIncluded: 'Current Total Capacity checkbox',
  areasToBeExtendedIncluded: 'Areas to be Extended checkbox',
  floorPlanIncluded: 'Floor Plan checkbox',
  writtenApprovalIncluded: 'Written Approval checkbox',
};

@Component({
  selector: 'app-application-covid-temporary-extension',
  templateUrl: './application-covid-temporary-extension.component.html',
  styleUrls: ['./application-covid-temporary-extension.component.scss']
})
export class ApplicationCovidTemporaryExtensionComponent extends FormBase implements OnInit {
  form: FormGroup;
  busy: any;
  showValidationMessages: boolean;
  validationMessages: string[] = [];

  

  @ViewChild('uploadedFloorplanDocuments', { static: false }) uploadedFloorplanDocuments: DelayedFileUploaderComponent;
  @ViewChild('uploadedLicenseeRepresentativeNotficationFormDocuments', { static: false }) uploadedLicenseeRepresentativeNotficationFormDocuments: DelayedFileUploaderComponent;

  constructor(private fb: FormBuilder,
        private applicationDataService: ApplicationDataService,
        private snackBar: MatSnackBar) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      description1: ['', [Validators.required, Validators.maxLength(7)]], //this holds the licenceNumber
      licenceType: ['', [Validators.required]],
      nameOfApplicant: ['', [Validators.required]],
      establishmentName: ['', [Validators.required]],
      establishmentAddressStreet: ['', [Validators.required]],
      establishmentAddressCity: ['', [Validators.required]],
      establishmentAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      contactPersonPhone: ['', [Validators.required]],
      contactPersonEmail: ['', [Validators.required, Validators.email]],
      contactPersonFirstName: ['', [Validators.required]],
      contactPersonLastName: ['', [Validators.required]],
      contactPersonRole: ['', [Validators.required]],
      sameAddresses: [''],
      addressStreet: ['', [Validators.required]],
      addressCity: ['', [Validators.required]],
      addressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      addressCountry: ['Canada'], // only used client side for validation

      receivedLGPermission: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]]
    });

    this.form.get('sameAddresses').valueChanges
      .pipe(filter(value => !!value)) // only when the box is checked
      .subscribe(() => {
        // copy the address values over
        this.form.get('addressStreet').setValue(this.form.get('establishmentAddressStreet').value);
        this.form.get('addressCity').setValue(this.form.get('establishmentAddressCity').value);
        this.form.get('addressPostalCode').setValue(this.form.get('establishmentAddressPostalCode').value);
      });

  }

  submitApplication() {
    if (!this.form.valid) {
      this.showValidationMessages = true;
      this.markConstrolsAsTouched(this.form);
      this.validationMessages = this.listControlsWithErrors(this.form, FormValidationErrorMap)
    }
    else {
      this.validationMessages = [];
      this.busy = this.applicationDataService.createCovidApplication(this.form.value)
        .subscribe(res => {
          this.snackBar.open('Application Submitted', 'Success',
            { duration: 2500, panelClass: ['green-snackbar'] });
        },
        err => {
          this.snackBar.open('Failed to submit application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        });
    }
  }

}
