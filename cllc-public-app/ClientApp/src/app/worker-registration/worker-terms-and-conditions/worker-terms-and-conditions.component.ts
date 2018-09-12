import { Component, OnInit, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-worker-terms-and-conditions',
  templateUrl: './worker-terms-and-conditions.component.html',
  styleUrls: ['./worker-terms-and-conditions.component.scss']
})
export class WorkerTermsAndConditionsComponent implements OnInit {

  @Output() termsAccepted = new EventEmitter<boolean>();
  window = window;
  _termsAccepted: boolean;

  constructor() { }

  ngOnInit() {
  }
}
