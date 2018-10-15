import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerInformationComponent } from './worker-information.component';

describe('WorkerInformationComponent', () => {
  let component: WorkerInformationComponent;
  let fixture: ComponentFixture<WorkerInformationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerInformationComponent ]
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
