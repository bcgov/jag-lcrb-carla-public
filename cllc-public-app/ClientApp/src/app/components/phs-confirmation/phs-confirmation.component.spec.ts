import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { PhsConfirmationComponent } from "./phs-confirmation.component";

describe("PhsConfirmationComponent",
  () => {
    let component: PhsConfirmationComponent;
    let fixture: ComponentFixture<PhsConfirmationComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [PhsConfirmationComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(PhsConfirmationComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
