import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { faLightbulb } from '@fortawesome/free-regular-svg-icons';
import { faCheckCircle, faExclamationTriangle, faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication } from '@models/sep-application.model';
import { SepDrinkType } from '@models/sep-drink-type.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { FormBase } from '@shared/form-base';
import { Subscription } from 'rxjs';
import configuration, { DrinkConfig, GST_MULTIPLIER, HOURS_OF_LIQUOR_SERVICE, SERVINGS_PER_PERSON } from './config';
@Component({
  selector: 'app-drink-planner',
  templateUrl: './drink-planner.component.html',
  styleUrls: ['./drink-planner.component.scss']
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
    'Beer/Cider/Cooler': SepDrinkType;
    Wine: SepDrinkType;
    Spirits: SepDrinkType;
  };

  @Input() set sepApplication(value: SepApplication) {
    if (value) {
      this._app = value;
      this.totalServings = this._app.totalServings;
      this.form.patchValue(this._app);
      this.updateFormValidationAndDefaults();
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
    beer_free: [0],
    wine_free: [0],
    spirits_free: [0],
    averageBeerPrice: [null],
    averageWinePrice: [null],
    averageSpiritsPrice: [null]
  });

  // Some of the drinks are provided free
  get someFree(): boolean {
    return (
      this.sepApplication?.chargingForLiquorReason === 'Combination' ||
      this.sepApplication?.chargingForLiquorReason === 'RaiseMoney'
    );
  }

  getTotalServings(): number {
    const { hours, totalMaximumNumberOfGuests } = this.form.value;
    return (hours / HOURS_OF_LIQUOR_SERVICE) * totalMaximumNumberOfGuests * SERVINGS_PER_PERSON;
  }

  get totalPercentage(): number {
    if (!this.someFree) {
      const { beer, wine, spirits } = this.form.value as { beer: number; wine: number; spirits: number };
      return beer + wine + spirits;
    } else {
      const { beer, beer_free, wine, wine_free, spirits, spirits_free } = this.form.value as {
        beer: number;
        beer_free: number;
        wine: number;
        wine_free: number;
        spirits: number;
        spirits_free: number;
      };
      return beer + beer_free + wine + wine_free + spirits + spirits_free;
    }
  }

  getAVControlName(groupName: string): string {
    switch (groupName) {
      case 'beer':
        return 'averageBeerPrice';
      case 'wine':
        return 'averageWinePrice';
      case 'spirits':
        return 'averageSpiritsPrice';
      default:
        return '';
    }
  }

  constructor(
    private fb: FormBuilder,
    private sepDataService: SpecialEventsDataService
  ) {
    super();
    this.drinkTypes = <any>{};
    this.sepDataService.getSepDrinkTypes().subscribe((data) => {
      this.drinkTypes = <any>{};
      (data || []).forEach((drinkType) => {
        this.drinkTypes[drinkType.name] = drinkType;
      });
      this.updateFormValidationAndDefaults();

      if (this.disabled) {
        this.form.disable();
      }
    });
  }

  isValid(): boolean {
    const isRaiseMoney = this.canPriceAboveMax();
    const notCharging = this.sepApplication?.chargingForLiquorReason === 'LiquorIsFree';
    //const multiplier = this.sepApplication?.isGSTRegisteredOrg ? GST_MULTIPLIER : 1;  // Add GST for GST Registered Organizations
    const multiplier = 1; // Don't use the GST multiplier until LCSD-8637 has the goahead.

    if (isRaiseMoney != true && notCharging != true) {
      if (
        (this.form.value.averageBeerPrice &&
          this.drinkTypes?.['Beer/Cider/Cooler']?.pricePerServing * multiplier < this.form.value.averageBeerPrice) ||
        (this.form.value.averageWinePrice &&
          this.drinkTypes?.['Wine']?.pricePerServing * multiplier < this.form.value.averageWinePrice) ||
        (this.form.value.averageSpiritsPrice &&
          this.drinkTypes?.['Spirits']?.pricePerServing * multiplier < this.form.value.averageSpiritsPrice)
      ) {
        return false;
      }
    }
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
    this.form.valueChanges.subscribe((_) => {
      this.validChange.emit(this.isValid());
    });

    this.form.get('hours').valueChanges.subscribe((_) => {
      if (!this.hideGuestsAndHours) {
        this.totalServings = this.getTotalServings();
      }
    });

    this.form.get('totalMaximumNumberOfGuests').valueChanges.subscribe((_) => {
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
      return '0';
    }
    return ((servings / this.totalServings) * 100).toFixed(1);
  }

  servingPercentFree(config: DrinkConfig): string {
    const servings: number = this.form.get(config.group).value + this.form.get(config.group_free).value || 0;
    if (servings === 0 || this.totalServings === 0) {
      return '0';
    }
    return ((servings / this.totalServings) * 100).toFixed(1);
  }

  storageUnits(config: DrinkConfig): number {
    //calculate free and paid servings
    const servings: number = this.form.get(config.group).value + this.form.get(config.group_free).value || 0;
    const servingSizemL = this.drinkTypes[config.drinkTypeName]?.servingSizeMl || 0;
    const storageSizemL = this.drinkTypes[config.drinkTypeName]?.storageSizeMl || 0;
    const storageUnits = storageSizemL > 0 ? (servings * servingSizemL) / storageSizemL : 0; // div by zero check
    return storageUnits > 0 && storageUnits < 1 ? 1 : storageUnits;
  }

  unitSize(config: DrinkConfig): string {
    const curDrinkType = this.drinkTypes[config.drinkTypeName];
    return curDrinkType?.unitSize;
  }

  // Applicants can raise drink prices above the maximum when operating certain types of charitable/significant events
  canPriceAboveMax(): boolean {
    return (
      this.sepApplication?.chargingForLiquorReason === 'RaiseMoney' ||
      this.sepApplication?.isLocalSignificance ||
      this.sepApplication?.isMajorSignificance
    );
  }

  storageMethodDescription(config: DrinkConfig): string {
    const curDrinkType = this.drinkTypes[config.drinkTypeName];

    if (curDrinkType) {
      // drinkTypes may not have been initialized yet, so check for null before accessing properties
      if (curDrinkType.storageMethod === 'kegs') {
        return `${curDrinkType.storageMethod} of ${curDrinkType.group}`;
      } else {
        return `${curDrinkType.storageSizeMl} ml ${curDrinkType.storageMethod} of ${curDrinkType.group}`;
      }
    }
  }

  updateFormValidationAndDefaults() {
    if (!this.drinkTypes || !this.sepApplication) {
      return;
    }

    const isFree = this.sepApplication.chargingForLiquorReason === 'LiquorIsFree';
    
    // GST Registered Organizations can add GST to the sell price;
    // otherwise the max price is the price set by LCRB
    const multiplier = this.sepApplication.isGSTRegisteredOrg ? GST_MULTIPLIER : 1;

    const maxBeerPrice = isFree ? 0 : this.drinkTypes['Beer/Cider/Cooler'].pricePerServing * multiplier;
    const maxWinePrice = isFree ? 0 : this.drinkTypes['Wine'].pricePerServing * multiplier;
    const maxSpiritsPrice = isFree ? 0 : this.drinkTypes['Spirits'].pricePerServing * multiplier;

    let beerPriceControl = this.form.get('averageBeerPrice')!;
    let winePriceControl = this.form.get('averageWinePrice')!;
    let spiritsPriceControl = this.form.get('averageSpiritsPrice')!;

    beerPriceControl.setValidators([Validators.required, Validators.min(0)]);
    winePriceControl.setValidators([Validators.required, Validators.min(0)]);
    spiritsPriceControl.setValidators([Validators.required, Validators.min(0)]);

    if (!this.canPriceAboveMax()) {
      beerPriceControl.addValidators(Validators.max(maxBeerPrice));
      winePriceControl.addValidators(Validators.max(maxWinePrice));
      spiritsPriceControl.addValidators(Validators.max(maxSpiritsPrice));
    }

    if (this.someFree) {
      const values: { [key: string]: number } = {};
      for (const item of this.sepApplication.drinksSalesForecasts.filter((k) => k.isCharging == false)) {
        if (item.name.indexOf('Wine') > -1) {
          values['wine_free'] = item.estimatedServings;
        } else if (item.name.indexOf('Spirits') > -1) {
          values['spirits_free'] = item.estimatedServings;
        } else if (item.name.indexOf('Beer/Cider/Cooler') > -1) {
          values['beer_free'] = item.estimatedServings;
        }
      }
      this.form.patchValue(values);
    }

    // if there is no price; or if the price is greater than the max, set it to the max
    // if they've entered a different value, they will need to change it back if they're navigating back and forth
    if (!this.form.value?.averageBeerPrice || this.form.value?.averageBeerPrice > maxBeerPrice) {
      beerPriceControl.setValue(maxBeerPrice);
    }
    if (!this.form.value?.averageWinePrice || this.form.value?.averageWinePrice > maxWinePrice) {
      winePriceControl.setValue(maxWinePrice);
    }
    if (!this.form.value?.averageSpiritsPrice || this.form.value?.averageSpiritsPrice > maxSpiritsPrice) {
      spiritsPriceControl.setValue(maxSpiritsPrice);
    }

    beerPriceControl.updateValueAndValidity();
    winePriceControl.updateValueAndValidity();
    spiritsPriceControl.updateValueAndValidity();
  }

  getErrorMessage(controlName) {
    const control = this.form.get(controlName);
    let error = 'Invalid input';

    if (control?.errors?.min) {
      error = `Please enter a value greater than or equal to ${control.errors.min.min}`;
    } else if (control?.errors?.max) {
      error = `Please enter a value less than or equal to ${control.errors.max.max}`;
    }
    return error;
  }

  //validate input should only accept numbers
  validateInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    input.value = input.value.replace(/-/g, '');
  }
}
