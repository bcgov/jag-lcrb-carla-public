import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { LgZoningConfirmationComponent } from "./lg-zoning-confirmation.component";

describe("LgZoningComfirmationComponent",
  () => {
    let component: LgZoningConfirmationComponent;
    let fixture: ComponentFixture<LgZoningConfirmationComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [LgZoningConfirmationComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LgZoningConfirmationComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
