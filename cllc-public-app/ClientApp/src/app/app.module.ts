import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CookieService } from 'ngx-cookie-service';
import { ToastModule } from 'ng2-toastr/ng2-toastr';
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

import { AppComponent } from './app.component';
import { SearchBoxDirective } from './search-box/search-box.directive';
import { GeneralDataService } from './general-data.service';
import { AdminModule } from './admin/admin.module';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { DynamicsDataService } from './services/dynamics-data.service';
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
import { SurveyEditorComponent } from './survey/editor.component';
import { SurveySidebarComponent } from './survey/sidebar.component';
import { SurveyDataService } from "./services/survey-data.service";
import { ResultComponent } from './result/result.component';
import { AccordionComponent } from './accordion/accordion.component';
import { VoteComponent } from './vote/vote.component';
import { VoteDataService } from "./services/vote-data.service";
import { NewsletterSignupComponent } from './newsletter-signup/newsletter-signup.component';
import { NewsletterConfirmationComponent } from './newsletter-confirmation/newsletter-confirmation.component';
import { NewsletterDataService } from "./services/newsletter-data.service";


@NgModule({
  declarations: [
    AppComponent,
    SearchBoxDirective,
    HomeComponent,
    ResultComponent,
    BreadcrumbComponent,
    InsertComponent,
    PolicyDocumentComponent,
    PolicyDocumentSidebarComponent,
    StatusBadgeComponent,
    SurveyComponent,
    SurveyPrimaryComponent,
    SurveyTestComponent,
    SurveyEditorComponent,
    SurveySidebarComponent,
    StaticComponent,
    AccordionComponent,
    VoteComponent,
    NewsletterConfirmationComponent,
    NewsletterSignupComponent,
    FormViewerComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    AppRoutingModule,
    NgbModule,
    AdminModule,
    BrowserAnimationsModule,
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
    ToastModule.forRoot()
  ],
  exports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    AppRoutingModule,
    NgbModule,
    AdminModule,
    BrowserAnimationsModule,
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
  ],
  providers: [
    CookieService,
    DynamicsDataService,
    InsertService,
    GeneralDataService,
    NewsletterDataService,
    PolicyDocumentDataService,
    SurveyDataService,        
    VoteDataService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
