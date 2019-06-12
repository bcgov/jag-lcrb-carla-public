import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CorporateDetailsComponent } from './corporate-details.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('CorporateDetailsComponent', () => {
  let component: CorporateDetailsComponent;
  let fixture: ComponentFixture<CorporateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CorporateDetailsComponent ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CorporateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
