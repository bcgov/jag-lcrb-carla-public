import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerQualificationComponent } from './worker-qualification.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('WorkerQualificationComponent', () => {
  let component: WorkerQualificationComponent;
  let fixture: ComponentFixture<WorkerQualificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerQualificationComponent ],
      schemas: [ NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerQualificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
