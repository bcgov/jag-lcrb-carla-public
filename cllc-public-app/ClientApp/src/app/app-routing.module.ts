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
import { CannabisAssociateScreeningComponent } from '@components/cannabis-associate-screening/cannabis-associate-screening.component';
import { PersonalHistorySummaryComponent } from '@components/personal-history-summary/personal-history-summary.component';
import { SecurityScreeningConfirmationComponent } from '@components/security-screening-confirmation/security-screening-confirmation.component';
import { MultiStageApplicationFlowComponent } from '@components/multi-stage-application-flow/multi-stage-application-flow.component';
import { CateringEventFormComponent } from '@components/catering-event/catering-event-form.component';
import { EventSecurityFormComponent } from '@components/catering-event/security.component';
import { SecurityScreeningRequirementsComponent } from '@components/security-screening-requirements/security-screening-requirements.component';
import { EligibilityFormComponent } from '@components/eligibility-form/eligibility-form.component';
import { LiquorRenewalComponent } from '@components/applications/liquor-renewal/liquor-renewal.component';
import { TemporaryOffsiteComponent } from '@components/temporary-offsite/temporary-offsite.component';
import { ApplicationThirdPartyOperatorComponent } from '@components/applications/application-third-party-operator/application-third-party-operator.component';
import { CancelThirdPartyOperatorComponent } from '@components/applications/cancel-third-party-operator/cancel-third-party-operator.component';
import { ApplicationCovidTemporaryExtensionComponent } from '@components/applications/application-covid-temporary-extension/application-covid-temporary-extension.component';
import { CovidConfirmationComponent } from '@components/applications/application-covid-temporary-extension/covid-confirmation/covid-confirmation.component';
import { TerminateTPORelationshipComponent } from '@components/applications/terminate-tpo-relationship/terminate-tpo-relationship.component';
import { LgApprovalsComponent } from '@components/lg-approvals/lg-approvals.component';
import { LicenceRepresentativeFormComponent } from '@components/licence-representative-form/licence-representative-form.component';
import { MarketEventComponent } from '@components/market-event/market-event.component';
import { PermanentChangesToALicenseeComponent } from '@components/applications/permanent-changes-to-a-licensee/permanent-changes-to-a-licensee.component';
import { OffsiteStorageComponent } from '@components/offsite-storage/offsite-storage.component';


const routes: Routes = [
  {
    path: '',
    component: HomeComponent
  },
  {
    path: 'covid-temporary-extension',
    component: ApplicationCovidTemporaryExtensionComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'CovidApplication' }
  },
  {
    path: 'covid-confirmation',
    component: CovidConfirmationComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'CovidApplication' }
  },
  {
    path: 'org-structure',
    component: ApplicationLicenseeChangesComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    canDeactivate: [CanDeactivateGuard], // Comment this out if there are problems with duplicate saves
    data: { feature: 'LicenseeChanges' }
  },
  {
    path: 'lg-approvals',
    component: LgApprovalsComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    canDeactivate: [CanDeactivateGuard],
    data: { feature: 'LGApprovals' }
  },
  {
    path: 'security-screening/confirmation',
    component: SecurityScreeningConfirmationComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'LicenseeChanges' }
  },
  {
    path: 'personal-history-summary/:token',
    component: PersonalHistorySummaryComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'LicenseeChanges' }
  },
  {
    path: 'cannabis-associate-screening/:token',
    component: CannabisAssociateScreeningComponent,
    canActivate: [FeatureGuard],
    data: { feature: 'LicenseeChanges' }
  },

  {
    path: 'licences',
    component: LicencesComponent,
    canActivate: [BCeidAuthGuard],
  },
  {
    path: 'licence/:licenceId/representative',
    component: LicenceRepresentativeFormComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/event/:eventId/security',
    component: EventSecurityFormComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/event',
    component: CateringEventFormComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/event/:eventId',
    component: CateringEventFormComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/temporary-offsite',
    component: TemporaryOffsiteComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/temporary-offsite/:eventId',
    component: TemporaryOffsiteComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/market-event',
    component: MarketEventComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/market-event/:eventId',
    component: MarketEventComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'licence/:licenceId/offsite-storage',
    component: OffsiteStorageComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'federal-reporting/:licenceId/:monthlyReportId',
    component: FederalReportingComponent,
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
    path: 'renew-licence/:licenceType/:applicationId',
    component: LicenceRenewalStepsComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'permanent-changes-to-a-licensee',
    component: PermanentChangesToALicenseeComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'PermanentChangesToLicensee' }
  },
  {
    path: 'permanent-changes-to-a-licensee/:invoiceType',
    component: PermanentChangesToALicenseeComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    data: { feature: 'PermanentChangesToLicensee' }
  },
  {
    path: 'multi-step-application/:applicationId',
    component: MultiStageApplicationFlowComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    canDeactivate: [CanDeactivateGuard],
    data: { feature: 'LicenseeChanges' }
  },
  {
    path: 'multi-step-application/:stepType/:applicationId',
    component: MultiStageApplicationFlowComponent,
    canActivate: [BCeidAuthGuard, FeatureGuard],
    canDeactivate: [CanDeactivateGuard],
    data: { feature: 'LicenseeChanges' }
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
    path: 'ownership-cancel-transfer/:licenceId',
    component: ApplicationCancelOwnershipTransferComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'cancel-third-party-operator/:licenceId',
    component: CancelThirdPartyOperatorComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'terminate-third-party-operator/:licenceId',
    component: TerminateTPORelationshipComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'ownership-transfer/:licenceId',
    component: ApplicationOwnershipTransferComponent,
    canActivate: [BCeidAuthGuard]
  },
  {
    path: 'third-party-operator/:licenceId',
    component: ApplicationThirdPartyOperatorComponent,
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
    path: 'security-screening',
    component: SecurityScreeningRequirementsComponent
  },
  {
    path: 'result/:data',
    component: ResultComponent,
    data: {}
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
  {
    path: 'eligibility',
    component: EligibilityFormComponent
  },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule],
  providers: [SurveyResolver]
})
export class AppRoutingModule { }
