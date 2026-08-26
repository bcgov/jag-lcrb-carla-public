import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { PaymentDataService } from '@services/payment-data.service';
import { UserDataService } from '@services/user-data.service';
import { of } from 'rxjs';
import { PrePaymentComponent } from './pre-payment.component';

describe('PrePaymentComponent', () => {
  let component: PrePaymentComponent;
  let fixture: ComponentFixture<PrePaymentComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PrePaymentComponent],
      providers: [
        { provide: PaymentDataService, useValue: {} },
        { provide: WorkerDataService, useValue: {} },
        {
          provide: UserDataService,
          useValue: {
            getCurrentUser: () => of(null)
          }
        },
        { provide: ActivatedRoute, useValue: new ActivatedRouteStub() }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
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
