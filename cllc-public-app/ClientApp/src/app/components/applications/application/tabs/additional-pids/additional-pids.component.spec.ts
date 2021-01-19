import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { AdditionalPidsComponent } from "./additional-pids.component";

describe("AdditionalPidsComponent",
  () => {
    let component: AdditionalPidsComponent;
    let fixture: ComponentFixture<AdditionalPidsComponent>;

    beforeEach(async(() => {
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
