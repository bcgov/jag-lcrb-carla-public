import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { faMapMarkerAlt, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication} from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';
import { FormBase } from '@shared/form-base';
import { distinctUntilChanged } from 'rxjs/operators';
import { Account } from '@models/account.model';
import { SepLocation } from '@models/sep-location.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { SepServiceArea } from '@models/sep-service-are.model';

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
  saveComplete = new EventEmitter<boolean>();
  faQuestionCircle = faQuestionCircle;
  form: FormGroup;
  showValidationMessages: boolean;
  validationMessages: string[];
  get minDate() {
    return new Date();
  }
  @Input()
  set applicationId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
        debugger;
        if (this.form) {
          this.form.patchValue(this.sepApplication);
        }
      });
  };

  get locations(): FormArray {
    return this.form.get('locations') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private router: Router,
    private db: IndexDBService) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      lgIn: [''],
      isAnnualEvent: [''],
      locations: this.fb.array([]),
    });

    if (this.sepApplication) {
      this.form.patchValue(this.sepApplication);
    }

    if (this?.sepApplication?.locations?.length > 0) {
      this.sepApplication.locations.forEach(loc => {
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

  getEventDates(location: FormGroup): FormArray {
    let result = this.fb.array([]);
    if (location) {
      result = location.get('eventDates') as FormArray;
    }
    return result;
  }


  addLocation(location: SepLocation = new SepLocation()) {
    let locationForm = this.fb.group({
      locationPermitNumber: [''],
      locationName: [''],
      venueType: [''],
      locationMaxGuests: [''],
      eventLocationStreet1: [''],
      eventLocationStreet2: [''],
      eventLocationCity: [''],
      eventLocationProvince: [''],
      eventLocationPostalCode: [''],
      serviceAreas: this.fb.array([]),
    });
    locationForm.patchValue(location);

    if (!location.serviceAreas || location.serviceAreas.length == 0) {
      location.serviceAreas = [{} as SepServiceArea];
    }
    location.serviceAreas.forEach(area => {
      const areaForm = this.createServiceArea(area);
      (locationForm.get('serviceAreas') as FormArray).push(areaForm);

      if (!area.eventDates || area.eventDates.length == 0) {
        area.eventDates = [{} as SepSchedule];
      }
      area.eventDates.forEach(ed => {
        const edForm = this.createEventDate(ed);
        (areaForm.get('eventDates') as FormArray).push(edForm);
      });

    });


    this.locations.push(locationForm);
  }


  removeLocation(locationIndex: number) {
    this.locations.removeAt(locationIndex);
  }

  addEventDate(sched: SepSchedule, area: FormGroup){
    const eventDates = area.get('eventDates') as FormArray;
    const dates = this.createEventDate(sched);
    eventDates.push(dates);
  }

  createEventDate(eventDate: SepSchedule) {
    let datesForm = this.fb.group({
      eventDate: [''],
      eventStart: [''],
      eventEnd: [''],
      serviceStart: [''],
      serviceEnd: [''],
    });
    datesForm.patchValue(eventDate);
    return datesForm;
  }

  removeEventDate(eventDateIndex: number, serviceArea: FormGroup) {
    const eventDates = serviceArea.get('eventDates') as FormArray;
    eventDates.removeAt(eventDateIndex);
  }

  addServiceArea(area: SepServiceArea, location: FormGroup){
    const areaArray = location.get('serviceAreas') as FormArray;
    const areaFormGroup = this.createServiceArea(area);
    areaArray.push(areaFormGroup);
  }

  createServiceArea(area: SepServiceArea) {
    let areaForm = this.fb.group({
      description: [''],
      numAreaMaxGuests: [''],
      setting: [''],
      isMinorsPresent: [''],
      numMinors: [''],
      eventDates: this.fb.array([]),
    });
    areaForm.patchValue(area);
    return areaForm;
  }


  removeServiceArea(serviceAreaIndex: number, location: FormGroup) {
    let serviceAreas = location.get('serviceAreas') as FormArray;
    serviceAreas.removeAt(serviceAreaIndex);
  }


  isValid() {
    this.markControlsAsTouched(this.form);
    this.form.updateValueAndValidity();
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid;
  }

  save() {
    const data = {
      id: this._appID,
      ...this.sepApplication,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'event';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.sepApplication?.stepsCompleted || []),
      ...this.form.value,
    } as SepApplication;

    if (data.id) {
      this.db.applications.update(data.id, data);
    } else {
      console.error("The id should already exist at this point.")
    }
  }

  next() {
    this.showValidationMessages = false;
    if (this.isValid()) {
      this.save();
      this.saveComplete.emit(true);
    } else {
      this.showValidationMessages = true;
    }
  }

  saveForLater() {
    this.save();
    this.router.navigateByUrl('/sep/my-applications')
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
