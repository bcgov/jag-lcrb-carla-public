import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerInformationComponent } from './worker-information.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('WorkerInformationComponent', () => {
  let component: WorkerInformationComponent;
  let fixture: ComponentFixture<WorkerInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerInformationComponent ],
      schemas: [ NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerInformationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
