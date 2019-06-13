import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationsAndLicencesComponent } from './applications-and-licences.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router } from '@angular/router';
import { FeatureFlagService } from '@services/feature-flag.service';
import { of } from 'rxjs';

const applicationDataServiceStub: Partial<ApplicationDataService> = {
  getAllCurrentApplications: () => of([]),
  getSubmittedApplicationCount: () => of(0)
};
const licenceDataServiceStub: Partial<LicenseDataService> = {getAllCurrentLicenses: () => of([])};
const routerStub: Partial<Router> = {};
const paymentServiceStub: Partial<PaymentDataService> = {};
const snackBarStub: Partial<MatSnackBar> = {};
const featureFlagServiceStub: Partial<FeatureFlagService> = {featureOn: () => true};
const dialogStub: Partial<MatDialog> = {};

describe('ApplicationsAndLicencesComponent', () => {
  let component: ApplicationsAndLicencesComponent;
  let fixture: ComponentFixture<ApplicationsAndLicencesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApplicationsAndLicencesComponent ],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        {provide: ApplicationDataService, useValue: applicationDataServiceStub},
        {provide: LicenseDataService, useValue: licenceDataServiceStub},
        {provide: PaymentDataService, useValue: paymentServiceStub},
        {provide: MatSnackBar, useValue: snackBarStub},
        {provide: FeatureFlagService, useValue: featureFlagServiceStub},
        {provide: MatDialog, useValue: dialogStub},
        {provide: Router, useValue: routerStub},
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationsAndLicencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
