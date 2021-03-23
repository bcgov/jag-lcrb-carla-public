import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { Subscription } from 'rxjs';
import drinkPlannerDefaults, { DrinkSettings, HOURS_OF_LIQUOR_SERVICE, SERVINGS_PER_PERSON } from './settings';

@Component({
  selector: 'app-drink-planner',
  templateUrl: './drink-planner.component.html',
  styleUrls: ['./drink-planner.component.scss']
})
export class DrinkPlannerComponent extends FormBase implements OnInit {

  @Input()
  settings: Array<DrinkSettings> = drinkPlannerDefaults;

  // component state
  busy: Subscription;

  // drink planner form
  form = this.fb.group({
    hours: HOURS_OF_LIQUOR_SERVICE,
    guests: 400,
    beer: 0,
    wine: 0,
    spirits: 0,
  });

  constructor(private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    let initialValues = {};
    for (const item of this.settings) {
      initialValues[item.group] = item.defaultPercentage;
    }
    this.form.patchValue(initialValues);
  }

}
