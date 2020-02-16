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

const DEFAULT_START_TIME = {
  hour: 9,
  minute: 0
};
const DEFAULT_END_TIME = {
  hour: 2,
  minute: 0
};

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
  startDateMinimum: Date;
  endDateMinimum: Date;
  endDateMaximum: Date;
  scheduleIsInconsistent = false;

  timeForms = this.fb.array([]);
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
          this.resetDateFormsToDefault();
          this.eventForm.controls['status'].setValue(this.getOptionFromLabel(this.eventStatus, 'Draft').value);
        }
      });
      this.startDateMinimum = new Date();
      this.startDateMinimum.setDate(this.startDateMinimum.getDate() + 21);
      this.endDateMinimum = new Date();
      this.endDateMinimum.setDate(this.endDateMinimum.getDate() + 21);
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
    // this.packageUpTimeForms();
    if (this.isEditMode) {
      this.updateLicence();
    } else {
      this.createLicence();
    }
  }

  // packageUpTimeForms() {
  //   for(var i; i < this.timeForms.length; i++) {
  //   this.timeForms.forEach((form) => {

  //   });
  // }

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

  toggleScheduleConsistency() {
    this.scheduleIsInconsistent = !this.scheduleIsInconsistent;
    this.refreshTimeDays();
  }

  refreshTimeDays() {
    if (this.scheduleIsInconsistent) {
      const days = this.getDaysArray(this.eventForm.controls['startDate'].value, this.eventForm.controls['endDate'].value);
      this.resetDateFormsToArray(days);
    } else {
      this.resetDateFormsToDefault();
    }
  }

  startDateChanged() {
    this.updateEndDateMinimum();
    this.refreshTimeDays();
  }
  endDateChanged() {
    this.refreshTimeDays();
  }

  resetDateFormsToDefault() {
    this.timeForms = this.fb.array([]);
    this.timeForms.push(this.fb.group({
      dateTitle: ['Default Times', []],
      startTime: [DEFAULT_START_TIME, [Validators.required]],
      endTime: [DEFAULT_END_TIME, [Validators.required]],
      liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
      liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
    }));
  }

  resetDateFormsToArray(datesArray) {
    this.timeForms = this.fb.array([]);
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    datesArray.forEach(element => {
      this.timeForms.push(this.fb.group({
        dateTitle: [days[element.getDay()] + ', ' + element.toLocaleDateString('en-US'), []],
        date: [element, []],
        startTime: [DEFAULT_START_TIME, [Validators.required]],
        endTime: [DEFAULT_END_TIME, [Validators.required]],
        liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
        liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
      }));
    });
  }

  updateEndDateMinimum() {
    if (this.eventForm.controls['startDate'].value === null || this.eventForm.controls['startDate'].value === '') {
      this.endDateMinimum = new Date();
      this.endDateMinimum.setDate(this.endDateMinimum.getDate() + 21);
      this.endDateMaximum = null;
    } else if (this.eventForm.controls['endDate'].value === null || this.eventForm.controls['endDate'].value === '') {
      this.endDateMinimum = this.eventForm.controls['startDate'].value;
      this.endDateMaximum = new Date(this.eventForm.controls['startDate'].value);
      this.endDateMaximum.setDate(this.eventForm.controls['startDate'].value.getDate() + 30);
    } else {
      // start and end date
      if (this.eventForm.controls['endDate'].value.getTime() < this.eventForm.controls['startDate'].value.getTime()) {
        this.eventForm.controls['endDate'].setValue(null);
      }
      this.endDateMinimum = this.eventForm.controls['startDate'].value;
      this.endDateMaximum = new Date(this.eventForm.controls['startDate'].value);
      this.endDateMaximum.setDate(this.eventForm.controls['startDate'].value.getDate() + 30);
    }
  }

  getDaysArray(start, end) {
    start = new Date(start);
    end = new Date(end);
    for(var arr = [], dt = start; dt <= end; dt.setDate(dt.getDate() + 1)) {
        arr.push(new Date(dt));
    }
    return arr;
  }
}
