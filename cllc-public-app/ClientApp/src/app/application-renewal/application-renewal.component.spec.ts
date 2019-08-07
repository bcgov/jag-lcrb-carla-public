import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApplicationRenewalComponent } from './application-renewal.component';
import { PaymentDataService } from '@services/payment-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { MatDialog, MatSnackBar } from '@angular/material';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { of } from 'rxjs';
import { Application } from '@models/application.model';
import { ApplicationComponent } from '@app/application/application.component';
import { FileUploaderComponent } from '@shared/file-uploader/file-uploader.component';
import { FieldComponent } from '@appshared/field/field.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { StoreModule, Store } from '@ngrx/store';
import { reducers, metaReducers } from '@app/app-state/reducers/reducers';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { NO_ERRORS_SCHEMA } from '@angular/core';

let paymentDataServiceStub: Partial<PaymentDataService>;
let applicationDataServiceStub: Partial<ApplicationDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let tiedHouseConnectionsDataServiceStub: Partial<TiedHouseConnectionsDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe('ApplicationRenewalComponent', () => {
  let component: ApplicationRenewalComponent;
  let fixture: ComponentFixture<ApplicationRenewalComponent>;
  let store: MockStore<AppState>;
  let applicationService: ApplicationDataService;

  const initialState = {
    currentAccountState: { currentAccount: { businessType: 'PublicCorporation' } },
    currentUserState: { currentUser: {} }
  } as AppState;

  beforeEach(async(() => {
    paymentDataServiceStub = {};
    applicationDataServiceStub = {
      getSubmittedApplicationCount: () => of(0),
      cancelApplication: () => of(null),
      updateApplication: () => of(null),
      getApplicationById: () => of(<Application>{
        applicationType: <any>{
          contentTypes: []
        }
      }),

    };
    dynamicsDataServiceStub = { getRecord: () => of([]) };
    tiedHouseConnectionsDataServiceStub = {
      updateTiedHouse: () => of(null)
    };
    matDialogStub = {};
    matSnackBarStub = {};
    activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });
    TestBed.configureTestingModule({
      declarations: [ApplicationRenewalComponent, FileUploaderComponent, FieldComponent],
      imports: [
        RouterTestingModule,
        HttpClientTestingModule,
        StoreModule.forRoot(reducers, { metaReducers }),
      ],
      providers: [
        provideMockStore({ initialState }),
        FormBuilder,
        { provide: PaymentDataService, useValue: paymentDataServiceStub },
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
        { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
        { provide: TiedHouseConnectionsDataService, useValue: tiedHouseConnectionsDataServiceStub },
        { provide: MatDialog, useValue: matDialogStub },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: MatSnackBar, useValue: matSnackBarStub },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();

    store = TestBed.get(Store);
    applicationService = TestBed.get(ApplicationDataService);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationRenewalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
