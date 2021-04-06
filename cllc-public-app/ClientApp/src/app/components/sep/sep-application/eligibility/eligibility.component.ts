import { Component, EventEmitter, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-eligibility',
  templateUrl: './eligibility.component.html',
  styleUrls: ['./eligibility.component.scss']
})
export class EligibilityComponent implements OnInit {
  @Output()
  saveComplete = new EventEmitter<boolean>();
  constructor() { }

  ngOnInit(): void {
  }

}
