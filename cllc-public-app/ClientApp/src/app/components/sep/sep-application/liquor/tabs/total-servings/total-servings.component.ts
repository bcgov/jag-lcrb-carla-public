import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-total-servings',
  templateUrl: './total-servings.component.html',
  styleUrls: ['./total-servings.component.scss']
})
export class TotalServingsComponent implements OnInit {
  declaredMaxServings: number = 1;
  @Output() saved: EventEmitter<{declaredServings: number}> = new EventEmitter<{declaredServings: number}>();
  minServings = 1;
  maxServings = 114;

  constructor() { }

  ngOnInit(): void {
  }

  formatLabel(value: number) {
    return value;
  }

  next() {
    let declaredServings = this.declaredMaxServings;
    this.saved.next({declaredServings});
  }

}
