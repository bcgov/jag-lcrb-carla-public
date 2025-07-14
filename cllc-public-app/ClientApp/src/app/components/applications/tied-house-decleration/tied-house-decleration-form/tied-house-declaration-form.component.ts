import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { formatDate } from '@components/applications/tied-house-decleration/tide-house-utils';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import {
  BusinessTypes,
  RelationshipTypes,
  TiedHouseConnection,
  TiedHouseStatusCode,
  TiedHouseTypes,
  TiedHouseViewMode
} from '@models/tied-house-connection.model';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-tied-house-declaration-form',
  templateUrl: './tied-house-declaration-form.component.html',
  styleUrls: ['./tied-house-declaration-form.component.scss']
})
export class TiedHouseDeclarationFormComponent extends FormBase implements OnInit {
  @Input() set tiedHouseDecleration(val: TiedHouseConnection) {
    this._tiedHouseDecleration = val;
    this.tryPatchForm();
  }

  @Output() saveTiedHouseDecclaration: EventEmitter<TiedHouseConnection> = new EventEmitter<TiedHouseConnection>();
  @Output() removeTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancelTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();

  _tiedHouseDecleration = {} as TiedHouseConnection;

  faTrash = faTrash;

  tiedHouseTypes = TiedHouseTypes;
  relationshipTypes = RelationshipTypes;
  businessTypes = BusinessTypes;

  showOtherField = false;
  isEditable = true;

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.form.get('relationshipToLicence')?.valueChanges.subscribe((value) => {
      this.showOtherField = value == 845280002;
      this.updateFieldValidators();
    });

    this.form.get('isLegalEntity')?.valueChanges.subscribe(() => {
      this.updateFieldValidators();
      this.form.get('firstName')?.setValue('');
      this.form.get('middleName')?.setValue('');
      this.form.get('lastName')?.setValue('');
      this.form.get('dateOfBirth')?.setValue('');
      this.form.get('businessType')?.setValue('');
      this.form.get('legalEntityName')?.setValue('');
    });

    this.updateFieldValidators();
  }

  private tryPatchForm() {
    if (!this.form) {
      this.form = this.fb.group({
        isLegalEntity: [false, [Validators.required]],
        dateOfBirth: [''],
        firstName: [''],
        middleName: [''],
        lastName: [''],
        businessType: [''],
        legalEntityName: [''],
        relationshipToLicence: ['', [Validators.required]],
        associatedLiquorLicense: this.fb.array([], this.requiredFormArray),
        otherDescription: ['', [Validators.required]],
        autocompleteInput: ['']
      });

      if (this._tiedHouseDecleration) {
        this.setFormState(this._tiedHouseDecleration.viewMode);
        this._tiedHouseDecleration.dateOfBirth = formatDate(this._tiedHouseDecleration.dateOfBirth);
        this.form.patchValue(this._tiedHouseDecleration);
        this.updateAssociatedLicenses(this._tiedHouseDecleration.associatedLiquorLicense || []);
      }

      this.updateFieldValidators();
    }
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

    if (!this.form.valid) {
      return;
    }

    const tiedHouse: TiedHouseConnection = { ...this.form.value };

    this.saveTiedHouseDecclaration.emit(tiedHouse);
  }

  cancel() {
    if (
      this._tiedHouseDecleration.viewMode == TiedHouseViewMode.new ||
      this._tiedHouseDecleration.viewMode == TiedHouseViewMode.addNewRelationship
    ) {
      // If the declaration is new or in add relationship mode, we remove it as it has not yet been persisted.
      this.remove();
    } else {
      // If the declaration is existing, we reset the view mode and status code to existing (aka: stop editing).
      this._tiedHouseDecleration.viewMode = TiedHouseViewMode.existing;
      this._tiedHouseDecleration.statusCode = TiedHouseStatusCode.existing;
    }
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
    this.form.get('middleName')?.clearValidators();
    this.form.get('lastName')?.clearValidators();
    this.form.get('dateOfBirth')?.clearValidators();
    this.form.get('businessType')?.setValidators(Validators.required);
    this.form.get('legalEntityName')?.setValidators(Validators.required);
  }

  private updateIndividualFieldValidators() {
    this.form.get('firstName')?.setValidators(Validators.required);
    this.form.get('middleName')?.setValidators(Validators.required);
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
    this.form.get('middleName')?.updateValueAndValidity();
    this.form.get('lastName')?.updateValueAndValidity();
    this.form.get('dateOfBirth')?.updateValueAndValidity();
    this.form.get('businessType')?.updateValueAndValidity();
    this.form.get('legalEntityName')?.updateValueAndValidity();
    this.form.get('otherDescription')?.updateValueAndValidity();
  }

  /**
   * Update the form field values and validators based on the current state of the form.
   *
   */
  updateFieldValidators() {
    if (this.form.get('isLegalEntity')?.value == false) {
      this.updateIndividualFieldValidators();
    } else {
      this.updateLegalEntityFieldValidators();
    }

    this.updateOtherFieldValidators();

    this.updateFieldValuesAndValidities();
  }

  /**
   * A custom validator to ensure that a FormArray control has at least one item.
   * Marks the field as "required" if the control is not an array, or if it is empty.
   *
   * @param {AbstractControl} control
   * @return {*}
   */
  requiredFormArray(control: AbstractControl) {
    const isArray = Array.isArray(control?.value);
    const hasAtLeastOne = isArray && control.value.length > 0;
    return hasAtLeastOne ? null : { required: true };
  }

  /**
   * Sets the form state based on the provided view mode.
   *
   * @private
   * @param {*} viewMode
   */
  private setFormState(viewMode) {
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
        this.form.get('relationshipToLicence')?.enable();
        this.form.get('otherDescription')?.enable();
        this.form.get('autocompleteInput')?.enable();
        this.isEditable = true;
        break;

      default:
        this.form.enable();
        break;
    }
  }
}
