import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { PolicyDocumentComponent } from './policy-document/policy-document.component';
import { ResultComponent } from './result/result.component';
import { FormViewerComponent } from './form-viewer/form-viewer.component';
import { SurveyPrimaryComponent } from './survey/primary.component';
import { SurveyTestComponent } from './survey/test.component';
import { SurveyResolver } from './services/survey-resolver.service';
import { NewsletterConfirmationComponent } from './newsletter-confirmation/newsletter-confirmation.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { PaymentConfirmationComponent } from './payment-confirmation/payment-confirmation.component';
import { CanDeactivateGuard } from './services/can-deactivate-guard.service';
import { BCeidAuthGuard } from './services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from './services/service-card-auth-guard.service';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ApplicationComponent } from './application/application.component';
import { WorkerQualificationComponent } from './worker-qualification/worker-qualification.component';
import { WorkerDashboardComponent } from './worker-qualification/dashboard/dashboard.component';
import { WorkerApplicationComponent } from './worker-qualification/worker-application/worker-application.component';
import { UserConfirmationComponent } from './worker-qualification/user-confirmation/user-confirmation.component';
import { WorkerPaymentConfirmationComponent } from './worker-qualification/payment-confirmation/payment-confirmation.component';
import { SpdConsentComponent } from './worker-qualification/spd-consent/spd-consent.component';
import { WorkerHomeComponent } from './worker-qualification/worker-home/worker-home.component';
import { LicenceFeePaymentConfirmationComponent } from './licence-fee-payment-confirmation/licence-fee-payment-confirmation.component';
import { AssosiateWizardComponent } from './associate-wizard/associate-wizard.component';
import { AccountProfileComponent } from './account-profile/account-profile.component';
import { LicenceRenewalStepsComponent } from '@app/licence-renewal-steps/licence-renewal-steps.component';
import { MapComponent } from './map/map.component';
import { FeatureGuard } from './services/feaure-guard.service';
import { ApplicationAndLicenceFeeComponent } from './application-and-licence-fee/application-and-licence-fee.component';
import { ApplicationOwnershipTransferComponent } from './application-ownership-transfer/application-ownership-transfer.component';
import { LicenseeTreeComponent } from '@shared/licensee-tree/licensee-tree.component';
import { FederalReportingComponent } from './federal-reporting/federal-reporting.component';
import { ApplicationLicenseeChangesComponent } from './application-licensee-changes/application-licensee-changes.component';


const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'federal-reporting',
    component: FederalReportingComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'FederalReporting' }
  },
  {
    path: 'licensee-changes',
    component: LicenseeTreeComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licensee-changes/:applicationId',
    component: ApplicationLicenseeChangesComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'LicenseeChanges' }
  },
  {
    path: 'account-profile',
    component: AccountProfileComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'renew-crs-licence/application/:applicationId',
    component: LicenceRenewalStepsComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'account-profile/:applicationId',
    component: AccountProfileComponent,
    // canDeactivate: [CanDeactivateGuard],
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'dashboard-lite',
    component: DashboardComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'associate-wizard',
    component: AssosiateWizardComponent,
  },
  {
    path: 'application/:applicationId',
    component: ApplicationComponent,
    canDeactivate: [CanDeactivateGuard],
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'store-opening/:applicationId',
    component: ApplicationAndLicenceFeeComponent,
    canDeactivate: [CanDeactivateGuard],
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'ownership-transfer/:licenceId',
    component: ApplicationOwnershipTransferComponent,
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
  {
    path: 'form-viewer/:id',
    component: FormViewerComponent
  },
  {
    path: 'policy-document/worker-qualification-training',
    component: WorkerHomeComponent,
    data: { slug: 'worker-qualification-training' }
  },
  {
    path: 'policy-document/worker-qualification-home',
    component: WorkerHomeComponent,
    data: { slug: 'worker-qualification-home' }
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
  {
    path: 'map',
    component: MapComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'Maps' }
  },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule],
  providers: [SurveyResolver]
})
export class AppRoutingModule { }
