import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { LgInfoPanelComponent } from "./lg-info-panel.component";

describe("LgInfoPanelComponent",
  () => {
    let component: LgInfoPanelComponent;
    let fixture: ComponentFixture<LgInfoPanelComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [LgInfoPanelComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LgInfoPanelComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
