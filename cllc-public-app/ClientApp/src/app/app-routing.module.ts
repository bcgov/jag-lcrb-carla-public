import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ResultComponent } from './result/result.component';
import { SurveyPrimaryComponent } from './survey/primary.component';
import { SurveyTestComponent } from './survey/test.component';
import { SurveyResolver }   from './survey/survey-resolver.service';
import { SurveyEditorComponent } from './survey/editor.component';

const routes: Routes = [
  {
    path: '',
    //children: []
    component: HomeComponent
  },
  {
    path: 'prv',
    redirectTo: 'prv/survey'
  },
  {
    path: 'prv/survey',
    component: SurveyPrimaryComponent,
    resolve: {
      survey: SurveyResolver,
    },
    data: {
      breadcrumb: 'Potential Applicant Survey', //Provincial Family Test
      survey_path: 'assets/survey-primary.json',
    }
  },
  {
    path: 'result/:state',
    component: ResultComponent,
    data: {
      breadcrumb: 'Survey Results'
    }
  },
  {
    path: 'survey-test',
    component: SurveyTestComponent,
    data: {
      breadcrumb: 'Survey Test'
    }
  },
  {
    path: 'survey-editor',
    component: SurveyEditorComponent,
    resolve: {
      survey: SurveyResolver,
    },
    data: {
      breadcrumb: 'Survey Editor',
      survey_path: 'assets/survey-primary.json'
    }
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [SurveyResolver]
})
export class AppRoutingModule { }
