import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { DirectorsAndOfficersComponent } from "./directors-and-officers.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";

describe("DirectorsAndOfficersComponent",
  () => {
    let component: DirectorsAndOfficersComponent;
    let fixture: ComponentFixture<DirectorsAndOfficersComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [DirectorsAndOfficersComponent],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(DirectorsAndOfficersComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    // it('should create', () => {
    //   expect(component).toBeTruthy();
    // });
  });
