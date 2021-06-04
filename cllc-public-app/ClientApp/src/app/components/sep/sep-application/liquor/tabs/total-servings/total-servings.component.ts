import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SepApplication } from '@models/sep-application.model';
import { SepSchedule } from '@models/sep-schedule.model';

@Component({
  selector: 'app-total-servings',
  templateUrl: './total-servings.component.html',
  styleUrls: ['./total-servings.component.scss']
})
export class TotalServingsComponent implements OnInit {
  _application: SepApplication
  @Input()
  set application(value: SepApplication) {
    if (value) {
      this._application = Object.assign(new SepApplication(), value);
      this.total_servings = this._application?.totalServings || 0;
      this.setServings(this._application);
    }
  };

  get application() {
    return this._application;
  }
  @Output() saved: EventEmitter<{ totalServings: number }> = new EventEmitter<{ totalServings: number }>();

  suggested_servings = 0;
  max_servings = 0;
  total_servings = 0;
  total_guests = 0;
  total_minors = 0;
  total_service_hours = 0;

  form: FormGroup;
  //@Input() total_servings: number;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {

    // this.form.get('indigenousNationId').patchValue(data.indigenousNation.id);

  }



  setServings(app: SepApplication) {
    if (!app) {
      return;
    }
    this.total_guests += app.totalMaximumNumberOfGuests;
    this.total_minors += app.totalMaximumNumberOfGuests - app.maximumNumberOfAdults;

    // calculate the suggested servings and maximum servings by looping through each event location
    for (var location of app.eventLocations) {
      // accumulate the total hours of service by looping through the eventDates
      for (var eventDate of location.eventDates) {
        eventDate = Object.assign(new SepSchedule(null), eventDate);
        this.total_service_hours += eventDate.getServiceHours();
      }

      // count up the guests and minors

      this.total_guests += parseInt(location.maximumNumberOfGuests.toString(), 10) || 0;
      this.total_minors += parseInt(location.locationNumberMinors?.toString(), 10) || 0;


    }



    this.suggested_servings = Math.floor((this.total_service_hours / 3) * (this.total_guests - this.total_minors) * 4);
    this.max_servings = Math.floor(((this.total_service_hours / 3) * (this.total_guests - this.total_minors) * 5));
    if (this.total_servings == 0) {
      this.total_servings = this.suggested_servings;
    }


  }


  formatLabel(value: number) {
    return value;
  }

  next() {
    this.saved.next({ totalServings: this.total_servings });
  }

}
