import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssosiateWizardComponent } from './associate-wizard.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('AssosiateWizardComponent', () => {
  let component: AssosiateWizardComponent;
  let fixture: ComponentFixture<AssosiateWizardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssosiateWizardComponent ],
      schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssosiateWizardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
