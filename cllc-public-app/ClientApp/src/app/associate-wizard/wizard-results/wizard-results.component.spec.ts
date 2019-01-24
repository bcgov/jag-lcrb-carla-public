import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WizardResultsComponent } from './wizard-results.component';

describe('WizardResultsComponent', () => {
  let component: WizardResultsComponent;
  let fixture: ComponentFixture<WizardResultsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WizardResultsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WizardResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
