import { Component, OnInit, Input, forwardRef } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl, ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { SpecificLocation, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { MatTableDataSource } from '@angular/material';
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

    form: FormGroup;
    get serviceAreasArr(): FormArray { return this.form.get('serviceAreas') as FormArray; }

    constructor(private fb: FormBuilder) {
        super();

        this.form = fb.group({
            serviceAreas: fb.array([])
        });

        this.form.valueChanges.subscribe(val => {
            this.onChange(val);
            this.value = val.serviceAreas;
        });
    }

    writeValue(serviceAreas: ServiceArea[]) {
        if (serviceAreas) {
            super.writeValue(serviceAreas);
            // this sucks, maybe there's a better way, just trying to
            // set the value of the array to the new value
            while (this.serviceAreasArr.length > 0) { this.serviceAreasArr.removeAt(0); }
            serviceAreas.forEach(area => this.serviceAreasArr.push(this.fb.control(area)));
        } else {
            super.writeValue([]);
        }
    }


    registerOnChange(fn: any) {
        console.log(fn);
        this.onChange = fn;
    }

    // registerOnTouched(fn: any) {
    //     this._onTouched = fn;
    // }

    // validate({ value }: FormControl) {
    //     console.log('validating');
    //     return !this.rows || this.rows.valid
    //       ? null
    //       : { error: 'All fields are required' };
    // }

    addRow() {
        this.writeValue([...this.serviceAreasArr.value, {
            areaNumber: this.serviceAreasArr.length + 1,
            areaLocation: '',
            capacity: '',
            isIndoor: this.isIndoor,
            isOutdoor: !this.isIndoor,
            isPatio: false
        }]);
    }

    removeRow(index: number) {
        if (index >= 0 && index < this.serviceAreasArr.length) {
            this.serviceAreasArr.removeAt(index);
            this.reindex();
        }
    }

    reindex() {
        const areas: ServiceArea[] = [];
        this.serviceAreasArr.controls.forEach((row, index) => {
            areas.push({...row.value, areaNumber: index + 1});
        });
        this.writeValue(areas);
    }
}
