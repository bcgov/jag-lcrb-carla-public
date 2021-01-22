import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { OrgStructureComponent } from "./org-structure.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";

describe("OrgStructureComponent",
  () => {
    let component: OrgStructureComponent;
    let fixture: ComponentFixture<OrgStructureComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          imports: [ReactiveFormsModule],
          declarations: [OrgStructureComponent],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(OrgStructureComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
