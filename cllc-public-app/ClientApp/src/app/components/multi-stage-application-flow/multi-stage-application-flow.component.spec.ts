import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { MultiStageApplicationFlowComponent } from "./multi-stage-application-flow.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { AppState } from "@app/app-state/models/app-state";
import { provideMockStore } from "@ngrx/store/testing";
import { ApplicationLicenseeChangesComponent } from
  "@components/applications/application-licensee-changes/application-licensee-changes.component";
import { AccountProfileComponent } from "@components/account-profile/account-profile.component";
import { ApplicationComponent } from "@components/applications/application/application.component";
import { FeatureFlagService } from "@services/feature-flag.service";
import { of } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { MatDialogModule } from "@angular/material/dialog";
import { MatIconModule } from "@angular/material/icon";
import { MatSnackBarModule } from "@angular/material/snack-bar";
import { ApplicationDataService } from "@services/application-data.service";
import { PaymentDataService } from "@services/payment-data.service";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { ReactiveFormsModule } from "@angular/forms";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { HttpClient } from "@angular/common/http";
import { AccountDataService } from "@services/account-data.service";


const httpClientSpy: { get: jasmine.Spy } = jasmine.createSpyObj("HttpClient", ["get"]);

describe("MultiStageApplicationFlowComponent",
  () => {
    let component: MultiStageApplicationFlowComponent;
    let fixture: ComponentFixture<MultiStageApplicationFlowComponent>;
    const initialState = {
      onGoingLicenseeChangesApplicationIdState: { onGoingLicenseeChangesApplicationId: "1" },
    } as AppState;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [
            MultiStageApplicationFlowComponent, ApplicationLicenseeChangesComponent, AccountProfileComponent,
            ApplicationComponent
          ],
          imports: [ReactiveFormsModule, MatIconModule, MatSnackBarModule, MatDialogModule],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            provideMockStore({ initialState }),
            { provide: ActivatedRoute, useValue: new ActivatedRouteStub() },
            { provide: AccountDataService, useValue: {} },
            { provide: Router, useValue: {} },
            { provide: DynamicsDataService, useValue: {} },
            { provide: HttpClient, useValue: httpClientSpy },
            { provide: TiedHouseConnectionsDataService, useValue: {} },
            { provide: PaymentDataService, useValue: {} },
            { provide: FeatureFlagService, useValue: { featureOn: () => of(true) } }
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(MultiStageApplicationFlowComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    afterEach(() => { fixture.destroy(); });

    // it('should create', () => {
    //   expect(component).toBeTruthy();
    // });
  });
