import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
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
  drinkTypes: {
    "Beer/Cider/Cooler": SepDrinkType,
    "Wine": SepDrinkType,
    "Spirits": SepDrinkType
  };



  @Input() set sepApplication(value: SepApplication) {
    if (value) {
      this._app = value;
      this.totalServings = this._app.totalServings;
      this.form.patchValue(this._app);
      this.updateFormValidation();

    }
  }

  get sepApplication() {
    return this._app;
  }
  @Input() hideGuestsAndHours = false;
  @Input() disabled = false;
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

  getAVControlName(groupName: string): string {
    let controlName = "";
    switch (groupName) {
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

  constructor(
    private fb: FormBuilder,
    private sepDataService: SpecialEventsDataService
  ) {
    super();
    this.sepDataService.getSepDrinkTypes()
    .subscribe(data => {
      this.drinkTypes = <any>{};
      (data || []).forEach(drinkType => {
        this.drinkTypes[drinkType.name] = drinkType;
      });
        this.updateFormValidation();

        if (this.disabled) {
          this.form.disable();
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

    if (this.disabled) {
      this.form.disable();
    }

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
    if (servings === 0 || this.totalServings === 0) {
      return "0";
    }
    return (servings / this.totalServings * 100).toFixed(1);
  }

  storageUnits(config: DrinkConfig): number {
    const servings = this.servings(config);
    const storageUnits = servings * config.servingSizeMl / config.storageSizeMl;
    return storageUnits > 0 && storageUnits < 1 ? 1 : storageUnits;
  }

  editPrice(): boolean {
    return true;
  }

  canRaisePrice(): boolean {
    return this.sepApplication?.chargingForLiquorReason === "RaiseMoney" ||
    this.sepApplication?.isLocalSignificance ||
    this.sepApplication?.isMajorSignificance;
  }

  storageMethodDescription(config: DrinkConfig): string {
    if (config.storageMethod === "kegs") {
      return `${config.storageMethod} of ${config.group}`;
    } else {
      return `${config.storageSizeMl} ml ${config.storageMethod} of ${config.group}`;
    }
  }

  updateFormValidation() {
    if (!this.drinkTypes || !this.sepApplication) {
      return;
    }

    // applicants can change drink prices when operating certain types of charitable/significant events
    const isRaiseMoney = this.canRaisePrice();
    const notCharging = this.sepApplication?.chargingForLiquorReason === "LiquorIsFree";

    // GST Registered Organizations can add 5% to the sell price, to recover 
    // operating costs; otherwise the max price is the price set by LCRB
    const multiplier = this.sepApplication?.isGSTRegisteredOrg ? 1.05 : 1;

    let maxBeerPrice = 0;
    let maxWinePrice = 0;
    let maxSpiritsPrice = 0;

    // calculate the default/max price using the multiplier
    if(!notCharging){
      maxBeerPrice = (this.drinkTypes["Beer/Cider/Cooler"]?.pricePerServing || 0) * multiplier;
      maxWinePrice = (this.drinkTypes["Wine"]?.pricePerServing || 0) * multiplier;
      maxSpiritsPrice = (this.drinkTypes["Spirits"]?.pricePerServing || 0) * multiplier;
    }

    const minBeerPrice =0;
    const minWinePrice = 0;
    const minSpiritsPrice = 0;

    if (this.sepApplication && this.drinkTypes) {
      this.form.get("averageBeerPrice").clearValidators();
      this.form.get("averageWinePrice").clearValidators();
      this.form.get("averageSpiritsPrice").clearValidators();

      // if they aren't raising the price, set the maximum
      if(!isRaiseMoney){

        this.form.get("averageBeerPrice").setValidators([Validators.max(maxBeerPrice),Validators.min(0) ]);
        this.form.get("averageWinePrice").setValidators([Validators.max(maxWinePrice),Validators.min(0)]);
        this.form.get("averageSpiritsPrice").setValidators([Validators.max(maxSpiritsPrice),Validators.min(0)]);
      } else {

      this.form.get("averageBeerPrice").setValidators([Validators.min(minBeerPrice)]);
      this.form.get("averageWinePrice").setValidators([Validators.min(minWinePrice)]);
      this.form.get("averageSpiritsPrice").setValidators([Validators.min(minSpiritsPrice)]);

    }


      // if there is no price; or if the price is greater than the max, set it to the max
      // if they've entered a different value, they will need to change it back if they're navigating back and forth
    if (!this.form.value?.averageBeerPrice || this.form.value?.averageBeerPrice > maxBeerPrice) {
      this.form.get("averageBeerPrice").setValue(maxBeerPrice);
    }
    if (!this.form.value?.averageWinePrice || this.form.value?.averageWinePrice > maxWinePrice) {
      this.form.get("averageWinePrice").setValue(maxWinePrice);
    }
    if (!this.form.value?.averageSpiritsPrice || this.form.value?.averageSpiritsPrice > maxSpiritsPrice) {
      this.form.get("averageSpiritsPrice").setValue(maxSpiritsPrice);
    }

    this.form.get("averageBeerPrice").updateValueAndValidity();
    this.form.get("averageWinePrice").updateValueAndValidity();
    this.form.get("averageSpiritsPrice").updateValueAndValidity();

    }
  }

  greaterThanMax(field, max): boolean {
    return (field > max) && this.canRaisePrice();
  }

  getErrorMessage(controlName) {
    const control = this.form.get(controlName);
    let error = "Invalid input";

    if (control?.errors?.min) {
      error = `Please enter a value greater or equal to ${control.errors.min.min}`;
    } else if (control?.errors?.max) {
      error = `Please enter a value less or equal to ${control.errors.max.max}`;
    }
    return error;
  }
}
