import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerApplicationComponent } from './worker-application.component';

describe('WorkerApplicationComponent', () => {
  let component: WorkerApplicationComponent;
  let fixture: ComponentFixture<WorkerApplicationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerApplicationComponent ]
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
