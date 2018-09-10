import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerPaymentConfirmationComponent } from './payment-confirmation.component';

describe('WorkerPaymentConfirmationComponent', () => {
  let component: WorkerPaymentConfirmationComponent;
  let fixture: ComponentFixture<WorkerPaymentConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerPaymentConfirmationComponent ]
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
