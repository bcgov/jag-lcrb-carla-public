import { BrowserModule, Title } from "@angular/platform-browser";
import { NgModule, APP_INITIALIZER } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";
import { AppRoutingModule } from "./app-routing.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
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
import { CdkTableModule } from "@angular/cdk/table";
import { AccountDataService } from "@services/account-data.service";
import { ContactDataService } from "@services/contact-data.service";
import { ApplicationDataService } from "@services/application-data.service";
import { LegalEntityDataService } from "@services/legal-entity-data.service";
import { LicenseDataService } from "@services/license-data.service";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { PaymentDataService } from "@services/payment-data.service";
import { AppComponent } from "./app.component";
import { BceidConfirmationComponent } from "@components/bceid-confirmation/bceid-confirmation.component";
import { GeneralDataService } from "./general-data.service";
import { BreadcrumbComponent } from "@components/breadcrumb/breadcrumb.component";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { DynamicsFormComponent } from "@components/dynamics-form/dynamics-form.component";
import { DynamicsFormDataService } from "@services/dynamics-form-data.service";
import { FileDataService } from "@services/file-data.service";
import { AnnualVolumeService } from "@services/annual-volume.service";
import { CurrencyMaskModule } from "ng2-currency-mask";

import {
  EditShareholdersComponent,
  ShareholderDialogComponent,
} from "@components/account-profile/tabs/shareholders/shareholders.component";
import { FormViewerComponent } from "@components/form-viewer/form-viewer.component";
import { InsertComponent } from "@components/insert/insert.component";
import { InsertService } from "@components/insert/insert.service";
import { StaticComponent } from "@components/static/static.component";
import { HomeComponent } from "@components/home/home.component";
import { PolicyDocumentComponent } from "@components/policy-document/policy-document.component";
import { PolicyDocumentDataService } from "@services/policy-document-data.service";
import { PolicyDocumentSidebarComponent } from "@components/policy-document-sidebar/policy-document-sidebar.component";
import { StatusBadgeComponent } from "@components/status-badge/status-badge.component";
import { SurveyComponent } from "@components/survey/survey.component";
import { SurveyPrimaryComponent } from "@components/survey/primary.component";
import { SurveyTestComponent } from "@components/survey/test.component";
import { SurveySidebarComponent } from "@components/survey/sidebar.component";
import { SurveyDataService } from "@services/survey-data.service";
import { ResultComponent } from "@components/result/result.component";
import { AccordionComponent } from "@components/accordion/accordion.component";
import { VoteComponent } from "@components/vote/vote.component";
import { VoteDataService } from "@services/vote-data.service";
import { NewsletterSignupComponent } from "@components/newsletter-signup/newsletter-signup.component";
import { NewsletterConfirmationComponent } from "@components/newsletter-confirmation/newsletter-confirmation.component";
import { NewsletterDataService } from "@services/newsletter-data.service";
import { UserDataService } from "@services/user-data.service";
import { NotFoundComponent } from "@components/not-found/not-found.component";
import { FileUploaderComponent } from "@shared/components/file-uploader/file-uploader.component";
import { DelayedFileUploaderComponent } from "@shared/components/delayed-file-uploader/delayed-file-uploader.component";

import { CorporateDetailsComponent } from
  "@components/account-profile/tabs/corporate-details/corporate-details.component";
import {
  DirectorsAndOfficersComponent,
  DirectorAndOfficerPersonDialogComponent
} from "@components/account-profile/tabs/directors-and-officers/directors-and-officers.component";
import { SecurityAssessmentsComponent } from
  "@components/account-profile/tabs/security-assessments/security-assessments.component";
import { BeforeYouStartComponent } from "@components/account-profile/tabs/before-you-start/before-you-start.component";
import { FinancialInformationComponent } from
  "@components/account-profile/tabs/financial-information/financial-information.component";
import { AccountProfileSummaryComponent } from
  "@components/account-profile/account-profile-summary/account-profile-summary.component";

import { NgBusyModule } from "ng-busy";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { KeyPersonnelComponent, KeyPersonnelDialogComponent } from
  "@components/account-profile/tabs/key-personnel/key-personnel.component";
import { ConnectionToProducersComponent } from
  "@components/account-profile/tabs/connection-to-producers/connection-to-producers.component";
import { PaymentConfirmationComponent } from "@components/payment-confirmation/payment-confirmation.component";
import { LicenceFeePaymentConfirmationComponent } from
  "@components/licences/licence-fee-payment-confirmation/licence-fee-payment-confirmation.component";

import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { CanDeactivateGuard } from "@services/can-deactivate-guard.service";
import { BCeidAuthGuard } from "@services/bceid-auth-guard.service";
import { ServiceCardAuthGuard } from "@services/service-card-auth-guard.service";
import { metaReducers, reducers } from "./app-state/reducers/reducers";
import { StoreModule } from "@ngrx/store";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { DashboardComponent as SepDashboardComponent } from "@components/sep/dashboard/dashboard.component";
import { ApplicationComponent } from "@components/applications/application/application.component";
import { TermsOfUseComponent } from "@components/terms-of-use/terms-of-use.component";
import { WorkerApplicationComponent } from
  "@components/worker-qualification/worker-application/worker-application.component";
import { WorkerDashboardComponent } from "@components/worker-qualification/dashboard/dashboard.component";
import { AliasDataService } from "@services/alias-data.service";
import { PreviousAddressDataService } from "@services/previous-address-data.service";
import { SpdConsentComponent } from "@components/worker-qualification/spd-consent/spd-consent.component";
import { PrePaymentComponent } from "@components/worker-qualification/pre-payment/pre-payment.component";
import { UserConfirmationComponent } from
  "@components/worker-qualification/user-confirmation/user-confirmation.component";
import { WorkerQualificationComponent } from "@components/worker-qualification/worker-qualification.component";
import { WorkerPaymentConfirmationComponent } from
  "@components/worker-qualification/payment-confirmation/payment-confirmation.component";
import {
  WorkerTermsAndConditionsComponent
} from "@components/worker-qualification/worker-terms-and-conditions/worker-terms-and-conditions.component";
import { WorkerHomeComponent, WorkerHomeDialogComponent } from
  "@components/worker-qualification/worker-home/worker-home.component";
import { WorkerInformationComponent } from
  "@components/worker-qualification/worker-information/worker-information.component";
import { AssosiateWizardComponent } from "@components/associate-wizard/associate-wizard.component";
import { SolePropResultsComponent } from "@components/associate-wizard/sole-prop-results/sole-prop-results.component";
import { NgxFileDropModule } from "ngx-file-drop";
import { NgxMaskModule } from "ngx-mask";
import {
  IndividualAssociatesResultsComponent
} from "@components/associate-wizard/individual-associates-results/individual-associates-results.component";
import { OrganizationResultsComponent } from
  "@components/associate-wizard/organization-results/organization-results.component";
import { AccountProfileComponent } from "@components/account-profile/account-profile.component";
import { FieldComponent } from "@shared/components/field/field.component";
import { AppRemoveIfFeatureOnDirective } from "./directives/remove-if-feature-on.directive";
import { AppRemoveIfFeatureOffDirective } from "./directives/remove-if-feature-off.directive";
import { StoreDevtoolsModule } from "@ngrx/store-devtools";
import { EstablishmentWatchWordsService } from "@services/establishment-watch-words.service";
import {
  ConnectionToNonMedicalStoresComponent
} from "@components/account-profile/tabs/connection-to-non-medical-stores/connection-to-non-medical-stores.component";
import { AssociatePageComponent } from "@components/associate-page/associate-page.component";
import { LicenceRenewalStepsComponent } from
  "@components/licences/licence-renewal-steps/licence-renewal-steps.component";
import { ApplicationRenewalComponent } from
  "@components/applications/application-renewal/application-renewal.component";
import { MoreLessContentComponent } from "@shared/components/more-less-content/more-less-content.component";
import { MapComponent } from "@components/map/map.component";
import { AccountPickerComponent } from "@shared/components/account-picker/account-picker.component";
import { ApplicationCancelOwnershipTransferComponent } from
  "@components/applications/application-cancel-ownership-transfer/application-cancel-ownership-transfer.component";
import { ApplicationOwnershipTransferComponent } from
  "@components/applications/application-ownership-transfer/application-ownership-transfer.component";
import { ProductInventorySalesReportComponent } from
  "@components/federal-reporting/product-inventory-sales-report/product-inventory-sales-report.component";
import { LicenseeTreeComponent } from "@shared/components/licensee-tree/licensee-tree.component";
import {
  OrganizationLeadershipComponent
} from "@shared/components/licensee-tree/dialog-boxes/organization-leadership/organization-leadership.component";
import {
  ShareholdersAndPartnersComponent
} from "@shared/components/licensee-tree/dialog-boxes/shareholders-and-partners/shareholders-and-partners.component";
import { ApplicationLicenseeChangesComponent } from
  "@components/applications/application-licensee-changes/application-licensee-changes.component";
import { VersionInfoDataService } from "@services/version-info-data.service";
import { VersionInfoDialogComponent } from "@components/version-info/version-info-dialog.component";
import { FederalReportingComponent } from "@components/federal-reporting/federal-reporting.component";
import { LicencesComponent } from "@components/licences/licences.component";
import { LicenceRowComponent } from "@components/licences/licence-row/licence-row.component";
import { CapacityTableComponent } from "@components/tables/capacity-table.component";
import { CapacityTableRowComponent } from "@components/tables/capacity-table-row.component";
import { ApplicationsComponent } from "@components/applications/applications.component";
import { ApplicationCancellationDialogComponent, ApplicationsAndLicencesComponent } from
  "@components/dashboard/applications-and-licences/applications-and-licences.component";
import { AssociateContentComponent } from "@components/dashboard/associate-content/associate-content.component";
import { ApplicationAndLicenceFeeComponent } from
  "@components/applications/application-and-licence-fee/application-and-licence-fee.component";
import { ModalComponent } from "@shared/components/modal/modal.component";
import { AssociateListComponent } from "./shared/components/associate-list/associate-list.component";
import { OrgStructureComponent } from "./shared/components/org-structure/org-structure.component";
import { DynamicApplicationComponent } from
  "./components/applications/dynamic-application/dynamic-application.component";
import { PersonalHistorySummaryComponent } from
  "./components/personal-history-summary/personal-history-summary.component";
import { CannabisAssociateScreeningComponent } from
  "./components/cannabis-associate-screening/cannabis-associate-screening.component";
import { AccountCompletenessComponent } from "./components/account-completeness/account-completeness.component";
import { SecurityScreeningConfirmationComponent } from
  "./components/security-screening-confirmation/security-screening-confirmation.component";
import { MultiStageApplicationFlowComponent } from
  "./components/multi-stage-application-flow/multi-stage-application-flow.component";
import { CateringEventFormComponent } from "@components/catering-event/catering-event-form.component";
import { EventSecurityFormComponent } from "@components/catering-event/security.component";
import { LicenceEventsService } from "@services/licence-events.service";
import { SecurityScreeningRequirementsComponent } from
  "./components/security-screening-requirements/security-screening-requirements.component";
import { EligibilityFormComponent } from "./components/eligibility-form/eligibility-form.component";
import { EligibilityFormDataService } from "@services/eligibility-data.service";
import { LiquorRenewalComponent } from "./components/applications/liquor-renewal/liquor-renewal.component";
import { TemporaryOffsiteComponent } from "@components/temporary-offsite/temporary-offsite.component";
import { MarketEventComponent } from "@components/market-event/market-event.component";
import { ApplicationThirdPartyOperatorComponent } from
  "@components/applications/application-third-party-operator/application-third-party-operator.component";
import { CancelThirdPartyOperatorComponent } from
  "@components/applications/cancel-third-party-operator/cancel-third-party-operator.component";
import { FeatureFlagService } from "./services/feature-flag.service";
import { ApplicationCovidTemporaryExtensionComponent } from
  "./components/applications/application-covid-temporary-extension/application-covid-temporary-extension.component";
import { CovidConfirmationComponent } from
  "./components/applications/application-covid-temporary-extension/covid-confirmation/covid-confirmation.component";
import { TerminateTPORelationshipComponent } from
  "@components/applications/terminate-tpo-relationship/terminate-tpo-relationship.component";
import { LgApprovalsComponent } from "./components/lg-approvals/lg-approvals.component";
import { LiquorApprovalsCalloutComponent } from
  "./components/liquor-approvals-callout/liquor-approvals-callout.component";
import { LgInConfirmationOfReceiptComponent, LGDecisionDialogComponent } from
  "./components/applications/application/tabs/lg-in-confirmation-of-receipt/lg-in-confirmation-of-receipt.component";
import { LicenceRepresentativeFormComponent } from
  "@components/licence-representative-form/licence-representative-form.component";
import { BusinessPlanComponent } from
  "./components/applications/application/tabs/business-plan/business-plan.component";
import { LgInfoPanelComponent } from "./components/applications/application/tabs/lg-info-panel/lg-info-panel.component";
import { AdditionalPidsComponent } from
  "./components/applications/application/tabs/additional-pids/additional-pids.component";
import { TermsAndConditionsDataService } from "@services/terms-and-condtions-data.service";
import { LgZoningConfirmationComponent } from
  "./components/applications/application/tabs/lg-zoning-confirmation/lg-zoning-confirmation.component";
import { ProofOfZoningComponent } from
  "./components/applications/application/tabs/proof-of-zoning/proof-of-zoning.component";
import { ProductionStagesComponent } from
  "./components/applications/application/tabs/business-plan/production-stages/production-stages.component";
import { PhsConfirmationComponent } from "@components/phs-confirmation/phs-confirmation.component";
import { PermanentChangeToALicenseeComponent } from
  "./components/applications/permanent-change-to-a-licensee/permanent-change-to-a-licensee.component";
import { LegalEntityTypeUpdateCalloutboxComponent } from
  "./components/dashboard/legal-entity-type-update-calloutbox/legal-entity-type-update-calloutbox.component";
import { OffsiteStorageComponent } from "@components/offsite-storage/offsite-storage.component";
import { OffsiteTableComponent } from "@components/tables/offsite-table/offsite-table.component";
import { ContactComponent } from "./shared/components/contact/contact.component";
import { InvoiceDetailsComponent } from "./shared/components/invoice-details/invoice-details.component";
import { FontAwesomeModule } from "@fortawesome/angular-fontawesome";
import { NoticesComponent } from "@components/notices/notices.component";
import { NoticesTableComponent } from "@components/tables/notices-table/notices-table.component";
import { LEConnectionsDataService } from "@services/le-connections-data.service";
import { LicenseeRetailStoresComponent } from "./components/licensee-retail-stores/licensee-retail-stores.component";
import { WorkerDataService } from "@services/worker-data.service";
import { TuaEventComponent } from "@components/tua-event/tua-event.component";
import { EventLocationTableComponent } from "@components/tables/event-location-table/event-location-table.component";
import { ResolvedApplicationsComponent } from './components/lg-approvals/resolved-applications/resolved-applications.component';
import { RelatedLicencePickerComponent } from './shared/components/related-licence-picker/related-licence-picker.component';
import { ApplicationTiedHouseExemptionComponent } from './components/applications/application-tied-house-exemption/application-tied-house-exemption.component';
import { LiquorFreeEventComponent } from "@components/liquor-free-event/liquor-free-event.component";
import { LoginComponent } from './components/sep/login/login.component';
import { WorkerLandingPageComponent } from "@components/worker-qualification/worker-landing-page/worker-landing-page.component";
import { TakeHomeEventComponent } from "@components/take-home-event/take-home-event.component";
import { BCeidOrServiceCardAuthGuard } from "@services/bceid-or-service-card-auth-guard.service";
import { StarterChecklistComponent } from './components/sep/starter-checklist/starter-checklist.component';
import { ResourcesComponent } from './components/sep/resources/resources.component';
import { SepApplicationComponent } from './components/sep/sep-application/sep-application.component';
import { ApplicantComponent } from './components/sep/sep-application/applicant/applicant.component';
import { EligibilityComponent } from './components/sep/sep-application/eligibility/eligibility.component';
import { EventComponent } from './components/sep/sep-application/event/event.component';
import { LiquorComponent } from './components/sep/sep-application/liquor/liquor.component';
import { SummaryComponent } from './components/sep/sep-application/summary/summary.component';
import { IndexedDBService } from "@services/indexed-db.service";
import { MyApplicationsComponent } from './components/sep/my-applications/my-applications.component';
import { ServiceCardProfileComponent } from "@components/servicecard-profile/servicecard-profile.component";
import { ErrorAlertComponent } from './components/sep/error-alert/error-alert.component';
import { DrinkPlannerComponent } from "@components/sep/drink-planner/drink-planner.component";
import { DrinkPlannerDialog } from "@components/sep/drink-planner/drink-planner.dialog";
import { LiquorTastingDialog} from "@components/sep/liquor-tasting/liquor-tasting.dialog";
import { UserMenuComponent } from "@components/user-menu/user-menu.component";
import { ServicecardUserConfirmationComponent } from "@components/sep/servicecard-user-confirmation/servicecard-user-confirmation.component";
import { ServicecardUserTermsAndConditionsComponent } from "@components/sep/servicecard-user-terms-and-conditions/servicecard-user-terms-and-conditions.component";
import { TotalServingsComponent } from './components/sep/sep-application/liquor/tabs/total-servings/total-servings.component';
import { SellingDrinksComponent } from './components/sep/sep-application/liquor/tabs/selling-drinks/selling-drinks.component';
import { DrinkAmountsComponent } from './components/sep/sep-application/liquor/tabs/drink-amounts/drink-amounts.component';
import { ConversionToolComponent } from './components/sep/sep-application/liquor/tabs/total-servings/conversion-tool/conversion-tool.component';
import { SpecialEventsDataService } from "@services/special-events-data.service";
import { NavbarComponent } from "@components/navbar/navbar.component";
import { PublicNavComponent } from "@components/navbar/public-nav/public-nav.component";
import { UserNavComponent } from "@components/navbar/user-nav/user-nav.component";
import { PoliceNavComponent } from "@components/navbar/police-nav/police-nav.component";
import { DashboardComponent as PoliceDashboardComponent } from "@components/police-representative/dashboard/dashboard.component";
import { SepHomeComponent } from "@components/sep/home/home.component";
import { ApprovalSettingsComponent } from "@components/police-representative/approval-settings/approval-settings.component";
import { PoliceAuthGuard } from "@services/police-auth-guard.service";
import { AllApplicationsComponent } from "@components/police-representative/all-applications/all-applications.component";
import { MyJobsComponent } from './components/police-representative/my-jobs/my-jobs.component';
import { PoliceSummaryComponent } from './components/police-representative/police-summary/police-summary.component';
import { PoliceGridComponent } from './components/police-representative/police-grid/police-grid.component';
import { SubmittedApplicationsComponent } from './components/sep/my-applications/submitted-applications/submitted-applications.component';
import { SepPaymentConfirmationComponent } from "@components/sep/payment-confirmation/payment-confirmation.component";
import { AcceptDialogComponent } from './components/police-representative/police-summary/accept-dialog/accept-dialog.component';
import { DenyDialogComponent } from './components/police-representative/police-summary/deny-dialog/deny-dialog.component';
import { CancelDialogComponent } from './components/police-representative/police-summary/cancel-dialog/cancel-dialog.component';
import { NgxSliderModule } from "@angular-slider/ngx-slider";
import { SepClaimComponent } from './components/sep/sep-claim/sep-claim.component';
import { FinalConfirmationComponent } from './components/sep/sep-application/final-confirmation/final-confirmation.component';
import { CancelSepApplicationDialogComponent } from './components/sep/sep-application/cancel-sep-application-dialog/cancel-sep-application-dialog.component';


@NgModule({
    declarations: [
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
        AccountProfileSummaryComponent,
        ConnectionToProducersComponent,
        CorporateDetailsComponent,
        DashboardComponent,
        SepDashboardComponent,
        DelayedFileUploaderComponent,
        DirectorAndOfficerPersonDialogComponent,
        DirectorsAndOfficersComponent,
        DynamicsFormComponent,
        EditShareholdersComponent,
        FieldComponent,
        FileUploaderComponent,
        FinancialInformationComponent,
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
        PolicyDocumentComponent,
        PolicyDocumentSidebarComponent,
        PrePaymentComponent,
        ProofOfZoningComponent,
        ResultComponent,
        SecurityAssessmentsComponent,
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
        ContactComponent,
        InvoiceDetailsComponent,
        NoticesComponent,
        NoticesTableComponent,
        LicenseeRetailStoresComponent,
        TuaEventComponent,
        EventLocationTableComponent,
        ResolvedApplicationsComponent,
        RelatedLicencePickerComponent,
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
        SubmittedApplicationsComponent,
        AcceptDialogComponent,
        DenyDialogComponent,
        CancelDialogComponent,
        SepClaimComponent,
        FinalConfirmationComponent,
        CancelSepApplicationDialogComponent,
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
        StoreDevtoolsModule.instrument({
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
        MatBadgeModule
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
    bootstrap: [AppComponent]
})
export class AppModule {
}
