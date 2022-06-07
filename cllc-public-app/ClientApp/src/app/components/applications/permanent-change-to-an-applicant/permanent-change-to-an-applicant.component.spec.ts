import { ComponentFixture, TestBed } from "@angular/core/testing";

import { PermanentChangeToAnApplicantComponent } from "./permanent-change-to-an-applicant.component";

describe("PermanentChangeToAnApplicantComponent",
  () => {
    let component: PermanentChangeToAnApplicantComponent;
    let fixture: ComponentFixture<PermanentChangeToAnApplicantComponent>;

    beforeEach(async () => {
      await TestBed.configureTestingModule({
        declarations: [PermanentChangeToAnApplicantComponent]
        })
        .compileComponents();
    });

    beforeEach(() => {
      fixture = TestBed.createComponent(PermanentChangeToAnApplicantComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
