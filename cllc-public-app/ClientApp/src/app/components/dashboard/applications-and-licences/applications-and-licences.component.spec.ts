import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { Store } from '@ngrx/store';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { of } from 'rxjs';
import { ApplicationsAndLicencesComponent } from './applications-and-licences.component';

const applicationDataServiceStub: Partial<ApplicationDataService> = {
  getAllCurrentApplications: () => of([]),
  getSubmittedCannabisRetailStoreApplicationCount: () => of(0)
};
const licenceDataServiceStub: Partial<LicenseDataService> = { getAllCurrentLicenses: () => of([]) };
const routerStub: Partial<Router> = {};
const paymentServiceStub: Partial<PaymentDataService> = {};
const snackBarStub: Partial<MatSnackBar> = {};
const featureFlagServiceStub: Partial<FeatureFlagService> = { featureOn: () => of(true) };
const dialogStub: Partial<MatDialog> = {};

describe('ApplicationsAndLicencesComponent', () => {
  let component: ApplicationsAndLicencesComponent;
  let fixture: ComponentFixture<ApplicationsAndLicencesComponent>;

  let store: MockStore<AppState>;

  const account = new Account();
  account.businessType = 'PublicCorporation';
  const initialState = {
    currentAccountState: { currentAccount: account },
    currentUserState: { currentUser: {} },
    indigenousNationState: { indigenousNationModeActive: false }
  } as AppState;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ApplicationsAndLicencesComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
        { provide: LicenseDataService, useValue: licenceDataServiceStub },
        { provide: PaymentDataService, useValue: paymentServiceStub },
        { provide: MatSnackBar, useValue: snackBarStub },
        { provide: FeatureFlagService, useValue: featureFlagServiceStub },
        { provide: MatDialog, useValue: dialogStub },
        { provide: Router, useValue: routerStub },
        provideMockStore({ initialState })
      ]
    }).compileComponents();
    store = TestBed.get(Store);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationsAndLicencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
