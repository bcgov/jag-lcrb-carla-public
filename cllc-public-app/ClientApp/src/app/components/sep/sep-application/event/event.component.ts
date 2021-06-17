import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { Router } from '@angular/router';
import { faMapMarkerAlt, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';
import { SepLocation } from '@models/sep-location.model';
import { SepSchedule, TIME_SLOTS } from '@models/sep-schedule.model';
import { SepServiceArea } from '@models/sep-service-area.model';
import { AutoCompleteItem, SpecialEventsDataService } from '@services/special-events-data.service';
import { filter, tap, switchMap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.scss']
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
        }
      });
  }

  get cities(): AutoCompleteItem[] {
    return [...this.autocompleteCities, ...this.previewCities];
  }

  get locations(): FormArray {
    return this.form.get('eventLocations') as FormArray;
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

  ngOnInit(): void {

    // create a form for the basic details
    this.form = this.fb.group({
      sepCity: [''],
      isAnnualEvent: [''],
      maximumNumberOfGuests: [''],
      eventLocations: this.fb.array([]), // the form array for all of the locations and their data structures
    });
    if (this.sepApplication) {
      this.setFormValue(this.sepApplication);
    }

    this.form.get('sepCity').valueChanges
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
          this.snackBar.open('No match found', '', { duration: 2500, panelClass: ['green-snackbar'] });
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
      console.log("we have locations")
      app.eventLocations.forEach(loc => {
        console.log("loading location")
        loc.eventDates = loc.eventDates || [];
        loc.serviceAreas = loc.serviceAreas || [];
        this.addLocation(loc);
      });
    } else {
      // otherwise add a blank one
      console.log("adding blank location")
      this.addLocation();
    }
  }

  getServiceAreas(locationIndex: number): FormArray {
    let result = this.fb.array([]);
    if (location) {
      result = this.locations.at(locationIndex)
        .get('serviceAreas') as FormArray;
    }
    return result;
  }

  getEventDates(locationIndex: number): FormArray {
    let result = this.fb.array([]);
    if (location) {
      result = this.locations.at(locationIndex)
        .get('eventDates') as FormArray;
    }
    return result;
  }

  // add a location and its minimum required data structures
  addLocation(location: SepLocation = new SepLocation()) {
    let locationForm = this.fb.group({
      id: [null],
      locationPermitNumber: [''],
      locationName: [''],
      locationDescription: [''],
      maximumNumberOfGuests: [''],
      numberOfMinors: [''],
      eventLocationStreet1: [''],
      eventLocationStreet2: [''],
      eventLocationCity: [''],
      eventLocationProvince: [''],
      eventLocationPostalCode: [''],
      serviceAreas: this.fb.array([]),    // form array of service areas
      eventDates: this.fb.array([]),      // form array of event dates
    });

    // patch the values in
    locationForm.patchValue(location);

    // if there aren't any service areas..
    if (!location.serviceAreas || location.serviceAreas.length == 0) {
      // add one
      location.serviceAreas = [{} as SepServiceArea];
    }
    // then loop through the service areas
    location.serviceAreas.forEach(area => {
      // create the required structure
      const areaForm = this.createServiceArea(area);
      // then add it to the form array
      (locationForm.get('serviceAreas') as FormArray).push(areaForm);
    });

    // if there STILL aren't service areas
    if (!location.serviceAreas || location.serviceAreas.length == 0) {
      // add an empty one for some reason...
      location.serviceAreas = [{} as SepServiceArea];
    }

    // if there aren't event dates
    if (!location.eventDates || location.eventDates.length == 0) {
      // create one
      console.log(!location.eventDates)
      console.log(location.eventDates.length == 0)
      console.log("creating blank event date")
      location.eventDates = [{} as SepSchedule];
    }

    // loop through the event dates
    location.eventDates.forEach(ed => {
      // create the required structure
      const edForm = this.createEventDate(ed);
      // then add it to the form array
      (locationForm.get('eventDates') as FormArray).push(edForm);
    });

    // then add the whole mess to the location array
    this.locations.push(locationForm);
  }


  removeLocation(locationIndex: number) {
    this.locations.removeAt(locationIndex);
  }

  addEventDate(sched: SepSchedule, location: FormGroup) {
    const eventDates = location.get('eventDates') as FormArray;
    const dates = this.createEventDate(sched);

    eventDates.push(dates);
  }

  createEventDate(eventDate: SepSchedule) {
    const datesForm = this.fb.group({
      id: [null],
      eventDate: [''],
      eventStartValue: [''],
      eventEndValue: [''],
      serviceStartValue: [''],
      serviceEndValue: [''],
    });
    eventDate = Object.assign(new SepSchedule(null), eventDate);
    const val = eventDate.toEventFormValue();

    // Set default to event start date
    if (!val.eventDate) {
      val.eventDate = this?.sepApplication?.eventStartDate;
    }
    datesForm.patchValue(val);
    return datesForm;
  }

  removeEventDate(eventDateIndex: number, location: FormGroup) {
    const eventDates = location.get('eventDates') as FormArray;
    eventDates.removeAt(eventDateIndex);
  }

  addServiceArea(area: SepServiceArea, location: FormGroup) {
    const areaArray = location.get('serviceAreas') as FormArray;
    const areaFormGroup = this.createServiceArea(area);
    areaArray.push(areaFormGroup);
  }

  createServiceArea(area: SepServiceArea) {
    let areaForm = this.fb.group({
      id: [null],
      eventName: [''],
      licencedAreaDescription: [''],
      licencedAreaMaxNumberOfGuests: [''],
      minorPresent: [''],
      numberOfMinors: [''],
      setting: [''],
      stateCode: [''],
      statusCode: [''],
    });
    areaForm.patchValue(area);
    return areaForm;
  }

  removeServiceArea(serviceAreaIndex: number, location: FormGroup) {
    const serviceAreas = location.get('serviceAreas') as FormArray;
    serviceAreas.removeAt(serviceAreaIndex);
  }

  autocompleteDisplay(item: AutoCompleteItem) {
    return item?.name;
  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.form.updateValueAndValidity();
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid;
  }

  getFormValue(): SepApplication {
    let formData = {
      ...this.sepApplication,
      ...this.form.value
    };

    formData?.eventLocations.forEach(location => {
      let dateValues = [];
      location?.eventDates.forEach(sched => {
        dateValues.push(new SepSchedule(sched));
      });
      location.eventDates = dateValues;
    });

    const data = {
      localId: this._appID,
      lastUpdated: new Date(),
      eventStatus: 'Draft',
      lastStepCompleted: 'event',
      ...formData
    } as SepApplication;
    return data;
  }

  next() {
    this.showValidationMessages = false;
    if (this.isValid()) {
      this.saveComplete.emit(this.getFormValue());
    } else {
      this.showValidationMessages = true;
    }
  }

}
