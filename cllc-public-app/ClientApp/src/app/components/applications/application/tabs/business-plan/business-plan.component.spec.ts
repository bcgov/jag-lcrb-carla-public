import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BusinessPlanComponent } from './business-plan.component';

describe('BusinessPlanComponent', () => {
  let component: BusinessPlanComponent;
  let fixture: ComponentFixture<BusinessPlanComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BusinessPlanComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BusinessPlanComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
