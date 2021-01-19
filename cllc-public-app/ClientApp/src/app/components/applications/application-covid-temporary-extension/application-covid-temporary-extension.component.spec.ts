import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { ApplicationCovidTemporaryExtensionComponent } from "./application-covid-temporary-extension.component";

describe("ApplicationCovidTemporaryExtensionComponent",
  () => {
    let component: ApplicationCovidTemporaryExtensionComponent;
    let fixture: ComponentFixture<ApplicationCovidTemporaryExtensionComponent>;

    beforeEach(async(() => {
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
