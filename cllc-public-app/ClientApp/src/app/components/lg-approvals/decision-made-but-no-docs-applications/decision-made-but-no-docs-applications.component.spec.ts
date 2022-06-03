import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DecisionMadeButNoDocsApplicationsComponent } from './decision-made-but-no-docs-applications.component';

describe('DecisionMadeButNoDocsApplicationsComponent', () => {
  let component: DecisionMadeButNoDocsApplicationsComponent;
  let fixture: ComponentFixture<DecisionMadeButNoDocsApplicationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DecisionMadeButNoDocsApplicationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DecisionMadeButNoDocsApplicationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
