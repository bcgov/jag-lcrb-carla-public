import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-liquor',
  templateUrl: './liquor.component.html',
  styleUrls: ['./liquor.component.scss']
})
export class LiquorComponent implements OnInit {
  @Output()
  saveComplete = new EventEmitter<boolean>();
  constructor() { }

  ngOnInit(): void {
  }

}
