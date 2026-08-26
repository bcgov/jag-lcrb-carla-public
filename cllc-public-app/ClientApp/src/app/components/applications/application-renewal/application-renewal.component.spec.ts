import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { AppState } from '@app/app-state/models/app-state';
import { metaReducers, reducers } from '@app/app-state/reducers/reducers';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { Store, StoreModule } from '@ngrx/store';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { ApplicationDataService } from '@services/application-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { FieldComponent } from '@shared/components/field/field.component';
import { FileUploaderComponent } from '@shared/components/file-uploader/file-uploader.component';
import { of } from 'rxjs';
import { ApplicationRenewalComponent } from './application-renewal.component';

let paymentDataServiceStub: Partial<PaymentDataService>;
let applicationDataServiceStub: Partial<ApplicationDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let tiedHouseConnectionsDataServiceStub: Partial<TiedHouseConnectionsDataService>;
let licenseDataServiceStub: Partial<LicenseDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe('ApplicationRenewalComponent', () => {
  let component: ApplicationRenewalComponent;
  let fixture: ComponentFixture<ApplicationRenewalComponent>;
  let store: MockStore<AppState>;
  let applicationService: ApplicationDataService;

  const account = new Account();
  account.businessType = 'PublicCorporation';

  const initialState = {
    currentAccountState: { currentAccount: account },
    currentUserState: { currentUser: {} }
  } as AppState;

  beforeEach(waitForAsync(() => {
    paymentDataServiceStub = {};
    applicationDataServiceStub = {
      cancelApplication: () => of(null),
      updateApplication: () => of(null),
      getApplicationById: () =>
        of({
          applicationType: {
            contentTypes: []
          } as any
        } as Application)
    };
    licenseDataServiceStub = {};
    dynamicsDataServiceStub = { getRecord: () => of([]) };
    tiedHouseConnectionsDataServiceStub = {
      updateTiedHouse: () => of(null)
    };
    matDialogStub = {};
    matSnackBarStub = {};
    activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });
    TestBed.configureTestingModule({
      declarations: [ApplicationRenewalComponent, FileUploaderComponent, FieldComponent],
      imports: [RouterTestingModule, HttpClientTestingModule, StoreModule.forRoot(reducers, { metaReducers })],
      providers: [
        provideMockStore({ initialState }),
        FormBuilder,
        { provide: PaymentDataService, useValue: paymentDataServiceStub },
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
        { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
        { provide: TiedHouseConnectionsDataService, useValue: tiedHouseConnectionsDataServiceStub },
        { provide: LicenseDataService, useValue: licenseDataServiceStub },
        { provide: MatDialog, useValue: matDialogStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: MatSnackBar, useValue: matSnackBarStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    store = TestBed.get(Store);
    applicationService = TestBed.get(ApplicationDataService);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationRenewalComponent);
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
