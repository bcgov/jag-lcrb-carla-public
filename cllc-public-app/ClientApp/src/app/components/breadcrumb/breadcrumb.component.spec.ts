import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import {
  RouterTestingModule
} from "@angular/router/testing";

import { BreadcrumbComponent } from "./breadcrumb.component";

describe("BreadcrumbComponent",
  () => {
    let component: BreadcrumbComponent;
    let fixture: ComponentFixture<BreadcrumbComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [BreadcrumbComponent],
          imports: [RouterTestingModule]
        })
        .compileComponents();


    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(BreadcrumbComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should be created",
      () => {
        expect(component).toBeTruthy();
      });
  });
