import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { CovidConfirmationComponent } from "./covid-confirmation.component";

describe("ConfirmationComponent",
  () => {
    let component: CovidConfirmationComponent;
    let fixture: ComponentFixture<CovidConfirmationComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [CovidConfirmationComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(CovidConfirmationComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
