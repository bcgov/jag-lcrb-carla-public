import { Component, Input, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as SurveyKO from 'survey-knockout';
import * as SurveyEditor from 'surveyjs-editor';
import { addQuestionTypes, addToolboxOptions } from './question-types';

@Component({
  selector: 'survey-editor',
  template: `<div class="survey-editor-outer"><div id="editorElement"></div></div>`,
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    '../../../node_modules/surveyjs-editor/surveyeditor.css',
    './editor.component.scss'
  ]
})
export class SurveyEditorComponent  {
  @Input() jsonData: any;
  @Input() onComplete: Function;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {

    if(! this.jsonData) {
      this.jsonData = this.route.snapshot.data.survey;
    }

    addQuestionTypes(SurveyKO);

    var editorOptions = {
      // showTestSurveyTab: false
    };
    var editor = new SurveyEditor.SurveyEditor("editorElement", editorOptions);
    if(this.jsonData) {
      editor.text = JSON.stringify(this.jsonData, null, 2);
    }

    addToolboxOptions(editor);
  }
}

