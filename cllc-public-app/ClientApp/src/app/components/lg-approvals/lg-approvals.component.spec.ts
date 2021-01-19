import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { LgApprovalsComponent } from "./lg-approvals.component";

describe("LgApprovalsComponent",
  () => {
    let component: LgApprovalsComponent;
    let fixture: ComponentFixture<LgApprovalsComponent>;

    beforeEach(async(() => {
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
