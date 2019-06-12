import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerPaymentConfirmationComponent } from './payment-confirmation.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('WorkerPaymentConfirmationComponent', () => {
  let component: WorkerPaymentConfirmationComponent;
  let fixture: ComponentFixture<WorkerPaymentConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [WorkerPaymentConfirmationComponent],
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
