import { CdkTableModule } from "@angular/cdk/table";
import { HttpClientModule } from "@angular/common/http";
import { APP_INITIALIZER, NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatAutocompleteModule } from "@angular/material/autocomplete";
import { MatBadgeModule } from "@angular/material/badge";
import { MatButtonModule } from "@angular/material/button";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatCardModule } from "@angular/material/card";
import { MatCheckboxModule } from "@angular/material/checkbox";
import { MatChipsModule } from "@angular/material/chips";
import { MatNativeDateModule, MatRippleModule } from "@angular/material/core";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatDialogModule } from "@angular/material/dialog";
import { MatDividerModule } from "@angular/material/divider";
import { MatExpansionModule } from "@angular/material/expansion";
import { MatGridListModule } from "@angular/material/grid-list";
import { MatIconModule } from "@angular/material/icon";
import { MatInputModule } from "@angular/material/input";
import { MatListModule } from "@angular/material/list";
import { MatMenuModule } from "@angular/material/menu";
import { MatPaginatorModule } from "@angular/material/paginator";
import { MatProgressBarModule } from "@angular/material/progress-bar";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { MatRadioModule } from "@angular/material/radio";
import { MatSelectModule } from "@angular/material/select";
import { MatSidenavModule } from "@angular/material/sidenav";
import { MatSlideToggleModule } from "@angular/material/slide-toggle";
import { MatSliderModule } from "@angular/material/slider";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { MatSortModule } from "@angular/material/sort";
import { MatStepperModule } from "@angular/material/stepper";
import { MatTableModule } from "@angular/material/table";
import { MatTabsModule } from "@angular/material/tabs";
import { MatToolbarModule } from "@angular/material/toolbar";
import { MatTooltipModule } from "@angular/material/tooltip";
import { MatTreeModule } from "@angular/material/tree";
import { BrowserModule, Title } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { BceidConfirmationComponent } from "@components/bceid-confirmation/bceid-confirmation.component";
import { BreadcrumbComponent } from "@components/breadcrumb/breadcrumb.component";
import { DynamicsFormComponent } from "@components/dynamics-form/dynamics-form.component";
import { AccountDataService } from "@services/account-data.service";
import { AnnualVolumeService } from "@services/annual-volume.service";
import { ApplicationDataService } from "@services/application-data.service";
import { ContactDataService } from "@services/contact-data.service";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { DynamicsFormDataService } from "@services/dynamics-form-data.service";
import { FileDataService } from "@services/file-data.service";
import { LegalEntityDataService } from "@services/legal-entity-data.service";
import { LicenseDataService } from "@services/license-data.service";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { PaymentDataService } from "@services/payment-data.service";
import { CurrencyMaskModule } from "ng2-currency-mask";
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { GeneralDataService } from "./general-data.service";

import { AccordionComponent } from "@components/accordion/accordion.component";
import {
  EditShareholdersComponent,
  ShareholderDialogComponent,
} from "@components/account-profile/tabs/shareholders/shareholders.component";
import { FormViewerComponent } from "@components/form-viewer/form-viewer.component";
import { HomeComponent } from "@components/home/home.component";
import { InsertComponent } from "@components/insert/insert.component";
import { InsertService } from "@components/insert/insert.service";
import { NewsletterConfirmationComponent } from "@components/newsletter-confirmation/newsletter-confirmation.component";
import { NewsletterSignupComponent } from "@components/newsletter-signup/newsletter-signup.component";
import { NotFoundComponent } from "@components/not-found/not-found.component";
import { PolicyDocumentSidebarComponent } from "@components/policy-document-sidebar/policy-document-sidebar.component";
import { PolicyDocumentComponent } from "@components/policy-document/policy-document.component";
import { ResultComponent } from "@components/result/result.component";
import { StaticComponent } from "@components/static/static.component";
import { StatusBadgeComponent } from "@components/status-badge/status-badge.component";
import { SurveyPrimaryComponent } from "@components/survey/primary.component";
import { SurveySidebarComponent } from "@components/survey/sidebar.component";
import { SurveyComponent } from "@components/survey/survey.component";
import { SurveyTestComponent } from "@components/survey/test.component";
import { VoteComponent } from "@components/vote/vote.component";
import { NewsletterDataService } from "@services/newsletter-data.service";
import { PolicyDocumentDataService } from "@services/policy-document-data.service";
import { SurveyDataService } from "@services/survey-data.service";
import { UserDataService } from "@services/user-data.service";
import { VoteDataService } from "@services/vote-data.service";
import { DelayedFileUploaderComponent } from "@shared/components/delayed-file-uploader/delayed-file-uploader.component";
import { FileUploaderComponent } from "@shared/components/file-uploader/file-uploader.component";

import { BeforeYouStartComponent } from "@components/account-profile/tabs/before-you-start/before-you-start.component";
import {
  DirectorAndOfficerPersonDialogComponent,
  DirectorsAndOfficersComponent
} from "@components/account-profile/tabs/directors-and-officers/directors-and-officers.component";

import { ConnectionToProducersComponent } from "@components/account-profile/tabs/connection-to-producers/connection-to-producers.component";
import { KeyPersonnelComponent, KeyPersonnelDialogComponent } from "@components/account-profile/tabs/key-personnel/key-personnel.component";
import { LicenceFeePaymentConfirmationComponent } from "@components/licences/licence-fee-payment-confirmation/licence-fee-payment-confirmation.component";
import { PaymentConfirmationComponent } from "@components/payment-confirmation/payment-confirmation.component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { NgBusyModule } from "ng-busy";

import { AccountProfileComponent } from "@components/account-profile/account-profile.component";
import { ApplicationAndLicenceFeeComponent } from "@components/applications/application-and-licence-fee/application-and-licence-fee.component";
import { ApplicationCancelOwnershipTransferComponent } from "@components/applications/application-cancel-ownership-transfer/application-cancel-ownership-transfer.component";
import { ApplicationLicenseeChangesComponent } from "@components/applications/application-licensee-changes/application-licensee-changes.component";
import { ApplicationOwnershipTransferComponent } from "@components/applications/application-ownership-transfer/application-ownership-transfer.component";
import { ApplicationRenewalComponent } from "@components/applications/application-renewal/application-renewal.component";
import { ApplicationThirdPartyOperatorComponent } from "@components/applications/application-third-party-operator/application-third-party-operator.component";
import { ApplicationComponent } from "@components/applications/application/application.component";
import {
  ConnectionToNonMedicalStoresComponent
} from "@components/applications/application/tabs/connection-to-non-medical-stores/connection-to-non-medical-stores.component";
import { ApplicationsComponent } from "@components/applications/applications.component";
import { CancelThirdPartyOperatorComponent } from "@components/applications/cancel-third-party-operator/cancel-third-party-operator.component";
import { TerminateTPORelationshipComponent } from "@components/applications/terminate-tpo-relationship/terminate-tpo-relationship.component";
import { AssociatePageComponent } from "@components/associate-page/associate-page.component";
import { AssosiateWizardComponent } from "@components/associate-wizard/associate-wizard.component";
import {
  IndividualAssociatesResultsComponent
} from "@components/associate-wizard/individual-associates-results/individual-associates-results.component";
import { OrganizationResultsComponent } from "@components/associate-wizard/organization-results/organization-results.component";
import { SolePropResultsComponent } from "@components/associate-wizard/sole-prop-results/sole-prop-results.component";
import { CateringEventFormComponent } from "@components/catering-event/catering-event-form.component";
import { EventSecurityFormComponent } from "@components/catering-event/security.component";
import { ApplicationCancellationDialogComponent, ApplicationsAndLicencesComponent } from "@components/dashboard/applications-and-licences/applications-and-licences.component";
import { AssociateContentComponent } from "@components/dashboard/associate-content/associate-content.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { FederalReportingComponent } from "@components/federal-reporting/federal-reporting.component";
import { ProductInventorySalesReportComponent } from "@components/federal-reporting/product-inventory-sales-report/product-inventory-sales-report.component";
import { LicenceRenewalStepsComponent } from "@components/licences/licence-renewal-steps/licence-renewal-steps.component";
import { LicenceRowComponent } from "@components/licences/licence-row/licence-row.component";
import { LicencesComponent } from "@components/licences/licences.component";
import { MapComponent } from "@components/map/map.component";
import { MarketEventComponent } from "@components/market-event/market-event.component";
import { DashboardComponent as SepDashboardComponent } from "@components/sep/dashboard/dashboard.component";
import { CapacityTableRowComponent } from "@components/tables/capacity-table-row.component";
import { CapacityTableComponent } from "@components/tables/capacity-table.component";
import { TemporaryOffsiteComponent } from "@components/temporary-offsite/temporary-offsite.component";
import { TermsOfUseComponent } from "@components/terms-of-use/terms-of-use.component";
import { VersionInfoDialogComponent } from "@components/version-info/version-info-dialog.component";
import { WorkerDashboardComponent } from "@components/worker-qualification/dashboard/dashboard.component";
import { WorkerPaymentConfirmationComponent } from "@components/worker-qualification/payment-confirmation/payment-confirmation.component";
import { PrePaymentComponent } from "@components/worker-qualification/pre-payment/pre-payment.component";
import { SpdConsentComponent } from "@components/worker-qualification/spd-consent/spd-consent.component";
import { UserConfirmationComponent } from "@components/worker-qualification/user-confirmation/user-confirmation.component";
import { WorkerApplicationComponent } from "@components/worker-qualification/worker-application/worker-application.component";
import { WorkerHomeComponent, WorkerHomeDialogComponent } from "@components/worker-qualification/worker-home/worker-home.component";
import { WorkerInformationComponent } from "@components/worker-qualification/worker-information/worker-information.component";
import { WorkerQualificationComponent } from "@components/worker-qualification/worker-qualification.component";
import {
  WorkerTermsAndConditionsComponent
} from "@components/worker-qualification/worker-terms-and-conditions/worker-terms-and-conditions.component";
import { StoreModule } from "@ngrx/store";
import { StoreDevtoolsModule } from "@ngrx/store-devtools";
import { AliasDataService } from "@services/alias-data.service";
import { BCeidAuthGuard } from "@services/bceid-auth-guard.service";
import { CanDeactivateGuard } from "@services/can-deactivate-guard.service";
import { EligibilityFormDataService } from "@services/eligibility-data.service";
import { EstablishmentWatchWordsService } from "@services/establishment-watch-words.service";
import { LicenceEventsService } from "@services/licence-events.service";
import { PreviousAddressDataService } from "@services/previous-address-data.service";
import { ServiceCardAuthGuard } from "@services/service-card-auth-guard.service";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { VersionInfoDataService } from "@services/version-info-data.service";
import { AccountPickerComponent } from "@shared/components/account-picker/account-picker.component";
import { FieldComponent } from "@shared/components/field/field.component";
import {
  OrganizationLeadershipComponent
} from "@shared/components/licensee-tree/dialog-boxes/organization-leadership/organization-leadership.component";
import {
  ShareholdersAndPartnersComponent
} from "@shared/components/licensee-tree/dialog-boxes/shareholders-and-partners/shareholders-and-partners.component";
import { LicenseeTreeComponent } from "@shared/components/licensee-tree/licensee-tree.component";
import { ModalComponent } from "@shared/components/modal/modal.component";
import { MoreLessContentComponent } from "@shared/components/more-less-content/more-less-content.component";
import { NgxFileDropModule } from "ngx-file-drop";
import { NgxMaskModule } from "ngx-mask";
import { metaReducers, reducers } from "./app-state/reducers/reducers";
import { AccountCompletenessComponent } from "./components/account-completeness/account-completeness.component";
import { ApplicationCovidTemporaryExtensionComponent } from "./components/applications/application-covid-temporary-extension/application-covid-temporary-extension.component";
import { CovidConfirmationComponent } from "./components/applications/application-covid-temporary-extension/covid-confirmation/covid-confirmation.component";
import { DynamicApplicationComponent } from "./components/applications/dynamic-application/dynamic-application.component";
import { LiquorRenewalComponent } from "./components/applications/liquor-renewal/liquor-renewal.component";
import { CannabisAssociateScreeningComponent } from "./components/cannabis-associate-screening/cannabis-associate-screening.component";
import { EligibilityFormComponent } from "./components/eligibility-form/eligibility-form.component";
import { LgApprovalsComponent } from "./components/lg-approvals/lg-approvals.component";
import { MultiStageApplicationFlowComponent } from "./components/multi-stage-application-flow/multi-stage-application-flow.component";
import { PersonalHistorySummaryComponent } from "./components/personal-history-summary/personal-history-summary.component";
import { SecurityScreeningConfirmationComponent } from "./components/security-screening-confirmation/security-screening-confirmation.component";
import { SecurityScreeningRequirementsComponent } from "./components/security-screening-requirements/security-screening-requirements.component";
import { AppRemoveIfFeatureOffDirective } from "./directives/remove-if-feature-off.directive";
import { AppRemoveIfFeatureOnDirective } from "./directives/remove-if-feature-on.directive";
import { FeatureFlagService } from "./services/feature-flag.service";
import { AssociateListComponent } from "./shared/components/associate-list/associate-list.component";
import { OrgStructureComponent } from "./shared/components/org-structure/org-structure.component";
//LCSD-6374 split LgApprivalsComponent into following three components:
import { DecisionMadeButNoDocsApplicationsComponent } from "./components/lg-approvals/decision-made-but-no-docs-applications/decision-made-but-no-docs-applications.component";
import { DecisionNotMadeApplicationsComponent } from "./components/lg-approvals/decision-not-made-applications/decision-not-made-applications.component";
import { ForZoningApplicationsComponent } from "./components/lg-approvals/for-zoning-applications/for-zoning-applications.component";

import { NgxSliderModule } from "@angular-slider/ngx-slider";
import { LicenceRepresentativeFormComponent } from "@components/licence-representative-form/licence-representative-form.component";
import { LiquorFreeEventComponent } from "@components/liquor-free-event/liquor-free-event.component";
import { NavbarComponent } from "@components/navbar/navbar.component";
import { PoliceNavComponent } from "@components/navbar/police-nav/police-nav.component";
import { PublicNavComponent } from "@components/navbar/public-nav/public-nav.component";
import { UserNavComponent } from "@components/navbar/user-nav/user-nav.component";
import { NoticesComponent } from "@components/notices/notices.component";
import { OffsiteStorageComponent } from "@components/offsite-storage/offsite-storage.component";
import { PhsConfirmationComponent } from "@components/phs-confirmation/phs-confirmation.component";
import { AllApplicationsComponent } from "@components/police-representative/all-applications/all-applications.component";
import { ApprovalSettingsComponent } from "@components/police-representative/approval-settings/approval-settings.component";
import { DashboardComponent as PoliceDashboardComponent } from "@components/police-representative/dashboard/dashboard.component";
import { DrinkPlannerComponent } from "@components/sep/drink-planner/drink-planner.component";
import { DrinkPlannerDialog } from "@components/sep/drink-planner/drink-planner.dialog";
import { SepHomeComponent } from "@components/sep/home/home.component";
import { LiquorTastingDialog } from "@components/sep/liquor-tasting/liquor-tasting.dialog";
import { SepPaymentConfirmationComponent } from "@components/sep/payment-confirmation/payment-confirmation.component";
import { ServicecardUserConfirmationComponent } from "@components/sep/servicecard-user-confirmation/servicecard-user-confirmation.component";
import { ServicecardUserTermsAndConditionsComponent } from "@components/sep/servicecard-user-terms-and-conditions/servicecard-user-terms-and-conditions.component";
import { ServiceCardProfileComponent } from "@components/servicecard-profile/servicecard-profile.component";
import { EventLocationTableComponent } from "@components/tables/event-location-table/event-location-table.component";
import { NoticesTableComponent } from "@components/tables/notices-table/notices-table.component";
import { OffsiteTableComponent } from "@components/tables/offsite-table/offsite-table.component";
import { TakeHomeEventComponent } from "@components/take-home-event/take-home-event.component";
import { TuaEventComponent } from "@components/tua-event/tua-event.component";
import { UserMenuComponent } from "@components/user-menu/user-menu.component";
import { WorkerLandingPageComponent } from "@components/worker-qualification/worker-landing-page/worker-landing-page.component";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { BCeidOrServiceCardAuthGuard } from "@services/bceid-or-service-card-auth-guard.service";
import { IndexedDBService } from "@services/indexed-db.service";
import { LEConnectionsDataService } from "@services/le-connections-data.service";
import { PoliceAuthGuard } from "@services/police-auth-guard.service";
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { TermsAndConditionsDataService } from "@services/terms-and-condtions-data.service";
import { WorkerDataService } from "@services/worker-data.service";
import { PermanentChangeDeclarationsComponent } from "@shared/components/permanent-change/permanent-change-declarations/permanent-change-declarations.component";
import { ApplicationTiedHouseExemptionComponent } from './components/applications/application-tied-house-exemption/application-tied-house-exemption.component';
import { AdditionalPidsComponent } from "./components/applications/application/tabs/additional-pids/additional-pids.component";
import { BusinessPlanComponent } from "./components/applications/application/tabs/business-plan/business-plan.component";
import { ProductionStagesComponent } from "./components/applications/application/tabs/business-plan/production-stages/production-stages.component";
import { LGDecisionDialogComponent, LgInConfirmationOfReceiptComponent } from "./components/applications/application/tabs/lg-in-confirmation-of-receipt/lg-in-confirmation-of-receipt.component";
import { LgInfoPanelComponent } from "./components/applications/application/tabs/lg-info-panel/lg-info-panel.component";
import { LgZoningConfirmationComponent } from "./components/applications/application/tabs/lg-zoning-confirmation/lg-zoning-confirmation.component";
import { ProofOfZoningComponent } from "./components/applications/application/tabs/proof-of-zoning/proof-of-zoning.component";
import { PermanentChangeToALicenseeComponent } from "./components/applications/permanent-change-to-a-licensee/permanent-change-to-a-licensee.component";
import { LegalEntityTypeUpdateCalloutboxComponent } from "./components/dashboard/legal-entity-type-update-calloutbox/legal-entity-type-update-calloutbox.component";
import { ResolvedApplicationsComponent } from './components/lg-approvals/resolved-applications/resolved-applications.component';
import { LicenseeRetailStoresComponent } from "./components/licensee-retail-stores/licensee-retail-stores.component";
import { LiquorApprovalsCalloutComponent } from "./components/liquor-approvals-callout/liquor-approvals-callout.component";
import { MyJobsComponent } from './components/police-representative/my-jobs/my-jobs.component';
import { PoliceGridApprovedComponent } from './components/police-representative/police-grid-approved/police-grid-approved.component';
import { PoliceGridDeniedComponent } from './components/police-representative/police-grid-denied/police-grid-denied.component';
import { PoliceGridInProgressComponent } from './components/police-representative/police-grid-inprogress/police-grid-inprogress.component';
import { PoliceGridComponent } from './components/police-representative/police-grid/police-grid.component';
import { AcceptDialogComponent } from './components/police-representative/police-summary/accept-dialog/accept-dialog.component';
import { CancelDialogComponent } from './components/police-representative/police-summary/cancel-dialog/cancel-dialog.component';
import { DenyDialogComponent } from './components/police-representative/police-summary/deny-dialog/deny-dialog.component';
import { PoliceSummaryComponent } from './components/police-representative/police-summary/police-summary.component';
import { ErrorAlertComponent } from './components/sep/error-alert/error-alert.component';
import { LoginComponent } from './components/sep/login/login.component';
import { MyApplicationsComponent } from './components/sep/my-applications/my-applications.component';
import { SubmittedApplicationsComponent } from './components/sep/my-applications/submitted-applications/submitted-applications.component';
import { ResourcesComponent } from './components/sep/resources/resources.component';
import { ApplicantComponent } from './components/sep/sep-application/applicant/applicant.component';
import { CancelSepApplicationDialogComponent } from './components/sep/sep-application/cancel-sep-application-dialog/cancel-sep-application-dialog.component';
import { EligibilityComponent } from './components/sep/sep-application/eligibility/eligibility.component';
import { EventComponent } from './components/sep/sep-application/event/event.component';
import { FinalConfirmationComponent } from './components/sep/sep-application/final-confirmation/final-confirmation.component';
import { LiquorComponent } from './components/sep/sep-application/liquor/liquor.component';
import { DrinkAmountsComponent } from './components/sep/sep-application/liquor/tabs/drink-amounts/drink-amounts.component';
import { SellingDrinksComponent } from './components/sep/sep-application/liquor/tabs/selling-drinks/selling-drinks.component';
import { ConversionToolComponent } from './components/sep/sep-application/liquor/tabs/total-servings/conversion-tool/conversion-tool.component';
import { TotalServingsComponent } from './components/sep/sep-application/liquor/tabs/total-servings/total-servings.component';
import { SepApplicationComponent } from './components/sep/sep-application/sep-application.component';
import { SummaryComponent } from './components/sep/sep-application/summary/summary.component';
import { SepClaimComponent } from './components/sep/sep-claim/sep-claim.component';
import { StarterChecklistComponent } from './components/sep/starter-checklist/starter-checklist.component';
import { InvoiceDetailsComponent } from "./shared/components/invoice-details/invoice-details.component";
import { PermanentChangeContactComponent } from "./shared/components/permanent-change/permanent-change-contact/permanent-change-contact.component";
import { RelatedLicencePickerComponent } from './shared/components/related-licence-picker/related-licence-picker.component';

// LCSD - 6243: 2024-02-28 waynezen
import { ConnectionToOtherLiquorLicencesComponent } from "@components/account-profile/tabs/connection-to-other-liquor-licences/connection-to-other-liquor-licences.component";
import { LegalEntityReviewPermanentChangeToALicenseeComponent } from "@components/applications/legal-entity-review-permanent-change-to-a-licensee/legal-entity-review-permanent-change-to-a-licensee.component";
import { LegalEntityReviewComponent } from "@components/applications/legal-entity-review/legal-entity-review.component";
import { TiedHouseDeclarationFormComponent } from "@components/applications/tied-house-decleration/tied-house-decleration-form/tied-house-declaration-form.component";
import { FeedbackComponent } from "@components/feedback/feedback.component";
import { MaintenanceBannerComponent } from "@components/maintenance-banner/maintenance-banner.component";
import { RelocationTypeComponent } from "@components/relocation-type/relocation-type.component";
import { StandalonePaymentConfirmationComponent } from "@components/standalone-payment-confirmation/standalone-payment-confirmation.component";
import { GenericConfirmationDialogComponent } from "@shared/components/dialog/generic-confirmation-dialog/generic-confirmation-dialog.component";
import { GenericMessageDialogComponent } from "@shared/components/dialog/generic-message-dialog/generic-message-dialog.component";
import { ForbiddenModalComponent } from '@shared/components/forbidden-modal/forbidden-modal.component';
import { LegalEntityReviewDeclarationsComponent } from "@shared/components/legal-entity-review/legal-entity-review-declarations/legal-entity-review-declarations.component";
import { LegalEntityReviewDisclaimerComponent } from "@shared/components/legal-entity-review/legal-entity-review-disclaimer/legal-entity-review-disclaimer.component";
import { LegalEntityReviewJobComponent } from "@shared/components/legal-entity-review/legal-entity-review-job/legal-entity-review-job.component";
import { LegalEntityReviewNoticeLetterComponent } from "@shared/components/legal-entity-review/legal-entity-review-notice-letter/legal-entity-review-notice-letter.component";
import { LegalEntityReviewOutcomeLetterComponent } from "@shared/components/legal-entity-review/legal-entity-review-outcome-letter/legal-entity-review-outcome-letter.component";
import { LegalEntityReviewSupportingDocumentsComponent } from "@shared/components/legal-entity-review/legal-entity-review-supporting-documents/legal-entity-review-supporting-documents.component";
import { LegalEntityReviewTiedHouseComponent } from "@shared/components/legal-entity-review/legal-entity-review-tied-house/legal-entity-review-tied-house.component";
import { LegalEntityReviewTypesOfChangesRequiredComponent } from "@shared/components/legal-entity-review/legal-entity-review-types-of-changes-required/legal-entity-review-types-of-changes-required.component";
import { LegalEntityReviewWhatHappensNextComponent } from "@shared/components/legal-entity-review/legal-entity-review-what-happens-next/legal-entity-review-what-happens-next.component";
import { PermanentChangeAdditionReceiverExecutorComponent } from "@shared/components/permanent-change/permanent-change-addition-receiver-executor/permanent-change-addition-receiver-executor.component";
import { PermanentChangeCannabisSecurityScreeningFormsComponent } from "@shared/components/permanent-change/permanent-change-cannabis-security-screening-forms/permanent-change-cannabis-security-screening-forms.component";
import { PermanentChangeDirectorsOfficers } from "@shared/components/permanent-change/permanent-change-directors-officers/permanent-change-directors-officers.component";
import { PermanentChangeDisclaimerComponent } from "@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-disclaimer.component";
import { PermanentChangeHolderDetailsComponent } from "@shared/components/permanent-change/permanent-change-disclaimer/permanent-change-holder-details/permanent-change-holder-details.component";
import { PermanentChangeExternalTransferShares } from "@shared/components/permanent-change/permanent-change-external-transfer-shares/permanent-change-external-transfer-shares.component";
import { PermanentChangeInternalTransferShares } from "@shared/components/permanent-change/permanent-change-internal-transfer-shares/permanent-change-internal-transfer-shares.component";
import { PermanentChangeNameLicenseeCorporationComponent } from "@shared/components/permanent-change/permanent-change-name-licensee-corporation/permanent-change-name-licensee-corporation.component";
import { PermanentChangeNameLicenseePartnership } from "@shared/components/permanent-change/permanent-change-name-licensee-partnership/permanent-change-name-licensee-partnership.component";
import { PermanentChangeNameLicenseeSocitey } from "@shared/components/permanent-change/permanent-change-name-licensee-society/permanent-change-name-licensee-society.component";
import { PermanentChangeNamePersonComponent } from "@shared/components/permanent-change/permanent-change-name-person/permanent-change-name-person.component";
import { PermanentChangePaymentComponent } from "@shared/components/permanent-change/permanent-change-payment/permanent-change-payment.component";
import { PermanentChangePersonalHistorySummaryFormsComponent } from "@shared/components/permanent-change/permanent-change-personal-history-summary-forms/permanent-change-personal-history-summary-forms.component";
import { PermanentChangeTypesOfChangesRequestedComponent } from "@shared/components/permanent-change/permanent-change-types-of-changes-requested/permanent-change-types-of-changes-requested.component";
import { RelatedLicencePickerMulitiSelectComponent } from "@shared/components/related-licence-picker-multi-select/related-licence-picker-multi-select.component";
import { TiedHouseDeclarationComponent } from './components/applications/tied-house-decleration/tied-house-declaration.component';
import { MockApplicationComponent } from './components/mock-application/mock-application.component';
import { RelatedJobnumberPickerComponent } from './shared/components/related-jobnumber-picker/related-jobnumber-picker.component';
@NgModule({
  declarations: [
    MockApplicationComponent,
    ModalComponent,
    AccordionComponent,
    AppComponent,
    ApplicationCancellationDialogComponent,
    ApplicationComponent,
    ApplicationsAndLicencesComponent,
    LGDecisionDialogComponent,
    AssosiateWizardComponent,
    BceidConfirmationComponent,
    BeforeYouStartComponent,
    BreadcrumbComponent,
    AccountProfileComponent,
    ConnectionToProducersComponent,
    DashboardComponent,
    SepDashboardComponent,
    DelayedFileUploaderComponent,
    DirectorAndOfficerPersonDialogComponent,
    DirectorsAndOfficersComponent,
    DynamicsFormComponent,
    EditShareholdersComponent,
    FieldComponent,
    FileUploaderComponent,
    FormViewerComponent,
    HomeComponent,
    IndividualAssociatesResultsComponent,
    InsertComponent,
    KeyPersonnelComponent,
    LicenceFeePaymentConfirmationComponent,
    LicenceRepresentativeFormComponent,
    LoginComponent,
    CateringEventFormComponent,
    EventSecurityFormComponent,
    TemporaryOffsiteComponent,
    MarketEventComponent,
    NewsletterConfirmationComponent,
    NewsletterSignupComponent,
    NotFoundComponent,
    OrganizationResultsComponent,
    PaymentConfirmationComponent,
    SepPaymentConfirmationComponent,
    StandalonePaymentConfirmationComponent,
    PolicyDocumentComponent,
    PolicyDocumentSidebarComponent,
    PrePaymentComponent,
    ProofOfZoningComponent,
    ResultComponent,
    SolePropResultsComponent,
    SpdConsentComponent,
    StaticComponent,
    StatusBadgeComponent,
    SurveyComponent,
    SurveyPrimaryComponent,
    SurveySidebarComponent,
    SurveyTestComponent,
    TermsOfUseComponent,
    UserConfirmationComponent,
    VoteComponent,
    WorkerApplicationComponent,
    WorkerDashboardComponent,
    WorkerHomeComponent,
    WorkerHomeDialogComponent,
    WorkerInformationComponent,
    WorkerPaymentConfirmationComponent,
    WorkerQualificationComponent,
    WorkerTermsAndConditionsComponent,
    AssosiateWizardComponent,
    SolePropResultsComponent,
    IndividualAssociatesResultsComponent,
    OrganizationResultsComponent,
    AccountProfileComponent,
    AppRemoveIfFeatureOnDirective,
    AppRemoveIfFeatureOffDirective,
    AssociateContentComponent,
    ConnectionToNonMedicalStoresComponent,
    KeyPersonnelDialogComponent,
    AssociatePageComponent,
    ShareholderDialogComponent,
    LicenceRenewalStepsComponent,
    ApplicationRenewalComponent,
    MoreLessContentComponent,
    MapComponent,
    AccountPickerComponent,
    ApplicationAndLicenceFeeComponent,
    ApplicationCancelOwnershipTransferComponent,
    CancelThirdPartyOperatorComponent,
    TerminateTPORelationshipComponent,
    ApplicationOwnershipTransferComponent,
    ApplicationThirdPartyOperatorComponent,
    ProductInventorySalesReportComponent,
    LicenseeTreeComponent,
    FederalReportingComponent,
    OrganizationLeadershipComponent,
    ShareholdersAndPartnersComponent,
    ApplicationLicenseeChangesComponent,
    VersionInfoDialogComponent,
    LicencesComponent,
    LicenceRowComponent,
    CapacityTableComponent,
    CapacityTableRowComponent,
    ApplicationsComponent,
    AssociateListComponent,
    OrgStructureComponent,
    DynamicApplicationComponent,
    PersonalHistorySummaryComponent,
    CannabisAssociateScreeningComponent,
    AccountCompletenessComponent,
    SecurityScreeningConfirmationComponent,
    MultiStageApplicationFlowComponent,
    SecurityScreeningRequirementsComponent,
    EligibilityFormComponent,
    LiquorRenewalComponent,
    ApplicationCovidTemporaryExtensionComponent,
    CovidConfirmationComponent,
    LgApprovalsComponent,
    DecisionMadeButNoDocsApplicationsComponent,
    DecisionNotMadeApplicationsComponent,
    ForZoningApplicationsComponent,
    LiquorApprovalsCalloutComponent,
    LgInConfirmationOfReceiptComponent,
    BusinessPlanComponent,
    LgInfoPanelComponent,
    AdditionalPidsComponent,
    LgZoningConfirmationComponent,
    ProductionStagesComponent,
    PhsConfirmationComponent,
    PermanentChangeToALicenseeComponent,
    LegalEntityTypeUpdateCalloutboxComponent,
    OffsiteStorageComponent,
    OffsiteTableComponent,
    PermanentChangeContactComponent,
    InvoiceDetailsComponent,
    NoticesComponent,
    NoticesTableComponent,
    LicenseeRetailStoresComponent,
    TuaEventComponent,
    EventLocationTableComponent,
    ResolvedApplicationsComponent,
    RelatedLicencePickerComponent,
    RelatedLicencePickerMulitiSelectComponent,
    ApplicationTiedHouseExemptionComponent,
    LiquorFreeEventComponent,
    WorkerLandingPageComponent,
    TakeHomeEventComponent,
    StarterChecklistComponent,
    ResourcesComponent,
    SepApplicationComponent,
    ApplicantComponent,
    EligibilityComponent,
    EventComponent,
    LiquorComponent,
    SummaryComponent,
    MyApplicationsComponent,
    ServiceCardProfileComponent,
    ErrorAlertComponent,
    DrinkPlannerComponent,
    DrinkPlannerDialog,
    LiquorTastingDialog,
    UserMenuComponent,
    ServicecardUserConfirmationComponent,
    ServicecardUserTermsAndConditionsComponent,
    TotalServingsComponent,
    SellingDrinksComponent,
    DrinkAmountsComponent,
    ConversionToolComponent,
    NavbarComponent,
    PublicNavComponent,
    UserNavComponent,
    PoliceNavComponent,
    PoliceDashboardComponent,
    SepHomeComponent,
    ApprovalSettingsComponent,
    AllApplicationsComponent,
    MyJobsComponent,
    PoliceSummaryComponent,
    PoliceGridComponent,
    PoliceGridInProgressComponent,
    PoliceGridApprovedComponent,
    PoliceGridDeniedComponent,
    SubmittedApplicationsComponent,
    AcceptDialogComponent,
    DenyDialogComponent,
    CancelDialogComponent,
    SepClaimComponent,
    FinalConfirmationComponent,
    CancelSepApplicationDialogComponent,
    ForbiddenModalComponent,
    RelatedJobnumberPickerComponent,
    RelocationTypeComponent,
    FeedbackComponent,
    MaintenanceBannerComponent,
    PermanentChangeAdditionReceiverExecutorComponent,
    PermanentChangeAdditionReceiverExecutorComponent,
    PermanentChangeCannabisSecurityScreeningFormsComponent,
    PermanentChangeDeclarationsComponent,
    PermanentChangeDirectorsOfficers,
    PermanentChangeDisclaimerComponent,
    PermanentChangeExternalTransferShares,
    PermanentChangeHolderDetailsComponent,
    PermanentChangeInternalTransferShares,
    PermanentChangeNameLicenseeCorporationComponent,
    PermanentChangeNameLicenseePartnership,
    PermanentChangeNameLicenseeSocitey,
    PermanentChangeNamePersonComponent,
    PermanentChangePaymentComponent,
    PermanentChangePersonalHistorySummaryFormsComponent,
    PermanentChangeTypesOfChangesRequestedComponent,
    TiedHouseDeclarationFormComponent,
    TiedHouseDeclarationComponent,
    PermanentChangeTypesOfChangesRequestedComponent,
    LegalEntityReviewComponent,
    LegalEntityReviewDeclarationsComponent,
    LegalEntityReviewDisclaimerComponent,
    LegalEntityReviewJobComponent,
    LegalEntityReviewSupportingDocumentsComponent,
    LegalEntityReviewTiedHouseComponent,
    LegalEntityReviewTypesOfChangesRequiredComponent,
    LegalEntityReviewWhatHappensNextComponent,
    LegalEntityReviewPermanentChangeToALicenseeComponent,
    LegalEntityReviewNoticeLetterComponent,
    LegalEntityReviewOutcomeLetterComponent,
    ConnectionToOtherLiquorLicencesComponent,
    GenericConfirmationDialogComponent,
    GenericMessageDialogComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    CdkTableModule,
    CurrencyMaskModule,
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    NgxSliderModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSlideToggleModule,
    MatSliderModule,
    MatSnackBarModule,
    MatSortModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatStepperModule,
    MatTooltipModule,
    MatTreeModule,
    MatBadgeModule,
    NgBusyModule,
    NgxFileDropModule,
    NgbModule,
    NgxMaskModule.forRoot(),
    ReactiveFormsModule,
    StoreModule.forRoot(reducers, { metaReducers }),
    StoreDevtoolsModule.instrument
      ({
        maxAge: 5
      })
  ],
  exports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    CdkTableModule,
    FormsModule,
    HttpClientModule,
    MatAutocompleteModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSlideToggleModule,
    MatSliderModule,
    MatSnackBarModule,
    MatSortModule,
    MatStepperModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTreeModule,
    MatStepperModule,
    NgxFileDropModule,
    ReactiveFormsModule,
    MatBadgeModule,
    TiedHouseDeclarationFormComponent,
    TiedHouseDeclarationComponent
  ],
  providers: [
    AnnualVolumeService,
    AccountDataService,
    ApplicationDataService,
    LegalEntityDataService,
    LicenseDataService,
    LicenceEventsService,
    MonthlyReportDataService,
    AliasDataService,
    BCeidAuthGuard,
    CanDeactivateGuard,
    ContactDataService,
    DynamicsDataService,
    DynamicsFormDataService,
    FileDataService,
    GeneralDataService,
    InsertService,
    NewsletterDataService,
    PaymentDataService,
    PolicyDocumentDataService,
    PreviousAddressDataService,
    ServiceCardAuthGuard,
    IndexedDBService,
    EligibilityFormDataService,
    SurveyDataService,
    TiedHouseConnectionsDataService,
    TermsAndConditionsDataService,
    EstablishmentWatchWordsService,
    Title,
    UserDataService,
    VoteDataService,
    VersionInfoDataService,
    WorkerDataService,
    FeatureFlagService,
    LEConnectionsDataService,
    SpecialEventsDataService,
    BCeidOrServiceCardAuthGuard,
    PoliceAuthGuard,
    {
      provide: APP_INITIALIZER,
      useFactory: (featureFlagService: FeatureFlagService) => function () {
        return featureFlagService.init();
      },
      deps: [FeatureFlagService],
      multi: true
    },
    {
      provide: APP_INITIALIZER,
      useFactory: (us: UserDataService) => function () {
        return us.loadUserToStore();
      },
      deps: [UserDataService],
      multi: true
    }
  ],
  entryComponents: [
    ApplicationCancellationDialogComponent,
    DirectorAndOfficerPersonDialogComponent,
    LGDecisionDialogComponent,
    KeyPersonnelDialogComponent,
    WorkerHomeDialogComponent,
    ShareholderDialogComponent,
    ShareholdersAndPartnersComponent,
    OrganizationLeadershipComponent,
    VersionInfoDialogComponent,
    ModalComponent,
    StarterChecklistComponent,
    DrinkPlannerDialog,
    LiquorTastingDialog,
    PoliceSummaryComponent,
    AcceptDialogComponent,
    DenyDialogComponent,
    CancelDialogComponent,
    FinalConfirmationComponent,
    CancelSepApplicationDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
