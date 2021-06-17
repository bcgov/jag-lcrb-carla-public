import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SepApplication } from '@models/sep-application.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { SepServiceArea } from '@models/sep-service-area.model';

@Component({
  selector: 'app-total-servings',
  templateUrl: './total-servings.component.html',
  styleUrls: ['./total-servings.component.scss']
})
export class TotalServingsComponent implements OnInit {
  _application: SepApplication
  @Input()
  set application(app: SepApplication) {
    if (app) {
      const value = JSON.parse(JSON.stringify(app));
      delete value.totalMaximumNumberOfGuests;
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

    this.total_guests = app.totalMaximumNumberOfGuests;
    this.total_minors = app.totalMaximumNumberOfGuests - app.maximumNumberOfAdults;

    this.total_service_hours = app.serviceHours;

    this.suggested_servings = app.suggestedServings;
    this.max_servings = app.maxSuggestedServings;

    if (this.total_servings == 0) {
      app.totalServings = app.suggestedServings;
    }

  }


  formatLabel(value: number) {
    return value;
  }

  next() {
    this.saved.next({ totalServings: this.total_servings });
  }

}
