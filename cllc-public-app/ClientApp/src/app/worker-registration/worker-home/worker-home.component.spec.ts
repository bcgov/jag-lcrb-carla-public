import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkerHomeComponent } from './worker-home.component';

describe('WorkerHomeComponent', () => {
  let component: WorkerHomeComponent;
  let fixture: ComponentFixture<WorkerHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WorkerHomeComponent ]
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
