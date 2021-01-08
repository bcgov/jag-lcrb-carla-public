import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { EventCategory, LicenceEvent } from '@models/licence-event.model';
import { AppState } from '@app/app-state/models/app-state';
import { LicenceEventsService } from '@services/licence-events.service';
import { faQuestionCircle, faTrash, faSave } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-tua-event',
  templateUrl: './tua-event.component.html',
  styleUrls: ['./tua-event.component.scss']
})
export class TuaEventComponent extends FormBase implements OnInit {
  // icons
  faSave = faSave;
  faTrash = faTrash;
  faQuestionCircle = faQuestionCircle;

  // enums
  eventCategory = EventCategory;

  isEditMode = false;
  isReadOnly = false;

  licenceEvent: LicenceEvent;
  busy: Subscription;

  timeForms = this.fb.array([]);
  form = this.fb.group({
    status: ['', [Validators.required]],
    id: ['', []],
    name: ['', []],
    licenceId: ['', []],
    accountId: ['', []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, 'Temporary Use Area').value, []],
    eventName: ['', [Validators.required]],
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],
    clientHostname: ['', [Validators.required]],

    temporaryAreaEventType: ['', [Validators.required]],
    minorsAttending: ['', [Validators.required]],
    venueDescription: ['', [Validators.required]],

    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
    agreement: [false, [Validators.required]],


  });

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
  }

  ngOnInit() {
  }

}
