import { EventEmitter, Input, Output } from "@angular/core";
import { Component, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { SepApplication } from "@models/sep-application.model";
import { FormBase } from "@shared/form-base";
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: "app-selling-drinks",
  templateUrl: "./selling-drinks.component.html",
  styleUrls: ["./selling-drinks.component.scss"],
})
export class SellingDrinksComponent extends FormBase implements OnInit {
  _application: SepApplication;
  errorMessages: string[];
  @Input()
  set sepApplication(value: SepApplication) {
    this._application = value;
    if (this.form) {
      this.form.patchValue(value);
      if (this.disableForm) {
        this.form.disable();
      }
    }
  }
  get sepApplication() {
    return this._application;
  }

  get disableForm(): boolean {
    if(this.sepApplication){
      return this.sepApplication?.eventStatus && this.sepApplication?.eventStatus !== "Draft";
    }
    return false;
  }

  @Output() saved: EventEmitter<{ declaredServings: number }> = new EventEmitter<{ declaredServings: number }>();
  form: FormGroup;
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();
  faQuestionCircle = faQuestionCircle;
  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      chargingForLiquorReason: [this?.sepApplication?.chargingForLiquorReason, [Validators.required]],
      isGSTRegisteredOrg: [this?.sepApplication?.isGSTRegisteredOrg],
      donateOrConsular: [this?.sepApplication?.donateOrConsular],
      nonProfitName: [this?.sepApplication?.nonProfitName]//,
      // fundraisingPurpose: [this?.application?.fundraisingPurpose],
      // howProceedsWillBeUsedDescription: [this?.application?.howProceedsWillBeUsedDescription],
      //isManufacturingExclusivity: [this?.sepApplication?.isManufacturingExclusivity]
    });

    this.form.get("chargingForLiquorReason").valueChanges
      .subscribe(reason => {
        switch(reason) {
          case "RaiseMoney":
            this.form.get("nonProfitName").setValidators(Validators.required);
            this.form.get("donateOrConsular").setValidators(Validators.required);
            this.form.get("isGSTRegisteredOrg").clearValidators();
            break;
            case "RecoverCost":
              this.form.get("nonProfitName").clearValidators();
              this.form.get("donateOrConsular").clearValidators();
              this.form.get("isGSTRegisteredOrg").setValidators(Validators.required);
              break;
            default:
              this.form.get("nonProfitName").clearValidators();
              this.form.get("donateOrConsular").clearValidators();
              this.form.get("isGSTRegisteredOrg").clearValidators();
              break;

          }
          this.form.get("nonProfitName").updateValueAndValidity();
          this.form.get("donateOrConsular").updateValueAndValidity();
          this.form.get("isGSTRegisteredOrg").updateValueAndValidity();

        });

    }

  isValid(): boolean {
    this.markControlsAsTouched(this.form);
    this.errorMessages = this.listControlsWithErrors(this.form, {
      // chargingForLiquorReason: "",
      // isGSTRegisteredOrg: "",
      // donateOrConsularPrevLiqour: "",
      // nonProfitName: "",
      // isManufacturingExclusivity: "",
    });
    const valid = this.form.valid;
    return valid;
  }

  next() {

    if (this.isValid()) {
      this.saved.next({ ...this.form.value });
    }
  }
}
