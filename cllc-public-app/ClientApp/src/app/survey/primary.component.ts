import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-survey-primary',
  templateUrl: './primary.component.html',
  styleUrls: ['./primary.component.scss']
})
export class SurveyPrimaryComponent implements OnInit {

  public survey: any;
  public complete: Function;

  constructor(private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.survey = this.route.snapshot.data.survey;
    this.complete = (data) => this.onComplete(data);
  }

  onComplete(data) {
    if (this.route.snapshot.url[0].path === 'qualify') {
      const ok = (data.question1 === 'y') ? 'qualified' : 'unqualified';
      this.router.navigate(['result', ok]);
    }
  }

}
