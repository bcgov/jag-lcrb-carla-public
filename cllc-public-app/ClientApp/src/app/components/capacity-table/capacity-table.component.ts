import { Component, Input, forwardRef } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
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
    }
  ]
})
export class CapacityTableComponent extends BaseControlValueAccessor<ServiceArea[]> {
    @Input() isIndoor: boolean;

    form: FormArray;
    registerOnChange(fn: any) { this.onChange = fn; }
    registerOnTouched(fn: any) { this.onTouched = fn; }

    constructor(private fb: FormBuilder) {
        super();

        this.form = fb.array([]);

        this.form.valueChanges.subscribe(val => {
            this.onChange(val);
            this.value = val;
        });
    }

    writeValue(serviceAreas: ServiceArea[]) {
        if (serviceAreas) {
            super.writeValue(serviceAreas);
            // this sucks, maybe there's a better way, just trying to
            // set the value of the array to the new value
            this.form = this.fb.array([]);
            serviceAreas.forEach(area => this.form.controls.push(this.fb.control(area)));
        } else {
            super.writeValue([]);
        }
    }

    addRow() {
        this.writeValue([...this.form.value, {
            areaNumber: this.form.controls.length + 1,
            areaLocation: '',
            capacity: '',
            isIndoor: this.isIndoor,
            isOutdoor: !this.isIndoor,
            isPatio: false
        }]);
    }

    removeRow(index: number) {
        if (index >= 0 && index < this.form.length) {
            this.form.removeAt(index);
            this.reindex();
        }
    }

    reindex() {
        const areas: ServiceArea[] = [];
        this.form.controls.forEach((row, index) => {
            areas.push({...row.value, areaNumber: index + 1});
        });
        this.writeValue(areas);
    }
}
