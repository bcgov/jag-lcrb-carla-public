import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { filter } from 'rxjs/operators';
import { DelayedFileUploaderComponent } from '@shared/components/delayed-file-uploader/delayed-file-uploader.component';

const FormValidationErrorMap = {
  licenceNumber: 'Licence Number',
  licenceType: 'Licence Type',
  nameOfApplicant: 'Licensee name',
  establishmentName: 'Establishment name',
  establishmentAddressStreet: 'Establishmen Address Street',
  establishmentAddressCity: 'Establishment Address City',
  establishmentAddressPostalCode: 'Establishment Address city',
  businessTelephone: 'Business Telephone',
  businessEmail: 'Business Email',
  contactFirstName: 'Contact First Name',
  contactLastName: 'Contact Last Name',
  contactRole: 'Contact Title/Position',
  establishmentMailingAddressStreet: 'Mailing Address Street',
  establishmentMailingAddressCity: 'Mailing Address City',
  establishmentMailingAddressPostalCode: 'Mailing Address Postal Code',

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

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      licenceNumber: ['', [Validators.required]],
      licenceType: ['', [Validators.required]],
      nameOfApplicant: ['', [Validators.required]],
      establishmentName: ['', [Validators.required]],
      establishmentAddressStreet: ['', [Validators.required]],
      establishmentAddressCity: ['', [Validators.required]],
      establishmentAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      businessTelephone: ['', [Validators.required]],
      businessEmail: ['', [Validators.required, Validators.email]],
      contactFirstName: ['', [Validators.required]],
      contactLastName: ['', [Validators.required]],
      contactRole: ['', [Validators.required]],
      sameAddresses: [''],
      establishmentMailingAddressStreet: ['', [Validators.required]],
      establishmentMailingAddressCity: ['', [Validators.required]],
      establishmentMailingAddressPostalCode: ['', [Validators.required, this.customZipCodeValidator('addressCountry')]],
      addressCountry: ['Canada'], // only used client side for validation

      receivedLGPermission: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],

      currentTotalCapicityIncluded: ['', [this.customRequiredCheckboxValidator()]],
      areasToBeExtendedIncluded: ['', [this.customRequiredCheckboxValidator()]],
      floorPlanIncluded: ['', [this.customRequiredCheckboxValidator()]],
      writtenApprovalIncluded: ['', [this.customRequiredCheckboxValidator()]],
    });

    this.form.get('sameAddresses').valueChanges
      .pipe(filter(value => !!value)) // only when the box is checked
      .subscribe(() => {
        // copy the address values over
        this.form.get('establishmentMailingAddressStreet').setValue(this.form.get('establishmentAddressStreet').value);
        this.form.get('establishmentMailingAddressCity').setValue(this.form.get('establishmentAddressCity').value);
        this.form.get('establishmentMailingAddressPostalCode').setValue(this.form.get('establishmentAddressPostalCode').value);
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
      alert('Save to be called');
    }
  }

}
