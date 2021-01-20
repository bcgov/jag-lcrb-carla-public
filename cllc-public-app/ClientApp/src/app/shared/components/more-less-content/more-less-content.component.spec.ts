import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { MoreLessContentComponent } from "./more-less-content.component";

describe("MoreLessContentComponent",
  () => {
    let component: MoreLessContentComponent;
    let fixture: ComponentFixture<MoreLessContentComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [MoreLessContentComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(MoreLessContentComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
