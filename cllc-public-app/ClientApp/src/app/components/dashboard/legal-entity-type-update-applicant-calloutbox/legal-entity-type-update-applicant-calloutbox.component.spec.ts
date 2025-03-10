import { ComponentFixture, TestBed } from "@angular/core/testing";

import { LegalEntityTypeUpdateCalloutboxComponent } from "./legal-entity-type-update-applicant-calloutbox.component";

describe("LegalEntityTypeUpdateCalloutboxComponent",
  () => {
    let component: LegalEntityTypeUpdateCalloutboxComponent;
    let fixture: ComponentFixture<LegalEntityTypeUpdateCalloutboxComponent>;

    beforeEach(async () => {
      await TestBed.configureTestingModule({
          declarations: [LegalEntityTypeUpdateCalloutboxComponent]
        })
        .compileComponents();
    });

    beforeEach(() => {
      fixture = TestBed.createComponent(LegalEntityTypeUpdateCalloutboxComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
