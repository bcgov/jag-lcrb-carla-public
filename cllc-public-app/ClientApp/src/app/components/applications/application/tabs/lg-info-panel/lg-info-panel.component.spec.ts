import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { LgInfoPanelComponent } from "./lg-info-panel.component";

describe("LgInfoPanelComponent",
  () => {
    let component: LgInfoPanelComponent;
    let fixture: ComponentFixture<LgInfoPanelComponent>;

    beforeEach(async(() => {
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
