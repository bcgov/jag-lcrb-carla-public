import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { LgInConfirmationOfReceiptComponent } from "./lg-in-confirmation-of-receipt.component";

describe("LgInConfirmationOfReceiptComponent",
  () => {
    let component: LgInConfirmationOfReceiptComponent;
    let fixture: ComponentFixture<LgInConfirmationOfReceiptComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [LgInConfirmationOfReceiptComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LgInConfirmationOfReceiptComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
