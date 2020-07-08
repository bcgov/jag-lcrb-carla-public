import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';
import { SpecificLocation, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';


@Component({
  selector: 'app-capacity-table',
  templateUrl: './capacity-table.component.html',
  styleUrls: ['./capacity-table.component.scss']
})
export class CapacityTableComponent extends FormBase implements OnInit {
    @Input() licence: ApplicationLicenseSummary;
    @Input() isIndoor: boolean;

    // rows = [];

    constructor(
        private fb: FormBuilder,
        private store: Store<AppState>,
        ) {
        super();
        // this.rows = this.licence.serviceAreas.map((area) => area);
    }

    ngOnInit() {
        console.log('t')
    }
}
