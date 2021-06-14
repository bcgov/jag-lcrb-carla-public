import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { Router } from '@angular/router';
import { faMapMarkerAlt, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';
import { SepLocation } from '@models/sep-location.model';
import { SepSchedule } from '@models/sep-schedule.model';
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
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
        if (this.form) {
          this.setFormValue(this.sepApplication);
        }
      });
  };

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
    this.form = this.fb.group({
      sepCity: [''],
      isAnnualEvent: [''],
      maximumNumberOfGuests: [''],
      eventLocations: this.fb.array([]),
    });
    this.setFormValue(this.sepApplication);

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
    if (app) {
      app.eventLocations = app.eventLocations || [];
      this.form.patchValue(app);
    }
    this.locations.clear();

    if (app?.eventLocations?.length > 0) {
      app.eventLocations.forEach(loc => {
        this.addLocation(loc);
      });
    } else {
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
      serviceAreas: this.fb.array([]),
      eventDates: this.fb.array([]),
    });
    locationForm.patchValue(location);

    if (!location.serviceAreas || location.serviceAreas.length == 0) {
      location.serviceAreas = [{} as SepServiceArea];
    }
    location.serviceAreas.forEach(area => {
      const areaForm = this.createServiceArea(area);
      (locationForm.get('serviceAreas') as FormArray).push(areaForm);
    });

    if (!location.serviceAreas || location.serviceAreas.length == 0) {
      location.serviceAreas = [{} as SepServiceArea];
    }

    if (!location.eventDates || location.eventDates.length == 0) {
      location.eventDates = [{} as SepSchedule];
    }

    location.eventDates.forEach(ed => {
      const edForm = this.createEventDate(ed);
      (locationForm.get('eventDates') as FormArray).push(edForm);
    });

    this.locations.push(locationForm);
  }


  removeLocation(locationIndex: number) {
    var loc: SepLocation = this.locations.at(locationIndex).value;
    if (loc.id && this.sepApplication.itemsToDelete.locations.indexOf(loc.id) === -1) {
      this.sepApplication.itemsToDelete.locations.push(loc.id);
    }
    loc.eventDates.forEach(ed => {
      const deleteEventDates = this.sepApplication.itemsToDelete.eventDates;
      if (ed.id && deleteEventDates.indexOf(ed.id) === -1) {
        deleteEventDates.push(ed.id);
      }
    });

    loc.serviceAreas.forEach(area => {
      const deleteServiceAreas = this.sepApplication.itemsToDelete.serviceAreas;
      if (area.id && deleteServiceAreas.indexOf(area.id) === -1) {
        deleteServiceAreas.push(area.id);
      }
    });
    this.locations.removeAt(locationIndex);
  }

  addEventDate(sched: SepSchedule, location: FormGroup) {
    const eventDates = location.get('eventDates') as FormArray;
    const dates = this.createEventDate(sched);
    eventDates.push(dates);
  }

  createEventDate(eventDate: SepSchedule) {
    let datesForm = this.fb.group({
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
    const deleteEventDates = this.sepApplication.itemsToDelete.eventDates;
    const ed: SepSchedule = eventDates.at(eventDateIndex).value;
    if (ed.id && deleteEventDates.indexOf(ed.id) === -1) {
      deleteEventDates.push(ed.id);
    }
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
      setting: [''],
      stateCode: [''],
      statusCode: [''],
    });
    areaForm.patchValue(area);
    return areaForm;
  }

  removeServiceArea(serviceAreaIndex: number, location: FormGroup) {
    let serviceAreas = location.get('serviceAreas') as FormArray;

    const deleteServiceAreas = this.sepApplication.itemsToDelete.serviceAreas;
    const area: SepServiceArea = serviceAreas.at(serviceAreaIndex).value;
    if (area.id && deleteServiceAreas.indexOf(area.id) === -1) {
      deleteServiceAreas.push(area.id);
    }
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

const TIME_SLOTS = [
  { value: "8:00 AM", name: "8:00 AM" },
  { value: "8:30 AM ", name: "8:30 AM " },
  { value: "9:00 AM", name: "9:00 AM" },
  { value: "9:30 AM", name: "9:30 AM" },
  { value: "10:00 AM", name: "10:00 AM" },
  { value: "10:30 AM ", name: "10:30 AM " },
  { value: "11:00 AM", name: "11:00 AM" },
  { value: "11:30 AM ", name: "11:30 AM " },
  { value: "12:00 PM", name: "12:00 PM" },
  { value: "12:30 PM ", name: "12:30 PM " },
  { value: "1:00 PM", name: "1:00 PM" },
  { value: "1:30 PM ", name: "1:30 PM " },
  { value: "2:00 PM", name: "2:00 PM" },
  { value: "2:30 PM ", name: "2:30 PM " },
  { value: "3:00 PM", name: "3:00 PM" },
  { value: "3:30 PM ", name: "3:30 PM " },
  { value: "4:00 PM", name: "4:00 PM" },
  { value: "4:30 PM ", name: "4:30 PM " },
  { value: "5:00 PM", name: "5:00 PM" },
  { value: "5:30 PM ", name: "5:30 PM " },
  { value: "6:00 PM", name: "6:00 PM" },
  { value: "6:30 PM ", name: "6:30 PM " },
  { value: "7:00 PM", name: "7:00 PM" },
  { value: "7:30 PM ", name: "7:30 PM " },
  { value: "8:00 PM", name: "8:00 PM" },
  { value: "8:30 PM ", name: "8:30 PM " },
  { value: "9:00 PM", name: "9:00 PM" },
  { value: "9:30 PM ", name: "9:30 PM " },
  { value: "10:00 PM", name: "10:00 PM" },
  { value: "10:30 PM ", name: "10:30 PM " },
  { value: "11:00 PM", name: "11:00 PM" },
  { value: "11:30 PM ", name: "11:30 PM " },
  { value: "12:00 AM", name: "12:00 AM" },
  { value: "12:30 AM ", name: "12:30 AM " },
  { value: "1:00 AM", name: "1:00 AM" },
  { value: "1:30 AM ", name: "1:30 AM " },
  { value: "2:00 AM", name: "2:00 AM" },
  { value: "2:30 AM ", name: "2:30 AM " },
  { value: "3:00 AM", name: "3:00 AM" },
  { value: "3:30 AM ", name: "3:30 AM " },
  { value: "4:00 AM", name: "4:00 AM" },
  { value: "4:30 AM ", name: "4:30 AM " },
  { value: "5:00 AM", name: "5:00 AM" },
  { value: "5:30 AM ", name: "5:30 AM " },
  { value: "6:00 AM", name: "6:00 AM" },
  { value: "6:30 AM ", name: "6:30 AM " },
  { value: "7:00 AM", name: "7:00 AM" },
];
