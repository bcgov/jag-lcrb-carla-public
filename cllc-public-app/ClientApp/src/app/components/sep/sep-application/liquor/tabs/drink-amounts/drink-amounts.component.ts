import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-drink-amounts',
  templateUrl: './drink-amounts.component.html',
  styleUrls: ['./drink-amounts.component.scss']
})
export class DrinkAmountsComponent implements OnInit {
  @Output() saved: EventEmitter<{declaredServings: number}> = new EventEmitter<{declaredServings: number}>();
  @Output() back: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }

  next() {
    this.saved.next(<any>{});
  }
}
