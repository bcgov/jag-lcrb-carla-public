import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { AdditionalPidsComponent } from "./additional-pids.component";

describe("AdditionalPidsComponent",
  () => {
    let component: AdditionalPidsComponent;
    let fixture: ComponentFixture<AdditionalPidsComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [AdditionalPidsComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(AdditionalPidsComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
