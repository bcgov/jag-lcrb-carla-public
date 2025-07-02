import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TiedHouseDeclaration, TiedHouseTypeEnum, TiedHouseTypes, TiedHouseViewMode } from '@models/tied-house-relationships.model';
import { FormBase } from '@shared/form-base';
import { faTrash } from
  "@fortawesome/free-solid-svg-icons";

@Component({
  selector: 'app-tied-house-declaration-form',
  templateUrl: './tied-house-declaration-form.component.html',
  styleUrls: ['./tied-house-declaration-form.component.scss']
})
export class TiedHouseDeclarationFormComponent extends FormBase implements OnInit {
  _tiedHouseDecleration = {} as TiedHouseDeclaration;
  showOtherField = false;
  faTrash = faTrash;
  isEditable = true;
  tiedHouseTypes = TiedHouseTypes;
  businessTypes = [{ id: 1, name: "Business 1" }]
  tiedHouseTypeEnum = TiedHouseTypeEnum;

  @Input() tiedHouseRelationships = {};
  @Input()
  set tiedHouseDecleration(val: TiedHouseDeclaration) {
    this._tiedHouseDecleration = val;
    this.tryPatchForm();
  }

  @Output() saveTiedHouseDecclaration: EventEmitter<TiedHouseDeclaration> = new EventEmitter<TiedHouseDeclaration>();
  @Output() removeTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();
  @Output() cancelTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();
  get associatedLiquorLicenses(): FormArray {
    return this.form.get('associatedLiquorLicense') as FormArray;
  }
  constructor(private fb: FormBuilder) {
    super();
  }
  ngOnInit(): void {

    this.form.get('relationshipToLicense')?.valueChanges.subscribe(value => {
      this.showOtherField = value === "2";
    })
    this.form.get('tiedHouseType')?.valueChanges.subscribe(() => {
      this.updateFieldValidators();
      this.form.get('firstName')?.setValue('');
      this.form.get('middleName')?.setValue('');
      this.form.get('lastName')?.setValue('');
      this.form.get('dateOfBirth')?.setValue('');
      this.form.get('businessType')?.setValue('');
      this.form.get('legalEntityName')?.setValue('');
    });
  }

  updateAssociatedLicenses(associatedLiquorLicense: any[]) {
    this.associatedLiquorLicenses.clear();
    associatedLiquorLicense.forEach(licence => {
      this.associatedLiquorLicenses.push(this.fb.control(licence));
    });
  }

  save() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      const tiedHouse: TiedHouseDeclaration = {
        ...this.form.value
      };
      this.saveTiedHouseDecclaration.emit(tiedHouse);
    }
  }

  cancle() {
    if (this._tiedHouseDecleration.viewMode == TiedHouseViewMode.new) {
      this.remove();
    }
    if (this._tiedHouseDecleration.viewMode == TiedHouseViewMode.new) {
      this.cancelTiedHouseDeclaration.emit(TiedHouseViewMode.table);
    }
  }

  remove() {
    this.removeTiedHouseDeclaration.emit();
  }

  getInvalidControls(form: FormGroup | FormArray, parentKey: string = ''): string[] {
    let invalidControls: string[] = [];

    Object.keys(form.controls).forEach(key => {
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


  private tryPatchForm() {
    if (!this.form) {
      this.form = this.fb.group({
        tiedHouseType: ["", [Validators.required]],
        dateOfBirth: [""],
        firstName: [""],
        middleName: [""],
        lastName: [""],
        businessType: [""],
        legalEntityName: [""],
        relationshipToLicense: ["", [Validators.required]],
        associatedLiquorLicense: this.fb.array([], this.requiredFormArray),
        otherDescription: ["", [Validators.required]],
        autocompleteInput: [""]
      });

      if (this._tiedHouseDecleration) {
        this.setFormState(this._tiedHouseDecleration.viewMode);
        this.form.patchValue(this._tiedHouseDecleration);
        this.updateAssociatedLicenses(this._tiedHouseDecleration.associatedLiquorLicense || []);
      }
      this.updateFieldValidators();
    }
  }

  updateFieldValidators() {

    if (this.form.get('tiedHouseType')?.value == TiedHouseTypeEnum.Individual) {
      this.form.get('firstName')?.setValidators(Validators.required);
      this.form.get('middleName')?.setValidators(Validators.required);
      this.form.get('lastName')?.setValidators(Validators.required);
      this.form.get('dateOfBirth')?.setValidators(Validators.required);
      this.form.get('businessType')?.clearValidators();
      this.form.get('legalEntityName')?.clearValidators();
    } else {
      this.form.get('firstName')?.clearValidators();
      this.form.get('middleName')?.clearValidators();
      this.form.get('lastName')?.clearValidators();
      this.form.get('dateOfBirth')?.clearValidators();
      this.form.get('businessType')?.setValidators(Validators.required);
      this.form.get('legalEntityName')?.setValidators(Validators.required);
    }
    if(this.showOtherField){
      this.form.get('otherDescription')?.setValidators(Validators.required);
    }
    else{
      this.form.get('otherDescription')?.clearValidators();
    }


    this.form.get('firstName')?.updateValueAndValidity();
    this.form.get('middleName')?.updateValueAndValidity();
    this.form.get('lastName')?.updateValueAndValidity();
    this.form.get('dateOfBirth')?.updateValueAndValidity();
    this.form.get('businessType')?.updateValueAndValidity();
    this.form.get('legalEntityName')?.updateValueAndValidity();
  }

  requiredFormArray(control: AbstractControl) {
    const isArray = Array.isArray(control?.value);
    const hasAtLeastOne = isArray && control.value.length > 0;
    return hasAtLeastOne ? null : { required: true };
  }

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
        this.form.get("relationshipToLicense")?.enable();
        this.form.get("autocompleteInput")?.enable();
        this.isEditable = true;
        break;
      case TiedHouseViewMode.editExistingRecord:
        this.form.disable();
        this.form.get("relationshipToLicense")?.enable();
        this.form.get("autocompleteInput")?.enable();
        this.isEditable = true;
        break;
      default:
        this.form.enable();
        break;
    }
  }
}