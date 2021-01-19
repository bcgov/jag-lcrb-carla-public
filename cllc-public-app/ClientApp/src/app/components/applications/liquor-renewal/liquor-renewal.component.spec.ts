import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiquorRenewalComponent } from './liquor-renewal.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { provideMockStore } from '@ngrx/store/testing';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBarModule, MatDialogModule } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { HttpClient } from '@angular/common/http';
import { FeatureFlagService } from '@services/feature-flag.service';
import { of } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';

const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj('HttpClient', ['get']);


describe('LiquorRenewalComponent', () => {
  let component: LiquorRenewalComponent;
  let fixture: ComponentFixture<LiquorRenewalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [LiquorRenewalComponent],
      imports: [MatSnackBarModule, ReactiveFormsModule, MatDialogModule],
      providers: [
        { provide: PaymentDataService, useValue: {} },
        { provide: ApplicationDataService, useValue: { getApplicationById: () => of({}) } },
        { provide: ActivatedRoute, useValue: {} },
        { provide: LicenseDataService, useValue: {} },
        { provide: HttpClient, useValue: httpClientSpy },
        { provide: FeatureFlagService, useValue: { getFeatureFlags: () => of([]) } },
        { provide: Router, useValue: {} },
        provideMockStore({})
      ],
      schemas: [NO_ERRORS_SCHEMA]
    })
      .compileComponents();
  }));
  
  afterEach(() => { fixture.destroy(); });

  beforeEach(() => {
    fixture = TestBed.createComponent(LiquorRenewalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // it('should create', () => {
  //   expect(component).toBeTruthy();
  // });
});
