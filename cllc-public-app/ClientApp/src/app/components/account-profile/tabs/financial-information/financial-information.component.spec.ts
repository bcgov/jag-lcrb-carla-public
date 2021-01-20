import { ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";

import { FinancialInformationComponent } from "./financial-information.component";
import { NO_ERRORS_SCHEMA } from "@angular/core";

describe("FinancialInformationComponent",
  () => {
    let component: FinancialInformationComponent;
    let fixture: ComponentFixture<FinancialInformationComponent>;

    beforeEach(waitForAsync(() => {
      TestBed.configureTestingModule({
          declarations: [FinancialInformationComponent],
          schemas: [NO_ERRORS_SCHEMA]
        })
        .compileComponents();
    }));

    beforeEach(() => {
      fixture = TestBed.createComponent(FinancialInformationComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    // it('should create', () => {
    //   expect(component).toBeTruthy();
    // });
  });
