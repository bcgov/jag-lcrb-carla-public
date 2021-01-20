import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { FederalReportingComponent } from "./federal-reporting.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { LicenseDataService } from "@services/license-data.service";
import { MonthlyReportDataService } from "@services/monthly-report.service";
import { of } from "rxjs";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { ActivatedRoute, Router } from "@angular/router";
import { MatDialogModule } from "@angular/material/dialog";
import { provideMockStore } from "@ngrx/store/testing";
import { AppState } from "@app/app-state/models/app-state";
import { ProductInventorySalesReportComponent } from
  "./product-inventory-sales-report/product-inventory-sales-report.component";

describe("FederalReportingComponent",
  () => {
    let component: FederalReportingComponent;
    let fixture: ComponentFixture<FederalReportingComponent>;
    const activatedRouteStub = new ActivatedRouteStub({ applicationId: 1 });
    const initialState = {
      currentUserState: { currentUser: {} }
    } as AppState;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [FederalReportingComponent, ProductInventorySalesReportComponent],
          schemas: [NO_ERRORS_SCHEMA],
          imports: [MatDialogModule],
          providers: [
            FormBuilder,
            provideMockStore({ initialState }),
            {
              provide: MonthlyReportDataService,
              useValue: { getAllCurrentMonthlyReports: () => of([]), getMonthlyReportsByLicence: () => of([]) }
            },
            { provide: Router, useValue: {} },
            { provide: ActivatedRoute, useValue: activatedRouteStub },
            { provide: LicenseDataService, useValue: { getAllCurrentLicenses: () => of([]) } },
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(FederalReportingComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    afterEach(() => { fixture.destroy(); });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
