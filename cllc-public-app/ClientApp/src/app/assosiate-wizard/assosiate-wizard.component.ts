import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-assosiate-wizard',
  templateUrl: './assosiate-wizard.component.html',
  styleUrls: ['./assosiate-wizard.component.scss']
})
export class AssosiateWizardComponent implements OnInit {
  currentQuestion = 'q1';
  questionHistory: string[] = [];
  answerCollection: any = {};
  constructor() { }

  ngOnInit() {
  }


  getOrganizationSection(answer: string): string {
    let nextQuestion = '';
    switch (answer) {
      case 'Public Corporation':
        nextQuestion = 'q4a';
        break;
      case 'Private Corporation':
        nextQuestion = 'q5a';
        break;
      case 'Limited Partnership':
        nextQuestion = 'q6a';
        break;
      case 'General Partnership':
        nextQuestion = 'q7a';
        break;
      case 'Limited Liability Partnership':
        nextQuestion = 'q8a';
        break;
      case 'Society':
        nextQuestion = 'q9a';
        break;
    }
    return nextQuestion;
  }

  setNextQuestion(question: string) {
    this.questionHistory.push(this.currentQuestion);
    this.currentQuestion = question;
  }

  goToPrevious() {
    this.currentQuestion = this.questionHistory.pop();
  }

}
