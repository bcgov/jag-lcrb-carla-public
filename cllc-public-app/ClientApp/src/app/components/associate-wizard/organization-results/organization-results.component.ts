import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-organization-results',
  templateUrl: './organization-results.component.html',
  styleUrls: ['./organization-results.component.css']
})
export class OrganizationResultsComponent implements OnInit {
  @Input() answerCollection: any = {};
  constructor() { }

  ngOnInit() {
  }

}
