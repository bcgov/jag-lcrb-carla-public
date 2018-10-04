import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerQualificationComponent } from './worker-qualification.component';

describe('WorkerQualificationComponent', () => {
  let component: WorkerQualificationComponent;
  let fixture: ComponentFixture<WorkerQualificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerQualificationComponent ]
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
