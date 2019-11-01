import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { SpdConsentComponent } from './spd-consent.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideMockStore } from '@ngrx/store/testing';
import { ActivatedRouteStub } from './../../testing/activated-route-stub';
import { UserDataService } from '@services/user-data.service';
import { AliasDataService } from '@services/alias-data.service';
import { PreviousAddressDataService } from '@services/previous-address-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { WorkerDataService } from '@services/worker-data.service.';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar } from '@angular/material';

const userDataServiceStub: Partial<UserDataService> = {
  getCurrentUser: () => of(null)
};
const aliasDataServiceStupb: Partial<AliasDataService> = {};
const previousAddressDataServiceStub: Partial<PreviousAddressDataService> = {};
const contactDataServiceStub: Partial<ContactDataService> = {};
const workerDataServiceStub: Partial<WorkerDataService> = {
  getWorker: () => of(<any>{})
};
const paymentDataServiceStub: Partial<PaymentDataService> = {};
const routeStub = new ActivatedRouteStub();
const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('SpdConsentComponent', () => {
  let component: SpdConsentComponent;
  let fixture: ComponentFixture<SpdConsentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [SpdConsentComponent],
      imports: [ReactiveFormsModule, FormsModule],
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
        { provide: MatSnackBar, useValue: {} },
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpdConsentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
