import { async, ComponentFixture, TestBed } from "@angular/core/testing";

import { CannabisAssociateScreeningComponent } from "./cannabis-associate-screening.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";
import { FormBuilder } from "@angular/forms";
import { ActivatedRouteStub } from "@app/testing/activated-route-stub";
import { ContactDataService } from "@services/contact-data.service";
import { of } from "rxjs";
import { ActivatedRoute, Router } from "@angular/router";


describe("CannabisAssociateScreeningComponent",
  () => {
    let component: CannabisAssociateScreeningComponent;
    let fixture: ComponentFixture<CannabisAssociateScreeningComponent>;
    const activatedRouteStub = new ActivatedRouteStub({ token: 1 });

    beforeEach(async(() => {
      TestBed.configureTestingModule({
          declarations: [CannabisAssociateScreeningComponent],
          providers: [
            { provide: ActivatedRoute, useValue: activatedRouteStub },
            { provide: Router, useValue: {} },
            {
              provide: ContactDataService,
              useValue: {
                getContactByPhsToken: () => of({})
              }
            },
            FormBuilder
          ],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(CannabisAssociateScreeningComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it("should create",
      () => {
        expect(component).toBeTruthy();
      });
  });
