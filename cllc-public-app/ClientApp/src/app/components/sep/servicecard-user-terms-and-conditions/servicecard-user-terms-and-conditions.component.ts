import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { faDownload } from "@fortawesome/free-solid-svg-icons"

@Component({
  selector: 'app-servicecard-user-terms-and-conditions',
  templateUrl: './servicecard-user-terms-and-conditions.component.html',
  styleUrls: ['./servicecard-user-terms-and-conditions.component.scss']
})
export class ServicecardUserTermsAndConditionsComponent implements OnInit {
  faDownload = faDownload;
  @Output() termsAccepted = new EventEmitter<boolean>();
  form = this.fb.group({
    agreement: false
  });

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
  }
}
