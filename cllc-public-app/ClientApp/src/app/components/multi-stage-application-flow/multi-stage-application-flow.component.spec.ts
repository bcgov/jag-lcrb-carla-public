import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MultiStageApplicationFlowComponent } from './multi-stage-application-flow.component';

describe('MultiStageApplicationFlowComponent', () => {
  let component: MultiStageApplicationFlowComponent;
  let fixture: ComponentFixture<MultiStageApplicationFlowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MultiStageApplicationFlowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MultiStageApplicationFlowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
