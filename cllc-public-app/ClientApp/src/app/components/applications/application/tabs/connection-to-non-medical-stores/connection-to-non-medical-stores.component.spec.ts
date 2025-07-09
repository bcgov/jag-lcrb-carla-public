import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { ConnectionToNonMedicalStoresComponent } from "./connection-to-non-medical-stores.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";


describe("ConnectionToNonMedicalStoresComponent",
  () => {
    let component: ConnectionToNonMedicalStoresComponent;
    let fixture: ComponentFixture<ConnectionToNonMedicalStoresComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [ConnectionToNonMedicalStoresComponent],
          providers: [
            FormBuilder,
            { provide: MatSnackBar, useValue: {} }
          ],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ConnectionToNonMedicalStoresComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
