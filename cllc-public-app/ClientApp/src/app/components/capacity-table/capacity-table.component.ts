import { Component, Input, forwardRef } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR, FormControl, NG_VALIDATORS, ValidationErrors } from '@angular/forms';
import { ServiceArea } from '@models/service-area.model';
import { BaseControlValueAccessor } from './BaseControlValueAccessor';


@Component({
  selector: 'app-capacity-table',
  templateUrl: './capacity-table.component.html',
  styleUrls: ['./capacity-table.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CapacityTableComponent),
      multi: true
    },
    {
        provide: NG_VALIDATORS,
        useExisting: CapacityTableComponent,
        multi: true
    }
  ]
})
export class CapacityTableComponent extends BaseControlValueAccessor<ServiceArea[]> {
    @Input() isIndoor: boolean;

    formGroup: FormGroup;
    get areasArr(): FormArray { return this.formGroup.get('areas') as FormArray; }

    registerOnChange(fn: any) { this.onChange = fn; }
    registerOnTouched(fn: any) { this.onTouched = fn; }

    constructor(private fb: FormBuilder) {
        super();

        this.formGroup = fb.group({
            areas: fb.array([])
        });

        this.formGroup.valueChanges.subscribe(val => {
            this.onChange(val);
            this.value = val.areas;
        });
    }

    writeValue(serviceAreas: ServiceArea[]) {
        if (serviceAreas) {
            super.writeValue(serviceAreas);
            // this sucks, maybe there's a better way, just trying to
            // set the value of the array to the new value
            while (this.areasArr.length > 0) { this.areasArr.removeAt(0); }
            serviceAreas.forEach(area => this.areasArr.push(this.fb.control(area)));
        } else {
            super.writeValue([]);
        }
    }

    addRow() {
        this.writeValue([...this.areasArr.value, {
            areaNumber: this.areasArr.controls.length + 1,
            areaLocation: '',
            capacity: '',
            isIndoor: this.isIndoor,
            isOutdoor: !this.isIndoor,
            isPatio: false
        }]);
    }

    removeRow(index: number) {
        if (index >= 0 && index < this.areasArr.length) {
            this.areasArr.removeAt(index);
            this.reindex();
        }
    }

    reindex() {
        const areas: ServiceArea[] = [];
        this.areasArr.controls.forEach((row, index) => {
            areas.push({...row.value, areaNumber: index + 1});
        });
        this.writeValue(areas);
    }

    validate({ value }: FormControl) {
        let isValid = true;
        this.areasArr.controls.forEach((row, index) => {
            if (row.invalid) {
                isValid = false;
            }
        });
        return !isValid && {
            invalid: true
        };
    }
}
