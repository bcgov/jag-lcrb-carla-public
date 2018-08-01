import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-terms-and-conditions',
  templateUrl: './terms-and-conditions.component.html',
  styleUrls: ['./terms-and-conditions.component.scss']
})
export class TermsAndConditionsComponent implements OnInit {

  @Output() termsAccepted = new EventEmitter<boolean>();
  window = window
  _termsAccepted: boolean;

  constructor() { }

  ngOnInit() {
  }
}
