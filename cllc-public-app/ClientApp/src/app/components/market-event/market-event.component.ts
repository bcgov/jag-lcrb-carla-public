import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators } from '@angular/forms';
import { LicenceEvent, EventStatus, MarketDuration, SpecificLocation, EventCategory, MarketEventType } from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';
import { getMonthlyWeekday } from '../../shared/date-fns';

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
  marketEventType = MarketEventType;
  marketDuration = MarketDuration;
  specificLocation = SpecificLocation;
  eventCategory = EventCategory;
  startDateMinimum: Date;
  endDateMinimum: Date;
  endDateMaximum: Date;
  daysChecked = 0;
  daysCheckedLimit = 3;
  scheduleIsInconsistent = false;
  showErrorSection = false;
  validationMessages: any[];
  monthlyDaySelected: string = null;

  timeForms = this.fb.array([]);
  eventForm = this.fb.group({
    status: ['', [Validators.required]],
    id: ['', []],
    isNoPreventingSaleofLiquor: ['', [Validators.required]],
    isMarketManagedorCarried: ['', [Validators.required]],
    isMarketOnlyVendors: ['', [Validators.required]],
    isNoImportedGoods: ['', [Validators.required]],
    isMarketHostsSixVendors: ['', [Validators.required]],
    isMarketMaxAmountorDuration: ['', [Validators.required]],
    isAllStaffServingitRight: ['', [Validators.required]],
    isSampleSizeCompliant: ['', [Validators.required]],
    name: ['', []],
    licenceId: ['', []],
    accountId: ['', []],
    contactName: ['', [Validators.required]],
    contactPhone: ['', [Validators.required]],
    contactEmail: ['', [Validators.required]],
    marketEventType: ['', [Validators.required]],
    eventTypeDescription: ['', []],
    mktOrganizerContactName: ['', []],
    mktOrganizerContactPhone: ['', []],
    businessNumber: ['', []],
    registrationNumber: ['', []],
    marketName: ['', [Validators.required]],
    marketWebsite: ['', []],
    marketDuration: ['', [Validators.required]],
    clientHostname: ['', [Validators.required]],
    venueDescription: ['', []],
    specificLocation: ['', []],
    additionalLocationInformation: ['', []],
    street1: ['', []],
    street2: ['', []],
    city: ['', [Validators.required]],
    province: ['BC', [Validators.required]],
    postalCode: ['', [Validators.required]],
    startDate: ['', [Validators.required]],
    endDate: ['', [Validators.required]],
    agreement: ['', [Validators.required]],
    sunday: [false, []],
    monday: [false, []],
    tuesday: [false, []],
    wednesday: [false, []],
    thursday: [false, []],
    friday: [false, []],
    saturday: [false, []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, 'Market').value, []],
    weekOfMonth: ['', []]
  },);

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
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
    
  }

  get isWeekly() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get('marketDuration').value).label === 'Weekly';
  }
  get isBiWeekly() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get('marketDuration').value).label === 'Bi-Weekly';
  }
  get isMonthly() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get('marketDuration').value).label === 'Monthly';
  }
  get isOnce() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get('marketDuration').value).label === 'Once';
  }

  get selectedDaysOfWeekArray() {
    var arr = [];
    if (this.eventForm.get('sunday').value) {
      arr.push('sunday');
    }
    if (this.eventForm.get('monday').value) {
      arr.push('monday');
    }
    if (this.eventForm.get('tuesday').value) {
      arr.push('tuesday');
    }
    if (this.eventForm.get('wednesday').value) {
      arr.push('wednesday');
    }
    if (this.eventForm.get('thursday').value) {
      arr.push('thursday');
    }
    if (this.eventForm.get('friday').value) {
      arr.push('friday');
    }
    if (this.eventForm.get('saturday').value) {
      arr.push('saturday');
    }
    return arr;
  }

  get selectedWeekOfMonth() {
    return this.eventForm.get('weekOfMonth').value;
  }

  retrieveSavedEvent(eventId: string) {
    this.busy = this.licenceEvents.getLicenceEvent(eventId)
    .subscribe((licenceEvent) => {
      this.licenceEvent = licenceEvent;
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
      street1: licenceEvent.street1,
      street2: licenceEvent.street2,
      city: licenceEvent.city,
      province: licenceEvent.province,
      postalCode: licenceEvent.postalCode,
      startDate: new Date(licenceEvent.startDate),
      endDate: new Date(licenceEvent.endDate),
      agreement: false,
      eventCategory: licenceEvent.eventCategory,
      isNoPreventingSaleofLiquor: licenceEvent.isNoPreventingSaleofLiquor,
      isMarketManagedorCarried: licenceEvent.isMarketManagedorCarried,
      isMarketOnlyVendors: licenceEvent.isMarketOnlyVendors,
      isNoImportedGoods: licenceEvent.isNoImportedGoods,
      isMarketHostsSixVendors: licenceEvent.isMarketHostsSixVendors,
      isMarketMaxAmountorDuration: licenceEvent.isMarketMaxAmountorDuration,
      isAllStaffServingitRight: licenceEvent.isAllStaffServingitRight,
      isSampleSizeCompliant: licenceEvent.isSampleSizeCompliant,
      venueDescription: licenceEvent.venueDescription,
      specificLocation: licenceEvent.specificLocation,
      additionalLocationInformation: licenceEvent.additionalLocationInformation,
      marketName: licenceEvent.marketName,
      marketWebsite: licenceEvent.marketWebsite,
      marketDuration: licenceEvent.marketDuration,
      marketEventType: licenceEvent.marketEventType,
      eventTypeDescription: licenceEvent.eventTypeDescription,
      mktOrganizerContactName: licenceEvent.mktOrganizerContactName,
      mktOrganizerContactPhone: licenceEvent.mktOrganizerContactPhone,
      businessNumber: licenceEvent.businessNumber,
      registrationNumber: licenceEvent.registrationNumber,
      clientHostname: licenceEvent.clientHostname,
      sunday: false,
      monday: false,
      tuesday: false,
      wednesday: false,
      thursday: false,
      friday: false,
      saturday: false
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
    this.formIsValid()
    if (this.showErrorSection) {
      return;
    }
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
    const days = this.getDaysArray(this.eventForm.controls['startDate'].value, this.eventForm.controls['endDate'].value);
    this.resetDateFormsToArray(days);
  }

  startDateChanged() {
    this.refreshTimeDays();
  }
  endDateChanged() {
    this.refreshTimeDays();
  }

  resetDateFormsToDefault() {
    this.timeForms = this.fb.array([]);
    // this.timeForms.push(this.fb.group({
    //   dateTitle: [null, []],
    //   date: [null, []],
    //   startTime: [DEFAULT_START_TIME, [Validators.required]],
    //   endTime: [DEFAULT_END_TIME, [Validators.required]],
    //   liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
    //   liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
    // }));
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

  formIsValid() {
    this.validationMessages = [...new Set(this.listControlsWithErrors(this.eventForm, this.getValidationErrorMap()))];
    if (this.eventForm.get('registrationNumber').value == '' && this.eventForm.get('businessNumber').value == '') {
      this.validationMessages.push(`Please enter either the 'Market Business Number' or the 'Incorporation/Registration Number'`);
    }
    if (this.timeForms.controls.length < 1) {
      this.validationMessages.push('No market dates selected')
    }
    
    if (this.validationMessages.length > 0) {
      this.showErrorSection = true;
    } else {
      this.showErrorSection = false;
    }
    this.markControlsAsTouched(this.eventForm);
  }

  getValidationErrorMap() {
    let errorMap = {
      contactName: 'Please enter the contact name',
      contactPhone: 'Please enter the contact phone number',
      contactEmail: 'Please enter the contact email address',
      marketEventType: 'Please enter the market type',
      marketName: 'Please enter the market name',
      marketDuration: 'Please enter the market frquency',
      clientHostname: 'Please enter the market business legal name',
      city: 'Please enter the city',
      postalCode: 'Please enter the postal code',
      startDate: 'Please enter the start date',
      endDate: 'Please enter the end date',
      isNoPreventingSaleofLiquor: 'Please agree to all terms',
      isMarketManagedorCarried: 'Please agree to all terms',
      isMarketOnlyVendors: 'Please agree to all terms',
      isNoImportedGoods: 'Please agree to all terms',
      isMarketHostsSixVendors: 'Please agree to all terms',
      isMarketMaxAmountorDuration: 'Please agree to all terms',
      isAllStaffServingitRight: 'Please agree to all terms',
      isSampleSizeCompliant: 'Please agree to all terms',
      agreement: 'Please agree to all terms'
    };
    return errorMap;
  }

  isOnSelectedDayOfWeek(d: Date): boolean {
    if (this.eventForm.get('sunday').value && d.getDay() == 0)
        return true;
    if (this.eventForm.get('monday').value && d.getDay() == 1)
      return true;
    if (this.eventForm.get('tuesday').value && d.getDay() == 2)
      return true;
    if (this.eventForm.get('wednesday').value && d.getDay() == 3)
      return true;
    if (this.eventForm.get('thursday').value && d.getDay() == 4)
      return true;
    if (this.eventForm.get('friday').value && d.getDay() == 5)
      return true;
    if (this.eventForm.get('saturday').value && d.getDay() == 6)
      return true;
    return false;
  }

  fallsOnSelectedMonthDay(d: Date) {
    var retVal = false;
    if (!this.eventForm.get('weekOfMonth').value) {
      return retVal;
    }
    this.selectedDaysOfWeekArray.forEach(element => {
      const date = getMonthlyWeekday(this.selectedWeekOfMonth, element, d.toLocaleString('default', { month: 'long'}), d.getFullYear());
      
      if(date == d.getDate()) {
        retVal = true;
      }
    });
    return retVal;
  }

  getDaysArray(start, end) {
    let dayNum = 0;
    start = new Date(start);
    end = new Date(end);
    for(var arr = [], dt = start; dt <= end; dt.setDate(dt.getDate() + 1)) {
      const d = new Date(dt);
      if((this.isWeekly || this.isBiWeekly)) {
        if (this.isOnSelectedDayOfWeek(d) && ((this.isBiWeekly && dayNum % 14 < 7) || !this.isBiWeekly)) {
          arr.push(d);
        }
      } else if (this.isMonthly) {
        if(this.fallsOnSelectedMonthDay(d))
          arr.push(d);
      } else {
        arr.push(d);
      }
      dayNum++;
    }
    return arr;
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

  weekDateChanged(day: string) {
    if (this.isMonthly) {
      this.monthlyDaySelected = day;
    } else {
      this.monthlyDaySelected = null;
    }
    if (this.eventForm.get(day).value) {
      this.daysChecked++;
    } else {
      this.daysChecked--;
    }
    this.refreshTimeDays();
  }

  frequencyChanged() {
    if (this.isMonthly) {
      this.daysCheckedLimit = 1;
      this.clearWeeklyChecks();
    } else if (this.isWeekly || this.isBiWeekly) {
      this.daysCheckedLimit = 3;
    }
    this.refreshTimeDays();
  }

  weekOfMonthChanged() {
    this.refreshTimeDays();
  }

  clearWeeklyChecks() {
    this.daysChecked = 0;
    this.eventForm.get('sunday').setValue(false);
    this.eventForm.get('monday').setValue(false);
    this.eventForm.get('tuesday').setValue(false);
    this.eventForm.get('wednesday').setValue(false);
    this.eventForm.get('thursday').setValue(false);
    this.eventForm.get('friday').setValue(false);
  }

  capitalize(val: string) {
    if (typeof val !== 'string') return ''
    return val.charAt(0).toUpperCase() + val.slice(1)
  }
}
