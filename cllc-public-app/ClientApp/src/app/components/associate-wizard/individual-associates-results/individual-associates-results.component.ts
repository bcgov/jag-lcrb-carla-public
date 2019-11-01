import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-individual-associates-results',
  templateUrl: './individual-associates-results.component.html',
  styleUrls: ['./individual-associates-results.component.css']
})
export class IndividualAssociatesResultsComponent implements OnInit {
  @Input() answerCollection: any = {};
  constructor() { }

  ngOnInit() {
  }

}
