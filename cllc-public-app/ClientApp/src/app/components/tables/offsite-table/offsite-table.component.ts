import { Component, Input, forwardRef } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR, FormControl, NG_VALIDATORS, Validators } from '@angular/forms';
import { OffsiteStorage, OffsiteStorageStatus } from '@models/offsite-storage.model';
import { BaseControlValueAccessor } from '../BaseControlValueAccessor';


@Component({
  selector: 'app-offsite-table',
  templateUrl: './offsite-table.component.html',
  styleUrls: ['./offsite-table.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OffsiteTableComponent),
      multi: true
    },
    {
      provide: NG_VALIDATORS,
      useExisting: OffsiteTableComponent,
      multi: true
    }
  ]
})
export class OffsiteTableComponent extends BaseControlValueAccessor<OffsiteStorage[]> {
  // Whether this control is enabled or not (affects edit mode)
  @Input() enabled: boolean = true;

  // Whether to run validation per row
  @Input() shouldValidate: boolean = false;

  rows = new FormArray([]);
  offsiteStorageStatus = OffsiteStorageStatus;
  validationMessages: string[] = [];

  registerOnChange(fn: any) { this.onChange = fn; }
  registerOnTouched(fn: any) { this.onTouched = fn; }

  constructor(private fb: FormBuilder) {
    super();
    this.rows.valueChanges.subscribe(val => {
      this.onChange(val);
      this.value = val;
    });
  }

  get showErrorSection(): boolean {
    return this.enabled && this.shouldValidate && this.validationMessages.length > 0;
  }

  writeValue(array: OffsiteStorage[]) {
    // return early if no value provided
    if (!array) {
      super.writeValue([]);
      return;
    }

    // First, clear previous state. Then, add the new state.
    super.writeValue(array);
    while (this.rows.length > 0) {
      this.rows.removeAt(0);
    }
    for (const obj of array) {
      this.addInternal(obj);
    }
  }

  private addInternal(value: OffsiteStorage) {
    const group = this.fb.group({
      id: [value.id],
      name: [value.name, [Validators.required, Validators.maxLength(250)]],
      street1: [value.street1, [Validators.required]],
      city: [value.city, [Validators.required]],
      province: ['BC', [Validators.required]],
      postalCode: [value.postalCode, [Validators.required]],
      status: [value.status],
      dateAdded: [value.dateAdded],
      dateRemoved: [value.dateRemoved]
    });

    this.rows.push(group);
  }

  addRow() {
    const newRow: OffsiteStorage = new OffsiteStorage();
    newRow.status = OffsiteStorageStatus.Active;
    newRow.dateAdded = new Date();
    this.writeValue([...this.rows.value, newRow]);
  }

  removeRow(index: number) {
    if (index >= 0 && index < this.rows.length) {
      const patchObj: Partial<OffsiteStorage> = {
        status: OffsiteStorageStatus.Removed,
        dateRemoved: new Date()
      };
      this.rows.at(index).patchValue(patchObj);
      this.writeValue(this.rows.value);
    }
  }

  readonly(index: number): boolean {
    return !this.enabled || this.rows.at(index).get('status').value === this.offsiteStorageStatus.Removed;
  }

  validate({ value }: FormControl) {
    let invalid = false;
    for (const row of this.rows.controls) {
      if (row.invalid) {
        invalid = true;
      }
    }
    this.validationMessages = [...new Set<string>(this.listControlsWithErrors(this.rows))];
    return invalid && { invalid: true };
  }

  private listControlsWithErrors(form: FormGroup | FormArray): string[] {
    let errors = [];
    if (form instanceof FormGroup || form instanceof FormArray) {
      for (const c in form.controls) {
        const control = form.get(c);

        if (control.valid || control.status === 'DISABLED') {
          continue;  // skip valid fields
        }

        if (control instanceof FormGroup || control instanceof FormArray) {
          errors = [...errors, ...this.listControlsWithErrors(control)];
        } else {
          if (control.errors?.required) {
            errors.push('All fields are required');
          }
          if (control.errors?.maxlength) {
            errors.push('Location name cannot exceed 250 characters');
          }
        }
      }
    }
    return errors;
  }
}
