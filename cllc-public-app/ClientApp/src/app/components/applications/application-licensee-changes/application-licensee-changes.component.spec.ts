import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { ApplicationLicenseeChangesComponent } from "./application-licensee-changes.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { LicenseeTreeComponent } from "@shared/components/licensee-tree/licensee-tree.component";
import { MatMenuModule, MatTreeModule, MatDialogModule, MatSnackBar } from "@angular/material";
import { ActivatedRoute, Router } from "@angular/router";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { ApplicationDataService } from "@services/application-data.service";
import { LegalEntityDataService } from "@services/legal-entity-data.service";
import { of } from "rxjs/internal/observable/of";
import { FormBuilder } from "@angular/forms";
import { provideMockStore } from "@ngrx/store/testing";
import { StoreModule } from "@ngrx/store";
import { reducers, metaReducers } from "@app/app-state/reducers/reducers";
import { LicenseDataService } from "@services/license-data.service";
import { AppState } from "@app/app-state/models/app-state";
import { Account } from "@models/account.model";
import { FeatureFlagService } from "@services/feature-flag.service";
import { PaymentDataService } from "@services/payment-data.service";

describe("ApplicationLicenseeChangesComponent",
  () => {
    let component: ApplicationLicenseeChangesComponent;
    let fixture: ComponentFixture<ApplicationLicenseeChangesComponent>;
    const initialState = {
      currentAccountState: { currentAccount: new Account() },
      onGoingLicenseeChangesApplicationIdState: { onGoingLicenseeChangesApplicationId: "1" }
    } as AppState;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [ApplicationLicenseeChangesComponent, LicenseeTreeComponent],
          imports: [
            MatMenuModule, MatTreeModule, MatDialogModule,
            StoreModule.forRoot(reducers, { metaReducers }),
          ],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            FormBuilder,
            provideMockStore({}),
            { provide: ActivatedRoute, useValue: new ActivatedRouteStub({}) },
            { provide: MatSnackBar, useValue: {} },
            { provide: LicenseDataService, useValue: { getAllCurrentLicenses: () => of([]) } },
            { provide: Router, useValue: {} },
            { provide: PaymentDataService, useValue: {} },
            { provide: FeatureFlagService, useValue: { featureOn: () => of(true) } },
            {
              provide: LegalEntityDataService,
              useValue: { getChangeApplicationLogs: () => of([]), getCurrentHierachy: () => of({}) }
            },
            {
              provide: ApplicationDataService,
              useValue: {
                getApplicationById: () => of({}),
                getAllCurrentApplications: () => of([]),
                getOngoingLicenseeChangeApplicationId: () => of("1")
              }
            },
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ApplicationLicenseeChangesComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    afterEach(() => { fixture.destroy(); });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
