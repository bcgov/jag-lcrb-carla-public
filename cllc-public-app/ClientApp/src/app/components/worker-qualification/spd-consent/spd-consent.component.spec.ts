import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormBuilder, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { provideMockStore } from '@ngrx/store/testing';
import { AliasDataService } from '@services/alias-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { PreviousAddressDataService } from '@services/previous-address-data.service';
import { UserDataService } from '@services/user-data.service';
import { of } from 'rxjs';
import { SpdConsentComponent } from './spd-consent.component';

const userDataServiceStub: Partial<UserDataService> = {
  getCurrentUser: () => of(null)
};
const aliasDataServiceStupb: Partial<AliasDataService> = {};
const previousAddressDataServiceStub: Partial<PreviousAddressDataService> = {};
const contactDataServiceStub: Partial<ContactDataService> = {};
const workerDataServiceStub: Partial<WorkerDataService> = {
  getWorker: () => of({} as any)
};
const paymentDataServiceStub: Partial<PaymentDataService> = {};
const routeStub = new ActivatedRouteStub();
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('SpdConsentComponent', () => {
  let component: SpdConsentComponent;
  let fixture: ComponentFixture<SpdConsentComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [SpdConsentComponent],
      imports: [ReactiveFormsModule, FormsModule, HttpClientTestingModule],
      providers: [
        provideMockStore({}),
        FormBuilder,
        { provide: ActivatedRoute, useValue: routeStub },
        { provide: Router, useValue: routerSpy },
        { provide: UserDataService, useValue: userDataServiceStub },
        { provide: AliasDataService, useValue: aliasDataServiceStupb },
        { provide: ContactDataService, useValue: contactDataServiceStub },
        { provide: WorkerDataService, useValue: workerDataServiceStub },
        { provide: PreviousAddressDataService, useValue: previousAddressDataServiceStub },
        { provide: PaymentDataService, useValue: paymentDataServiceStub },
        { provide: MatSnackBar, useValue: {} }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpdConsentComponent);
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
