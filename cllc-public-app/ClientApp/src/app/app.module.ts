import { BrowserModule, Title } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { NgbModule, NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { CookieService } from 'ngx-cookie-service';
import { ToastModule } from 'ng2-toastr';
import { AppRoutingModule } from './app-routing.module';

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
  MatTooltipModule
} from '@angular/material';
import { CdkTableModule } from '@angular/cdk/table';

import { AccountDataService } from './services/account-data.service';
import { AdoxioApplicationDataService } from './services/adoxio-application-data.service';
import { AdoxioLegalEntityDataService } from './services/adoxio-legal-entity-data.service';
import { AdoxioLicenseDataService } from './services/adoxio-license-data.service';
import { PaymentDataService } from './services/payment-data.service';
import { AppComponent } from './app.component';
import { BceidConfirmationComponent } from './bceid-confirmation/bceid-confirmation.component';
import { SearchBoxDirective } from './search-box/search-box.directive';
import { GeneralDataService } from './general-data.service';
import { AdminModule } from './admin/admin.module';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { DynamicsDataService } from './services/dynamics-data.service';
import { DynamicsFormComponent } from './dynamics-form/dynamics-form.component';
import {
  EditShareholdersComponent,
  ShareholderPersonDialogComponent,
  ShareholderOrganizationDialogComponent } from './business-profile/tabs/edit-shareholders/edit-shareholders.component';
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
import { ApplicationsListComponent } from './applications-list/applications-list.component';
import { BusinessProfileComponent } from './business-profile/business-profile.component';
import { LicenseApplicationSummaryComponent } from './license-application-summary/license-application-summary.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { FileDropModule } from 'ngx-file-drop';
import { FileUploaderComponent } from './file-uploader/file-uploader.component';
import { CorporateDetailsComponent } from './business-profile/tabs/corporate-details/corporate-details.component';
import {
  DirectorsAndOfficersComponent,
  DirectorAndOfficerPersonDialogComponent } from './business-profile/tabs/directors-and-officers/directors-and-officers.component';
import { SecurityAssessmentsComponent } from './business-profile/tabs/security-assessments/security-assessments.component';
import { OrganizationStructureComponent } from './business-profile/tabs/organization-structure/organization-structure.component';
import { BeforeYouStartComponent } from './business-profile/tabs/before-you-start/before-you-start.component';
import { FinancialInformationComponent } from './business-profile/tabs/financial-information/financial-information.component';
import { BusinessProfileSummaryComponent } from './business-profile-summary/business-profile-summary.component';

import {NgBusyModule} from 'ng-busy';
import { KeyPersonnelComponent } from './business-profile/tabs/key-personnel/key-personnel.component';
import { ConnectionToProducersComponent } from './business-profile/tabs/connection-to-producers/connection-to-producers.component';
import { LicenseApplicationComponent } from './license-application/license-application.component';
import { PaymentConfirmationComponent } from './payment-confirmation/payment-confirmation.component';
import { ContactDetailsComponent } from './license-application/tabs/contact-details/contact-details.component';
import { PropertyDetailsComponent } from './license-application/tabs/property-details/property-details.component';
import { StoreInformationComponent } from './license-application/tabs/store-information/store-information.component';
import { FloorPlanComponent } from './license-application/tabs/floor-plan/floor-plan.component';
import { SiteMapComponent } from './license-application/tabs/site-map/site-map.component';
import { DeclarationComponent } from './license-application/tabs/declaration/declaration.component';
import { SubmitPayComponent } from './license-application/tabs/submit-pay/submit-pay.component';

import { BsDatepickerModule, AlertModule } from 'ngx-bootstrap';
import { TiedHouseConnectionsDataService } from './services/tied-house-connections-data.service';
import { CanDeactivateGuard } from './services/can-deactivate-guard.service';
import { metaReducers, reducers } from './app-state/reducers/reducers';
import { StoreModule } from '@ngrx/store';
import { DashboardLiteComponent } from './dashboard-lite/dashboard-lite.component';
import { ClientConfigDataService } from './services/client-config.service';
import { ApplicationComponent } from './lite/application/application.component';
import { LiteApplicationDashboardComponent } from './lite-application-dashboard/lite-application-dashboard.component';
import { TermsAndConditionsComponent } from './lite/terms-and-conditions/terms-and-conditions.component';

@NgModule({
  declarations: [
    AccordionComponent,
    AppComponent,
    ApplicationsListComponent,
    BceidConfirmationComponent,
    BusinessProfileComponent,
    BreadcrumbComponent,
    DynamicsFormComponent,
    EditShareholdersComponent,
    ShareholderPersonDialogComponent,
    ShareholderOrganizationDialogComponent,
    FormViewerComponent,
    HomeComponent,
    InsertComponent,
    NewsletterConfirmationComponent,
    NewsletterSignupComponent,
    NotFoundComponent,
    PolicyDocumentComponent,
    PolicyDocumentSidebarComponent,
    ResultComponent,
    SearchBoxDirective,
    StaticComponent,
    StatusBadgeComponent,
    SurveyComponent,
    SurveyPrimaryComponent,
    SurveySidebarComponent,
    SurveyTestComponent,
    VoteComponent,
    LicenseApplicationSummaryComponent,
    DashboardComponent,
    FileUploaderComponent,
    CorporateDetailsComponent,
    OrganizationStructureComponent,
    DirectorsAndOfficersComponent,
    DirectorAndOfficerPersonDialogComponent,
    SecurityAssessmentsComponent,
    BeforeYouStartComponent,
    FinancialInformationComponent,
    BusinessProfileSummaryComponent,
    KeyPersonnelComponent,
    ConnectionToProducersComponent,
    LicenseApplicationComponent,
    PaymentConfirmationComponent,
    ContactDetailsComponent,
    PropertyDetailsComponent,
    StoreInformationComponent,
    FloorPlanComponent,
    SiteMapComponent,
    DeclarationComponent,
    SubmitPayComponent,
    DashboardLiteComponent,
    ApplicationComponent,
    LiteApplicationDashboardComponent,
    TermsAndConditionsComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule.forRoot(),
    AdminModule,
    BrowserAnimationsModule,
    NgBusyModule,
    CdkTableModule,
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
    FileDropModule,
    ToastModule.forRoot(),
    BsDatepickerModule.forRoot(),
    StoreModule.forRoot(reducers, { metaReducers }),
    AlertModule.forRoot()
  ],
  exports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule,
    AppRoutingModule,
    NgbModule,
    AdminModule,
    BrowserAnimationsModule,
    CdkTableModule,
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
    FileDropModule,
    MatTooltipModule
  ],
  providers: [
    CookieService,
    ClientConfigDataService,
    DynamicsDataService,
    InsertService,
    GeneralDataService,
    NewsletterDataService,
    PolicyDocumentDataService,
    SurveyDataService,
    VoteDataService,
    UserDataService,
    AdoxioApplicationDataService,
    AdoxioLegalEntityDataService,
    AdoxioLicenseDataService,
    AccountDataService,
    PaymentDataService,
    TiedHouseConnectionsDataService,
    NgbDropdown,
    Title,
    CanDeactivateGuard
  ],
  entryComponents: [
    ShareholderPersonDialogComponent,
    ShareholderOrganizationDialogComponent,
    DirectorAndOfficerPersonDialogComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
