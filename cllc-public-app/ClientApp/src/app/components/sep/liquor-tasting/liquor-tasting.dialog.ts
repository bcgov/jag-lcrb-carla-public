import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faLightbulb } from '@fortawesome/free-regular-svg-icons';
import { faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-liquor-tasting-dialog',
  templateUrl: './liquor-tasting.dialog.html',
  styleUrls: ['./liquor-tasting.dialog.scss']
})
export class LiquorTastingDialog implements OnInit {
  // icons
  faTimes = faTimes;
  faLightbulb = faLightbulb;

  get dialog() {
    return this.data;
  }

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit() {}
}
