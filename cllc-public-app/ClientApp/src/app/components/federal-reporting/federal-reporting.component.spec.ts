import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FederalReportingComponent } from './federal-reporting.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { LicenseDataService } from '@services/license-data.service';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { of } from 'rxjs';
import { ActivatedRouteStub } from '@app/testing/activated-route-stub';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialogModule } from '@angular/material';

describe('FederalReportingComponent', () => {
  let component: FederalReportingComponent;
  let fixture: ComponentFixture<FederalReportingComponent>;
  let activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [FederalReportingComponent],
      schemas: [NO_ERRORS_SCHEMA],
      imports: [MatDialogModule],
      providers: [
        FormBuilder,
        { provide: MonthlyReportDataService, useValue: { getMonthlyReportsByLicence: () => of([]) } },
        { provide: Router, useValue: {} },
        { provide: ActivatedRoute, useValue: activatedRouteStub },
        { provide: LicenseDataService, useValue: { getAllCurrentLicenses: () => of([]) } },
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
