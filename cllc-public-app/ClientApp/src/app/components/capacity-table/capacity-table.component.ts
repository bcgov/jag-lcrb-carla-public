import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR, FormControl, NG_VALIDATORS, ValidationErrors } from '@angular/forms';
import { ServiceArea, AreaCategory } from '@models/service-area.model';
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
export class CapacityTableComponent extends BaseControlValueAccessor<ServiceArea[]> implements OnInit {
    @Input() areaCategory: number;
    @Input() applicationTypeName: string;
    total: number;

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

    ngOnInit() {
        if (this.isCapacityArea()) {
            this.addRow();
        }
    }

    writeValue(serviceAreas: ServiceArea[]) {
        console.log('writing valu')
        console.log(serviceAreas);
        if (serviceAreas) {
            super.writeValue(serviceAreas);

            while (this.areasArr.length > 0) { this.areasArr.removeAt(0); }
            if (serviceAreas.length > 0) {
                serviceAreas.forEach(area => { this.areasArr.push(this.fb.control(area)); });
                this.updateTotal();
            }
        } else {
            super.writeValue([]);
        }
    }

    updateTotal() {
        this.total = 0;
        this.areasArr.value.forEach(area => {
            if (typeof area['capacity'] === 'number') {
                this.total += area['capacity'];
            } else {
                var num = parseInt(area['capacity'], 10);
                if (num > 0) {
                    this.total += num;
                }
            }
        });
    }

    addRow() {
        console.log('adding row');
        this.writeValue([...this.areasArr.value, {
            areaCategory: this.areaCategory,
            areaNumber: this.areasArr.controls.length + 1,
            areaLocation: this.areaCategory !== AreaCategory.Capacity ? '' : this.applicationTypeName,
            capacity: '',
            isIndoor: this.areaCategory === AreaCategory.Service,
            isOutdoor: this.areaCategory === AreaCategory.OutsideArea,
            isPatio: false
        }]);
    }

    removeRow(index: number) {
        if (index >= 0 && index < this.areasArr.length) {
            this.areasArr.removeAt(index);
            const newArr: ServiceArea[] = this.areasArr.controls.map((control, i) => {
                return { ...control.value, areaNumber: i + 1 };
            });
            this.writeValue(newArr);
        }
    }

    onRowChange(val) {
        this.updateTotal();
    }

    validate({ value }: FormControl) {
        let isValid = true;
        console.log('validating')
        this.areasArr.controls.forEach((row, index) => {
            console.log(row);
            if (row.invalid) {
                isValid = false;
            }
        });
        return !isValid && {
            invalid: true
        };
    }

    isService(): boolean {
        return this.areaCategory === AreaCategory.Service;
    }

    isOutsideArea(): boolean {
        return this.areaCategory === AreaCategory.OutsideArea;
    }

    isCapacityArea(): boolean {
        return this.areaCategory === AreaCategory.Capacity;
    }
}
