import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { AssociateListComponent } from "./associate-list.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { MatSnackBar } from "@angular/material/snack-bar";

describe("AssociateListComponent",
  () => {
    let component: AssociateListComponent;
    let fixture: ComponentFixture<AssociateListComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [AssociateListComponent],
          imports: [ReactiveFormsModule, FormsModule],
          schemas: [NO_ERRORS_SCHEMA],
          providers: [
            { provide: MatSnackBar, useValue: {} },
          ]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(AssociateListComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
