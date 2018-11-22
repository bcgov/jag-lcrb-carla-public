import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssosiateWizardComponent } from './assosiate-wizard.component';

describe('AssosiateWizardComponent', () => {
  let component: AssosiateWizardComponent;
  let fixture: ComponentFixture<AssosiateWizardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssosiateWizardComponent ]
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
