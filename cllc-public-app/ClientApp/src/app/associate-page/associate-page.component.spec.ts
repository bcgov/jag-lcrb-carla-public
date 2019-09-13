import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssociatePageComponent } from './associate-page.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder } from '@angular/forms';
import { Store, StoreModule } from '@ngrx/store';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute } from '@angular/router';
import { PaymentDataService } from '@services/payment-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { MatDialog, MatSnackBar } from '@angular/material';
import { of } from 'rxjs';
import { Application } from '@models/application.model';
import { Account } from '@models/account.model';
import { FileUploaderComponent } from '@shared/file-uploader/file-uploader.component';
import { FieldComponent } from '@shared/field/field.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { reducers, metaReducers } from '@app/app-state/reducers/reducers';


let paymentDataServiceStub: Partial<PaymentDataService>;
let applicationDataServiceStub: Partial<ApplicationDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let tiedHouseConnectionsDataServiceStub: Partial<TiedHouseConnectionsDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe('AssociatePageComponent', () => {
  let component: AssociatePageComponent;
  let fixture: ComponentFixture<AssociatePageComponent>;


  const account = Object.assign(new Account(), {
    businessType: 'PublicCorporation',
    legalEntity: { id: '0' }
  });
  const initialState = {
    currentAccountState: {
      currentAccount: account,
    },
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
      declarations: [AssociatePageComponent, FileUploaderComponent, FieldComponent],
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

  }));


  beforeEach(() => {
    fixture = TestBed.createComponent(AssociatePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
