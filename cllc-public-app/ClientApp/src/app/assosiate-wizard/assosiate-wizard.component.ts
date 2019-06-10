import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-assosiate-wizard',
  templateUrl: './assosiate-wizard.component.html',
  styleUrls: ['./assosiate-wizard.component.css']
})
export class AssosiateWizardComponent implements OnInit {
  currentQuestion = 'q1';
  answerCollection: any = {};
  constructor() { }

  ngOnInit() {
  }

}
