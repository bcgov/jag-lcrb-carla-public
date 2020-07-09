import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { SpecificLocation, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { MatTableDataSource } from '@angular/material';
import { ServiceArea } from '@models/service-area.model';


@Component({
  selector: 'app-capacity-table',
  templateUrl: './capacity-table.component.html',
  styleUrls: ['./capacity-table.component.scss']
})
export class CapacityTableComponent extends FormBase implements OnInit {
    @Input() licence: ApplicationLicenseSummary;
    @Input() isIndoor: boolean;

    rows: FormArray;
    isDirty = false;

    constructor(
        private fb: FormBuilder,
        private store: Store<AppState>,
        ) {
        super();
        this.rows = fb.array([]);
        // this.rows = this.licence.serviceAreas.map((area) => area);
    }

    ngOnInit() {
        console.log('t');
    }

    addRow() {
        const form = this.fb.group({
            'areaNumber': [{ value: this.rows.length + 1, disabled: true }, [Validators.required]],
            'areaLocation': ['', [Validators.required]],
            'capacity': ['', [Validators.required]],
        });
        if (this.isIndoor) {
            form.addControl('isIndoor', new FormControl({ value: true, disabled: true }, [Validators.required]));
            form.addControl('isPatio', new FormControl('', []));
        }
        this.rows.push(form);
    }

    removeRow(index: number) {
        if (index >= 0 && index < this.rows.length) {
            this.rows.removeAt(index);
            this.reindex();
        }
    }

    reindex() {
        this.rows.controls.forEach((row, index) => {
            row.get('areaNumber').setValue(index + 1);
        });
    }
}
