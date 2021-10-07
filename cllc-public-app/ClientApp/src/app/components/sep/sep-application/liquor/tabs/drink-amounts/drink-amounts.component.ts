import { Component, EventEmitter, OnInit, Input, Output } from "@angular/core";
import { FormArray, FormBuilder } from "@angular/forms";
import { SepDrinkType } from "@models/sep-drink-type.model";
import { SepApplication } from "@models/sep-application.model";
import { SpecialEventsDataService } from "@services/special-events-data.service";

@Component({
  selector: "app-drink-amounts",
  templateUrl: "./drink-amounts.component.html",
  styleUrls: ["./drink-amounts.component.scss"]
})
export class DrinkAmountsComponent implements OnInit {
  drinkAmountsValid = false;
  _application: SepApplication;
  @Input() savingToAPI: boolean;
  @Input()
  set application(app: SepApplication) {
    if (app) {
      const value = JSON.parse(JSON.stringify(app));
      delete value.totalMaximumNumberOfGuests;
      this._application = Object.assign(new SepApplication(), value);
    }
  }

  get application() {
    return this._application;
  }

  get disableForm(): boolean {
    if(this.application){
      return this.application?.eventStatus && this.application?.eventStatus !== "Draft";
    }
    return false;
  }

  @Output() saved: EventEmitter<{ declaredServings: number }> = new EventEmitter<{ declaredServings: number }>();
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();
  form: FormArray;
  // a list of drink types that will be fetched from the server
  drinkTypes: SepDrinkType[] = [];

  constructor(private fb: FormBuilder,
    private sepDataService: SpecialEventsDataService) { }

  ngOnInit(): void {
    this.form = this.fb.array([]);
    this.sepDataService.getSepDrinkTypes()
      .subscribe(data => {
        this.drinkTypes = data;
      });
  }

  addDrinkType(value: any = {}) {
    const drinkType = this.fb.group({
      id: [""],
      estimatedServings: [""],
      drinkTypeId: [""],
      pricePerServing: [""]
    });
    drinkType.patchValue(value);
    this.form.push(drinkType);
  }

  next(planner) {
    if (this.drinkAmountsValid) {
      const plannerValue = planner?.form?.value || {};
      this.saved.next(<any>{ drinksSalesForecasts: this.form.value, ...plannerValue });
    }
  }
}
