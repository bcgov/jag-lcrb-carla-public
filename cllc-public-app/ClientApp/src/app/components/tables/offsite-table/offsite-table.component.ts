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
  @Input() enabled: boolean = true;
  rows = new FormArray([]);
  registerOnChange(fn: any) { this.onChange = fn; }
  registerOnTouched(fn: any) { this.onTouched = fn; }

  constructor(private fb: FormBuilder) {
    super();

    this.rows.valueChanges.subscribe(val => {
      this.onChange(val);
      this.value = val;
    });
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
      id: [value.id || ''],
      street1: [value.street1, [Validators.required]],
      city: [value.city, [Validators.required]],
      province: ['BC', [Validators.required]],
      postalCode: [value.postalCode, [Validators.required]],
      status: [value.status]
    });

    this.rows.push(group);
  }

  addRow() {
    const newRow: OffsiteStorage = new OffsiteStorage();
    newRow.status = OffsiteStorageStatus.Added;
    this.writeValue([...this.rows.value, newRow]);
  }

  removeRow(index: number) {
    if (index >= 0 && index < this.rows.length) {
      this.rows.removeAt(index);
      this.writeValue(this.rows.value);
    }
  }

  validate({ value }: FormControl) {
    let isNotValid = false;
    for (const row of this.rows.controls) {
      if (row.invalid) {
        isNotValid = true;
      }
    }
    return isNotValid && { invalid: true };
  }
}
