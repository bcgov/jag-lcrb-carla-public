import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormGroup, FormBuilder, FormArray, Validators, ValidatorFn, AbstractControl, ValidationErrors } from "@angular/forms";
import { Router } from "@angular/router";
import { faMapMarkerAlt, faQuestionCircle } from "@fortawesome/free-solid-svg-icons";
import { SepApplication } from "@models/sep-application.model";
import { IndexedDBService } from "@services/indexed-db.service";
import { CanadaPostalRegex, FormBase } from "@shared/form-base";
import { Account } from "@models/account.model";
import { SepLocation } from "@models/sep-location.model";
import { SepSchedule, TIME_SLOTS } from "@models/sep-schedule.model";
import { SepServiceArea } from "@models/sep-service-area.model";
import { AutoCompleteItem, SpecialEventsDataService } from "@services/special-events-data.service";
import { filter, tap, switchMap, takeUntil, takeWhile, distinct } from "rxjs/operators";
import { MatSnackBar } from "@angular/material/snack-bar";
import { timeMasks } from "ngx-mask";

@Component({
  selector: "app-event",
  templateUrl: "./event.component.html",
  styleUrls: ["./event.component.scss"]
})
export class EventComponent extends FormBase implements OnInit {
  faMapMarkerAlt = faMapMarkerAlt;
  timeSlots = TIME_SLOTS;
  @Input() account: Account;
  _appID: number;
  sepApplication: SepApplication;
  @Output()
  saveComplete = new EventEmitter<SepApplication>();
  faQuestionCircle = faQuestionCircle;
  form: FormGroup;
  showValidationMessages: boolean;
  validationMessages: string[];
  previewCities: AutoCompleteItem[] = [];
  autocompleteCities: AutoCompleteItem[] = [];
  get minDate() {
    return new Date();
  }
  @Input()
  set localId(value: number) {
    this._appID = value;
    // get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
        if (this.form) {
          this.setFormValue(this.sepApplication);
        if (this.disableForm) {
          this.form.disable();
        }
      }
    });
  }

  get cities(): AutoCompleteItem[] {
    return [...this.autocompleteCities, ...this.previewCities];
  }

  get locations(): FormArray {
    return this.form.get("eventLocations") as FormArray;
  }
  sepCityRequestInProgress: boolean;

  constructor(private fb: FormBuilder,
    private router: Router,
    private cd: ChangeDetectorRef,
    private snackBar: MatSnackBar,
    private specialEventsDataService: SpecialEventsDataService,
    private db: IndexedDBService) {
    super();
    specialEventsDataService.getSepCityAutocompleteData(null, true)
      .subscribe(results => {
        this.previewCities = results;
      });
  }

  get disableForm(): boolean {
    if(this.sepApplication){
      return this.sepApplication?.eventStatus && this.sepApplication?.eventStatus !== "Draft";
    }
    return false;
  }

  ngOnInit(): void {
    // create a form for the basic details
    this.form = this.fb.group({
      sepCity: ["", [Validators.required, Validators.minLength(2)]],
      isAnnualEvent: [""],
      maximumNumberOfGuests: [""],
      eventLocations: this.fb.array([]), // the form array for all of the locations and their data structures
    });

    if (this.sepApplication) {
      this.setFormValue(this.sepApplication);

      // Disable the form if it has been submitted
      if (this.disableForm) {
        this.form.disable();
      }
    }

   this.form.get("sepCity").valueChanges
      .pipe(filter(value => value && value.length >= 3),
        tap(_ => {
          this.autocompleteCities = [];
          this.sepCityRequestInProgress = true;
        }),
        switchMap(value => this.specialEventsDataService.getSepCityAutocompleteData(value, false))
        )
        .subscribe(data => {
          this.autocompleteCities = data;
          this.sepCityRequestInProgress = false;

          this.cd.detectChanges();
          if (data && data.length === 0) {
            this.snackBar.open("No match found", "", { duration: 2500, panelClass: ["green-snackbar"] });
          }
        });
  }

  setFormValue(app: SepApplication) {
    // if there's an app
    if (app) {
      // create an empty array of locations or use what's there
      app.eventLocations = app.eventLocations || [];
      // add the form values
      this.form.patchValue(app);
    }

    // clear out the locations form array
    this.locations.clear();

    // if we've got any event locations loaded
    if (app?.eventLocations?.length > 0) {
      //console.log("we have locations");
      app.eventLocations.forEach(loc => {
        //console.log("loading location");
        loc.eventDates = loc.eventDates || [];
        loc.serviceAreas = loc.serviceAreas || [];
        this.addLocation(loc);
      });
    } else {
      // otherwise add a blank one
      //console.log("adding blank location");
      this.addLocation();
    }
  }

  getServiceAreas(locationIndex: number): FormArray {
    let result = this.fb.array([]);
    if (location) {
      result = this.locations.at(locationIndex)
        .get("serviceAreas") as FormArray;
    }
    return result;
  }

  getEventDates(locationIndex: number): FormArray {
    let result = this.fb.array([]);
    //console.log("loading dates")
    if (location) {
      result = this.locations.at(locationIndex)
        .get("eventDates") as FormArray;
    }
    return result;
  }

  // add a location and its minimum required data structures
  addLocation(location: SepLocation = new SepLocation()) {
    const locationForm = this.fb.group({
      id: [null],
      locationName: ["", [Validators.required]],
      locationDescription: ["", [Validators.required]],
      maximumNumberOfGuests: ["", [Validators.required]],
      // numberOfMinors: ['', [Validators.required]],
      eventLocationStreet1: ["", [Validators.required]],
      eventLocationStreet2: [""],
      // eventLocationCity: ['', [Validators.required]],
      // eventLocationProvince: ['', [Validators.required]],
      eventLocationPostalCode: ["", [Validators.required, Validators.pattern(CanadaPostalRegex)]],
      serviceAreas: this.fb.array([]),    // form array of service areas
      eventDates: this.fb.array([]),      // form array of event dates
    });

    // patch the values in
    locationForm.patchValue(location);

    // if there aren't any service areas..
    if (!location.serviceAreas || location.serviceAreas.length === 0) {
      // add one
      location.serviceAreas = [{} as SepServiceArea];
    }
    // then loop through the service areas
    location.serviceAreas.forEach(area => {
      // create the required structure
      const areaForm = this.createServiceArea(area);
      // then add it to the form array
      (locationForm.get("serviceAreas") as FormArray).push(areaForm);
    });

    // if there STILL aren't service areas
    if (!location.serviceAreas || location.serviceAreas.length === 0) {
      // add an empty one for some reason...
      location.serviceAreas = [{} as SepServiceArea];
    }

    // if there aren't event dates
    if (!location.eventDates || location.eventDates.length === 0) {
      // create one
      //console.log(!location.eventDates);
      //console.log(location.eventDates.length === 0);
      //console.log("creating blank event date");
      location.eventDates = [{} as SepSchedule];
    }

    // loop through the event dates
    location.eventDates.forEach(ed => {
      // create the required structure
      const edForm = this.createEventDate(ed);
      // then add it to the form array
      (locationForm.get("eventDates") as FormArray).push(edForm);
    });

    // then add the whole mess to the location array
    this.locations.push(locationForm);
  }


  removeLocation(locationIndex: number) {
    this.locations.removeAt(locationIndex);
  }

  addEventDate(sched: SepSchedule, location: FormGroup) {
    const eventDates = location.get("eventDates") as FormArray;
    const dates = this.createEventDate(sched);

    eventDates.push(dates);
  }

  createEventDate(eventDate: SepSchedule) {
    const eventTimesValidator: ValidatorFn = (fg: FormGroup) => {
      const errorMsg = this.getEventTimeValidationError(fg);
      return errorMsg === null
        ? null
        : { range: errorMsg };
    };

    const datesForm = this.fb.group({
      id: [null],
      eventDate: ["", [Validators.required]],
      eventStartValue: ["9:00 AM", [Validators.required]],
      eventEndValue: ["10:00 PM", [Validators.required]],
      serviceStartValue: ["9:00 AM", [Validators.required]],
      serviceEndValue: ["9:30 PM", [Validators.required]],
      liquorServiceHoursExtensionReason: [""],
      disturbancePreventionMeasuresDetails: [""]
    }, { validators: eventTimesValidator });

    eventDate = Object.assign(new SepSchedule(null), eventDate);
    const val = eventDate.toEventFormValue();

    // Set default to event start date
    if (!val.eventDate) {
      val.eventDate = this?.sepApplication?.eventStartDate;
    }
    datesForm.patchValue(val);
    return datesForm;
  }

  customRequiredCheckboxValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === true) {
        return null;
      } else {
        return { "shouldBeTrue": "But value is false" };
      }
    };
  }

  removeEventDate(eventDateIndex: number, location: FormGroup) {
    const eventDates = location.get("eventDates") as FormArray;
    eventDates.removeAt(eventDateIndex);
  }

  addServiceArea(area: SepServiceArea, location: FormGroup) {
    const areaArray = location.get("serviceAreas") as FormArray;
    const areaFormGroup = this.createServiceArea(area);
    areaArray.push(areaFormGroup);
  }

  createServiceArea(area: SepServiceArea) {
    const areaForm = this.fb.group({
      id: [null],
      eventName: ["", [Validators.required]],
      licencedAreaMaxNumberOfGuests: ["", [Validators.required]],
      minorPresent: ["", [Validators.required]],
      numberOfMinors: [""],
      setting: ["", [Validators.required]],
    });

    areaForm.get("minorPresent").valueChanges
      .pipe(distinct(value => value))
      .subscribe(val => {
        if (val === true) {
          areaForm.get("numberOfMinors").setValidators([Validators.required]);
        } else {
          areaForm.get("numberOfMinors").clearValidators();
          areaForm.get("numberOfMinors").reset();
        }
      });
    areaForm.patchValue(area);
    return areaForm;
  }

  removeServiceArea(serviceAreaIndex: number, location: FormGroup) {
    const serviceAreas = location.get("serviceAreas") as FormArray;
    serviceAreas.removeAt(serviceAreaIndex);
  }

  autocompleteDisplay(item: AutoCompleteItem) {
    return item?.name;
  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.form.updateValueAndValidity();
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    const valid = this.form.valid;
    return valid;
  }

  getEventTimeValidationError(dateForm: FormGroup): string {
    let error: string = null;
    const eventFromItem = TIME_SLOTS.find(time => time.value === dateForm.get("eventStartValue").value);
    const eventFromIndex = TIME_SLOTS.indexOf(eventFromItem);

    const eventToItem = TIME_SLOTS.find(time => time.value === dateForm.get("eventEndValue").value);
    const eventToIndex = TIME_SLOTS.indexOf(eventToItem);

    const serviceFromItem = TIME_SLOTS.find(time => time.value === dateForm.get("serviceStartValue").value);
    const serviceFromIndex = TIME_SLOTS.indexOf(serviceFromItem);

    const serviceToItem = TIME_SLOTS.find(time => time.value === dateForm.get("serviceEndValue").value);
    const serviceToIndex = TIME_SLOTS.indexOf(serviceToItem);

    if (eventFromIndex >= eventToIndex) {
      error = "The event should end after the start time, not before.";
    } else if (serviceFromIndex > serviceFromIndex) {
      error = "The liquor service should end after the start time, not before.";
    } else if (eventToIndex <= serviceToIndex) {
      error = "Liquor service  must end at least 30 minutes prior to the end of the specified event time";
    } else if (eventFromIndex > serviceFromIndex) {
      error = "Liquor service must not start earlier than the event."
    }
    return error;
  }

  getFormValue(): SepApplication {
    const formData = {
      ...this.sepApplication,
      ...this.form.value
    };

    formData?.eventLocations.forEach(location => {
      const dateValues = [];
      location?.eventDates.forEach(sched => {
        dateValues.push(new SepSchedule(sched));
      });
      location.eventDates = dateValues;
    });

    const data = {
      localId: this._appID,
      lastUpdated: new Date(),
      eventStatus: "Draft",
      lastStepCompleted: "event",
      ...formData
    } as SepApplication;
    return data;
  }

  showHoursAlert(eventDate: FormGroup, location: FormGroup) {
    const serviceAreas = location.get("serviceAreas").value as SepServiceArea[];
    const outdoorAreaExists = !!serviceAreas.find(area => area.setting === "Outdoors" || area.setting === "BothOutdoorsAndIndoors");
    const indoorAreaExists = !!serviceAreas.find(area => area.setting === "Indoors" || area.setting === "BothOutdoorsAndIndoors");

    const serviceEndTime = eventDate.get("serviceEndValue").value;
    const serviceEndTimeIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.value === serviceEndTime));

    let show = false;
    if (indoorAreaExists) {
      const indoorLimitIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.value === "2:00 AM"));
      show = serviceEndTimeIndex > indoorLimitIndex;
    } else if (outdoorAreaExists) {
      const outdoorLimitIndex = TIME_SLOTS.indexOf(TIME_SLOTS.find(slot => slot.value === "10:00 PM"));
      show = serviceEndTimeIndex > outdoorLimitIndex;
    }
    return show;
  }

  indoorAreaExists(eventDate: FormGroup, location: FormGroup) {
    const serviceAreas = location.get("serviceAreas").value as SepServiceArea[];
    const indoorAreaExists = !!serviceAreas.find(area => area.setting === "Indoors" || area.setting === "BothOutdoorsAndIndoors");
    return indoorAreaExists;
  }

  outdoorAreaExists(eventDate: FormGroup, location: FormGroup) {
    const serviceAreas = location.get("serviceAreas").value as SepServiceArea[];
    const outdoorAreaExists = !!serviceAreas.find(area => area.setting === "Outdoors" || area.setting === "BothOutdoorsAndIndoors");
    return outdoorAreaExists;
  }

  next() {
    this.showValidationMessages = false;
    if (this.isValid() && this.form.get('sepCity')?.value?.id) {
      this.saveComplete.emit(this.getFormValue());
      //console.log("saving...")
    } else {
      //console.log('showing validation messages')
      this.showValidationMessages = true;
    }
  }

}
