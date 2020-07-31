import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';
import { SpecificLocation, FoodService, Entertainment, EventType, LicenceEvent, EventStatus } from '../../models/licence-event.model';
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
const DAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

@Component({
  selector: 'app-market-event',
  templateUrl: './market-event.component.html',
  styleUrls: ['./market-event.component.scss'],
})
export class MarketEventComponent extends FormBase implements OnInit {
  isEditMode = false;
  isReadOnly = false;
  licenceEvent: LicenceEvent;

  busy: Subscription;
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
    clientHostname: ['', [Validators.required]],
    street1: ['', [Validators.required]],
    street2: ['', []],
    city: ['', [Validators.required]],
    province: ['BC', [Validators.required]],
    postalCode: ['', [Validators.required]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
    agreement: [false, [Validators.required]]
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
      this.startDateMinimum.setDate(this.startDateMinimum.getDate());
      this.endDateMinimum = new Date();
      this.endDateMinimum.setDate(this.endDateMinimum.getDate());
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
    if (licenceEvent.status === this.getOptionFromLabel(this.eventStatus, 'Approved').value) {
      this.isReadOnly = true;
    }

    const schedules = licenceEvent['schedules'];
    this.eventForm.setValue({
      status: licenceEvent.status,
      id: licenceEvent.id,
      name: licenceEvent.name,
      licenceId: licenceEvent.licenceId,
      accountId: licenceEvent.accountId,
      contactName: licenceEvent.contactName,
      contactPhone: licenceEvent.contactPhone,
      contactEmail: licenceEvent.contactEmail,
      sepLicenceNumber: licenceEvent.sepLicenceNumber,
      sepLicensee: licenceEvent.sepLicensee,
      sepContactName: licenceEvent.sepContactName,
      sepContactPhoneNumber: licenceEvent.sepContactPhoneNumber,
      street1: licenceEvent.street1,
      street2: licenceEvent.street2,
      city: licenceEvent.city,
      province: licenceEvent.province,
      postalCode: licenceEvent.postalCode,
      startDate: new Date(licenceEvent.startDate),
      endDate: new Date(licenceEvent.endDate),
      agreement: false
    });
    
    if (this.isReadOnly) {
      this.eventForm.disable();
    }

    if (schedules.length > 0) {
      this.setTimeFormsToLicenceEventSchedule(schedules);
    }
  }

  setTimeFormsToLicenceEventSchedule(schedules: []) {
    schedules.forEach(sched => {
      const startDate = (new Date(sched['eventStartDateTime']));
      const endDate = (new Date(sched['eventEndDateTime']));
      const liquorStart = (new Date(sched['serviceStartDateTime']));
      const liquorEnd = (new Date(sched['serviceEndDateTime']));

      const isDefault = ((endDate.getTime() - startDate.getTime()) / (1000 * 3600 * 24)) > 1;
      if (!isDefault) {
        this.scheduleIsInconsistent = true;
      }
      const timeForm = this.fb.group({
        dateTitle: [isDefault ? null : DAYS[startDate.getDay()] + ', ' + startDate.toLocaleDateString('en-US'), []],
        date: [isDefault ? null : startDate, []],
        startTime: [{hour: startDate.getHours(), minute: startDate.getMinutes()}, [Validators.required]],
        endTime: [{hour: endDate.getHours(), minute: endDate.getMinutes()}, [Validators.required]],
        liquorStartTime: [{hour: liquorStart.getHours(), minute: liquorStart.getMinutes()}, [Validators.required]],
        liquorEndTime: [{hour: liquorEnd.getHours(), minute: liquorEnd.getMinutes()}, [Validators.required]]
      });

      if (this.isReadOnly) {
        timeForm.disable();
      }

      this.timeForms.push(timeForm);
    });
  }

  save(submit = false) {
    if (submit) {
      this.eventForm.controls['status'].setValue(this.getOptionFromLabel(this.eventStatus, 'Submitted').value);
    }

    const schedules = this.packageUpTimeForms();
    if (this.isEditMode) {
      this.updateLicence(schedules);
    } else {
      this.createLicence(schedules);
    }
  }

  /* Packages up time forms and combines with dates for submission to API */
  packageUpTimeForms() {
    let dateArray = new Array();

    for (var i = 0; i < this.timeForms.controls.length; i++) {
      if (this.timeForms.controls[i].invalid) {
        return new Array();
      }
      let eventBegin, eventEnd, serviceBegin, serviceEnd;

      if (this.timeForms.controls[i]['controls']['dateTitle'].value === null) {
        const beginDate = this.eventForm.controls['startDate'].value;
        const endDate = this.eventForm.controls['endDate'].value;

        eventBegin = new Date(beginDate);
        eventEnd = new Date(endDate);
        serviceBegin = new Date(beginDate);
        serviceEnd = new Date(endDate);
      } else {
        const date = this.timeForms.controls[i]['controls']['date'].value;

        eventBegin = new Date(date);
        eventEnd = new Date(date);
        serviceBegin = new Date(date);
        serviceEnd = new Date(date);
      }

      eventBegin.setHours(this.timeForms.controls[i]['controls']['startTime'].value['hour']);
      eventBegin.setMinutes(this.timeForms.controls[i]['controls']['startTime'].value['minute']);
      eventEnd.setHours(this.timeForms.controls[i]['controls']['endTime'].value['hour']);
      eventEnd.setMinutes(this.timeForms.controls[i]['controls']['endTime'].value['minute']);
      serviceBegin.setHours(this.timeForms.controls[i]['controls']['liquorStartTime'].value['hour']);
      serviceBegin.setMinutes(this.timeForms.controls[i]['controls']['liquorStartTime'].value['minute']);
      serviceEnd.setHours(this.timeForms.controls[i]['controls']['liquorEndTime'].value['hour']);
      serviceEnd.setMinutes(this.timeForms.controls[i]['controls']['liquorEndTime'].value['minute']);

      if ((eventEnd.getTime() - eventBegin.getTime()) < 0) {
        eventEnd.setDate(eventEnd.getDate() + 1);
      }

      if ((serviceEnd.getTime() - serviceBegin.getTime()) < 0) {
        serviceEnd.setDate(serviceEnd.getDate() + 1);
      }

      dateArray.push({
        'eventStartDateTime': eventBegin,
        'eventEndDateTime': eventEnd,
        'serviceStartDateTime': serviceBegin,
        'serviceEndDateTime': serviceEnd
      });
    }

    return dateArray;
  }

  updateLicence(schedules) {
    this.busy = this.licenceEvents.updateLicenceEvent(this.eventForm.get('id').value, {...this.eventForm.value, schedules})
    .subscribe((licenceEvent) => {
      this.router.navigate(['/licences']);
    });
  }

  createLicence(schedules) {
    this.eventForm.removeControl('id');
    this.busy = this.licenceEvents.createLicenceEvent({...this.eventForm.value, schedules: schedules})
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
      dateTitle: [null, []],
      date: [null, []],
      startTime: [DEFAULT_START_TIME, [Validators.required]],
      endTime: [DEFAULT_END_TIME, [Validators.required]],
      liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
      liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
    }));
  }

  resetDateFormsToArray(datesArray) {
    this.timeForms = this.fb.array([]);
    datesArray.forEach(element => {
      this.timeForms.push(this.fb.group({
        dateTitle: [DAYS[element.getDay()] + ', ' + element.toLocaleDateString('en-US'), []],
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

  isFormValid() {
    return this.eventForm.invalid || !this.eventForm.controls['agreement'].value;
  }

  cancel() {
    if (this.isEditMode) {
      const id = this.eventForm.get('id').value;
      const status = this.getOptionFromLabel(this.eventStatus, 'Cancelled').value;
      this.busy = this.licenceEvents.updateLicenceEvent(id, {...this.eventForm.value, status: status, licenceId: this.eventForm.get('licenceId').value})
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
    } else {
      this.router.navigate(['/licences']);
    }
  }
}
