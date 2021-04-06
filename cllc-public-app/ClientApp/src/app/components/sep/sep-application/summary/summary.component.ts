import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  @Output()
  saveComplete = new EventEmitter<boolean>();
  constructor() { }

  ngOnInit(): void {
  }

}
