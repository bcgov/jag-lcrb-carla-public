import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SecurityAssessmentsComponent } from './security-assessments.component';

describe('SecurityAssessmentsComponent', () => {
  let component: SecurityAssessmentsComponent;
  let fixture: ComponentFixture<SecurityAssessmentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SecurityAssessmentsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SecurityAssessmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
