import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StandalonePaymentConfirmationComponent } from './standalone-payment-confirmation.component';

describe('StandalonePaymentConfirmationComponent', () => {
  let fixture: ComponentFixture<StandalonePaymentConfirmationComponent>;
  let component: StandalonePaymentConfirmationComponent;
  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      providers: [],
      declarations: [StandalonePaymentConfirmationComponent]
    });

    fixture = TestBed.createComponent(StandalonePaymentConfirmationComponent);
    component = fixture.componentInstance;
  });

  it('should be able to create component instance', () => {
    expect(component).toBeDefined();
  });
});
