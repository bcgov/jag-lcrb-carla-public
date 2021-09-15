import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { Form, FormBuilder, FormGroup, Validators } from "@angular/forms";
import { SepApplication } from "@models/sep-application.model";
import { SepSchedule } from "@models/sep-schedule.model";
import { SepServiceArea } from "@models/sep-service-area.model";
import { Options } from "@angular-slider/ngx-slider";

@Component({
  selector: "app-total-servings",
  templateUrl: "./total-servings.component.html",
  styleUrls: ["./total-servings.component.scss"]
})
export class TotalServingsComponent implements OnInit {
  options: Options = null;
  _application: SepApplication;
  @Input()
  set application(app: SepApplication) {
    if (app) {
      const value = JSON.parse(JSON.stringify(app));
      delete value.totalMaximumNumberOfGuests;
      this._application = Object.assign(new SepApplication(), value);
      this.setServings(this._application);
    }
  }

  get application() {
    return this._application;
  }

  get disableForm(): boolean {
    return this.application && this.application.eventStatus !== "Draft";
  }

  @Output() saved: EventEmitter<{ totalServings: number }> = new EventEmitter<{ totalServings: number }>();

  suggested_servings = 0;
  max_servings = 0;
  total_servings = 0;
  total_servingsSlider = 0;
  total_guests = 0;
  total_minors = 0;
  total_service_hours = 0;

  form: FormGroup;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
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
    this.total_servings = app?.totalServings || this.suggested_servings;
    this.total_servingsSlider = this.total_servings;

    this.options = {
      showTicks: false,
      floor: 1,
      ceil: this.max_servings,
      disabled: this.disableForm
    };
    // console.log("setting servings:", this.total_guests, this.total_minors, this.total_service_hours, this.suggested_servings, this.max_servings)

  }

  totalServingsChange(value: number) {
    this.options.disabled = false;
    if (value <= this.max_servings && value !== this.total_servingsSlider) {
      this.total_servingsSlider = value;
    } else {
      // disable slider
      this.options.disabled = true;
    }
    this.options = {...this.options};
  }


  formatLabel(value: number) {
    return value;
  }

  isValid(): boolean {
    return this.total_servings > 0;
  }

  next() {
    this.saved.next({ totalServings: this.total_servings });
  }

}
