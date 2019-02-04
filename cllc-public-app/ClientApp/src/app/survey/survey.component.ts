import { Component, Input } from '@angular/core';
import * as Survey from 'survey-angular';
import { InsertService } from '../insert/insert.service';
import { addQuestionTypes } from './question-types';
import { BehaviorSubject } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'survey',
  template: `<div class="survey-container contentcontainer codecontainer"><div id="surveyElement"></div></div>`,
  styleUrls: ['./survey.component.scss']
})
export class SurveyComponent {
  @Input() jsonData: any;
  @Input() onComplete: Function;
  public surveyModel: Survey.SurveyModel;
  public onPageUpdate: BehaviorSubject<Survey.SurveyModel> = new BehaviorSubject<Survey.SurveyModel>(null);
  private clientId: string;
  constructor(private insertService: InsertService, private _router: Router) { }

  ngOnInit() {
    addQuestionTypes(Survey);

    let surveyModel = new Survey.Model(this.jsonData);
    surveyModel.showQuestionNumbers = 'off';
    // Survey.Survey.cssType = "bootstrap";  //This breaks IE 11
    Survey.defaultBootstrapCss.page.root = "sv_page";
    Survey.defaultBootstrapCss.pageDescription = "sv_page_description";
    Survey.defaultBootstrapCss.pageTitle = "sv_page_title";
    Survey.defaultBootstrapCss.navigationButton = "btn btn-primary";
    Survey.defaultBootstrapCss.question.title = "sv_q_title";
    Survey.defaultBootstrapCss.question.description = "sv_q_description small";
    Survey.defaultBootstrapCss.panel.title = "sv_p_title";
    Survey.defaultBootstrapCss.panel.container = "sv_p_container";
    Survey.defaultBootstrapCss.panel.description = "sv_p_description";
    Survey.defaultBootstrapCss.row = "sv_row";
    Survey.defaultBootstrapCss.matrixdynamic.button = "btn btn-default";
    Survey.defaultBootstrapCss.paneldynamic.button = "btn btn-default";
    Survey.defaultBootstrapCss.paneldynamic.root = "sv_p_dynamic"; // not used?
    Survey.dxSurveyService.serviceUrl = "api/survey";

    surveyModel.onComplete.add((sender, options) => {
      // generate a client identifier.  replace this with a hashed value.
      let date = new Date();
      this.clientId = date.getTime() + "-survey";

      surveyModel.sendResult("PotentialApplicantResult", this.clientId);
      if (this.onComplete) {
        this.onComplete(sender.data);        
      }
    });

    // redirect the user after the send to database is finished.
    surveyModel.onSendResult.add((sender, options) => {
      // redirect to results page
      var data = JSON.stringify(sender.data);
      // redirect to the results page.
      this._router.navigate(['result', this.clientId])
    });
    
    surveyModel.onCurrentPageChanged.add((sender, options) => {
      this.onPageUpdate.next(sender)
    });

    Survey.SurveyNG.render('surveyElement', { model: surveyModel });
    this.surveyModel = surveyModel;

    //  this.insertService.updateInsert('sidebar-left',
    //    {type: 'survey-sidebar', inputs: {survey: this}});
    //  this.onPageUpdate.next(surveyModel);
    //}

    //changePage(pageNo: number) {
    //  this.surveyModel.currentPageNo = pageNo;
    //}

  }

}
