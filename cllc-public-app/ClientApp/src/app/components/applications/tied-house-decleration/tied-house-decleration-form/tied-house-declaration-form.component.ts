import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { formatDate } from '@components/applications/tied-house-decleration/tide-house-utils';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import {
  BusinessTypes,
  LIQTiedHouseTypeCodes,
  LIQTiedHouseTypes,
  RelationshipTypes,
  TiedHouseConnection,
  TiedHouseStatusCode,
  TiedHouseViewMode
} from '@models/tied-house-connection.model';
import { FormBase } from '@shared/form-base';
import { endOfToday, subYears } from 'date-fns';

@Component({
  selector: 'app-tied-house-declaration-form',
  templateUrl: './tied-house-declaration-form.component.html',
  styleUrls: ['./tied-house-declaration-form.component.scss']
})
export class TiedHouseDeclarationFormComponent extends FormBase implements OnInit {
  /**
   * Whether the form is in read-only mode. Default is false.
   */
  @Input() isReadOnly = false;
  /**
   * The tied house declaration data to load into the form.
   */
  @Input() set tiedHouseDecleration(val: TiedHouseConnection) {
    this._tiedHouseDecleration = val;

    if (this.form) {
      this.patchFormFromInput();
    }
  }

  @Output() saveTiedHouseDecclaration: EventEmitter<TiedHouseConnection> = new EventEmitter<TiedHouseConnection>();
  @Output() removeTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancelTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();

  _tiedHouseDecleration = {} as TiedHouseConnection;

  faTrash = faTrash;

  tiedHouseTypes = LIQTiedHouseTypes;
  LIQTiedHouseTypeCodes = LIQTiedHouseTypeCodes;
  relationshipTypes = RelationshipTypes;
  businessTypes = BusinessTypes;

  showOtherField = false;
  isEditable = true;

  maxDate = endOfToday();
  minDate = subYears(endOfToday(), 125);

  requiredFormArray(control: AbstractControl): ValidationErrors | null {
    return control instanceof FormArray && control.length > 0 ? null : { required: true };
  }

  get isExistingDeclaration() {
    return this._tiedHouseDecleration.supersededById;
  }

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.initForm();

    if (this._tiedHouseDecleration) {
      this.patchFormFromInput();
    }

    this.showOtherField = this.form.get('relationshipToLicence')?.value == 845280009;

    this.form.get('relationshipToLicence')?.valueChanges.subscribe((value) => {
      this.showOtherField = value == 845280009;

      this.updateFieldValidators();
    });

    this.form.get('liqTiedHouseType')?.valueChanges.subscribe(() => {
      if (this.form.get('liqTiedHouseType')?.value === LIQTiedHouseTypeCodes.LegalEntity) {
        // Clear "individual" form fields
        this.form.get('firstName')?.setValue('');
        this.form.get('middleName')?.setValue('');
        this.form.get('lastName')?.setValue('');
        this.form.get('dateOfBirth')?.setValue('');
        this.form.get('otherDescription')?.setValue('');
      } else {
        // Clear "legal entity" form fields
        this.form.get('businessType')?.setValue('');
        this.form.get('legalEntityName')?.setValue('');
      }

      this.updateFieldValidators();
    });

    this.updateFieldValidators();

    if (this.isReadOnly) {
      this.form?.disable();
    }
  }

  private patchFormFromInput() {
    if (this._tiedHouseDecleration) {
      this.setFormState();
      this._tiedHouseDecleration.dateOfBirth = formatDate(this._tiedHouseDecleration.dateOfBirth);
      this.form.patchValue(this._tiedHouseDecleration);
      this.updateAssociatedLicenses(this._tiedHouseDecleration.associatedLiquorLicense || []);
    }

    this.updateFieldValidators();

    if (this.isReadOnly) {
      this.form?.disable();
    }
  }

  private initForm() {
    this.form = this.fb.group({
      liqTiedHouseType: [LIQTiedHouseTypeCodes.Individual, [Validators.required]],
      dateOfBirth: [''],
      firstName: [''],
      middleName: [''],
      lastName: [''],
      businessType: [''],
      legalEntityName: [''],
      relationshipToLicence: [null, [Validators.required]],
      associatedLiquorLicense: this.fb.array([], [this.requiredFormArray]),
      otherDescription: ['', [Validators.required]],
      autocompleteInput: ['']
    });
  }

  get associatedLiquorLicenses(): FormArray {
    return this.form.get('associatedLiquorLicense') as FormArray;
  }

  updateAssociatedLicenses(associatedLiquorLicense: any[]) {
    this.associatedLiquorLicenses.clear();

    associatedLiquorLicense.forEach((licence) => {
      this.associatedLiquorLicenses.push(this.fb.control(licence));
    });
  }

  save() {
    this.form.markAllAsTouched();

    if (!this.form.valid || this.form.get('associatedLiquorLicense')?.value.length == 0) {
      return;
    }

    const tiedHouse: TiedHouseConnection = { ...this.form.getRawValue() };
    tiedHouse.applicationId = this._tiedHouseDecleration.applicationId;
    tiedHouse.id = this._tiedHouseDecleration.id;

    let dateOfBirth = this.form.get('dateOfBirth').value ? new Date(this.form.get('dateOfBirth').value) : undefined;

    tiedHouse.dateOfBirth = dateOfBirth?.toISOString();

    this.saveTiedHouseDecclaration.emit(tiedHouse);
    this._tiedHouseDecleration.viewMode = TiedHouseViewMode.disabled;
    this.setFormState();
  }

  cancel() {
    if (!this._tiedHouseDecleration.applicationId && !this._tiedHouseDecleration.accountId) {
      // If the declaration is new (not yet persisted), we remove it when the user clicks cancel.
      this.remove();
    } else if (this.isExistingDeclaration) {
      // If the declaration is existing, we reset the view mode and status code to existing (aka: stop editing).
      this._tiedHouseDecleration.viewMode = TiedHouseViewMode.existing;
      this._tiedHouseDecleration.statusCode = TiedHouseStatusCode.existing;
    } else {
      this._tiedHouseDecleration.viewMode = TiedHouseViewMode.disabled;
      this.updateFormValues();
    }
  }

  edit() {
    if (this._tiedHouseDecleration.supersededById != null) {
      this._tiedHouseDecleration.viewMode = TiedHouseViewMode.editExistingRecord;
    } else {
      this._tiedHouseDecleration.viewMode = TiedHouseViewMode.addNewRelationship;
    }

    this.setFormState();
    this.form.patchValue(this._tiedHouseDecleration);
  }

  remove() {
    this.removeTiedHouseDeclaration.emit();
  }

  getInvalidControls(form: FormGroup | FormArray, parentKey: string = ''): string[] {
    let invalidControls: string[] = [];

    Object.keys(form.controls).forEach((key) => {
      const control = form.get(key);
      const controlPath = parentKey ? `${parentKey}.${key}` : key;

      if (control instanceof FormGroup || control instanceof FormArray) {
        invalidControls = invalidControls.concat(this.getInvalidControls(control, controlPath));
      } else if (control && control.invalid) {
        invalidControls.push(controlPath);
      }
    });

    return invalidControls;
  }

  private updateLegalEntityFieldValidators() {
    this.form.get('firstName')?.clearValidators();
    this.form.get('lastName')?.clearValidators();
    this.form.get('dateOfBirth')?.clearValidators();
    this.form.get('businessType')?.setValidators(Validators.required);
    this.form.get('legalEntityName')?.setValidators(Validators.required);
  }

  private updateIndividualFieldValidators() {
    this.form.get('firstName')?.setValidators(Validators.required);
    this.form.get('lastName')?.setValidators(Validators.required);
    this.form.get('dateOfBirth')?.setValidators(Validators.required);
    this.form.get('businessType')?.clearValidators();
    this.form.get('legalEntityName')?.clearValidators();
  }

  private updateOtherFieldValidators() {
    if (this.showOtherField) {
      this.form.get('otherDescription')?.setValidators(Validators.required);
    } else {
      this.form.get('otherDescription')?.clearValidators();
    }
  }

  private updateFieldValuesAndValidities() {
    this.form.get('firstName')?.updateValueAndValidity();
    this.form.get('lastName')?.updateValueAndValidity();
    this.form.get('dateOfBirth')?.updateValueAndValidity();
    this.form.get('businessType')?.updateValueAndValidity();
    this.form.get('legalEntityName')?.updateValueAndValidity();
    this.form.get('otherDescription')?.updateValueAndValidity();
  }

  private updateFormValues() {
    this.setFormState();
    this.showOtherField = this.form.get('relationshipToLicence').value == 845280009;
    this.form.patchValue(this._tiedHouseDecleration);
    this.updateAssociatedLicenses(this._tiedHouseDecleration.associatedLiquorLicense || []);
  }

  /**
   * Update the form field values and validators based on the current state of the form.
   *
   */
  updateFieldValidators() {
    if (this.form.get('liqTiedHouseType')?.value === LIQTiedHouseTypeCodes.LegalEntity) {
      this.updateLegalEntityFieldValidators();
    } else {
      this.updateIndividualFieldValidators();
    }

    this.updateOtherFieldValidators();

    this.updateFieldValuesAndValidities();
  }

  /**
   * Sets the form state based on the provided view mode.
   *
   * @private
   * @param {*} viewMode
   */
  private setFormState() {
    var viewMode = this._tiedHouseDecleration.viewMode;
    this.form.enable();

    switch (viewMode) {
      case TiedHouseViewMode.disabled:
        this.form.disable();
        this.isEditable = false;
        break;

      case TiedHouseViewMode.new:
        this.form.enable();
        this.isEditable = true;
        break;

      case TiedHouseViewMode.addNewRelationship:
        this.form.disable();
        // A related record inherits some of the values of the related record, and so only a subset of fields are
        // editable
        this.form.get('relationshipToLicence')?.enable();
        this.form.get('otherDescription')?.enable();
        this.form.get('autocompleteInput')?.enable();
        this.isEditable = true;
        break;

      case TiedHouseViewMode.editExistingRecord:
        this.form.disable();
        this.form.get('autocompleteInput')?.enable();
        this.isEditable = true;
        break;

      default:
        this.form.enable();
        break;
    }
  }
}
