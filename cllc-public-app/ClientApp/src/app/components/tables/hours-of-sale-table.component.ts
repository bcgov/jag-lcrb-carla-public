import { Component, Input, forwardRef } from '@angular/core';
import { FormBuilder, FormArray, FormGroup, NG_VALUE_ACCESSOR, FormControl, NG_VALIDATORS } from '@angular/forms';
import { ServiceArea, AreaCategory } from '@models/service-area.model';
import { ServiceHour } from '@models/service-hour.model';
import { BaseControlValueAccessor } from './BaseControlValueAccessor';

const ServiceHours = [
    // '00:00', '00:15', '00:30', '00:45', '01:00', '01:15', '01:30', '01:45', '02:00', '02:15', '02:30', '02:45', '03:00',
    // '03:15', '03:30', '03:45', '04:00', '04:15', '04:30', '04:45', '05:00', '05:15', '05:30', '05:45', '06:00', '06:15',
    // '06:30', '06:45', '07:00', '07:15', '07:30', '07:45', '08:00', '08:15', '08:30', '08:45',
    '09:00', '09:15', '09:30',
    '09:45', '10:00', '10:15', '10:30', '10:45', '11:00', '11:15', '11:30', '11:45', '12:00', '12:15', '12:30', '12:45',
    '13:00', '13:15', '13:30', '13:45', '14:00', '14:15', '14:30', '14:45', '15:00', '15:15', '15:30', '15:45', '16:00',
    '16:15', '16:30', '16:45', '17:00', '17:15', '17:30', '17:45', '18:00', '18:15', '18:30', '18:45', '19:00', '19:15',
    '19:30', '19:45', '20:00', '20:15', '20:30', '20:45', '21:00', '21:15', '21:30', '21:45', '22:00', '22:15', '22:30',
    '22:45', '23:00'
    // , '23:15', '23:30', '23:45'
];

@Component({
  selector: 'app-hours-of-sale-table',
  templateUrl: './hours-of-sale-table.component.html',
  styleUrls: ['./tables.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => HoursOfSaleTableComponent),
      multi: true
    },
    {
        provide: NG_VALIDATORS,
        useExisting: HoursOfSaleTableComponent,
        multi: true
    }
  ]
})
export class HoursOfSaleTableComponent extends BaseControlValueAccessor<ServiceArea[]> {
    @Input() applicationTypeName: string;
    @Input() enabled: boolean = true;
    ServiceHours = ServiceHours;

    formGroup: FormGroup;
    get hoursArr(): FormArray { return this.formGroup.get('hours') as FormArray; }

    registerOnChange(fn: any) { this.onChange = fn; }
    registerOnTouched(fn: any) { this.onTouched = fn; }

    constructor(private fb: FormBuilder) {
        super();

        this.formGroup = fb.group({
            hours: fb.array([])
        });

        this.formGroup.valueChanges.subscribe(val => {
            this.onChange(val);
            this.value = val.areas;
        });
    }

    writeValue(serviceHours: ServiceHour[]) {
        if (serviceHours) {
            super.writeValue(serviceHours);

            while (this.hoursArr.length > 0) { this.hoursArr.removeAt(0); }
            if (serviceHours.length > 0) {
                serviceHours.forEach(area => { this.hoursArr.push(this.fb.control(area)); });
                /* afterwrite here */
            }
        } else {
            super.writeValue([]);
        }
    }

    addDay() {
        this.writeValue([...this.hoursArr.value, {
            areaCategory: this.areaCategory,
            areaNumber: this.areasArr.controls.length + 1,
            areaLocation: '',
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
        /* on row change here */
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
