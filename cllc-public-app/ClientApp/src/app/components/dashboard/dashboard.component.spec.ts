import { Component, Input, NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { metaReducers, reducers } from '@app/app-state/reducers/reducers';
import { Account } from '@models/account.model';
import { Store, StoreModule } from '@ngrx/store';
import { MockStore, provideMockStore } from '@ngrx/store/testing';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagDataService } from '@services/feature-flag-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { of } from 'rxjs/internal/observable/of';
import { AssociateContentComponent } from './associate-content/associate-content.component';
import { DashboardComponent } from './dashboard.component';

const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj('HttpClient', ['get']);

@Component({ selector: 'app-applications-and-licences', template: '' })
class ApplicationsAndLicencesComponent {
  @Input()
  account: any;
}

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
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
      imports: [StoreModule.forRoot(reducers, { metaReducers })],
      declarations: [DashboardComponent, AssociateContentComponent, ApplicationsAndLicencesComponent],
      schemas: [NO_ERRORS_SCHEMA],
      providers: [
        provideMockStore({ initialState }),
        { provide: Router, useValue: {} },
        { provide: FeatureFlagDataService, useValue: { getFeatureFlags: () => of([]) } },
        { provide: ApplicationDataService, useValue: {} },
        { provide: LicenseDataService, useValue: { getAllCurrentLicenses: () => of([]) } },
        { provide: LegalEntityDataService, useValue: { getCurrentHierachy: () => of({}) } },
        { provide: MatSnackBar, useValue: {} }
      ]
    }).compileComponents();

    store = TestBed.get(Store);
    // applicationService = TestBed.get(ApplicationDataService)
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    fixture.destroy();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should have "Cannabis" in the title', () => {
    const bannerElement: HTMLElement = fixture.nativeElement;
    const header = bannerElement.querySelector('h1');
    expect(header.textContent).toContain('Cannabis');
  });
});
