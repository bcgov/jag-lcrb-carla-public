import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-liquor',
  templateUrl: './liquor.component.html',
  styleUrls: ['./liquor.component.scss']
})
export class LiquorComponent implements OnInit {
  selectedIndex = 0;
  value: any = {};
  @Output()
  saveComplete = new EventEmitter<boolean>();

  constructor() { }

  updateValue(value) {
    this.value = { ...this.value, ...value };
  }

  ngOnInit(): void {
  }

  save() {
    debugger;
  }

}
