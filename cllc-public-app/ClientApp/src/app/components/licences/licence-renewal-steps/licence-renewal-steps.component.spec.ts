import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { LicenceRenewalStepsComponent } from "./licence-renewal-steps.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FeatureFlagService } from "@services/feature-flag.service";
import { of } from "rxjs";
import { MatIconModule } from "@angular/material/icon";
import { ActivatedRoute } from "@angular/router";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";

describe("LicenceRenewalStepsComponent",
  () => {
    let component: LicenceRenewalStepsComponent;
    let fixture: ComponentFixture<LicenceRenewalStepsComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [LicenceRenewalStepsComponent],
          imports: [MatIconModule,],
          providers: [
            { provide: FeatureFlagService, useValue: { featureOn: () => of(true) } },
            { provide: ActivatedRoute, useValue: new ActivatedRouteStub() }
          ],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LicenceRenewalStepsComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
