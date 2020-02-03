import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { ChartsModule } from 'ng2-charts';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import {
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
  MatSliderModule,
  MatSlideToggleModule,
  MatSnackBarModule,
  MatSortModule,
  MatStepperModule,
  MatTableModule,
  MatTabsModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTreeModule,
  MatBadgeModule
} from '@angular/material';
import { CdkTableModule } from '@angular/cdk/table';
import { AccountDataService } from '@services/account-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { PaymentDataService } from '@services/payment-data.service';
import { AppComponent } from './app.component';
import { BceidConfirmationComponent } from '@components/bceid-confirmation/bceid-confirmation.component';
import { GeneralDataService } from './general-data.service';
import { BreadcrumbComponent } from '@components/breadcrumb/breadcrumb.component';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { DynamicsFormComponent } from '@components/dynamics-form/dynamics-form.component';
import { DynamicsFormDataService } from '@services/dynamics-form-data.service';
import { FileDataService } from '@services/file-data.service';


import {
  EditShareholdersComponent, ShareholderDialogComponent,
} from '@components/account-profile/tabs/shareholders/shareholders.component';
import { FormViewerComponent } from '@components/form-viewer/form-viewer.component';
import { InsertComponent } from '@components/insert/insert.component';
import { InsertService } from '@components/insert/insert.service';
import { StaticComponent } from '@components/static/static.component';
import { HomeComponent } from '@components/home/home.component';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { PolicyDocumentDataService } from '@services/policy-document-data.service';
import { PolicyDocumentSidebarComponent } from '@components/policy-document-sidebar/policy-document-sidebar.component';
import { StatusBadgeComponent } from '@components/status-badge/status-badge.component';
import { SurveyComponent } from '@components/survey/survey.component';
import { SurveyPrimaryComponent } from '@components/survey/primary.component';
import { SurveyTestComponent } from '@components/survey/test.component';
import { SurveySidebarComponent } from '@components/survey/sidebar.component';
import { SurveyDataService } from '@services/survey-data.service';
import { ResultComponent } from '@components/result/result.component';
import { AccordionComponent } from '@components/accordion/accordion.component';
import { VoteComponent } from '@components/vote/vote.component';
import { VoteDataService } from '@services/vote-data.service';
import { NewsletterSignupComponent } from '@components/newsletter-signup/newsletter-signup.component';
import { NewsletterConfirmationComponent } from '@components/newsletter-confirmation/newsletter-confirmation.component';
import { NewsletterDataService } from '@services/newsletter-data.service';
import { UserDataService } from '@services/user-data.service';
import { NotFoundComponent } from '@components/not-found/not-found.component';
import { FileUploaderComponent } from '@shared/components/file-uploader/file-uploader.component';
import { CorporateDetailsComponent } from '@components/account-profile/tabs/corporate-details/corporate-details.component';
import {
  DirectorsAndOfficersComponent,
  DirectorAndOfficerPersonDialogComponent
} from '@components/account-profile/tabs/directors-and-officers/directors-and-officers.component';
import { SecurityAssessmentsComponent } from '@components/account-profile/tabs/security-assessments/security-assessments.component';
import { OrganizationStructureComponent } from '@components/account-profile/tabs/organization-structure/organization-structure.component';
import { BeforeYouStartComponent } from '@components/account-profile/tabs/before-you-start/before-you-start.component';
import { FinancialInformationComponent } from '@components/account-profile/tabs/financial-information/financial-information.component';
import { AccountProfileSummaryComponent } from '@components/account-profile/account-profile-summary/account-profile-summary.component';

import { NgBusyModule } from 'ng-busy';
import { KeyPersonnelComponent, KeyPersonnelDialogComponent } from '@components/account-profile/tabs/key-personnel/key-personnel.component';
import { ConnectionToProducersComponent } from '@components/account-profile/tabs/connection-to-producers/connection-to-producers.component';
import { PaymentConfirmationComponent } from '@components/payment-confirmation/payment-confirmation.component';
import { LicenceFeePaymentConfirmationComponent } from '@components/licences/licence-fee-payment-confirmation/licence-fee-payment-confirmation.component';

import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { CanDeactivateGuard } from '@services/can-deactivate-guard.service';
import { BCeidAuthGuard } from '@services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from '@services/service-card-auth-guard.service';
import { metaReducers, reducers } from './app-state/reducers/reducers';
import { StoreModule, Store } from '@ngrx/store';
import { DashboardComponent } from '@components/dashboard/dashboard.component';
import { ApplicationComponent } from '@components/applications/application/application.component';
import { TermsOfUseComponent } from '@components/terms-of-use/terms-of-use.component';
import { WorkerApplicationComponent } from '@components/worker-qualification/worker-application/worker-application.component';
import { WorkerDashboardComponent } from '@components/worker-qualification/dashboard/dashboard.component';
import { AliasDataService } from '@services/alias-data.service';
import { PreviousAddressDataService } from '@services/previous-address-data.service';
import { WorkerDataService } from '@services/worker-data.service.';
import { SpdConsentComponent } from '@components/worker-qualification/spd-consent/spd-consent.component';
import { PrePaymentComponent } from '@components/worker-qualification/pre-payment/pre-payment.component';
import { UserConfirmationComponent } from '@components/worker-qualification/user-confirmation/user-confirmation.component';
import { WorkerQualificationComponent } from '@components/worker-qualification/worker-qualification.component';
import { WorkerPaymentConfirmationComponent } from '@components/worker-qualification/payment-confirmation/payment-confirmation.component';
import {
  WorkerTermsAndConditionsComponent
} from '@components/worker-qualification/worker-terms-and-conditions/worker-terms-and-conditions.component';
import { WorkerHomeComponent, WorkerHomeDialogComponent } from '@components/worker-qualification/worker-home/worker-home.component';
import { WorkerInformationComponent } from '@components/worker-qualification/worker-information/worker-information.component';
import { AssosiateWizardComponent } from '@components/associate-wizard/associate-wizard.component';
import { SolePropResultsComponent } from '@components/associate-wizard/sole-prop-results/sole-prop-results.component';
import { NgxFileDropModule  } from 'ngx-file-drop';
import {
  IndividualAssociatesResultsComponent
} from '@components/associate-wizard/individual-associates-results/individual-associates-results.component';
import { OrganizationResultsComponent } from '@components/associate-wizard/organization-results/organization-results.component';
import { AccountProfileComponent } from '@components/account-profile/account-profile.component';
import { FieldComponent } from '@shared/components/field/field.component';
import { AppRemoveIfFeatureOnDirective } from './directives/remove-if-feature-on.directive';
import { AppRemoveIfFeatureOffDirective } from './directives/remove-if-feature-off.directive';
import { InputThousandsDirective } from './directives/input-thousands.directive';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AppState } from '@app/app-state/models/app-state';
import { SetCurrentUserAction } from '@app/app-state/actions/current-user.action';
import { map } from 'rxjs/operators';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import {
  ConnectionToNonMedicalStoresComponent
} from '@components/account-profile/tabs/connection-to-non-medical-stores/connection-to-non-medical-stores.component';
import { AssociatePageComponent } from '@components/associate-page/associate-page.component';
import { LicenceRenewalStepsComponent } from '@components/licences/licence-renewal-steps/licence-renewal-steps.component';
import { ApplicationRenewalComponent } from '@components/applications/application-renewal/application-renewal.component';
import { MoreLessContentComponent } from '@shared/components/more-less-content/more-less-content.component';
import { MapComponent } from '@components/map/map.component';
import { AccountPickerComponent } from '@shared/components/account-picker/account-picker.component';
import { ApplicationCancelOwnershipTransferComponent } from '@components/applications/application-cancel-ownership-transfer/application-cancel-ownership-transfer.component';
import { ApplicationOwnershipTransferComponent } from '@components/applications/application-ownership-transfer/application-ownership-transfer.component';
import { ProductInventorySalesReportComponent } from '@components/federal-reporting/product-inventory-sales-report/product-inventory-sales-report.component';
import { LicenseeTreeComponent } from '@shared/components/licensee-tree/licensee-tree.component';
import {
  OrganizationLeadershipComponent
} from '@shared/components/licensee-tree/dialog-boxes/organization-leadership/organization-leadership.component';
import {
  ShareholdersAndPartnersComponent
} from '@shared/components/licensee-tree/dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { ApplicationLicenseeChangesComponent } from '@components/applications/application-licensee-changes/application-licensee-changes.component';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { VersionInfoDialogComponent } from '@components/version-info/version-info-dialog.component';
import { FederalReportingComponent } from '@components/federal-reporting/federal-reporting.component';
import { LicencesComponent } from '@components/licences/licences.component';
import { LicenceEventComponent} from '@components/licences/licence-event/licence-event.component';
import { ApplicationsComponent } from '@components/applications/applications.component';
import { ApplicationCancellationDialogComponent, ApplicationsAndLicencesComponent } from '@components/dashboard/applications-and-licences/applications-and-licences.component';
import { AssociateContentComponent } from '@components/dashboard/associate-content/associate-content.component';
import { ApplicationAndLicenceFeeComponent } from '@components/applications/application-and-licence-fee/application-and-licence-fee.component';
import { ModalComponent } from '@shared/components/modal/modal.component';
import { PhoneMaskDirective } from './directives/phone-mask.directive';
import { AssociateListComponent } from './shared/components/associate-list/associate-list.component';
import { OrgStructureComponent } from './shared/components/org-structure/org-structure.component';
import { DynamicApplicationComponent } from './components/applications/dynamic-application/dynamic-application.component';
import { CateringDemoComponent } from './components/catering-demo/catering-demo.component';
import { PersonalHistorySummaryComponent } from './components/personal-history-summary/personal-history-summary.component';
import { AccountCompletenessComponent } from './components/account-completeness/account-completeness.component';
import { PhsConfirmationComponent } from './components/phs-confirmation/phs-confirmation.component';
import { MultiStageApplicationFlowComponent } from './components/multi-stage-application-flow/multi-stage-application-flow.component';


@NgModule({
  declarations: [
    ModalComponent,
    AccordionComponent,
    AppComponent,
    ApplicationCancellationDialogComponent,
    ApplicationComponent,
    ApplicationsAndLicencesComponent,
    AssosiateWizardComponent,
    BceidConfirmationComponent,
    BeforeYouStartComponent,
    BreadcrumbComponent,
    AccountProfileComponent,
    AccountProfileSummaryComponent,
    ConnectionToProducersComponent,
    CorporateDetailsComponent,
    DashboardComponent,
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
    LicenceEventComponent,
    NewsletterConfirmationComponent,
    NewsletterSignupComponent,
    NotFoundComponent,
    OrganizationResultsComponent,
    OrganizationStructureComponent,
    PaymentConfirmationComponent,
    PolicyDocumentComponent,
    PolicyDocumentSidebarComponent,
    PrePaymentComponent,
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
    InputThousandsDirective,
    PhoneMaskDirective,
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
    ApplicationOwnershipTransferComponent,
    ProductInventorySalesReportComponent,
    LicenseeTreeComponent,
    FederalReportingComponent,
    OrganizationLeadershipComponent,
    ShareholdersAndPartnersComponent,
    ApplicationLicenseeChangesComponent,
    VersionInfoDialogComponent,
    LicencesComponent,
    ApplicationsComponent,
    AssociateListComponent,
    OrgStructureComponent,    
    DynamicApplicationComponent,
    CateringDemoComponent,
    PersonalHistorySummaryComponent,
    AccountCompletenessComponent,
    PhsConfirmationComponent,
    MultiStageApplicationFlowComponent
  ],
  imports: [
    ChartsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    CdkTableModule,
    FormsModule,
    ReactiveFormsModule,
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
    MatStepperModule,
    MatTooltipModule,
    MatTreeModule,
    MatBadgeModule,
    NgBusyModule,
    NgxFileDropModule,
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
    PhoneMaskDirective
  ],
  providers: [
    AccountDataService,
    ApplicationDataService,
    LegalEntityDataService,
    LicenseDataService,
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
    SurveyDataService,
    TiedHouseConnectionsDataService,
    EstablishmentWatchWordsService,
    Title,
    UserDataService,
    VoteDataService,
    VersionInfoDataService,
    WorkerDataService,
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
    KeyPersonnelDialogComponent,
    WorkerHomeDialogComponent,
    ShareholderDialogComponent,
    ShareholdersAndPartnersComponent,
    OrganizationLeadershipComponent,
    VersionInfoDialogComponent,
    ModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
