import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerApplicationComponent } from './worker-application.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('WorkerApplicationComponent', () => {
  let component: WorkerApplicationComponent;
  let fixture: ComponentFixture<WorkerApplicationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerApplicationComponent ],
      schemas: [ NO_ERRORS_SCHEMA ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerApplicationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
