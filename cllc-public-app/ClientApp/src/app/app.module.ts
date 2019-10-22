import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule, APP_INITIALIZER } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { CookieService } from 'ngx-cookie-service';
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
  MatTreeModule
} from '@angular/material';
import { CdkTableModule } from '@angular/cdk/table';

import { AccountDataService } from './services/account-data.service';
import { ContactDataService } from './services/contact-data.service';
import { ApplicationDataService } from './services/application-data.service';
import { LegalEntityDataService } from './services/legal-entity-data.service';
import { LicenseDataService } from './services/license-data.service';
import { PaymentDataService } from './services/payment-data.service';
import { AppComponent } from './app.component';
import { BceidConfirmationComponent } from './bceid-confirmation/bceid-confirmation.component';
import { GeneralDataService } from './general-data.service';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { DynamicsDataService } from './services/dynamics-data.service';
import { DynamicsFormComponent } from './dynamics-form/dynamics-form.component';
import {
  EditShareholdersComponent, ShareholderDialogComponent,
} from './account-profile/tabs/shareholders/shareholders.component';
import { FormViewerComponent } from './form-viewer/form-viewer.component';
import { InsertComponent } from './insert/insert.component';
import { InsertService } from './insert/insert.service';
import { StaticComponent } from './static/static.component';
import { HomeComponent } from './home/home.component';
import { PolicyDocumentComponent } from './policy-document/policy-document.component';
import { PolicyDocumentDataService } from './services/policy-document-data.service';
import { PolicyDocumentSidebarComponent } from './policy-document-sidebar/policy-document-sidebar.component';
import { StatusBadgeComponent } from './status-badge/status-badge.component';
import { SurveyComponent } from './survey/survey.component';
import { SurveyPrimaryComponent } from './survey/primary.component';
import { SurveyTestComponent } from './survey/test.component';
import { SurveySidebarComponent } from './survey/sidebar.component';
import { SurveyDataService } from './services/survey-data.service';
import { ResultComponent } from './result/result.component';
import { AccordionComponent } from './accordion/accordion.component';
import { VoteComponent } from './vote/vote.component';
import { VoteDataService } from './services/vote-data.service';
import { NewsletterSignupComponent } from './newsletter-signup/newsletter-signup.component';
import { NewsletterConfirmationComponent } from './newsletter-confirmation/newsletter-confirmation.component';
import { NewsletterDataService } from './services/newsletter-data.service';
import { UserDataService } from './services/user-data.service';
import { NotFoundComponent } from './not-found/not-found.component';
import { FileDropModule } from 'ngx-file-drop';
import { FileUploaderComponent } from './shared/file-uploader/file-uploader.component';
import { CorporateDetailsComponent } from './account-profile/tabs/corporate-details/corporate-details.component';
import {
  DirectorsAndOfficersComponent,
  DirectorAndOfficerPersonDialogComponent
} from './account-profile/tabs/directors-and-officers/directors-and-officers.component';
import { SecurityAssessmentsComponent } from './account-profile/tabs/security-assessments/security-assessments.component';
import { OrganizationStructureComponent } from './account-profile/tabs/organization-structure/organization-structure.component';
import { BeforeYouStartComponent } from './account-profile/tabs/before-you-start/before-you-start.component';
import { FinancialInformationComponent } from './account-profile/tabs/financial-information/financial-information.component';
import { AccountProfileSummaryComponent } from './account-profile/account-profile-summary/account-profile-summary.component';

import { NgBusyModule } from 'ng-busy';
import { KeyPersonnelComponent, KeyPersonnelDialogComponent } from './account-profile/tabs/key-personnel/key-personnel.component';
import { ConnectionToProducersComponent } from './account-profile/tabs/connection-to-producers/connection-to-producers.component';
import { PaymentConfirmationComponent } from './payment-confirmation/payment-confirmation.component';
import { LicenceFeePaymentConfirmationComponent } from './licence-fee-payment-confirmation/licence-fee-payment-confirmation.component';

import { BsDatepickerModule, AlertModule } from 'ngx-bootstrap';
import { TiedHouseConnectionsDataService } from './services/tied-house-connections-data.service';
import { CanDeactivateGuard } from './services/can-deactivate-guard.service';
import { BCeidAuthGuard } from './services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from './services/service-card-auth-guard.service';
import { metaReducers, reducers } from './app-state/reducers/reducers';
import { StoreModule, Store } from '@ngrx/store';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ApplicationComponent } from './application/application.component';
import { TermsOfUseComponent } from './terms-of-use/terms-of-use.component';
import { WorkerApplicationComponent } from './worker-qualification/worker-application/worker-application.component';
import { WorkerDashboardComponent } from './worker-qualification/dashboard/dashboard.component';
import { AliasDataService } from './services/alias-data.service';
import { PreviousAddressDataService } from './services/previous-address-data.service';
import { WorkerDataService } from './services/worker-data.service.';
import { SpdConsentComponent } from './worker-qualification/spd-consent/spd-consent.component';
import { PrePaymentComponent } from './worker-qualification/pre-payment/pre-payment.component';
import { UserConfirmationComponent } from './worker-qualification/user-confirmation/user-confirmation.component';
import { WorkerQualificationComponent } from './worker-qualification/worker-qualification.component';
import { WorkerPaymentConfirmationComponent } from './worker-qualification/payment-confirmation/payment-confirmation.component';
import {
  WorkerTermsAndConditionsComponent
} from './worker-qualification/worker-terms-and-conditions/worker-terms-and-conditions.component';
import { WorkerHomeComponent, WorkerHomeDialogComponent } from './worker-qualification/worker-home/worker-home.component';
import { WorkerInformationComponent } from './worker-qualification/worker-information/worker-information.component';
import { AssosiateWizardComponent } from './associate-wizard/associate-wizard.component';
import { SolePropResultsComponent } from './associate-wizard/sole-prop-results/sole-prop-results.component';
import {
  IndividualAssociatesResultsComponent
} from './associate-wizard/individual-associates-results/individual-associates-results.component';
import { OrganizationResultsComponent } from './associate-wizard/organization-results/organization-results.component';
import { AccountProfileComponent } from './account-profile/account-profile.component';
import { FieldComponent } from './shared/field/field.component';
import {
  ApplicationsAndLicencesComponent,
  ApplicationCancellationDialogComponent
} from './applications-and-licences/applications-and-licences.component';
import { AppRemoveIfFeatureOnDirective } from './directives/remove-if-feature-on.directive';
import { AppRemoveIfFeatureOffDirective } from './directives/remove-if-feature-off.directive';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AppState } from '@app/app-state/models/app-state';
import { SetCurrentUserAction } from '@app/app-state/actions/current-user.action';
import { map } from 'rxjs/operators';
import { EstablishmentWatchWordsService } from './services/establishment-watch-words.service';
import { AssociateContentComponent } from './associate-content/associate-content.component';
import {
  ConnectionToNonMedicalStoresComponent
} from './account-profile/tabs/connection-to-non-medical-stores/connection-to-non-medical-stores.component';
import { AssociatePageComponent } from './associate-page/associate-page.component';
import { LicenceRenewalStepsComponent } from './licence-renewal-steps/licence-renewal-steps.component';
import { ApplicationRenewalComponent } from './application-renewal/application-renewal.component';
import { MoreLessContentComponent } from './shared/more-less-content/more-less-content.component';
import { MapComponent } from './map/map.component';
import { AccountPickerComponent } from './shared/account-picker/account-picker.component';
import { ApplicationAndLicenceFeeComponent } from './application-and-licence-fee/application-and-licence-fee.component';
import { ApplicationOwnershipTransferComponent } from './application-ownership-transfer/application-ownership-transfer.component';
import { ProductInventoryPackagedComponent } from './shared/product-inventory-packaged/product-inventory-packaged.component';
import { LicenseeTreeComponent } from './shared/licensee-tree/licensee-tree.component';
import {
  OrganizationLeadershipComponent
} from './shared/licensee-tree/dialog-boxes/organization-leadership/organization-leadership.component';
import {
  ShareholdersAndPartnersComponent
} from './shared/licensee-tree/dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { ApplicationLicenseeChangesComponent } from './application-licensee-changes/application-licensee-changes.component';
import { VersionInfoDataService } from './services/version-info-data.service';
import { VersionInfoDialogComponent } from './version-info/version-info-dialog.component';


@NgModule({
  declarations: [
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
    ApplicationOwnershipTransferComponent,
    ProductInventoryPackagedComponent,
    LicenseeTreeComponent,
    OrganizationLeadershipComponent,
    ShareholdersAndPartnersComponent,
    ApplicationLicenseeChangesComponent,
    VersionInfoDialogComponent
  ],
  imports: [
    ChartsModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    CdkTableModule,
    FileDropModule,
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
    MatStepperModule,
    MatTooltipModule,
    MatTreeModule,
    NgBusyModule,
    NgbModule.forRoot(),
    ReactiveFormsModule,
    BsDatepickerModule.forRoot(),
    StoreModule.forRoot(reducers, { metaReducers }),
    StoreDevtoolsModule.instrument
      ({
        maxAge: 5
      }),
    AlertModule.forRoot()
  ],
  exports: [
    AppRoutingModule,
    BrowserAnimationsModule,
    BrowserModule,
    CdkTableModule,
    FileDropModule,
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
    NgbModule,
    ReactiveFormsModule
  ],
  providers: [
    AccountDataService,
    ApplicationDataService,
    LegalEntityDataService,
    LicenseDataService,
    AliasDataService,
    BCeidAuthGuard,
    CanDeactivateGuard,
    ContactDataService,
    CookieService,
    DynamicsDataService,
    GeneralDataService,
    InsertService,
    NewsletterDataService,
    NgbDropdown,
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
    VersionInfoDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
