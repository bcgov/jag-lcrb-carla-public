import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerRegistrationComponent } from './worker-registration.component';

describe('WorkerRegistrationComponent', () => {
  let component: WorkerRegistrationComponent;
  let fixture: ComponentFixture<WorkerRegistrationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerRegistrationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkerRegistrationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
