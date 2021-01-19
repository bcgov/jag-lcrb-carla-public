/* tslint:disable:no-unused-variable */
import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { NO_ERRORS_SCHEMA } from "@angular/core";

import { ModalComponent } from "./modal.component";
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA, MatDialog } from "@angular/material";

describe("ModalComponent",
  () => {
    let component: ModalComponent;
    let fixture: ComponentFixture<ModalComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [ModalComponent],
          imports: [MatDialogModule],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            { provide: MatDialogRef, useValue: {} },
            { provide: MAT_DIALOG_DATA, useValue: {} },
            { provide: MatDialog, useValue: {} }
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(ModalComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
