import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { ProofOfZoningComponent } from "./proof-of-zoning.component";

describe("ProofOfZoningComponent",
  () => {
    let component: ProofOfZoningComponent;
    let fixture: ComponentFixture<ProofOfZoningComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [ProofOfZoningComponent]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ProofOfZoningComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
