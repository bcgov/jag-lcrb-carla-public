import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { LgApprovalsComponent } from "./lg-approvals.component";

describe("LgApprovalsComponent",
  () => {
    let component: LgApprovalsComponent;
    let fixture: ComponentFixture<LgApprovalsComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [LgApprovalsComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LgApprovalsComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
