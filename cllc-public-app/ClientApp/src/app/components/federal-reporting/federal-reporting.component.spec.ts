import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FederalReportingComponent } from './federal-reporting.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { of } from 'rxjs';

describe('FederalReportingComponent', () => {
  let component: FederalReportingComponent;
  let fixture: ComponentFixture<FederalReportingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [FederalReportingComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        FormBuilder,
        { provide: MonthlyReportDataService, useValue: {} },
        { provide: LicenseDataService, useValue: { getAllCurrentLicenses: () => of([])} },
      ]
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
