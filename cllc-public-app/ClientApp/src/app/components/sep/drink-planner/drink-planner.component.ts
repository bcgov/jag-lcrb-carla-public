import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { FormBase } from "@shared/form-base";
import { Subscription } from "rxjs";
import { faLightbulb } from "@fortawesome/free-regular-svg-icons";
import configuration, { DrinkConfig, HOURS_OF_LIQUOR_SERVICE, SERVINGS_PER_PERSON } from "./config";
import { SepApplication } from "@models/sep-application.model";
import { faQuestionCircle, faExclamationTriangle, faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { SepDrinkType } from "@models/sep-drink-type.model";

@Component({
  selector: "app-drink-planner",
  templateUrl: "./drink-planner.component.html",
  styleUrls: ["./drink-planner.component.scss"],
})
export class DrinkPlannerComponent extends FormBase implements OnInit {
  @Output() validChange: EventEmitter<boolean> = new EventEmitter<boolean>();
  // icons
  faLightbulb = faLightbulb;
  faQuestionCircle = faQuestionCircle;
  faExclamationTriange = faExclamationTriangle;
  faCheckCircle = faCheckCircle;

  totalServings = 0;
  _app: SepApplication;
  drinkTypes: SepDrinkType[];

  @Input() set sepApplication(value: SepApplication) {
    if (value) {
      this._app = value;
      this.totalServings = this._app.totalServings;
      this.form.patchValue(this._app);
    }
  }

  get sepApplication() {
    return this._app;
  }
  @Input() hideGuestsAndHours = false;
  @Input()
  config: Array<DrinkConfig> = configuration;

  // component state
  busy: Subscription;

  // drink planner form
  form = this.fb.group({
    hours: [HOURS_OF_LIQUOR_SERVICE],
    totalMaximumNumberOfGuests: [400],
    beer: [0],
    wine: [0],
    spirits: [0],
    averageBeerPrice: [null],
    averageWinePrice: [null],
    averageSpiritsPrice: [null],
  });

  getTotalServings(): number {
    const { hours, totalMaximumNumberOfGuests } = this.form.value;
    return (hours / HOURS_OF_LIQUOR_SERVICE * totalMaximumNumberOfGuests * SERVINGS_PER_PERSON);
  }

  get totalPercentage(): number {
    const { beer, wine, spirits } = this.form.value as { beer: number; wine: number; spirits: number };
    return beer + wine + spirits;
  }

  getControlName(name: string): string {
    let controlName = "";
    switch (name) {
      case "beer":
        controlName = "averageBeerPrice";
        break;
      case "wine":
        controlName = "averageWinePrice";
        break;
      case "spirits":
        controlName = "averageSpiritsPrice";
        break;
    }
    return controlName;
  }

  constructor(private fb: FormBuilder,
    private sepDataService: SpecialEventsDataService) {
    super();
    this.sepDataService.getSepDrinkTypes()
      .subscribe(data => {
        this.drinkTypes = data;
        const beerDefaultPrice = data.find(item => item.name === "Beer/Cider/Cooler")?.costPerServing || 0;
        const wineDefaultPrice = data.find(item => item.name === "Wine")?.costPerServing || 0;
        const spiritsDefaultPrice = data.find(item => item.name === "Spirits")?.costPerServing || 0;

        if (!this.form.value?.averageBeerPrice) {
          this.form.get("averageBeerPrice").setValue(beerDefaultPrice);
        }
        if (!this.form.value?.averageWinePrice) {
          this.form.get("averageWinePrice").setValue(wineDefaultPrice);
        }
        if (!this.form.value?.averageSpiritsPrice) {
          this.form.get("averageSpiritsPrice").setValue(spiritsDefaultPrice);
        }
      });
  }

  isValid(): boolean {
    return this.totalPercentage === this.totalServings;
  }

  ngOnInit() {
    const values = {};
    for (const item of this.config) {
      values[item.group] = item.defaultPercentage;
    }
    if (!this.hideGuestsAndHours) {
      this.totalServings = this.getTotalServings();
    }

    this.form.patchValue(values);

    // setup the form's percentage fields so they always add-up to 100%
    this.initForm();
    this.validChange.emit(this.isValid());
  }

  private initForm(): void {

    this.form.valueChanges
      .subscribe(_ => {
        this.validChange.emit(this.isValid());
      });

    this.form.get("hours").valueChanges
      .subscribe(_ => {
        if (!this.hideGuestsAndHours) {
          this.totalServings = this.getTotalServings();
        }
      });

    this.form.get("totalMaximumNumberOfGuests").valueChanges
      .subscribe(_ => {
        if (!this.hideGuestsAndHours) {
          this.totalServings = this.getTotalServings();
        }
      });

  }

  servings(config: DrinkConfig): number {
    const servings: number = this.form.get(config.group).value;
    return servings;
  }

  servingPercent(config: DrinkConfig): string {
    const servings: number = this.form.get(config.group).value || 0;
    if (servings == 0 || this.totalServings == 0) {
      return "0";
    }
    return (servings / this.totalServings * 100).toFixed(1);
  }

  storageUnits(config: DrinkConfig): number {
    const servings = this.servings(config);
    const storageUnits = servings * config.servingSizeMl / config.storageSizeMl;
    return storageUnits > 0 && storageUnits < 1 ? 1 : storageUnits;
  }

  storageMethodDescription(config: DrinkConfig): string {
    if (config.storageMethod === "kegs") {
      return `${config.storageMethod} of ${config.group}`;
    } else {
      return `${config.storageSizeMl} ml ${config.storageMethod} of ${config.group}`;
    }
  }
}
