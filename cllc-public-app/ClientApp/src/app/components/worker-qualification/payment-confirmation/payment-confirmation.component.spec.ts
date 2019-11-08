import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerPaymentConfirmationComponent } from './payment-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ActivatedRoute,  Router } from '@angular/router';
import { PaymentDataService } from '@services/payment-data.service';
import { of } from 'rxjs';

const routerSpy = jasmine.createSpyObj('Router', ['navigateByUrl']);

describe('WorkerPaymentConfirmationComponent', () => {
  let component: WorkerPaymentConfirmationComponent;
  let fixture: ComponentFixture<WorkerPaymentConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [WorkerPaymentConfirmationComponent],
      providers: [
        { provide: Router, useValue: routerSpy },
        { provide: PaymentDataService, useValue: { verifyWorkerPaymentSubmission: () => of({}) } },
        { provide: ActivatedRoute, useValue: { queryParams: of({}), params: {} } }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerPaymentConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
