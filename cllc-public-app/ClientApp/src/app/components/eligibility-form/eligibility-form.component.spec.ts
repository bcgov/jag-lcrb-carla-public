import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { EligibilityFormComponent } from "./eligibility-form.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { ReactiveFormsModule, FormBuilder } from "@angular/forms";
import { MatRadioModule, MatCheckboxModule, MatDialogRef } from "@angular/material";
import { EligibilityFormDataService } from "@services/eligibility-data.service";

describe("EligibilityFormComponent",
  () => {
    let component: EligibilityFormComponent;
    let fixture: ComponentFixture<EligibilityFormComponent>;

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [EligibilityFormComponent],
          imports: [ReactiveFormsModule, MatRadioModule, MatCheckboxModule],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            FormBuilder,
            { provide: MatDialogRef, useValue: {} },
            { provide: EligibilityFormDataService, useValue: {} }
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(EligibilityFormComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
