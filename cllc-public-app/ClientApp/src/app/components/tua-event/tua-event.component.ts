import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { dateRangeValidator, DAYS, DEFAULT_END_TIME, DEFAULT_START_TIME, getDaysArray } from '@shared/date-fns';
import { EventCategory, EventStatus, LicenceEvent } from '@models/licence-event.model';
import { AppState } from '@app/app-state/models/app-state';
import { LicenceEventsService } from '@services/licence-events.service';
import { faQuestionCircle, faTrash, faSave } from '@fortawesome/free-solid-svg-icons';
import { LicenceEventSchedule } from '@models/licence-event-schedule';

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
  eventStatus = EventStatus;

  // component state
  busy: Subscription;
  licenceEvent: LicenceEvent;
  isEditMode = false;
  isReadOnly = false;
  showErrorSection = false;
  validationMessages: any[];
  startDateMinimum = new Date();
  endDateMinimum = new Date(this.startDateMinimum.valueOf());
  scheduleIsInconsistent = false;

  // TUA event form
  timeForms = this.fb.array([]);
  form = this.fb.group({
    id: ['', []],
    status: ['', [Validators.required]],
    licenceId: ['', []],
    accountId: ['', []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, 'Temporary Use Area').value, []],

    name: ['', [Validators.required]], // TUA event name
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],

    // TODO: TUA locations

    // date & time
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],

    //... TODO:

    tuaEventType: ['', [Validators.required]],
    minorsAttending: ['', [Validators.required]],

    agreement: [false, [Validators.required]],
  }, {
    // end date must be later than or equal to start date
    validators: dateRangeValidator('startDate', 'endDate')
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
      this.form.controls['licenceId'].setValue(params.get('licenceId'));
      if (params.get('eventId')) {
        this.isEditMode = true;
        this.retrieveSavedEvent(params.get('eventId'));
      } else {
        this.resetTimeFormsToDefault();
        this.form.get('status').setValue(this.getOptionFromLabel(this.eventStatus, 'Draft').value);
      }
    });
  }

  ngOnInit() {
  }

  get status(): string {
    const statusObj = this.getOptionFromValue(this.eventStatus, this.form?.get('status')?.value);
    return statusObj == null ? '' : statusObj.label;
  }

  retrieveSavedEvent(eventId: string) {
    this.busy = this.licenceEvents.getLicenceEvent(eventId)
      .subscribe((licenceEvent) => {
        this.licenceEvent = licenceEvent;
        this.setFormToLicenceEvent(licenceEvent);
      });
  }

  setFormToLicenceEvent(licenceEvent: LicenceEvent) {
    if (
      licenceEvent.status === this.getOptionFromLabel(this.eventStatus, 'Approved').value ||
      licenceEvent.status === this.getOptionFromLabel(this.eventStatus, 'Cancelled').value ||
      licenceEvent.status === this.getOptionFromLabel(this.eventStatus, 'Denied').value
    ) {
      this.isReadOnly = true;
    }

    this.form.setValue({
      status: licenceEvent.status,
      id: licenceEvent.id,
      name: licenceEvent.name,
      licenceId: licenceEvent.licenceId,
      accountId: licenceEvent.accountId,
      contactName: licenceEvent.contactName,
      contactPhone: licenceEvent.contactPhone,
      contactEmail: licenceEvent.contactEmail,

      // ... TODO:

      startDate: new Date(licenceEvent.startDate),
      endDate: new Date(licenceEvent.endDate),
      eventCategory: this.getOptionFromLabel(this.eventCategory, 'Temporary Use Area').value,
      agreement: false
    });

    const schedules = licenceEvent.schedules;
    if (schedules.length > 0) {
      this.setTimeFormsToLicenceEventSchedule(schedules);
    }

    if (this.isReadOnly) {
      this.form.disable();
    }
  }

  setTimeFormsToLicenceEventSchedule(schedules: LicenceEventSchedule[]) {
    for (const sched of schedules) {
      const startDate = new Date(sched.eventStartDateTime);
      const endDate = new Date(sched.eventEndDateTime);
      const liquorStart = new Date(sched.serviceStartDateTime);
      const liquorEnd = new Date(sched.serviceEndDateTime);

      // TODO: fix this logic - or improve it
      const isDefault = ((endDate.getTime() - startDate.getTime()) / (1000 * 3600 * 24)) > 1;
      if (!isDefault) {
        this.scheduleIsInconsistent = true;
      }
      const timeForm = this.fb.group({
        dateTitle: [isDefault ? null : DAYS[startDate.getDay()] + ', ' + startDate.toLocaleDateString('en-US'), []],
        date: [isDefault ? null : startDate, []],
        startTime: [{ hour: startDate.getHours(), minute: startDate.getMinutes() }, [Validators.required]],
        endTime: [{ hour: endDate.getHours(), minute: endDate.getMinutes() }, [Validators.required]],
        liquorStartTime: [{ hour: liquorStart.getHours(), minute: liquorStart.getMinutes() }, [Validators.required]],
        liquorEndTime: [{ hour: liquorEnd.getHours(), minute: liquorEnd.getMinutes() }, [Validators.required]]
      });

      if (this.isReadOnly) {
        timeForm.disable();
      }

      this.timeForms.push(timeForm);
    }
  }

  toggleScheduleConsistency() {
    this.scheduleIsInconsistent = !this.scheduleIsInconsistent;
    this.refreshTimeDays();
  }

  startDateChanged() {
    this.refreshTimeDays();
  }

  endDateChanged() {
    this.refreshTimeDays();
  }

  refreshTimeDays() {
    if (this.scheduleIsInconsistent) {
      const days = getDaysArray(this.form.get('startDate').value, this.form.get('endDate').value);
      this.resetTimeFormsToArray(days);
    } else {
      this.resetTimeFormsToDefault();
    }
  }

  resetTimeFormsToDefault() {
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

  resetTimeFormsToArray(datesArray: Date[]) {
    this.timeForms = this.fb.array([]);
    for (const dt of datesArray) {
      this.timeForms.push(this.fb.group({
        dateTitle: [DAYS[dt.getDay()] + ', ' + dt.toLocaleDateString('en-US'), []],
        date: [dt, []],
        startTime: [DEFAULT_START_TIME, [Validators.required]],
        endTime: [DEFAULT_END_TIME, [Validators.required]],
        liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
        liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
      }));
    }
  }

  get validationErrorMap() {
    return {
      contactName: 'Please enter the contact name',
      contactPhone: 'Please enter the contact phone number',
      contactEmail: 'Please enter the contact email address',
      startDate: 'Please enter the start date',
      endDate: 'Please enter the end date',
      agreement: 'Please agree to all terms'
    };
  }

  validateForm() {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.form, this.validationErrorMap))];

    // ... TODO: add more validation rules here - e.g. date range validation

    if (this.timeForms.controls.length < 1) {
      this.validationMessages.push('No event dates selected');
    }

    if (this.validationMessages.length > 0) {
      this.showErrorSection = true;
    } else {
      this.showErrorSection = false;
    }
    this.markControlsAsTouched(this.form);
  }

  cancel() {
    if (this.isEditMode) {
      const id = this.form.get('id').value;
      const licenceId = this.form.get('licenceId').value;
      const status = this.getOptionFromLabel(this.eventStatus, 'Cancelled').value;
      const payload: LicenceEvent = { ...this.form.value, status, licenceId };
      this.busy = this.licenceEvents.updateLicenceEvent(id, payload)
        .subscribe((licenceEvent) => {
          this.router.navigate(['/licences']);
        });
    } else {
      this.router.navigate(['/licences']);
    }
  }

}
