import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpdConsentComponent } from './spd-consent.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('SpdConsentComponent', () => {
  let component: SpdConsentComponent;
  let fixture: ComponentFixture<SpdConsentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SpdConsentComponent ],
      schemas: [ NO_ERRORS_SCHEMA ]
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
