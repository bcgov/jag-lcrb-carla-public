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
  configuration: Array<DrinkSettings> = drinkPlannerDefaults;

  // component state
  busy: Subscription;
  state: { [key: string]: DrinkSettings } = {};

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
    for (const item of this.configuration) {
      this.state[item.group] = item;
      initialValues[item.group] = item.defaultPercentage;
    }
    this.form.patchValue(initialValues);
  }

  get totalServings(): number {
    const { hours, guests } = this.form.value as { hours: number; guests: number };
    return (hours / HOURS_OF_LIQUOR_SERVICE * guests * SERVINGS_PER_PERSON);
  }

  getServings(group: string): number {
    const percentage: number = this.form.get(group).value;
    return this.totalServings * percentage / 100;
  }

  getStorage(group: string): number {
    const config = this.state[group];
    const servings = this.getServings(group);
    const storageUnits = servings * config.servingSizeMl / config.storageSizeMl;
    return storageUnits > 0 && storageUnits < 1 ? 1 : storageUnits;
  }
}
