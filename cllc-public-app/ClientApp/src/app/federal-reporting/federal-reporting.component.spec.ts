import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FederalReportingComponent } from './federal-reporting.component';

describe('FederalReportingComponent', () => {
  let component: FederalReportingComponent;
  let fixture: ComponentFixture<FederalReportingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FederalReportingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FederalReportingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
