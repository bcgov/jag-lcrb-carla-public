import { TestBed, async, ComponentFixture } from "@angular/core/testing";
import {
  RouterTestingModule
} from "@angular/router/testing";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { AppComponent } from "./app.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { AccountDataService } from "@services/account-data.service";
import { provideMockStore } from "@ngrx/store/testing";
import { AppState } from "@app/app-state/models/app-state";
import { FeatureFlagService } from "@services/feature-flag.service";
import { Account } from "@models/account.model";
import { of, Observable } from "rxjs";
import { MatDialog } from "@angular/material/dialog";
import { VersionInfoDataService } from "@services/version-info-data.service";
import { BreadcrumbComponent } from "@components/breadcrumb/breadcrumb.component";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { MatSnackBar } from "@angular/material";
import { ApplicationDataService } from "@services/application-data.service";
import { UserDataService } from "@services/user-data.service";

let accountDataServiceStub: Partial<AccountDataService>;
let featureFlagServiceStub: Partial<FeatureFlagService>;
let userDataServiceStub: Partial<UserDataService>;

describe("AppComponent",
  () => {

    let fixture: ComponentFixture<AppComponent>;
    const initialState = {
      currentAccountState: { currentAccount: new Account() },
      currentUserState: { currentUser: {} }
    } as AppState;

    beforeEach(async(() => {

      accountDataServiceStub = {};
      featureFlagServiceStub = { featureOn: () => of(true) };
      userDataServiceStub = {
        getCurrentUser: () => new Observable(),
        loadUserToStore: () => new Observable().toPromise().then()
      };

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

    afterEach(() => { fixture.destroy(); });

    it("should create the app",
      async(() => {
        const app = fixture.debugElement.componentInstance;
        expect(app).toBeTruthy();
      }));

    it("should render title in a span tag",
      async(() => {
        fixture = TestBed.createComponent(AppComponent);
        fixture.detectChanges();
        const compiled = fixture.debugElement.nativeElement;
        expect(compiled.querySelector("span.title").textContent.trim()).toContain("Cannabis Licensing");
      }));
  });
