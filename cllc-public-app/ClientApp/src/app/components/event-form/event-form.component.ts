import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';
import { SpecificLocation, EventClass, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { takeWhile, switchMap } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { User } from '@models/user.model';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-event-form',
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.scss']
})
export class EventFormComponent extends FormBase implements OnInit {
  isEditMode = false;
  licenceEvent: LicenceEvent;

  busy: Subscription;
  specificLocation = SpecificLocation;
  foodService = FoodService;
  entertainment = Entertainment;
  eventClass = EventClass;
  eventType = EventType;
  eventStatus = EventStatus;

  eventForm = this.fb.group({
    status: ['', [Validators.required]],
    id: ['', []],
    name: ['', []],
    licenceId: ['', []],
    accountId: ['', []],
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],
    eventClass: ['', [Validators.required]],
    eventType: ['', [Validators.required]],
    eventTypeDescription: ['', [Validators.required]],
    clientHostname: ['', [Validators.required]],
    maxAttendance: ['', [Validators.required]],
    minorsAttending: ['', [Validators.required]],
    foodService: ['', [Validators.required]],
    foodServiceDescription: ['', []],
    entertainment: ['', [Validators.required]],
    entertainmentDescription: ['', []],
    venueDescription: ['', [Validators.required]],
    specificLocation: ['', [Validators.required]],
    additionalLocationInformation: ['', []],
    street1: ['', [Validators.required]],
    street2: ['', []],
    city: ['', [Validators.required]],
    province: ['BC', [Validators.required]],
    postalCode: ['', [Validators.required]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
    agreement: [false, [Validators.required]],
    // unused
    externalId: ['', []],
    eventNumber: ['', []],
    importSequenceNumber: ['', []],
    communityApproval: ['', []],
    notifyEventInspector: ['', []]
  });

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
    ) {
      super();
      this.route.paramMap.subscribe(params => {
        this.eventForm.controls['licenceId'].setValue(params.get('licenceId'));
        if (params.get('eventId')) {
          this.isEditMode = true;
          this.retrieveSavedEvent(params.get('eventId'));
        } else {
          console.log(this.getOptionFromLabel(this.eventStatus, 'Draft').value);
          this.eventForm.controls['status'].setValue(this.getOptionFromLabel(this.eventStatus, 'Draft').value);
        }
      });
    }

  ngOnInit() {
    this.store.select(state => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: User) => {
        this.eventForm.controls['contactEmail'].setValue(data.email);
      });
  }

  retrieveSavedEvent(eventId: string) {
    this.busy = this.licenceEvents.getLicenceEvent(eventId)
    .subscribe((licenceEvent) => {
      this.setFormToLicenceEvent(licenceEvent);
    });
  }

  setFormToLicenceEvent(licenceEvent: LicenceEvent) {
    this.eventForm.setValue({
      ...licenceEvent,
      agreement: false
    });
  }

  save(submit = false) {
    if (submit) {
      this.eventForm.controls['status'].setValue(this.getOptionFromLabel(this.eventStatus, 'In Review').value);
    }
    if (this.isEditMode) {
      this.updateLicence();
    } else {
      this.createLicence();
    }
  }

  clearRelatedFormFieldIfNotOther(options: any, fieldName: string, relatedField: string) {
    const option = this.getOptionFromValue(options, this.eventForm.controls[fieldName].value);
    if (option.label !== 'Other') {
      this.eventForm.controls[relatedField].setValue('');
    }
  }

  updateLicence() {
    this.busy = this.licenceEvents.updateLicenceEvent(this.eventForm.get('id').value, this.eventForm.value)
    .subscribe((licenceEvent) => {
      this.router.navigate(['/licences']);
    });
  }

  createLicence() {
    this.eventForm.removeControl('id');
    this.busy = this.licenceEvents.createLicenceEvent(this.eventForm.value)
    .subscribe((licenceEvent) => {
      this.router.navigate(['/licences']);
    });
  }

  compareFn(c1: any, c2: any): boolean {
    return c1 && c2 ? c1.value === c2.value : c1 === c2;
  }

  getOptionFromValue(options: any, value: number) {
    const idx = options.findIndex(opt => opt.value === value);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }

  getOptionFromLabel(options: any, label: string) {
    const idx = options.findIndex(opt => opt.label === label);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }
}
