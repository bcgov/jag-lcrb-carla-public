import { Component, Injector, Input } from '@angular/core';
import { SurveyComponent } from './survey.component';
//import { SurveyModel } from 'survey-angular';

@Component({
  selector: 'survey-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SurveySidebarComponent  {
  title: string;
  links: any[];
  private survey: SurveyComponent;
  /*
  constructor(private injector : Injector) {
    // survey will be passed by the injector when instantiated by InsertComponent
    this.survey = <SurveyComponent>this.injector.get('survey');
    this.survey.onPageUpdate.subscribe(survey => {
      this.updateContent(survey);
    });
  }

  updateContent(model : SurveyModel) {
    if(model) {
      this.title = 'Survey Steps'; // model.title;
      let links = [];
      model.pages.forEach( (page, idx) => {
        links.push({
          index: idx,
          title: page.title || page.name,
          active: idx === model.currentPageNo});
      });
      this.links = links;
    }
  }
  */
  changePage(pageNo: number) {
    //this.survey.changePage(pageNo);
  }

}

