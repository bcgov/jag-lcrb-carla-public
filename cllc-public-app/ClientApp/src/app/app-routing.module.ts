import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BusinessProfileComponent } from './business-profile/business-profile.component';
import { HomeComponent } from './home/home.component';
import { PolicyDocumentComponent } from './policy-document/policy-document.component';
import { ResultComponent } from './result/result.component';
import { FormViewerComponent } from './form-viewer/form-viewer.component';
import { SurveyPrimaryComponent } from './survey/primary.component';
import { SurveyTestComponent } from './survey/test.component';
import { SurveyResolver } from './services/survey-resolver.service';
import { NewsletterConfirmationComponent } from './newsletter-confirmation/newsletter-confirmation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { LicenseApplicationComponent } from './license-application/license-application.component';
import { PaymentConfirmationComponent } from './payment-confirmation/payment-confirmation.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { BeforeYouStartComponent } from './business-profile/tabs/before-you-start/before-you-start.component';
import { CorporateDetailsComponent } from './business-profile/tabs/corporate-details/corporate-details.component';
import { OrganizationStructureComponent } from './business-profile/tabs/organization-structure/organization-structure.component';
import { DirectorsAndOfficersComponent } from './business-profile/tabs/directors-and-officers/directors-and-officers.component';
import { KeyPersonnelComponent } from './business-profile/tabs/key-personnel/key-personnel.component';
import { EditShareholdersComponent } from './business-profile/tabs/edit-shareholders/edit-shareholders.component';
import { ConnectionToProducersComponent } from './business-profile/tabs/connection-to-producers/connection-to-producers.component';
import { FinancialInformationComponent } from './business-profile/tabs/financial-information/financial-information.component';
import { SecurityAssessmentsComponent } from './business-profile/tabs/security-assessments/security-assessments.component';
import { CanDeactivateGuard } from './services/can-deactivate-guard.service';
import { BCeidAuthGuard } from './services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from './services/service-card-auth-guard.service';
import { ContactDetailsComponent } from './license-application/tabs/contact-details/contact-details.component';
import { DeclarationComponent } from './license-application/tabs/declaration/declaration.component';
import { FloorPlanComponent } from './license-application/tabs/floor-plan/floor-plan.component';
import { PropertyDetailsComponent } from './license-application/tabs/property-details/property-details.component';
import { SiteMapComponent } from './license-application/tabs/site-map/site-map.component';
import { StoreInformationComponent } from './license-application/tabs/store-information/store-information.component';
import { SubmitPayComponent } from './license-application/tabs/submit-pay/submit-pay.component';
import { DashboardLiteComponent } from './dashboard-lite/dashboard-lite.component';
import { ApplicationComponent } from './lite/application/application.component';
import { AssociatesDashboardComponent } from './lite/associates-dashboard/associates-dashboard.component';
import { WorkerQualificationComponent } from './worker-qualification/worker-qualification.component';
import { WorkerDashboardComponent } from './worker-qualification/dashboard/dashboard.component';
import { WorkerApplicationComponent } from './worker-qualification/worker-application/worker-application.component';
import { UserConfirmationComponent } from './worker-qualification/user-confirmation/user-confirmation.component';
import { PrePaymentComponent } from './worker-qualification/pre-payment/pre-payment.component';
import { WorkerPaymentConfirmationComponent } from './worker-qualification/payment-confirmation/payment-confirmation.component';
import { SpdConsentComponent } from './worker-qualification/spd-consent/spd-consent.component';
import { WorkerHomeComponent } from './worker-qualification/worker-home/worker-home.component';
import { LicenceFeePaymentConfirmationComponent } from './licence-fee-payment-confirmation/licence-fee-payment-confirmation.component';
import { WorkerInformationComponent } from './worker-qualification/worker-information/worker-information.component';

const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'dashboard',
    component: DashboardLiteComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'dashboard-lite',
    component: DashboardLiteComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'associates-lite',
    component: AssociatesDashboardComponent,
    canActivate: [ServiceCardAuthGuard]
  },
  {
    path: 'application-lite/:applicationId',
    component: ApplicationComponent,
    canDeactivate: [CanDeactivateGuard],
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'worker-qualification/home',
    component: WorkerHomeComponent
  },
  {
    path: 'worker-qualification/user-comfirmation',
    component: UserConfirmationComponent,
    canActivate: [ServiceCardAuthGuard]
  },
  {
    path: 'worker-qualification/payment-confirmation',
    component: WorkerPaymentConfirmationComponent,
    canActivate: [ServiceCardAuthGuard]
  },
  {
    path: 'worker-qualification/dashboard',
    component: WorkerDashboardComponent,
    canActivate: [ServiceCardAuthGuard]
  },
  {
    path: 'worker-qualification/application/:id',
    component: WorkerApplicationComponent,
    canDeactivate: [CanDeactivateGuard],
    canActivate: [ServiceCardAuthGuard]
  },
  {
    path: 'worker-qualification/spd-consent/:id',
    component: SpdConsentComponent,
    canDeactivate: [CanDeactivateGuard],
    canActivate: [ServiceCardAuthGuard]
  },
  // {
  //   path: 'worker-qualification',
  //   component: WorkerqualificationComponent,
  //   canActivate: [ServiceCardAuthGuard],
  //   children: [
  //   ]
  // },
  // {
  //   path: 'business-profile/:accountId/:legalEntityId',
  //   component: BusinessProfileComponent,
  //   children: [
  //     {
  //       path: 'before-you-start',
  //       component: BeforeYouStartComponent
  //     },
  //     {
  //       path: 'corporate-details',
  //       component: CorporateDetailsComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'organization-structure',
  //       component: OrganizationStructureComponent
  //     },
  //     {
  //       path: 'directors-and-officers',
  //       component: DirectorsAndOfficersComponent
  //     },
  //     {
  //       path: 'key-personnel',
  //       component: KeyPersonnelComponent
  //     },
  //     {
  //       path: 'shareholders',
  //       component: EditShareholdersComponent
  //     },
  //     {
  //       path: 'connections-to-producers',
  //       component: ConnectionToProducersComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'finance-integrity',
  //       component: FinancialInformationComponent
  //     },
  //     {
  //       path: 'security-assessment',
  //       component: SecurityAssessmentsComponent
  //     },
  //   ]
  // },
  {
    path: 'form-viewer/:id',
    component: FormViewerComponent
  },
  {
    path: 'policy-document/worker-qualification-training',
    component: WorkerInformationComponent
  },
  {
    path: 'policy-document/:slug',
    component: PolicyDocumentComponent
  },
  {
    path: 'newsletter-confirm/:slug',
    component: NewsletterConfirmationComponent
  },
  // {
  //   path: 'license-application/:applicationId',
  //   component: LicenseApplicationComponent,
  //   children: [
  //     {
  //       path: 'contact-details',
  //       component: ContactDetailsComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'declaration',
  //       component: DeclarationComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'floor-plan',
  //       component: FloorPlanComponent
  //     },
  //     {
  //       path: 'property-details',
  //       component: PropertyDetailsComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'site-map',
  //       component: SiteMapComponent
  //     },
  //     {
  //       path: 'store-information',
  //       component: StoreInformationComponent,
  //       canDeactivate: [CanDeactivateGuard]
  //     },
  //     {
  //       path: 'submit-pay',
  //       component: SubmitPayComponent
  //     },
  //   ]
  // },
  {
    path: 'payment-confirmation',
    component: PaymentConfirmationComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence-fee-payment-confirmation',
    component: LicenceFeePaymentConfirmationComponent,
    canActivate: [BCeidAuthGuard]
  },

  {
    path: 'prv/survey',
    component: SurveyPrimaryComponent,
    resolve: {
      survey: SurveyResolver,
    },
    data: {
      // do not show breadcrumb
      // breadcrumb: 'Potential Applicant Survey',
      survey_path: 'assets/survey-primary.json',
    }
  },
  {
    path: 'prv',
    redirectTo: 'prv/survey'
  },
  {
    path: 'worker-qualification',
    component: WorkerQualificationComponent
  },
  {
    path: 'result/:data',
    component: ResultComponent,
    data: {
    }
  },
  {
    path: 'survey-test',
    component: SurveyTestComponent,
    data: {
      breadcrumb: 'Survey Test'
    }
  },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [SurveyResolver]
})
export class AppRoutingModule { }
