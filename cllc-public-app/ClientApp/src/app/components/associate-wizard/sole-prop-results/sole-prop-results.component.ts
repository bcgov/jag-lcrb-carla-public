import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-sole-prop-results',
  templateUrl: './sole-prop-results.component.html',
  styleUrls: ['./sole-prop-results.component.css']
})
export class SolePropResultsComponent implements OnInit {
  @Input() answerCollection: any = {};
  constructor() { }

  ngOnInit() {
  }

}
