import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EligibilityFormComponent } from './eligibility-form.component';

describe('EligibilityFormComponent', () => {
  let component: EligibilityFormComponent;
  let fixture: ComponentFixture<EligibilityFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EligibilityFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EligibilityFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
