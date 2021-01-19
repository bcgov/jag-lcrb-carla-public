import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { AssociateContentComponent } from "./associate-content.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FeatureFlagService } from "@services/feature-flag.service";
import { of } from "rxjs";

describe("AssociateContentComponent",
  () => {
    let component: AssociateContentComponent;
    let fixture: ComponentFixture<AssociateContentComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [AssociateContentComponent],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            { provide: FeatureFlagService, useValue: { featureOn: () => of(true) } }
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(AssociateContentComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
