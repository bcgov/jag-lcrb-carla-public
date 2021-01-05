import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { faDownload } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-terms-of-use',
  templateUrl: './terms-of-use.component.html',
  styleUrls: ['./terms-of-use.component.scss']
})
export class TermsOfUseComponent implements OnInit {
  faDownload = faDownload;
  @Output() termsAccepted = new EventEmitter<boolean>();
  window = window;
  _termsAccepted: boolean;

  constructor() { }

  ngOnInit() {
  }
}
