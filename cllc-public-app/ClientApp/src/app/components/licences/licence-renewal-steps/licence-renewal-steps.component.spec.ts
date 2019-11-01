import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenceRenewalStepsComponent } from './licence-renewal-steps.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('LicenceRenewalStepsComponent', () => {
  let component: LicenceRenewalStepsComponent;
  let fixture: ComponentFixture<LicenceRenewalStepsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenceRenewalStepsComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenceRenewalStepsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
