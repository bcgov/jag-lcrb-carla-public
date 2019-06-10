import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-terms-of-use',
  templateUrl: './terms-of-use.component.html',
  styleUrls: ['./terms-of-use.component.scss']
})
export class TermsOfUseComponent implements OnInit {

  @Output() termsAccepted = new EventEmitter<boolean>();
  window = window;
  _termsAccepted: boolean;

  constructor() { }

  ngOnInit() {
  }
}
