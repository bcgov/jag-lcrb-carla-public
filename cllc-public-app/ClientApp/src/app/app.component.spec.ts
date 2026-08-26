import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RouterTestingModule } from '@angular/router/testing';
import { AppState } from '@app/app-state/models/app-state';
import { BreadcrumbComponent } from '@components/breadcrumb/breadcrumb.component';
import { Account } from '@models/account.model';
import { provideMockStore } from '@ngrx/store/testing';
import { AccountDataService } from '@services/account-data.service';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { MonthlyReportDataService } from '@services/monthly-report.service';
import { UserDataService } from '@services/user-data.service';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { Observable, of } from 'rxjs';
import { AppComponent } from './app.component';

let accountDataServiceStub: Partial<AccountDataService>;
let featureFlagServiceStub: Partial<FeatureFlagService>;
let userDataServiceStub: Partial<UserDataService>;

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  const initialState = {
    currentAccountState: { currentAccount: new Account() },
    currentUserState: { currentUser: {} }
  } as AppState;

  beforeEach(waitForAsync(() => {
    accountDataServiceStub = {};
    featureFlagServiceStub = { featureOn: () => of(true) };
    userDataServiceStub = {
      getCurrentUser: () => new Observable(),
      loadUserToStore: () => new Observable().toPromise().then()
    };

    TestBed.configureTestingModule({
      declarations: [AppComponent, BreadcrumbComponent],
      imports: [RouterTestingModule, HttpClientTestingModule],
      providers: [
        { provide: MatSnackBar, useValue: {} },
        { provide: ApplicationDataService, useValue: { getOngoingLicenseeChangeApplicationId: () => of({}) } },
        provideMockStore({ initialState }),
        { provide: VersionInfoDataService, useValue: { getVersionInfo: () => of({}) } },
        { provide: MonthlyReportDataService, useValue: { getAllCurrentMonthlyReports: () => of([]) } },
        { provide: MatDialog, useValue: {} },
        { provide: FeatureFlagService, useValue: featureFlagServiceStub },
        { provide: AccountDataService, useValue: accountDataServiceStub },
        { provide: UserDataService, useValue: userDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create the app', waitForAsync(() => {
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));

  it('should render title in a span tag', waitForAsync(() => {
    fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('span.title').textContent.trim()).toContain('Cannabis Licensing');
  }));
});
