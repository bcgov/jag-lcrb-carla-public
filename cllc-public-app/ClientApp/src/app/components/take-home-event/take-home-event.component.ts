import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { dateRangeValidator, DAYS, DEFAULT_END_TIME, DEFAULT_START_TIME, getDaysArray } from '@shared/date-fns';
import { EventCategory, EventStatus, EventType, LicenceEvent, SpecificLocation } from '@models/licence-event.model';
import { AppState } from '@app/app-state/models/app-state';
import { LicenceEventsService } from '@services/licence-events.service';
import { LicenseDataService } from '@services/license-data.service';
import { faSave } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle, faTrash } from '@fortawesome/free-solid-svg-icons';
import { LicenceEventSchedule } from '@models/licence-event-schedule';
import { License } from '@models/license.model';

@Component({
  selector: 'app-take-home-event',
  templateUrl: './take-home-event.component.html',
  styleUrls: ['./take-home-event.component.scss']
})
export class TakeHomeEventComponent extends FormBase implements OnInit {
  // icons
  faSave = faSave;
  faTrash = faTrash;
  faQuestionCircle = faQuestionCircle;

  // enums
  eventCategory = EventCategory;
  eventStatus = EventStatus;
  eventType = EventType;
  specificLocation = SpecificLocation;

  // component state
  busy: Subscription;
  licence: License;
  licenceEvent: LicenceEvent;
  isEditMode = false;
  isReadOnly = false;
  showErrorSection = false;
  validationMessages: any[];
  startDateMinimum = new Date();
  endDateMinimum = new Date(this.startDateMinimum.valueOf());
  scheduleIsInconsistent = false;

  // Take-Home Sampling event form
  timeForms = this.fb.array([]);
  form = this.fb.group({
    id: ['', []],
    status: ['', [Validators.required]],
    licenceId: ['', []],
    accountId: ['', []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, 'Take Home Sampling').value, []],

    // event details
    eventName: ['', [Validators.required]],
    eventTypeDescription: ['', []],
    eventType: ['', [Validators.required]],
    clientHostname: ['', [Validators.required]],
    venueDescription: ['', [Validators.required]],
    specificLocation: ['', [Validators.required]],
    additionalLocationInformation: ['', []],
    street1: ['', [Validators.required]],
    street2: ['', []],
    city: ['', [Validators.required]],
    province: ["BC", [Validators.required]],
    postalCode: ['', [Validators.required]],

    // contact information
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],
    contactEmailConfirmation: ['', [Validators.required]],

    // date & time
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],

    isAgreement1: [false, [Validators.requiredTrue]],
    isAgreement2: [false, [Validators.requiredTrue]],
  }, {
    // end date must be later than or equal to start date
    validators: dateRangeValidator('startDate', 'endDate')
  });

  constructor(private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      const licenceId = params.get('licenceId');
      this.form.get('licenceId').setValue(licenceId);
      this.retrieveLicence(licenceId);

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

  retrieveLicence(licenceId: string) {
    this.busy = this.licenceDataService.getLicenceById(licenceId)
      .subscribe((licence) => {
        this.licence = licence;
      });
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
      licenceId: licenceEvent.licenceId,
      accountId: licenceEvent.accountId,
      // event details
      eventName: licenceEvent.eventName,
      eventTypeDescription: licenceEvent.eventTypeDescription,
      eventCategory: this.getOptionFromLabel(this.eventCategory, 'All Ages Liquor Free').value,
      eventType: licenceEvent.eventType,
      clientHostname: licenceEvent.clientHostname,
      venueDescription: licenceEvent.venueDescription,
      specificLocation: licenceEvent.specificLocation,
      additionalLocationInformation: licenceEvent.additionalLocationInformation,
      street1: licenceEvent.street1,
      street2: licenceEvent.street2,
      city: licenceEvent.city,
      province: licenceEvent.province,
      postalCode: licenceEvent.postalCode,
      // contact information
      contactName: licenceEvent.contactName,
      contactPhone: licenceEvent.contactPhone,
      contactEmail: licenceEvent.contactEmail,
      contactEmailConfirmation: licenceEvent.contactEmail,
      // date & time
      startDate: new Date(licenceEvent.startDate),
      endDate: new Date(licenceEvent.endDate),
      // agreements
      isAgreement1: licenceEvent.isAgreement1,
      isAgreement2: licenceEvent.isAgreement2,
    });

    const schedules = licenceEvent.schedules;
    if (schedules.length > 0) {
      this.setTimeFormsToLicenceEventSchedule(schedules);
    }

    if (this.isReadOnly) {
      this.form.disable();
    } else {
      // Make them re-sign the declaration section whenever changes are made to the form
      this.form.patchValue({
        isAgreement1: true,
        isAgreement2: true,
      });
    }
  }

  setTimeFormsToLicenceEventSchedule(schedules: LicenceEventSchedule[]) {
    for (const sched of schedules) {
      const startDate = new Date(sched.eventStartDateTime);
      const endDate = new Date(sched.eventEndDateTime);
      const liquorStart = new Date(sched.serviceStartDateTime);
      const liquorEnd = new Date(sched.serviceEndDateTime);

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

  cancel() {
    if (this.isEditMode) {
      const id = this.form.get('id').value;
      const licenceId = this.form.get('licenceId').value;
      const statusCancelled = this.getOptionFromLabel(this.eventStatus, 'Cancelled').value;
      const payload: LicenceEvent = { ...this.form.value, status: statusCancelled, licenceId };
      this.busy = this.licenceEvents.updateLicenceEvent(id, payload)
        .subscribe((licenceEvent) => {
          this.router.navigate(['/licences']);
        });
    } else {
      this.router.navigate(['/licences']);
    }
  }

  save(submit = false) {
    const validForm = this.validateForm();
    if (!validForm) {
      this.showErrorSection = true;
      return;
    }

    // Do not show validation errors when form is valid
    this.showErrorSection = false;

    if (submit) {
      const statusSubmitted = this.getOptionFromLabel(this.eventStatus, 'Submitted').value;
      this.form.get('status').setValue(statusSubmitted);
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
    const dateArray = new Array();

    for (var i = 0; i < this.timeForms.controls.length; i++) {
      if (this.timeForms.controls[i].invalid) {
        return new Array();
      }
      let eventBegin, eventEnd, serviceBegin, serviceEnd;

      if (this.timeForms.controls[i]['controls']['dateTitle'].value === null) {
        const beginDate = this.form.controls['startDate'].value;
        const endDate = this.form.controls['endDate'].value;
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

      const sched = new LicenceEventSchedule();
      sched.eventStartDateTime = eventBegin;
      sched.eventEndDateTime = eventEnd;
      sched.serviceStartDateTime = serviceBegin;
      sched.serviceEndDateTime = serviceEnd;
      dateArray.push(sched);
    }

    return dateArray;
  }

  updateLicence(schedules: LicenceEventSchedule[]) {
    this.busy = this.licenceEvents
      .updateLicenceEvent(this.form.get('id').value, { ...this.form.value, schedules })
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
  }

  createLicence(schedules: LicenceEventSchedule[]) {
    this.form.removeControl('id');
    this.busy = this.licenceEvents
      .createLicenceEvent({ ...this.form.value, schedules: schedules })
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
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
      eventName: 'Please enter the event name',
      contactName: 'Please enter the contact name',
      contactPhone: 'Please enter the contact phone number',
      contactEmail: 'Please enter the contact email address',
      contactEmailConfirmation: 'The email address confirmation does not match provided email',
      eventType: 'Please enter the type of event',
      eventTypeDescription: 'Please enter a description of the event',
      clientHostname: 'Please enter the client or host name',
      venueDescription: 'Please enter the name and a description of the venue',
      specificLocation: 'Please enter the location',
      street1: 'Please enter the address line 1',
      city: 'Please enter the city',
      postalCode: 'Please enter the postal code',
      startDate: 'Please enter the start date',
      endDate: 'Please enter the end date',
      isAgreement1: 'Please agree to all terms',
      isAgreement2: 'Please agree to all terms',
    };
  }

  validateForm(): boolean {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.form, this.validationErrorMap))];
    if (this.timeForms.controls.length < 1) {
      this.validationMessages.push('No event dates selected');
    }
    this.markControlsAsTouched(this.form);
    if (this.validationMessages.length > 0) {
      return false; // form is invalid
    }
    return true; // valid form
  }
}
