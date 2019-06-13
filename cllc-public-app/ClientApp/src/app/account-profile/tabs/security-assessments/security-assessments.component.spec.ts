import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SecurityAssessmentsComponent } from './security-assessments.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('SecurityAssessmentsComponent', () => {
  let component: SecurityAssessmentsComponent;
  let fixture: ComponentFixture<SecurityAssessmentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SecurityAssessmentsComponent ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SecurityAssessmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
