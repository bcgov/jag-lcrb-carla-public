import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-applicant',
  templateUrl: './applicant.component.html',
  styleUrls: ['./applicant.component.scss']
})
export class ApplicantComponent implements OnInit {
  @Output()
  saveComplete = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }
}
