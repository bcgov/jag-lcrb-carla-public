import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DecisionNotMadeApplicationsComponent } from './decision-not-made-applications.component';

describe('DecisionNotMadeApplicationsComponent', () => {
  let component: DecisionNotMadeApplicationsComponent;
  let fixture: ComponentFixture<DecisionNotMadeApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DecisionNotMadeApplicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DecisionNotMadeApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
