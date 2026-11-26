import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationTypeNames } from '@models/application-type.model';
import { Store } from '@ngrx/store';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { UserDataService } from '@services/user-data.service';
import { startOfToday, subDays, subYears } from 'date-fns';
import { of, Subject } from 'rxjs';
import { ApplicationsAndLicencesComponent } from './applications-and-licences.component';

const applicationDataServiceStub: Partial<ApplicationDataService> = {
  getAllCurrentApplications: () => of([]),
  getSubmittedCannabisRetailStoreApplicationCount: () => of(0)
};
const licenceDataServiceStub: Partial<LicenseDataService> = { getAllCurrentLicenses: () => of([]) };
const routerStub: Partial<Router> = {};
const paymentServiceStub: Partial<PaymentDataService> = {};
const snackBarStub: Partial<MatSnackBar> = {};
const featureFlagServiceStub: Partial<FeatureFlagService> = { featureOn: () => of(true) };
const dialogStub: Partial<MatDialog> = {};

describe('ApplicationsAndLicencesComponent', () => {
  let component: ApplicationsAndLicencesComponent;
  let fixture: ComponentFixture<ApplicationsAndLicencesComponent>;

  let store: MockStore<AppState>;

  const account = new Account();
  account.businessType = 'PublicCorporation';
  const initialState = {
    currentAccountState: { currentAccount: account },
    currentUserState: { currentUser: {} },
    indigenousNationState: { indigenousNationModeActive: false }
  } as AppState;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ApplicationsAndLicencesComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        { provide: ApplicationDataService, useValue: applicationDataServiceStub },
        { provide: LicenseDataService, useValue: licenceDataServiceStub },
        { provide: PaymentDataService, useValue: paymentServiceStub },
        { provide: UserDataService, useValue: {} },
        { provide: MatSnackBar, useValue: snackBarStub },
        { provide: FeatureFlagService, useValue: featureFlagServiceStub },
        { provide: MatDialog, useValue: dialogStub },
        { provide: Router, useValue: routerStub },
        provideMockStore({ initialState })
      ]
    }).compileComponents();
    store = TestBed.get(Store);
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApplicationsAndLicencesComponent);
    component = fixture.componentInstance;
    component.account = account;
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(component).toBeTruthy();
  });

  function load(applications: ApplicationSummary[], licenses: ApplicationLicenseSummary[]) {
    spyOn(TestBed.get(ApplicationDataService), 'getAllCurrentApplications').and.returnValue(of(applications));
    spyOn(TestBed.get(LicenseDataService), 'getAllCurrentLicenses').and.returnValue(of(licenses));
    fixture.detectChanges();
  }

  const marketerApplication = {
    applicationTypeName: ApplicationTypeNames.Marketer,
    licenceId: 'marketer-licence'
  } as ApplicationSummary;

  function marketerLicense(status: string, expiryDate: Date): ApplicationLicenseSummary {
    return {
      licenceTypeName: ApplicationTypeNames.Marketer,
      licenseId: 'marketer-licence',
      status,
      expiryDate
    } as ApplicationLicenseSummary;
  }

  it('allows applications when the linked marketer licence is cancelled', () => {
    load([marketerApplication], [marketerLicense('Cancelled', startOfToday())]);
    expect(component.marketerExists).toBe(false);
  });

  it('allows applications when the linked marketer licence expired more than one year ago', () => {
    load([marketerApplication], [marketerLicense('Expired', subDays(subYears(startOfToday(), 1), 1))]);
    expect(component.marketerExists).toBe(false);
  });

  it('blocks applications on the one-year anniversary', () => {
    load([marketerApplication], [marketerLicense('Expired', subYears(startOfToday(), 1))]);
    expect(component.marketerExists).toBe(true);
  });

  it('blocks applications when another marketer licence is valid', () => {
    load(
      [marketerApplication],
      [
        marketerLicense('Cancelled', startOfToday()),
        { ...marketerLicense('Active', startOfToday()), licenseId: 'other-licence' }
      ]
    );
    expect(component.marketerExists).toBe(true);
  });

  it('blocks applications for an unlicensed marketer application', () => {
    load([{ ...marketerApplication, licenceId: null }], []);
    expect(component.marketerExists).toBe(true);
  });

  it('allows applications for an approved marketer application whose cancelled licence is filtered out', () => {
    load([{ ...marketerApplication, applicationStatus: 'Approved' }], []);
    expect(component.marketerExists).toBe(false);
  });

  it('keeps cannabis applications hidden until both requests complete when licences arrive first', () => {
    const applications = new Subject<ApplicationSummary[]>();
    const licenses = new Subject<ApplicationLicenseSummary[]>();
    spyOn(TestBed.get(ApplicationDataService), 'getAllCurrentApplications').and.returnValue(applications);
    spyOn(TestBed.get(LicenseDataService), 'getAllCurrentLicenses').and.returnValue(licenses);
    fixture.detectChanges();

    expect(component.marketerExists).toBe(true);
    licenses.next([marketerLicense('Cancelled', startOfToday())]);
    licenses.complete();
    expect(component.marketerExists).toBe(true);
    applications.next([marketerApplication]);
    expect(component.marketerExists).toBe(true);
    applications.complete();
    expect(component.marketerExists).toBe(false);
    expect(component.dataLoaded).toBe(true);
  });

  it('keeps cannabis applications hidden until both requests complete when applications arrive first', () => {
    const applications = new Subject<ApplicationSummary[]>();
    const licenses = new Subject<ApplicationLicenseSummary[]>();
    spyOn(TestBed.get(ApplicationDataService), 'getAllCurrentApplications').and.returnValue(applications);
    spyOn(TestBed.get(LicenseDataService), 'getAllCurrentLicenses').and.returnValue(licenses);
    fixture.detectChanges();

    applications.next([marketerApplication]);
    applications.complete();
    expect(component.marketerExists).toBe(true);
    expect(component.dataLoaded).toBe(false);
    licenses.next([marketerLicense('Cancelled', startOfToday())]);
    licenses.complete();
    expect(component.marketerExists).toBe(false);
    expect(component.dataLoaded).toBe(true);
  });
});
