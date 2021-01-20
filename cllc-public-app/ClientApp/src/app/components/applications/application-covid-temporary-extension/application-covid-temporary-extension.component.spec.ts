import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { ApplicationCovidTemporaryExtensionComponent } from "./application-covid-temporary-extension.component";

describe("ApplicationCovidTemporaryExtensionComponent",
  () => {
    let component: ApplicationCovidTemporaryExtensionComponent;
    let fixture: ComponentFixture<ApplicationCovidTemporaryExtensionComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [ApplicationCovidTemporaryExtensionComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ApplicationCovidTemporaryExtensionComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
