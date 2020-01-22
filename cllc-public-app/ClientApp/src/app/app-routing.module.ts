import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from '@components/home/home.component';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { ResultComponent } from '@components/result/result.component';
import { FormViewerComponent } from '@components/form-viewer/form-viewer.component';
import { SurveyPrimaryComponent } from '@components/survey/primary.component';
import { SurveyTestComponent } from '@components/survey/test.component';
import { SurveyResolver } from '@services/survey-resolver.service';
import { NewsletterConfirmationComponent } from '@components/newsletter-confirmation/newsletter-confirmation.component';
import { NotFoundComponent } from '@components/not-found/not-found.component';
import { PaymentConfirmationComponent } from '@components/payment-confirmation/payment-confirmation.component';
import { CanDeactivateGuard } from '@services/can-deactivate-guard.service';
import { BCeidAuthGuard } from '@services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from '@services/service-card-auth-guard.service';
import { DashboardComponent } from '@components/dashboard/dashboard.component';
import { ApplicationComponent } from '@components/applications/application/application.component';
import { WorkerQualificationComponent } from '@components/worker-qualification/worker-qualification.component';
import { WorkerDashboardComponent } from '@components/worker-qualification/dashboard/dashboard.component';
import { WorkerApplicationComponent } from '@components/worker-qualification/worker-application/worker-application.component';
import { UserConfirmationComponent } from '@components/worker-qualification/user-confirmation/user-confirmation.component';
import { WorkerPaymentConfirmationComponent } from '@components/worker-qualification/payment-confirmation/payment-confirmation.component';
import { SpdConsentComponent } from '@components/worker-qualification/spd-consent/spd-consent.component';
import { WorkerHomeComponent } from '@components/worker-qualification/worker-home/worker-home.component';
import { LicenceFeePaymentConfirmationComponent } from '@components/licences/licence-fee-payment-confirmation/licence-fee-payment-confirmation.component';
import { AssosiateWizardComponent } from '@components/associate-wizard/associate-wizard.component';
import { AccountProfileComponent } from '@components/account-profile/account-profile.component';
import { LicenceRenewalStepsComponent } from '@components/licences/licence-renewal-steps/licence-renewal-steps.component';
import { MapComponent } from '@components/map/map.component';
import { FeatureGuard } from '@services/feaure-guard.service';
import { ApplicationCancelOwnershipTransferComponent } from '@components/applications/application-cancel-ownership-transfer/application-cancel-ownership-transfer.component';
import { ApplicationOwnershipTransferComponent } from '@components/applications/application-ownership-transfer/application-ownership-transfer.component';
import { FederalReportingComponent } from '@components/federal-reporting/federal-reporting.component';
import { ApplicationLicenseeChangesComponent } from '@components/applications/application-licensee-changes/application-licensee-changes.component';
import { LicencesComponent } from '@components/licences/licences.component';
import { ApplicationAndLicenceFeeComponent } from '@components/applications/application-and-licence-fee/application-and-licence-fee.component';
import { LicenceEventComponent } from '@components/licences/licence-event/licence-event.component';
import { CateringDemoComponent } from './components/catering-demo/catering-demo.component';
import { DynamicApplicationComponent } from './components/applications/dynamic-application/dynamic-application.component';
import { AssociateListComponent } from '@shared/components/associate-list/associate-list.component';
import { OrganizationStructureComponent } from '@components/account-profile/tabs/organization-structure/organization-structure.component';
import { PersonalHistorySummaryComponent } from '@components/personal-history-summary/personal-history-summary.component';


const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'org-structure',
    component: OrganizationStructureComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],

  },
  {
    path: 'personal-history-summary/:token',
    component: PersonalHistorySummaryComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'OrgStructure' }
  },
  {
    path: 'catering-demo',
    component: CateringDemoComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licences',
    component: LicencesComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'FederalReporting' }
  },  
  {
    path: 'licence-event',
    component: LicenceEventComponent,
    canActivate: [BCeidAuthGuard],
  
  },
  {
    path: 'federal-reporting/:licenceId',
    component: FederalReportingComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'FederalReporting' }
  },
  {
    path: 'federal-reporting/:licenceId/:monthlyReportId',
    component: FederalReportingComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'FederalReporting' }
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
    path: 'application/:formmName/:applicationId',
    component: DynamicApplicationComponent,
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
    path: 'ownership-cancel-transfer/:licenceId',
      component: ApplicationCancelOwnershipTransferComponent,
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
