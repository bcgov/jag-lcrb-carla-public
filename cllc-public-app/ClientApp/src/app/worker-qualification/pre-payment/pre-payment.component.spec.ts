import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrePaymentComponent } from './pre-payment.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('PrePaymentComponent', () => {
  let component: PrePaymentComponent;
  let fixture: ComponentFixture<PrePaymentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PrePaymentComponent],
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
