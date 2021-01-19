import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { LiquorApprovalsCalloutComponent } from "./liquor-approvals-callout.component";

describe("LiquorApprovalsCalloutComponent",
  () => {
    let component: LiquorApprovalsCalloutComponent;
    let fixture: ComponentFixture<LiquorApprovalsCalloutComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [LiquorApprovalsCalloutComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(LiquorApprovalsCalloutComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
