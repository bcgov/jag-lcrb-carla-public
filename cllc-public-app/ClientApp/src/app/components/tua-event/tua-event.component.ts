import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { dateRangeValidator, DAYS, DEFAULT_END_TIME, DEFAULT_START_TIME, getDaysArray } from '@shared/date-fns';
import { EventCategory, EventStatus, LicenceEvent, TuaEventType } from '@models/licence-event.model';
import { AppState } from '@app/app-state/models/app-state';
import { LicenceEventsService } from '@services/licence-events.service';
import { LicenseDataService } from '@services/license-data.service';
import { faSave } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle, faTrash } from '@fortawesome/free-solid-svg-icons';
import { LicenceEventSchedule } from '@models/licence-event-schedule';
import { License } from '@models/license.model';
import { HoursOfService } from '../../models/endorsement.model';

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
  tuaEventType = TuaEventType;

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

  // TUA event form
  timeForms = this.fb.array([]);
  form = this.fb.group({
    id: ['', []],
    status: ['', [Validators.required]],
    licenceId: ['', []],
    accountId: ['', []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, 'Temporary Use Area').value, []],

    eventName: ['', [Validators.required]], // TUA event name
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],

    // TUA locations
    eventLocations: ['', []],

    // date & time
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],

    maxAttendance: ['', [Validators.required, Validators.max(100000)]],
    //minorsAttending: ['', [Validators.required]],
    tuaEventType: ['', [Validators.required]],
    eventTypeDescription: ['', [Validators.required]],

    isClosedToPublic: [false, []],
    isWedding: [false, []],
    isNetworkingParty: [false, []],
    isConcert: [false, []],
    isNoneOfTheAbove: [false, []],
    isBanquet: [false, []],
    isAmplifiedSound: [false, []],
    isDancing: [false, []],
    isReception: [false, []],
    isLiveEntertainment: [false, []],
    isGambling: [false, []],

    contactEmail: ['', [Validators.required]],
    contactEmailConfirmation: ['', [Validators.required]],

    isAgreement1: [false, [Validators.required]],
    isAgreement2: [false, [Validators.required]],
  }, {
    // end date must be later than or equal to start date
    validators: dateRangeValidator('startDate', 'endDate')
  });

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private licenceDataService: LicenseDataService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      const licenceId = params.get('licenceId');
      this.form.controls['licenceId'].setValue(licenceId);
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
      eventName: licenceEvent.eventName,
      contactName: licenceEvent.contactName,
      contactPhone: licenceEvent.contactPhone,
      contactEmail: licenceEvent.contactEmail,
      contactEmailConfirmation: licenceEvent.contactEmail,
      eventLocations: licenceEvent.eventLocations,
      startDate: new Date(licenceEvent.startDate),
      endDate: new Date(licenceEvent.endDate),
      eventCategory: this.getOptionFromLabel(this.eventCategory, 'Temporary Use Area').value,
      maxAttendance: licenceEvent.maxAttendance,
      minorsAttending: licenceEvent.minorsAttending,
      tuaEventType: licenceEvent.tuaEventType,
      eventTypeDescription: licenceEvent.eventTypeDescription,
      isClosedToPublic: licenceEvent.isClosedToPublic,
      isWedding: licenceEvent.isWedding,
      isNetworkingParty: licenceEvent.isNetworkingParty,
      isConcert: licenceEvent.isConcert,
      isNoneOfTheAbove: licenceEvent.isNoneOfTheAbove,
      isBanquet: licenceEvent.isBanquet,
      isAmplifiedSound: licenceEvent.isAmplifiedSound,
      isDancing: licenceEvent.isDancing,
      isReception: licenceEvent.isReception,
      isLiveEntertainment: licenceEvent.isLiveEntertainment,
      isGambling: licenceEvent.isGambling,
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
    this.busy = this.licenceEvents.updateLicenceEvent(this.form.get('id').value, { ...this.form.value, schedules })
      .subscribe((licenceEvent) => {
        this.router.navigate(['/licences']);
      });
  }

  createLicence(schedules: LicenceEventSchedule[]) {
    this.form.removeControl('id');
    this.busy = this.licenceEvents.createLicenceEvent({ ...this.form.value, schedules: schedules })
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
      startDate: 'Please enter the start date',
      endDate: 'Please enter the end date',
      maxAttendance: 'Please enter the maximum attendance (must be a number)',
      //minorsAttending: 'Please indicate if minors are attending',
      tuaEventType: 'Please indicate the type of event',
      eventTypeDescription: 'Please enter a description of the event',
      isClosedToPublic: 'Please indicate if licensed establishment will be closed',
      isWedding: '',
      isNetworkingParty: '',
      isConcert: '',
      isNoneOfTheAbove: '',
      isBanquet: '',
      isAmplifiedSound: '',
      isDancing: '',
      isReception: '',
      isLiveEntertainment: '',
      isGambling: '',
      agreement1: 'Please agree to all terms',
      agreement2: 'Please agree to all terms',
    };
  }

  validateForm(): boolean {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.form, this.validationErrorMap))];

    // ... TODO: add more validation rules here - e.g. date range validation
    var outDoor = false;
    if (this.licence != undefined && this.licence.serviceAreas != undefined && this.licence.serviceAreas.length > 0) {
      outDoor = this.licence.serviceAreas[this.licence.serviceAreas.length - 1].isOutdoor;
    }
    if (this.timeForms.controls.length < 1) {
      this.validationMessages.push('No event dates selected');
    }
    // Validate Capacity
    var locationAttendance = 0;
    for (var i = 0; i < this.form.value.eventLocations.length; i++) {
      locationAttendance += Number(this.form.value.eventLocations[i].attendance);
    }
    var endorsements = this.licence.endorsements.filter(k => k.endorsementName = 'Temporary Use Area Endorsement');
    if (endorsements != null && endorsements != undefined && endorsements.length > 0) {
      var lastEndorsement = endorsements[endorsements.length - 1];

      if (locationAttendance > lastEndorsement.areaCapacity) {
        this.validationMessages.push('Capacity cannot exceed the current licence limits');
      }
      //Validate Timing 
      if (this.timeForms.length > 0) {
        var hoursOfServiceList = lastEndorsement.hoursOfServiceList;
        if (hoursOfServiceList != null && hoursOfServiceList != undefined && hoursOfServiceList.length > 0) {

          //Check if different timings
          if (this.timeForms.length > 1) {
            for (var i = 0; i < this.timeForms.length; i++) {
              var startDate = this.timeForms.value[i].date;
              var dayOfWeek = startDate.getDay();
              var hoursOfService = hoursOfServiceList.filter(k => k.dayOfWeek == dayOfWeek)[0];
              if (hoursOfService == null || hoursOfService == undefined) {
                this.validationMessages.push('Service hours should be within endorsement service hours');
                break;
              } else {
                var liquorStartTimeHour = this.timeForms.value[i].startTime.hour;
                var liquorStartTimeMinute = this.timeForms.value[i].startTime.minute;
                var liquorEndTimeHour = this.timeForms.value[i].endTime.hour;
                var liquorEndTimeMinute = this.timeForms.value[i].endTime.minute;
                if (hoursOfService.endTimeHour < hoursOfService.startTimeHour) {
                  hoursOfService.endTimeHour += 24;
                }
                if (liquorEndTimeHour < liquorStartTimeHour) {
                  liquorEndTimeHour += 24;
                }
                //If event is outdoor, Maximum closing hours is 10 PM
                if (outDoor) {
                  //Check if the approved closing hours is before 10 PM
                  if (hoursOfService.endTimeHour > 22 || (hoursOfService.endTimeHour == 22 && hoursOfService.endTimeMinute > 0)) {
                    hoursOfService.endTimeHour = 22;
                    hoursOfService.endTimeMinute = 0;
                  }
                }
                if (liquorStartTimeHour < hoursOfService.startTimeHour ||
                  (liquorStartTimeHour == hoursOfService.startTimeHour
                    && liquorStartTimeMinute < hoursOfService.startTimeMinute)) {
                  this.validationMessages.push('Service hours should be within endorsement service hours');
                  break;
                }
                if (liquorEndTimeHour > hoursOfService.endTimeHour ||
                  (liquorEndTimeHour == hoursOfService.endTimeHour
                    && liquorEndTimeMinute > hoursOfService.endTimeMinute)) {
                  this.validationMessages.push('Service hours should be within endorsement service hours');
                  break;
                }

              }

            }

          }
          else {
            //Every day at the same time
            var startDate = this.form.value.startDate;
            var dayOfWeek = startDate.getDay();
            var existedHoursOfService = hoursOfServiceList.filter(k => k.dayOfWeek == dayOfWeek)[0];
            var hoursOfService = new HoursOfService();

            hoursOfService.dayOfWeek = existedHoursOfService.dayOfWeek;
            hoursOfService.startTimeHour = existedHoursOfService.startTimeHour;
            hoursOfService.startTimeMinute = existedHoursOfService.startTimeMinute;
            hoursOfService.endTimeHour = existedHoursOfService.endTimeHour;
            hoursOfService.endTimeMinute = existedHoursOfService.endTimeMinute;

            var liquorStartTimeHour = this.timeForms.value[0].startTime.hour;
            var liquorStartTimeMinute = this.timeForms.value[0].startTime.minute;
            var liquorEndTimeHour = this.timeForms.value[0].endTime.hour;
            var liquorEndTimeMinute = this.timeForms.value[0].endTime.minute;
            if (hoursOfService.endTimeHour < hoursOfService.startTimeHour) {
              hoursOfService.endTimeHour += 24;
            }
            if (liquorEndTimeHour < liquorStartTimeHour) {
              liquorEndTimeHour += 24;
            }
            //If event is outdoor, Maximum closing hours is 10 PM
            if (outDoor) {
              //Check if the approved closing hours is before 10 PM
              if (hoursOfService.endTimeHour > 22 || (hoursOfService.endTimeHour == 22 && hoursOfService.endTimeMinute > 0)) {
                hoursOfService.endTimeHour = 22;
                hoursOfService.endTimeMinute = 0;
              }
            }
            if (liquorStartTimeHour < hoursOfService.startTimeHour ||
              (liquorStartTimeHour == hoursOfService.startTimeHour
                && liquorStartTimeMinute < hoursOfService.startTimeMinute)) {
              this.validationMessages.push('Service hours should be within endorsement service hours');
            }
            if (liquorEndTimeHour > hoursOfService.endTimeHour ||
              (liquorEndTimeHour == hoursOfService.endTimeHour
                && liquorEndTimeMinute > hoursOfService.endTimeMinute)) {
              this.validationMessages.push('Service hours should be within endorsement service hours');
            }
          }
        }
      }

    }
    this.markControlsAsTouched(this.form);

    if (this.validationMessages.length > 0) {
      return false; // form is invalid
    }
    return true; // valid form
  }
}
