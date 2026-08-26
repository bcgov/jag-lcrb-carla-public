import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faLightbulb } from '@fortawesome/free-regular-svg-icons';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-drink-planner-dialog',
  templateUrl: './drink-planner.dialog.html',
  styleUrls: ['./drink-planner.dialog.scss']
})
export class DrinkPlannerDialog implements OnInit {
  // icons
  faTimes = faTimes;
  faLightbulb = faLightbulb;

  get dialog() {
    return this.data;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit() {}
}
