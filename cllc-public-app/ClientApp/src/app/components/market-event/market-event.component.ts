import { Component, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { FormBuilder, Validators } from "@angular/forms";
import {
  LicenceEvent,
  EventStatus,
  MarketDuration,
  SpecificLocation,
  EventCategory,
  MarketEventType
} from '../../models/licence-event.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';
import { DAYS, DEFAULT_END_TIME, DEFAULT_START_TIME, getMonthlyWeekday } from '../../shared/date-fns';
import { getWeekOfMonth } from 'date-fns';
import { faQuestionCircle, faTrash } from '@fortawesome/free-solid-svg-icons';
import { faSave } from '@fortawesome/free-regular-svg-icons';
import { LicenceEventSchedule } from '@models/licence-event-schedule';


@Component({
  selector: "app-market-event",
  templateUrl: "./market-event.component.html",
  styleUrls: ["./market-event.component.scss"],
})
export class MarketEventComponent extends FormBase implements OnInit {
  faSave = faSave;
  faTrash = faTrash;
  faQuestionCircle = faQuestionCircle;
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
  defaultTimeForm = this.fb.group({
    startTime: [DEFAULT_START_TIME, [Validators.required]],
    endTime: [DEFAULT_END_TIME, [Validators.required]],
    liquorStartTime: [DEFAULT_START_TIME, [Validators.required]],
    liquorEndTime: [DEFAULT_END_TIME, [Validators.required]]
  });
  eventForm = this.fb.group({
    status: ["", [Validators.required]],
    id: ["", []],
    isNoPreventingSaleofLiquor: ["", [Validators.required]],
    isMarketManagedorCarried: ["", [Validators.required]],
    isMarketOnlyVendors: ["", [Validators.required]],
    isMarketHostsSixVendors: ["", [Validators.required]],
    isMarketMaxAmountorDuration: ["", [Validators.required]],
    isAllStaffServingitRight: ["", [Validators.required]],
    isSampleSizeCompliant: ["", [Validators.required]],
    name: ["", []],
    licenceId: ["", []],
    accountId: ["", []],
    contactName: ["", [Validators.required]],
    contactPhone: ["", [Validators.required]],
    contactEmail: ["", [Validators.required]],
    marketEventType: ["", [Validators.required]],
    eventTypeDescription: ["", []],
    mktOrganizerContactName: ["", []],
    mktOrganizerContactPhone: ["", []],
    businessNumber: ["", []],
    registrationNumber: ["", []],
    marketName: ["", [Validators.required]],
    marketWebsite: ["", []],
    marketDuration: ["", [Validators.required]],
    clientHostname: ["", [Validators.required]],
    venueDescription: ["", []],
    specificLocation: ["", []],
    additionalLocationInformation: ["", []],
    street1: ["", [Validators.required]],
    street2: ["", []],
    city: ["", [Validators.required]],
    province: ["BC", [Validators.required]],
    postalCode: ["", [Validators.required]],
    startDate: ["", [Validators.required]],
    endDate: ["", [Validators.required]],
    agreement: ["", [Validators.required]],
    sunday: [false, []],
    monday: [false, []],
    tuesday: [false, []],
    wednesday: [false, []],
    thursday: [false, []],
    friday: [false, []],
    saturday: [false, []],
    eventCategory: [this.getOptionFromLabel(this.eventCategory, "Market").value, []],
    weekOfMonth: ["", []]
  });

  constructor(
    private fb: FormBuilder,
    private licenceEvents: LicenceEventsService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      this.eventForm.controls["licenceId"].setValue(params.get("licenceId"));
      if (params.get("eventId")) {
        this.isEditMode = true;
        this.retrieveSavedEvent(params.get("eventId"));
      } else {
        this.resetDateFormsToDefault();
        this.eventForm.controls["status"].setValue(this.getOptionFromLabel(this.eventStatus, "Draft").value);
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
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get("marketDuration").value).label === "Weekly";
  }

  get isBiWeekly() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get("marketDuration").value).label ===
      "Bi-Weekly";
  }

  get isMonthly() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get("marketDuration").value).label === "Monthly";
  }

  get isOnce() {
    return this.getOptionFromValue(this.marketDuration, this.eventForm.get("marketDuration").value).label === "Once";
  }

  get selectedDaysOfWeekArray() {
    const arr = [];
    if (this.eventForm.get("sunday").value) {
      arr.push("sunday");
    }
    if (this.eventForm.get("monday").value) {
      arr.push("monday");
    }
    if (this.eventForm.get("tuesday").value) {
      arr.push("tuesday");
    }
    if (this.eventForm.get("wednesday").value) {
      arr.push("wednesday");
    }
    if (this.eventForm.get("thursday").value) {
      arr.push("thursday");
    }
    if (this.eventForm.get("friday").value) {
      arr.push("friday");
    }
    if (this.eventForm.get("saturday").value) {
      arr.push("saturday");
    }
    return arr;
  }

  get selectedWeekOfMonth() {
    return this.eventForm.get("weekOfMonth").value;
  }

  retrieveSavedEvent(eventId: string) {
    this.busy = this.licenceEvents.getLicenceEvent(eventId)
      .subscribe((licenceEvent) => {
        this.licenceEvent = licenceEvent;
        this.setFormToLicenceEvent(licenceEvent);
      });
  }

  setFormToLicenceEvent(licenceEvent: LicenceEvent) {
    if (licenceEvent.status === this.getOptionFromLabel(this.eventStatus, "Approved").value) {
      this.isReadOnly = true;
    }

    const schedules = licenceEvent["schedules"];
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
      saturday: false,
      weekOfMonth: ""
    });

    if (this.isReadOnly) {
      this.eventForm.patchValue({ agreement: true }); // read-only forms have already been affirmed by the user
      this.frequencyChanged(false); // set the appropriate limit for days checked (1 for monthly, 3 for weekly/bi-weekly)
      this.eventForm.disable();
      this.defaultTimeForm.disable();
    }

    if (schedules.length > 0) {
      this.setTimeFormsToLicenceEventSchedule(schedules);
    }
  }

  setTimeFormsToLicenceEventSchedule(schedules: LicenceEventSchedule[]) {
    // Restore state of weekday checkboxes and monthly frequency radio
    this.setWeekdaysFromEventSchedule(schedules);
    this.setMarketTimeFromEventSchedule(schedules);
    if (this.isMonthly) {
      this.setWeekOfMonthFromEventSchedule(schedules);
    }

    schedules.forEach(sched => {
      const startDate = (new Date(sched["eventStartDateTime"]));
      const endDate = (new Date(sched["eventEndDateTime"]));
      const liquorStart = (new Date(sched["serviceStartDateTime"]));
      const liquorEnd = (new Date(sched["serviceEndDateTime"]));

      const isDefault = ((endDate.getTime() - startDate.getTime()) / (1000 * 3600 * 24)) > 1;
      if (!isDefault) {
        this.scheduleIsInconsistent = true;
      }

      const timeForm = this.fb.group({
        dateTitle: [isDefault ? null : DAYS[startDate.getDay()] + ", " + startDate.toLocaleDateString("en-US"), []],
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
    });
  }

  /* Restores the state of the week checkboxes when showing the form in read-only mode */
  setWeekdaysFromEventSchedule(schedules: any[] = []) {
    // these match the form properties that drive the week checkboxes
    const weekdays = ["sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday"];
    let state = {};
    for (const sched of schedules) {
      const d = new Date(sched["eventStartDateTime"]);
      const index = d.getDay();
      const day = weekdays[index];
      state = {
        ...state,
        [day]: true,
      };
    }
    // update form values
    this.eventForm.patchValue(state);
    // manually run the on-change logic (as we are updating the form fields directly)
    Object.keys(state).forEach(day => this.weekDateChanged(day, false));
  }

  /* Restores the state of the week of month (1st Monday of the month, etc) when showing the form in read-only mode */
  setWeekOfMonthFromEventSchedule(schedules: any[] = []) {
    const first = schedules.length > 0 ? schedules[0] : null;
    const d = new Date(first["eventStartDateTime"]);
    const week = getWeekOfMonth(d);
    this.eventForm.patchValue({ weekOfMonth: week });
  }

  /* Restores the state of the market time section when showing the form in read-only mode */
  setMarketTimeFromEventSchedule(schedules: any[] = []) {
    if (schedules.length < 1) {
      return;
    }
    const sched = schedules[0];
    const eventBegin = new Date(sched["eventStartDateTime"]);
    const eventEnd = new Date(sched["eventEndDateTime"]);
    const serviceBegin = new Date(sched["serviceStartDateTime"]);
    const serviceEnd = new Date(sched["serviceEndDateTime"]);

    this.defaultTimeForm.patchValue({
      startTime: { hour: eventBegin.getHours(), minute: eventBegin.getMinutes() },
      endTime: { hour: eventEnd.getHours(), minute: eventEnd.getMinutes() },
      liquorStartTime: { hour: serviceBegin.getHours(), minute: serviceBegin.getMinutes() },
      liquorEndTime: { hour: serviceEnd.getHours(), minute: serviceEnd.getMinutes() },
    });
  }

  save(submit = false) {
    this.formIsValid();
    if (this.showErrorSection) {
      return;
    }
    if (submit) {
      this.eventForm.controls["status"].setValue(this.getOptionFromLabel(this.eventStatus, "Submitted").value);
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

    for (let i = 0; i < this.timeForms.controls.length; i++) {
      if (this.timeForms.controls[i].invalid) {
        return new Array();
      }
      const date = this.timeForms.controls[i]["controls"]["date"].value;
      const eventBegin = new Date(date);
      const eventEnd = new Date(date);
      const serviceBegin = new Date(date);
      const serviceEnd = new Date(date);

      eventBegin.setHours(this.defaultTimeForm.get("startTime").value["hour"]);
      eventBegin.setMinutes(this.defaultTimeForm.get("startTime").value["minute"]);
      eventEnd.setHours(this.defaultTimeForm.get("endTime").value["hour"]);
      eventEnd.setMinutes(this.defaultTimeForm.get("endTime").value["minute"]);
      serviceBegin.setHours(this.defaultTimeForm.get("liquorStartTime").value["hour"]);
      serviceBegin.setMinutes(this.defaultTimeForm.get("liquorStartTime").value["minute"]);
      serviceEnd.setHours(this.defaultTimeForm.get("liquorEndTime").value["hour"]);
      serviceEnd.setMinutes(this.defaultTimeForm.get("liquorEndTime").value["minute"]);

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
    this.busy = this.licenceEvents
      .updateLicenceEvent(this.eventForm.get("id").value, { ...this.eventForm.value, schedules })
      .subscribe((licenceEvent) => {
        this.router.navigate(["/licences"]);
      });
  }

  createLicence(schedules) {
    this.eventForm.removeControl("id");
    this.busy = this.licenceEvents.createLicenceEvent({ ...this.eventForm.value, schedules: schedules })
      .subscribe((licenceEvent) => {
        this.router.navigate(["/licences"]);
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
      label: ""
    };
  }

  getOptionFromLabel(options: any, label: string) {
    const idx = options.findIndex(opt => opt.label === label);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ""
    };
  }

  toggleScheduleConsistency() {
    this.scheduleIsInconsistent = !this.scheduleIsInconsistent;
    this.refreshTimeDays();
  }

  refreshTimeDays() {
    const days = this.getDaysArray(this.eventForm.controls["startDate"].value,
      this.eventForm.controls["endDate"].value);
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
  }

  resetDateFormsToArray(datesArray) {
    this.timeForms = this.fb.array([]);
    datesArray.forEach(element => {
      this.timeForms.push(this.fb.group({
        dateTitle: [DAYS[element.getDay()] + ", " + element.toLocaleDateString("en-US"), []],
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
    if (this.eventForm.get("registrationNumber").value === "" && this.eventForm.get("businessNumber").value === "") {
      this.validationMessages.push(
        `Please enter either the 'Market Business Number' or the 'Incorporation/Registration Number'`);
    }
    if (this.timeForms.controls.length < 1) {
      this.validationMessages.push("No market dates selected");
    }

    if (this.validationMessages.length > 0) {
      this.showErrorSection = true;
    } else {
      this.showErrorSection = false;
    }
    this.markControlsAsTouched(this.eventForm);
  }

  getValidationErrorMap() {
    const errorMap = {
      contactName: "Please enter the contact name",
      contactPhone: "Please enter the contact phone number",
      contactEmail: "Please enter the contact email address",
      marketEventType: "Please enter the market type",
      marketName: "Please enter the market name",
      marketDuration: "Please enter the market frequency",
      clientHostname: "Please enter the market business legal name",
      city: "Please enter the city",
      postalCode: "Please enter the postal code",
      startDate: "Please enter the start date",
      endDate: "Please enter the end date",
      isNoPreventingSaleofLiquor: "Please agree to all terms",
      isMarketManagedorCarried: "Please agree to all terms",
      isMarketOnlyVendors: "Please agree to all terms",
      isMarketHostsSixVendors: "Please agree to all terms",
      isMarketMaxAmountorDuration: "Please agree to all terms",
      isAllStaffServingitRight: "Please agree to all terms",
      isSampleSizeCompliant: "Please agree to all terms",
      agreement: "Please agree to all terms"
    };
    return errorMap;
  }

  isOnSelectedDayOfWeek(d: Date): boolean {
    if (this.eventForm.get("sunday").value && d.getDay() === 0) {
      return true;
    }
    if (this.eventForm.get("monday").value && d.getDay() === 1) {
      return true;
    }
    if (this.eventForm.get("tuesday").value && d.getDay() === 2) {
      return true;
    }
    if (this.eventForm.get("wednesday").value && d.getDay() === 3) {
      return true;
    }
    if (this.eventForm.get("thursday").value && d.getDay() === 4) {
      return true;
    }
    if (this.eventForm.get("friday").value && d.getDay() === 5) {
      return true;
    }
    if (this.eventForm.get("saturday").value && d.getDay() === 6) {
      return true;
    }
    return false;
  }

  fallsOnSelectedMonthDay(d: Date) {
    let retVal = false;
    if (!this.selectedWeekOfMonth) {
      return retVal;
    }
    this.selectedDaysOfWeekArray.forEach(element => {
      const date = getMonthlyWeekday(this.selectedWeekOfMonth,
        element,
        d.toLocaleString("default", { month: "long" }),
        d.getFullYear());

      if (date === d.getDate()) {
        retVal = true;
      }
    });
    return retVal;
  }

  getDaysArray(start, end) {
    let dayNum = 0;
    start = new Date(start);
    end = new Date(end);
    for (var arr = [], dt = start; dt <= end; dt.setDate(dt.getDate() + 1)) {
      const d = new Date(dt);
      if ((this.isWeekly || this.isBiWeekly)) {
        if (this.isOnSelectedDayOfWeek(d) && ((this.isBiWeekly && dayNum % 14 < 7) || !this.isBiWeekly)) {
          arr.push(d);
        }
      } else if (this.isMonthly) {
        if (this.fallsOnSelectedMonthDay(d)) {
          arr.push(d);
        }
      } else {
        arr.push(d);
      }
      dayNum++;
    }
    return arr;
  }

  cancel() {
    if (this.isEditMode) {
      const id = this.eventForm.get("id").value;
      const status = this.getOptionFromLabel(this.eventStatus, "Cancelled").value;
      this.busy = this.licenceEvents
        .updateLicenceEvent(id,
          { ...this.eventForm.value, status: status, licenceId: this.eventForm.get("licenceId").value })
        .subscribe((licenceEvent) => {
          this.router.navigate(["/licences"]);
        });
    } else {
      this.router.navigate(["/licences"]);
    }
  }

  weekDateChanged(day: string, resetDateForms = true) {
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
    if (resetDateForms) {
      this.refreshTimeDays();
    }
  }

  frequencyChanged(resetDateForms = true) {
    if (this.isMonthly) {
      this.daysCheckedLimit = 1;
      this.clearWeeklyChecks();
    } else if (this.isWeekly || this.isBiWeekly) {
      this.daysCheckedLimit = 3;
    }
    if (resetDateForms) {
      this.refreshTimeDays();
    }
  }

  weekOfMonthChanged() {
    this.refreshTimeDays();
  }

  clearWeeklyChecks() {
    this.daysChecked = 0;
    this.eventForm.get("sunday").setValue(false);
    this.eventForm.get("monday").setValue(false);
    this.eventForm.get("tuesday").setValue(false);
    this.eventForm.get("wednesday").setValue(false);
    this.eventForm.get("thursday").setValue(false);
    this.eventForm.get("friday").setValue(false);
  }

  capitalize(val: string) {
    if (typeof val !== "string") {
      return "";
    }
    return val.charAt(0).toUpperCase() + val.slice(1);
  }
}
