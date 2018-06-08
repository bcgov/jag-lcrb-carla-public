import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BusinessProfileComponent } from './business-profile/business-profile.component';
import { HomeComponent } from './home/home.component';
import { PolicyDocumentComponent } from './policy-document/policy-document.component';
import { ResultComponent } from './result/result.component';
import { FormViewerComponent } from './form-viewer/form-viewer.component';
import { SurveyPrimaryComponent } from './survey/primary.component';
import { SurveyTestComponent } from './survey/test.component';
import { SurveyResolver }   from './services/survey-resolver.service';
import { NewsletterConfirmationComponent } from './newsletter-confirmation/newsletter-confirmation.component';
import { ApplicationsListComponent } from './applications-list/applications-list.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { LicenseApplicationSummaryComponent } from './license-application-summary/license-application-summary.component';
import { DashboardComponent } from './dashboard/dashboard.component';

const routes: Routes = [
  {
    path: '',
    //children: []
    component: HomeComponent
  },
  {
    path: 'dashboard',
    //children: []
    component: DashboardComponent
  },
  {
    path: 'business-profile',
    component: BusinessProfileComponent
  },
  {
    path: 'form-viewer/:id',
    component: FormViewerComponent
  },
  {
    path: 'policy-document/:slug',
    component: PolicyDocumentComponent
  },
  {
    path: 'newsletter-confirm/:slug',
    component: NewsletterConfirmationComponent
  },
  {
    path: 'applications-list',
    component: ApplicationsListComponent
  },
  {
    path: 'lic-app-summary',
    component: LicenseApplicationSummaryComponent
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
      // do not show breadcrumb
      //breadcrumb: 'Potential Applicant Survey', 
      survey_path: 'assets/survey-primary.json',
    }
  },
  {
    path: 'result/:data',
    component: ResultComponent,
    data: {
      // do not show breadcrumb
      //breadcrumb: 'Survey Results'
    }
  },
  {
    path: 'survey-test',
    component: SurveyTestComponent,
    data: {
      breadcrumb: 'Survey Test'
    }
  },
  //{ path: '404', component: NotFoundComponent },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [SurveyResolver]
})
export class AppRoutingModule { }
