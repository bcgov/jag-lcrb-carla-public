import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SurveyDataService } from '../survey/survey-data.service';

@Component({
  selector: 'app-result',
  templateUrl: './result.component.html',
  styleUrls: ['./result.component.scss']
})
export class ResultComponent implements OnInit {
  public clientId: string;
  public data: string;

  constructor(private route: ActivatedRoute, private surveyDataService: SurveyDataService) { }

  ngOnInit() {
    this.route.params.subscribe((data) => {
      this.clientId = data.data;
      console.log(data);
      console.log(this.clientId);
      if (this.clientId != null) {
        this.surveyDataService.getSurveyData(this.clientId)
          .then((surveyResult) => {
            this.data = JSON.parse(surveyResult);            
          });
      }        
    });
  }

}
