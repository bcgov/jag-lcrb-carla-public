import { TestBed, async } from '@angular/core/testing';
import {
  RouterTestingModule
} from '@angular/router/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AppComponent } from './app.component';
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { StoreModule, Store } from '@ngrx/store';
import { reducers, metaReducers } from '@app/app-state/reducers/reducers';
import { AccountDataService } from '@services/account-data.service';
import { of } from 'rxjs';
import { provideMockStore, MockStore } from '@ngrx/store/testing';
import { AppState } from '@app/app-state/models/app-state';
import { cold } from 'jasmine-marbles';
import { FeatureFlagService } from '@services/feature-flag.service';

let accountDataServiceStub: Partial<AccountDataService>;
let featureFlagServiceStub: Partial<FeatureFlagService>;

describe('AppComponent', () => {

  let store: MockStore<AppState>;
  const initialState = {
    currentAccountState: {currentAccount: {}},
    currentUserState: { currentUser: {}}
  } as AppState;

  beforeEach(async(() => {

    accountDataServiceStub = {};
    featureFlagServiceStub = {initialized: true};

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
        { provide: FeatureFlagService, useValue: featureFlagServiceStub },
        { provide: AccountDataService, useValue: accountDataServiceStub }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    store = TestBed.get(Store);
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
