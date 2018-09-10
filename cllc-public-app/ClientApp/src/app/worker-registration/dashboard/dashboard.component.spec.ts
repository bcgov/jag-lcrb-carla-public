import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerDashboardComponent } from './dashboard.component';

describe('WorkerDashboardComponent', () => {
  let component: WorkerDashboardComponent;
  let fixture: ComponentFixture<WorkerDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
