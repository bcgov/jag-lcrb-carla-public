import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LicenseApplicationSummaryComponent } from './license-application-summary.component';

describe('LicenseApplicationSummaryComponent', () => {
  let component: LicenseApplicationSummaryComponent;
  let fixture: ComponentFixture<LicenseApplicationSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LicenseApplicationSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LicenseApplicationSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
