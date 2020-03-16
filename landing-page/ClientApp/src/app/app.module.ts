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
import { DynamicsFormDataService } from '@services/dynamics-form-data.service';
import { FileDataService } from '@services/file-data.service';
import { InsertComponent } from '@components/insert/insert.component';
import { InsertService } from '@components/insert/insert.service';
import { StaticComponent } from '@components/static/static.component';
import { HomeComponent } from '@components/home/home.component';
import { PolicyDocumentDataService } from '@services/policy-document-data.service';
import { StatusBadgeComponent } from '@components/status-badge/status-badge.component';
import { SurveyDataService } from '@services/survey-data.service';
import { VoteComponent } from '@components/vote/vote.component';
import { VoteDataService } from '@services/vote-data.service';
import { NewsletterDataService } from '@services/newsletter-data.service';
import { UserDataService } from '@services/user-data.service';
import { NotFoundComponent } from '@components/not-found/not-found.component';
import { FileUploaderComponent } from '@shared/components/file-uploader/file-uploader.component';

import { NgBusyModule } from 'ng-busy';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { CanDeactivateGuard } from '@services/can-deactivate-guard.service';
import { BCeidAuthGuard } from '@services/bceid-auth-guard.service';
import { ServiceCardAuthGuard } from '@services/service-card-auth-guard.service';
import { metaReducers, reducers } from './app-state/reducers/reducers';
import { StoreModule, Store } from '@ngrx/store';
import { AliasDataService } from '@services/alias-data.service';
import { PreviousAddressDataService } from '@services/previous-address-data.service';
import { WorkerDataService } from '@services/worker-data.service.';
import { NgxFileDropModule  } from 'ngx-file-drop';
import { NgxMaskModule } from 'ngx-mask';
import { FieldComponent } from '@shared/components/field/field.component';
import { AppRemoveIfFeatureOnDirective } from './directives/remove-if-feature-on.directive';
import { AppRemoveIfFeatureOffDirective } from './directives/remove-if-feature-off.directive';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';
import { AppState } from '@app/app-state/models/app-state';
import { SetCurrentUserAction } from '@app/app-state/actions/current-user.action';
import { map } from 'rxjs/operators';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import { MoreLessContentComponent } from '@shared/components/more-less-content/more-less-content.component';
import { AccountPickerComponent } from '@shared/components/account-picker/account-picker.component';
import { LicenseeTreeComponent } from '@shared/components/licensee-tree/licensee-tree.component';
import {
  OrganizationLeadershipComponent
} from '@shared/components/licensee-tree/dialog-boxes/organization-leadership/organization-leadership.component';
import {
  ShareholdersAndPartnersComponent
} from '@shared/components/licensee-tree/dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { VersionInfoDialogComponent } from '@components/version-info/version-info-dialog.component';
import { ModalComponent } from '@shared/components/modal/modal.component';

import { LicenceEventsService } from '@services/licence-events.service';
import { EligibilityFormDataService } from '@services/eligibility-data.service';
import { TermsOfUseComponent } from '@components/terms-of-use/terms-of-use.component';

@NgModule({
  declarations: [
    ModalComponent,
    AppComponent,
    BceidConfirmationComponent,
    BreadcrumbComponent,
    FieldComponent,
    FileUploaderComponent,
    HomeComponent,
    InsertComponent,
    NotFoundComponent,
    StaticComponent,
    StatusBadgeComponent,
    VoteComponent,
    AppRemoveIfFeatureOnDirective,
    AppRemoveIfFeatureOffDirective,
    MoreLessContentComponent,
    AccountPickerComponent,
    LicenseeTreeComponent,
    OrganizationLeadershipComponent,
    ShareholdersAndPartnersComponent,
    VersionInfoDialogComponent,
    TermsOfUseComponent
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
    MatBadgeModule
  ],
  providers: [
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
    EligibilityFormDataService,
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
    ShareholdersAndPartnersComponent,
    OrganizationLeadershipComponent,
    VersionInfoDialogComponent,
    ModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
