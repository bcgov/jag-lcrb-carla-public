import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerHomeComponent } from './worker-home.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('WorkerHomeComponent', () => {
  let component: WorkerHomeComponent;
  let fixture: ComponentFixture<WorkerHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerHomeComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
