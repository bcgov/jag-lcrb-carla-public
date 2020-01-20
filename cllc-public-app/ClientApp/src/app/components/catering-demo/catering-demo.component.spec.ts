import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CateringDemoComponent } from './catering-demo.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { of } from 'rxjs';
import { LicenseDataService } from '@services/license-data.service';
import { License } from '@models/license.model';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute, Router } from '@angular/router';
import { Account } from '@models/account.model';
import { AppState } from '@app/app-state/models/app-state';
import { provideMockStore, MockStore } from '@ngrx/store/testing';
import { MatSnackBar, MatDialog } from '@angular/material';
import { PaymentDataService } from '@services/payment-data.service';
import { HttpClient } from '@angular/common/http';
import { FeatureFlagDataService } from '@services/feature-flag-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { Store } from '@ngrx/store';


let applicationDataServiceStub: Partial<ApplicationDataService>;
let licenceDataServiceStub: Partial<LicenseDataService>;
let activatedRouteStub: ActivatedRouteStub;
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);
const initialState = {
  currentAccountState: { currentAccount: new Account() },
  currentUserState: { currentUser: {} },
  indigenousNationState: {indigenousNationModeActive: false }
} as AppState;
const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj('HttpClient', ['get']);
let paymentDataServiceStub: Partial<PaymentDataService> = {};

describe('CateringDemoComponent', () => {
  let component: CateringDemoComponent;
  let fixture: ComponentFixture<CateringDemoComponent>;
  let store: MockStore<AppState>;

  beforeEach(async(() => {
    applicationDataServiceStub = {
      getSubmittedApplicationCount: () => of(0),
      cancelApplication: () => of(null),
      updateApplication: () => of(null),
      getAllCurrentApplications: () => of([]),
      getApplicationById: () => of(<Application>{
        applicationType: <any>{
          contentTypes: []
        }
      }), 
    };

    licenceDataServiceStub = {
      getLicenceById: () => of(<License>{}),
      getAllCurrentLicenses: () => of([]),
    };
    activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });

    TestBed.configureTestingModule({
      declarations: [CateringDemoComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
        { provide: LicenseDataService, useValue: licenceDataServiceStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: Router, useValue: routerSpy },
        provideMockStore({initialState}),
        { provide: MatSnackBar, useValue: {} },
        { provide: MatDialog, useValue: {} },
        { provide: LegalEntityDataService, useValue: {getCurrentHierachy: () => of([])} },
        { provide: PaymentDataService, useValue: paymentDataServiceStub },
        {provide: HttpClient, useValue: httpClientSpy},
        {provide: FeatureFlagDataService, useValue: {getFeatureFlags: () => of([])}},
      ]
    })
      .compileComponents();
      store =  TestBed.get(Store);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CateringDemoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
