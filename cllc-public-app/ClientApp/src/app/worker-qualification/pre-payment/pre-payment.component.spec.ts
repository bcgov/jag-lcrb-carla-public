import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrePaymentComponent } from './pre-payment.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { PaymentDataService } from '@services/payment-data.service';
import { WorkerDataService } from '@services/worker-data.service.';
import { UserDataService } from '@services/user-data.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('PrePaymentComponent', () => {
  let component: PrePaymentComponent;
  let fixture: ComponentFixture<PrePaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PrePaymentComponent],
      providers: [
        {provide: PaymentDataService, useValue: {}},
        {provide: WorkerDataService, useValue: {}},
        {provide: UserDataService, useValue: {
          getCurrentUser: () => of(null)
        }},
        {provide: ActivatedRoute, useValue: new ActivatedRouteStub()},
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrePaymentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
