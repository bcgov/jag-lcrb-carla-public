import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TiedHouseDeclaration, TiedHouseViewMode } from '@models/tied-house-relationships.model';
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
  @Input() tiedHouseTypes = [];
  @Input() tiedHouseRelationships = {};
  @Input()
  set tiedHouseDecleration(val: TiedHouseDeclaration) {
    this._tiedHouseDecleration = val;
    this.tryPatchForm();
  }

  @Output() saveTiedHouseDecclaration: EventEmitter<TiedHouseDeclaration> = new EventEmitter<TiedHouseDeclaration>();
  @Output() removeTiedHouseDeclaration: EventEmitter<any> = new EventEmitter<any>();
  get associatedLiquorLicenses(): FormArray {
    return this.form.get('associatedLiquorLicense') as FormArray;
  }
  constructor(private fb: FormBuilder) {
    super();
  }
  ngOnInit(): void {
    this.form = this.fb.group({
      tiedHouseType: ["", [Validators.required]],
      dateOfBirth: ["", [Validators.required]],
      firstName: ["", [Validators.required]],
      middleName: ["", [Validators.required]],
      lastName: ["", [Validators.required]],
      relationshipToLicense: ["", [Validators.required]],
      associatedLiquorLicense: this.fb.array([]),
      autocompleteInput: [""]
    });
    this.tryPatchForm();
    this.form.get('relationshipToLicense')?.valueChanges.subscribe(value => {
      this.showOtherField = value === "2";
    })
  }

  updateAssociatedLicenses(associatedLiquorLicense: any[]) {
    this.associatedLiquorLicenses.clear(); 
    associatedLiquorLicense.forEach(licence => {
      this.associatedLiquorLicenses.push(this.fb.control(licence));
    });
  }

  save() {
    this.form.markAllAsTouched();
    // if (this.form.valid) {
    const tiedHouse: TiedHouseDeclaration = {
      ...this.form.value
    };
    this.saveTiedHouseDecclaration.emit(tiedHouse);
    // }
  }

  remove() {
    this.removeTiedHouseDeclaration.emit();
  }

  private tryPatchForm() {
    if (this.form && this._tiedHouseDecleration) {
      this.form.patchValue(this._tiedHouseDecleration);
      this.updateAssociatedLicenses(this._tiedHouseDecleration.associatedLiquorLicense || []);
      this.setFormState(this._tiedHouseDecleration.viewMode);
    }
  }

  private setFormState(viewMode) {
    switch (viewMode) {
      case TiedHouseViewMode.disabled:
        this.form.disable();
        this.isEditable = false;
        break;

      case TiedHouseViewMode.editable:
        this.form.enable();
        this.isEditable = true;
        break;

      case TiedHouseViewMode.addNewRelationship:
        this.form.disable();
        this.form.get("relationshipToLicense").enable();
        this.form.get("autocompleteInput").enable();
        this.isEditable = true;
        break;
      case TiedHouseViewMode.editExistingRecord:
        this.form.disable();
        this.form.get("relationshipToLicense").enable();
        this.form.get("autocompleteInput").enable();
        this.isEditable = true;
        break;
      default:
        this.form.enable();
        break;
    }
  }
}