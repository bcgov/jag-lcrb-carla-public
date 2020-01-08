import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { LicencesComponent } from './licences.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router } from '@angular/router';
import { FeatureFlagService } from '@services/feature-flag.service';
import { of } from 'rxjs';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { Account } from '@models/account.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

const applicationDataServiceStub: Partial<ApplicationDataService> = {
  getAllCurrentApplications: () => of([]),
  getSubmittedApplicationCount: () => of(0)
};
const licenceDataServiceStub: Partial<LicenseDataService> = { getAllCurrentLicenses: () => of([]) };
const routerStub: Partial<Router> = {};
const paymentServiceStub: Partial<PaymentDataService> = {};
const snackBarStub: Partial<MatSnackBar> = {};
const featureFlagServiceStub: Partial<FeatureFlagService> = { featureOn: () => of(true) };
const dialogStub: Partial<MatDialog> = {}

describe('LicencesComponent', () => {
  let component: LicencesComponent;
  let fixture: ComponentFixture<LicencesComponent>;

  let store: MockStore<AppState>;

  const account = new Account();
  account.businessType = 'PublicCorporation';
  const initialState = {
    currentAccountState: { currentAccount: account },
    currentUserState: { currentUser: {} },
    indigenousNationState: { indigenousNationModeActive: false }
  } as AppState;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LicencesComponent],
      imports: [HttpClientTestingModule, ReactiveFormsModule],
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
    })
      .compileComponents();
    store = TestBed.get(Store);
    fixture = TestBed.createComponent(LicencesComponent);
    component = fixture.componentInstance;
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
