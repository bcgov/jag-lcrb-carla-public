import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { faMapMarkerAlt, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication, SepLocation, SepSchedule, SepServiceArea } from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';
import { FormBase } from '@shared/form-base';
import { distinctUntilChanged } from 'rxjs/operators';
import { Account } from '@models/account.model';

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

  getServiceAreas(location: FormGroup): AbstractControl[] {
    let result = [];
    if (location) {
      result = (location.get('serviceAreas') as FormArray).controls;
    }
    return result;
  }

  getEventDates(location: FormGroup): AbstractControl[] {
    let result = [];
    if (location) {
      result = (location.get('eventDates') as FormArray).controls;
    }
    return result;
  }


  addLocation(location: SepLocation = new SepLocation()) {
    let locationForm = this.fb.group({
      locationPermitNumber: [''],
      locationName: ['1'],
      venueType: [''],
      locationMaxGuests: [''],
      eventLocationStreet1: [''],
      eventLocationStreet2: [''],
      eventLocationCity: [''],
      eventLocationProvince: [''],
      eventLocationPostalCode: [''],
      eventDates: this.fb.array([]),
      serviceAreas: this.fb.array([]),
    });
    locationForm.patchValue(location);

    if (location?.eventDates?.length > 0) {
      location.eventDates.forEach(ed => {
        this.addEventDate(ed, locationForm.get('eventDates') as FormArray);
      });
    } else {
      this.addEventDate({} as SepSchedule, locationForm.get('eventDates') as FormArray);
      this.addEventDate({} as SepSchedule, locationForm.get('eventDates') as FormArray);
    }

    if (location?.serviceAreas?.length > 0) {
      location.serviceAreas.forEach(ed => {
        this.addServiceArea(ed, locationForm.get('serviceAreas') as FormArray);
      });
    } else {
      this.addServiceArea({} as SepServiceArea, locationForm.get('serviceAreas') as FormArray);
      this.addServiceArea({} as SepServiceArea, locationForm.get('serviceAreas') as FormArray);
    }

    this.locations.push(locationForm);
  }

  addEventDate(eventDate: SepSchedule, eventDates: FormArray) {
    let datesForm = this.fb.group({
      eventDate: [''],
      eventStart: [''],
      eventEnd: [''],
      ServiceStart: [''],
      ServiceEnd: [''],
    });
    datesForm.patchValue(eventDate);
    eventDates.push(datesForm);
  }

  addServiceArea(area: SepServiceArea, serviceAreas: FormArray) {
    let areaForm = this.fb.group({
      description: [''],
      numAreaMaxGuests: [''],
      setting: [''],
      isMinorsPresent: [''],
      numMinors: [''],
    });
    areaForm.patchValue(area);
    serviceAreas.push(areaForm);
  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.form.updateValueAndValidity();
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid;
  }

  save() {
    const data = {
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
      ...this.form.value
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
  { value: "8: 00 AM", name: "8: 00 AM" },
  { value: "8:30 AM ", name: "8:30 AM " },
  { value: "9: 00 AM", name: "9: 00 AM" },
  { value: "9:30 AM", name: "9:30 AM" },
  { value: "10: 00 AM", name: "10: 00 AM" },
  { value: "10:30 AM ", name: "10:30 AM " },
  { value: "11: 00 AM", name: "11: 00 AM" },
  { value: "11:30 AM ", name: "11:30 AM " },
  { value: "12: 00 PM", name: "12: 00 PM" },
  { value: "12:30 PM ", name: "12:30 PM " },
  { value: "1: 00 PM", name: "1: 00 PM" },
  { value: "1:30 PM ", name: "1:30 PM " },
  { value: "2: 00 PM", name: "2: 00 PM" },
  { value: "2:30 PM ", name: "2:30 PM " },
  { value: "3: 00 PM", name: "3: 00 PM" },
  { value: "3:30 PM ", name: "3:30 PM " },
  { value: "4: 00 PM", name: "4: 00 PM" },
  { value: "4:30 PM ", name: "4:30 PM " },
  { value: "5: 00 PM", name: "5: 00 PM" },
  { value: "5:30 PM ", name: "5:30 PM " },
  { value: "6: 00 PM", name: "6: 00 PM" },
  { value: "6:30 PM ", name: "6:30 PM " },
  { value: "7: 00 PM", name: "7: 00 PM" },
  { value: "7:30 PM ", name: "7:30 PM " },
  { value: "8: 00 PM", name: "8: 00 PM" },
  { value: "8:30 PM ", name: "8:30 PM " },
  { value: "9: 00 PM", name: "9: 00 PM" },
  { value: "9:30 PM ", name: "9:30 PM " },
  { value: "10: 00 PM", name: "10: 00 PM" },
  { value: "10:30 PM ", name: "10:30 PM " },
  { value: "11: 00 PM", name: "11: 00 PM" },
  { value: "11:30 PM ", name: "11:30 PM " },
  { value: "12: 00 AM", name: "12: 00 AM" },
  { value: "12:30 AM ", name: "12:30 AM " },
  { value: "1: 00 AM", name: "1: 00 AM" },
  { value: "1:30 AM ", name: "1:30 AM " },
  { value: "2: 00 AM", name: "2: 00 AM" },
  { value: "2:30 AM ", name: "2:30 AM " },
  { value: "3: 00 AM", name: "3: 00 AM" },
  { value: "3:30 AM ", name: "3:30 AM " },
  { value: "4: 00 AM", name: "4: 00 AM" },
  { value: "4:30 AM ", name: "4:30 AM " },
  { value: "5: 00 AM", name: "5: 00 AM" },
  { value: "5:30 AM ", name: "5:30 AM " },
  { value: "6: 00 AM", name: "6: 00 AM" },
  { value: "6:30 AM ", name: "6:30 AM " },
  { value: "7: 00 AM", name: "7: 00 AM" },
];