import { TestBed, async } from '@angular/core/testing';
import {
  RouterTestingModule
} from '@angular/router/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { AccountDataService } from '@services/account-data.service';
import { provideMockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { FeatureFlagService } from '@services/feature-flag.service';
import { Account } from '@models/account.model';
import { of } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { VersionInfoDataService } from '@services/version-info-data.service';
import { BreadcrumbComponent } from '@components/breadcrumb/breadcrumb.component';

let accountDataServiceStub: Partial<AccountDataService>;
let featureFlagServiceStub: Partial<FeatureFlagService>;

describe('AppComponent', () => {

  const initialState = {
    currentAccountState: {currentAccount: new Account()},
    currentUserState: { currentUser: {}}
  } as AppState;

  beforeEach(async(() => {

    accountDataServiceStub = {};
    featureFlagServiceStub = {featureOn: () => of(true)};

    TestBed.configureTestingModule({
      declarations: [
        AppComponent,
        BreadcrumbComponent
      ],
      imports: [
        RouterTestingModule,
        HttpClientTestingModule
      ],
      providers: [
        provideMockStore({ initialState }),
        { provide: VersionInfoDataService, useValue: {getVersionInfo: () => of({})} },
        { provide: MatDialog, useValue: {} },
        { provide: FeatureFlagService, useValue: featureFlagServiceStub },
        { provide: AccountDataService, useValue: accountDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

  }));

  it('should create the app', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.debugElement.componentInstance;
    expect(app).toBeTruthy();
  }));

  it('should render title in a span tag', async(() => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.debugElement.nativeElement;
    expect(compiled.querySelector('span.title').textContent.trim()).toContain('Cannabis Licensing');
  }));
});
