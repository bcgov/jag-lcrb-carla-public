
import { ApplicationAndLicenceFeeComponent } from "./application-and-licence-fee.component";
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { ActivatedRoute } from "@angular/router";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { StoreModule } from "@ngrx/store";
import { reducers, metaReducers } from "@app/app-state/reducers/reducers";
import { PaymentDataService } from "@services/payment-data.service";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { RouterTestingModule } from "@angular/router/testing";
import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ApplicationDataService } from "@services/application-data.service";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { FormBuilder } from "@angular/forms";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { of } from "rxjs";
import { Application } from "@models/application.model";
import { provideMockStore } from "@ngrx/store/testing";
import { AppState } from "@app/app-state/models/app-state";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { Account } from "@models/account.model";
import { FieldComponent } from "@shared/components/field/field.component";

let paymentDataServiceStub: Partial<PaymentDataService>;
let applicationDataServiceStub: Partial<ApplicationDataService>;
let dynamicsDataServiceStub: Partial<DynamicsDataService>;
let tiedHouseConnectionsDataServiceStub: Partial<TiedHouseConnectionsDataService>;
let matDialogStub: Partial<MatDialog>;
let matSnackBarStub: Partial<MatSnackBar>;
let activatedRouteStub: ActivatedRouteStub;

describe("ApplicationAndLicenceFeeComponent",
  () => {
    let component: ApplicationAndLicenceFeeComponent;
    let fixture: ComponentFixture<ApplicationAndLicenceFeeComponent>;

    const account = new Account();
    account.businessType = "PublicCorporation";
    const initialState = {
      currentAccountState: { currentAccount: account },
      currentUserState: { currentUser: {} }
    } as AppState;

    beforeEach(waitForAsync(() => {
      paymentDataServiceStub = {};
      applicationDataServiceStub = {
        cancelApplication: () => of(null),
        updateApplication: () => of(null),
        getApplicationById: () => of({
          applicationType: {
            contentTypes: []
          } as any
        } as Application),

      };
      dynamicsDataServiceStub = { getRecord: () => of([]) };
      tiedHouseConnectionsDataServiceStub = {
        updateTiedHouse: () => of(null)
      };
      matDialogStub = {};
      matSnackBarStub = {};
      activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });
      TestBed.configureTestingModule({
          declarations: [ApplicationAndLicenceFeeComponent, FieldComponent],
          imports: [
            RouterTestingModule,
            HttpClientTestingModule,
            StoreModule.forRoot(reducers, { metaReducers }),
          ],
          providers: [
            provideMockStore({ initialState }),
            FormBuilder,
            { provide: PaymentDataService, useValue: paymentDataServiceStub },
            { provide: ApplicationDataService, useValue: applicationDataServiceStub },
            { provide: DynamicsDataService, useValue: dynamicsDataServiceStub },
            { provide: TiedHouseConnectionsDataService, useValue: tiedHouseConnectionsDataServiceStub },
            { provide: MatDialog, useValue: matDialogStub },
            { provide: ActivatedRoute, useValue: activatedRouteStub },
            { provide: MatSnackBar, useValue: matSnackBarStub },
          ],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ApplicationAndLicenceFeeComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    afterEach(() => { fixture.destroy(); });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
